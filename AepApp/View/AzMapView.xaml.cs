using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzMapView : ContentView
    {
        public static AzMapView ActiveMap { get; set; }
        

        // Just some random initial level and center position
        int level = 17;
        Point center = new Point(109824, 76940);
        Tuple<int, int> xrange = new Tuple<int, int>(109824, 109829);
        Tuple<int, int> yrange = new Tuple<int, int>(76940, 76947);

        // satellite tile dictionaries, for satellite map type
        Dictionary<string, Image> idxsatimgdict = new Dictionary<string, Image>();
        Dictionary<Image, Tuple<int, int, int>> satimgidxdict = new Dictionary<Image, Tuple<int, int, int>>();

        // road overlay tile dictionaries, for hybrid map type
        Dictionary<string, Image> idxroadimgdict = new Dictionary<string, Image>();
        Dictionary<Image, Tuple<int, int, int>> roadimgidxdict = new Dictionary<Image, Tuple<int, int, int>>();

        // tile vector data dictionaries, for normal map type
        Dictionary<string, TileVector> idxvectordict = new Dictionary<string, TileVector>();
        Dictionary<TileVector, Tuple<int, int, int>> vectoridxdict = new Dictionary<TileVector, Tuple<int, int, int>>();

        int mapwidth = 1;
        int mapheight = 1;

        const int tilesize = 192;

        double vpminx = 0;
        double vpmaxy = 0;

        private AzmMapType _maptype = AzmMapType.Satellite;
        public AzmMapType MapType
        {
            get { return _maptype; }
            set { _maptype = value;
                if (_maptype== AzmMapType.Normal)
                {
                    ReloadTile();
                    can.IsVisible = true;
                } else
                {
                    ReloadTile();
                    can.IsVisible = false;
                }
            }
        }

        //AzmCoord sw = new AzmCoord(0, 0);
        //AzmCoord ne = new AzmCoord(0, 0);

        public AzmMarkerView TappedMarker { get; set; }
        public AzmLabelView MarkerPopupLabel { get; set; }

        public AzMapView()
        {
            InitializeComponent();
            Overlays = new ObservableCollection<AzmOverlayView>();
            UpdateRanges();
            this.SizeChanged += AzMapView_SizeChanged;
            ActiveMap = this;
            TappedMarker = null;
            MarkerPopupLabel = null;

            MapType = AzmMapType.Normal;
        }

        private void UpdateRanges()
        {
            int sx = (int)Math.Floor(center.X - (double)mapwidth / tilesize / 2);
            int ex = (int)Math.Floor(center.X + (double)mapwidth / tilesize / 2);
            int sy = (int)Math.Floor(center.Y - (double)mapheight / tilesize / 2);
            int ey = (int)Math.Floor(center.Y + (double)mapheight / tilesize / 2);

            xrange = new Tuple<int, int>(sx, ex);
            yrange = new Tuple<int, int>(sy, ey);

            vpminx = center.X - (double)mapwidth / tilesize / 2;
            vpmaxy = center.Y + (double)mapheight / tilesize / 2;

            lbllevel.Text = ((int)level).ToString();
            lblcx.Text = ((int)center.X).ToString();
            lblcy.Text = ((int)center.Y).ToString();


            ReloadTile();

            foreach (object v in overlayviews.Children)
            {
                if (v is AzmOverlayView)
                {
                    AzmOverlayView ov = v as AzmOverlayView;
                    Point p = GetXYFromCoord(level, ov.Coord);
                    ov.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((p.X - vpminx) * tilesize - ov.Anchor.X, (vpmaxy - p.Y) * tilesize - ov.Anchor.Y, ov.Size.Width, ov.Size.Height));
                }
            }
            can.InvalidateSurface();
        }

        private void AzMapView_SizeChanged(object sender, EventArgs e)
        {
            mapwidth = (int)Math.Ceiling(tile.Width);
            mapheight = (int)Math.Ceiling(tile.Height);
            UpdateRanges();
        }

        private AzmCoord GetCoordFromXY(int level, Point p)
        {
            //const float maxlat = 85.051128779806592377796715521925f;

            double n = Math.Pow(2.0, (double)level);
            double lng = p.X / n * 360.0 - 180.0;
            double lat = Math.Atan(Math.Exp((p.Y / n - 0.5) * Math.PI * 2)) / Math.PI * 360.0 - 90.0;
            return new AzmCoord(lng, lat);
        }

        private Point GetXYFromCoord(int level, AzmCoord coord)
        {
            //const float maxlat = 85.051128779806592377796715521925f;

            double n = Math.Pow(2.0, (double)level);
            double x = (coord.lng + 180.0) / 360.0 * n;
            double y = (Math.Log(Math.Tan((coord.lat + 90.0) / 360.0 * Math.PI)) / 2.0 / Math.PI + 0.5) * n;
            return new Point(x, y);
        }

        public void SetCenter(int _level, AzmCoord _center)
        {
            level = _level;
            center = GetXYFromCoord(level, _center);
            UpdateRanges();
            backupx = center.X;
            backupy = center.Y;
            backupxrange = new Tuple<int, int>(xrange.Item1, xrange.Item2);
            backupyrange = new Tuple<int, int>(yrange.Item1, yrange.Item2);
        }

        public void InvalidateSurface()
        {
            can.InvalidateSurface();
        }

        private void ReloadTile()
        {
            // remove all tile
            idxvectordict.Clear();
            vectoridxdict.Clear();

            if (_maptype == AzmMapType.Satellite || _maptype== AzmMapType.Hybrid)
            {
                List<Image> dellist = new List<Image>();
                foreach (var img in satimgidxdict)
                {
                    dellist.Add(img.Key);
                }
                foreach (var img in dellist)
                {
                    var t = satimgidxdict[img];
                    string key = t.Item1.ToString() + "_" + t.Item2.ToString() + "_" + t.Item3.ToString();

                    idxsatimgdict.Remove(key);
                    satimgidxdict.Remove(img);
                    tile.Children.Remove(img);
                }
                satimgidxdict.Clear();
                idxsatimgdict.Clear();


                dellist.Clear();
                foreach (var img in roadimgidxdict)
                {
                    dellist.Add(img.Key);
                }
                foreach (var img in dellist)
                {
                    var t = roadimgidxdict[img];
                    string key = t.Item1.ToString() + "_" + t.Item2.ToString() + "_" + t.Item3.ToString();

                    idxroadimgdict.Remove(key);
                    roadimgidxdict.Remove(img);
                    tile.Children.Remove(img);
                }
                roadimgidxdict.Clear();
                idxroadimgdict.Clear();

            }

            // add tile
            for (var y = yrange.Item1; y <= yrange.Item2; y++)
            {
                for (var x = xrange.Item1; x <= xrange.Item2; x++)
                {
                    int xb = x >> 4;
                    int yb = y >> 4;

                    if (_maptype == AzmMapType.Satellite || _maptype == AzmMapType.Hybrid)
                    {
                        string url = null;
                        url = string.Format("http://p2.map.gtimg.com/sateTiles/{0}/{1}/{2}/{3}_{4}.jpg?version=229", level, xb, yb, x, y);

                        Image img = new Image();
                        img.Source = new UriImageSource
                        {
                            Uri = new Uri(url),
                            CachingEnabled = true,
                            CacheValidity = new TimeSpan(5, 0, 0, 0)
                        };
                        img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                        img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                        tile.Children.Insert(0, img);

                        satimgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                        idxsatimgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);
                    }

                    if (_maptype == AzmMapType.Hybrid)
                    {
                        string url = null;
                        
                        url = string.Format("http://rt3.map.gtimg.com/tile?z={0}&x={1}&y={2}&styleid=2&version=274", level, x, y);
                        

                        Image img = new Image();
                        img.Source = new UriImageSource
                        {
                            Uri = new Uri(url),
                            CachingEnabled = true,
                            CacheValidity = new TimeSpan(5, 0, 0, 0)
                        };
                        img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                        img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                        tile.Children.Add(img);

                        roadimgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                        idxroadimgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);
                    }

                    if (_maptype == AzmMapType.Normal)
                    {
                        LoadTileVectors(level, x, y);
                    }

                }
            }

        }

        private void Deserialization_Error<ErrorEventArgs>(object sender, ErrorEventArgs e)
        {
            
            int a = 0;
        }

        private void LoadTileVectors(int level, int x, int y, int retrycount=0)
        {
            bool load = false;
            TileVector tp = null;
            if (!idxvectordict.ContainsKey((level.ToString() + "_" + x.ToString() + "_" + y.ToString())))
            {
                tp = new TileVector();
                idxvectordict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), tp);
                vectoridxdict.Add(tp, new Tuple<int, int, int>(level, x, y));
                load = true;
            } else
            {
                tp = idxvectordict[level.ToString() + "_" + x.ToString() + "_" + y.ToString()];
                if (!tp.loaded) load = true;
            }
            if (load)
            { 
                string vecurl = string.Format("http://rt3.map.gtimg.com/vector/?z={0}&x={1}&y={2}&type=jsonp&version=274", level, x, y);
                Uri uri = new Uri(vecurl);
                WebRequest request = WebRequest.Create(uri);
                int clevel = level;

                request.BeginGetResponse((IAsyncResult arg) =>
                {
                    if (clevel != level) return;

                    try
                    {
                        using (Stream stream = request.EndGetResponse(arg).GetResponseStream())
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            stream.CopyTo(memStream);
                            memStream.Seek(0, SeekOrigin.Begin);

                            using (StreamReader sr = new StreamReader(memStream))
                            {
                                string value = sr.ReadToEnd();

                                /// IMPORTANT NOTE!!
                                /// Sometimes, the JSON results retrieved from the Tencent Map URL has some missing characters,
                                /// which will cause the following statement to trigger exception. DO NOT try to fix the JSON string.
                                /// Simply reload will usually give the correct result.
                                JObject obj = JsonConvert.DeserializeObject(value) as JObject;

                                JArray features = obj["features"] as JArray;

                                foreach (JArray i in features)
                                {
                                    bool istext = true;
                                    if (i.Count == 2) istext = false;
                                    else
                                    {
                                        if (i[2].Type != JTokenType.String) istext = false;
                                    }


                                    if (!istext)
                                    {
                                        int key = (int)i[0];
                                        JArray coord = i[1] as JArray;
                                        SKPath p = new SKPath();
                                        float cx = (float)coord[0];
                                        float cy = (float)coord[1];
                                        p.MoveTo(cx, cy);
                                        for (int j = 1; j < coord.Count / 2; j++)
                                        {
                                            cx += (float)coord[j * 2];
                                            cy += (float)coord[j * 2 + 1];
                                            p.LineTo(cx, cy);
                                        }
                                        //p.Close();

                                        tp.paths.Add(new Tuple<int, SKPath>(key, p));
                                    }
                                    else
                                    {
                                        TileText tt = new TileText();
                                        tt.key = (int)i[0];
                                        JArray coord = i[1] as JArray;
                                        float cx = (float)coord[0];
                                        float cy = (float)coord[1];
                                        tt.points.Add(new SKPoint(cx, cy));
                                        for (int j = 1; j < coord.Count / 2; j++)
                                        {
                                            cx += (float)coord[j * 2];
                                            cy += (float)coord[j * 2 + 1];
                                            tt.points.Add(new SKPoint(cx, cy));
                                        }
                                        string label = (string)i[2];
                                        string[] lines = label.Split("/".ToCharArray());
                                        foreach (var line in lines) tt.strings.Add(line);

                                        if (i.Count==4) tt.rotdeg= (int)i[3];

                                        tp.texts.Add(tt);
                                    }
                                }
                            }
                            tp.loaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // limit to 5 retries to avoid from stack overflow
                        if (retrycount<5) LoadTileVectors(level, x, y, retrycount+1);
                    }

                    Device.BeginInvokeOnMainThread(() => can.InvalidateSurface());

                }, null);
            }
        }

        double backupx = 0;
        double backupy = 0;
        Tuple<int, int> backupxrange = null;
        Tuple<int, int> backupyrange = null;
        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                backupx = center.X;
                backupy = center.Y;
                backupxrange = new Tuple<int, int>(xrange.Item1, xrange.Item2);
                backupyrange = new Tuple<int, int>(yrange.Item1, yrange.Item2);
                Console.Write(level.ToString());
                Console.Write(": ");
                Console.Write(center.X.ToString());
                Console.Write("==== ");
                Console.WriteLine(center.Y.ToString());

            }
            else if (e.StatusType == GestureStatus.Running)
            {

                double newx = backupx - e.TotalX / tilesize;
                double newy = backupy + e.TotalY / tilesize;
                center = new Point(newx, newy);
                lbllevel.Text = ((int)level).ToString();
                lblcx.Text = ((int)center.X).ToString();
                lblcy.Text = ((int)center.Y).ToString();
                vpminx = center.X - (double)mapwidth / tilesize / 2;
                vpmaxy = center.Y + (double)mapheight / tilesize / 2;

                int sx = (int)Math.Floor(center.X - (double)mapwidth / tilesize / 2);
                int ex = (int)Math.Floor(center.X + (double)mapwidth / tilesize / 2);
                int sy = (int)Math.Floor(center.Y - (double)mapheight / tilesize / 2);
                int ey = (int)Math.Floor(center.Y + (double)mapheight / tilesize / 2);

                xrange = new Tuple<int, int>(sx, ex);
                yrange = new Tuple<int, int>(sy, ey);

                if (_maptype == AzmMapType.Satellite || _maptype == AzmMapType.Hybrid)
                {
                    foreach (object v in tile.Children)
                    {
                        if (v is Image)
                        {
                            Image img = v as Image;
                            if (satimgidxdict.ContainsKey(img))
                            {
                                Tuple<int, int, int> idx = satimgidxdict[img];
                                img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((idx.Item2 - vpminx) * tilesize, (vpmaxy - idx.Item3 - 1) * tilesize, tilesize, tilesize));
                            }
                        }
                    }
                }

                if (_maptype == AzmMapType.Hybrid)
                {
                    foreach (object v in tile.Children)
                    {
                        if (v is Image)
                        {
                            Image img = v as Image;
                            if (roadimgidxdict.ContainsKey(img))
                            {
                                Tuple<int, int, int> idx = roadimgidxdict[img];
                                img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((idx.Item2 - vpminx) * tilesize, (vpmaxy - idx.Item3 - 1) * tilesize, tilesize, tilesize));
                            }
                        }
                    }
                }

                foreach (object v in overlayviews.Children) { 
                    if (v is AzmOverlayView)
                    {
                        AzmOverlayView ov = v as AzmOverlayView;
                        Point p = GetXYFromCoord(level, ov.Coord);
                        ov.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((p.X - vpminx) * tilesize - ov.Anchor.X, (vpmaxy - p.Y) * tilesize - ov.Anchor.Y, ov.Size.Width, ov.Size.Height));
                    }
                }

                for (var y = yrange.Item1; y <= yrange.Item2; y++)
                {
                    for (var x = xrange.Item1; x <= xrange.Item2; x++)
                    {
                        int xb = x >> 4;
                        int yb = y >> 4;
                        string key = level.ToString() + "_" + x.ToString() + "_" + y.ToString();

                        if (_maptype == AzmMapType.Satellite || _maptype == AzmMapType.Hybrid)
                        {
                            if (!idxsatimgdict.ContainsKey(key))
                            {
                                string url = null;
                                url = string.Format("http://p2.map.gtimg.com/sateTiles/{0}/{1}/{2}/{3}_{4}.jpg?version=229", level, xb, yb, x, y);

                                Image img = new Image();
                                img.Source = new UriImageSource
                                {
                                    Uri = new Uri(url),
                                    CachingEnabled = true,
                                    CacheValidity = new TimeSpan(5, 0, 0, 0)
                                };
                                img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                                img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                                tile.Children.Insert(0, img);

                                satimgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                                idxsatimgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);
                            }
                        }

                        if (_maptype == AzmMapType.Hybrid)
                        {
                            if (!idxroadimgdict.ContainsKey(key))
                            {
                                string url = null;
                                url = string.Format("http://rt3.map.gtimg.com/tile?z={0}&x={1}&y={2}&styleid=2&version=274", level, x, y);
                                
                                Image img = new Image();
                                img.Source = new UriImageSource
                                {
                                    Uri = new Uri(url),
                                    CachingEnabled = true,
                                    CacheValidity = new TimeSpan(5, 0, 0, 0)
                                };
                                img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                                img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                                tile.Children.Add(img);

                                roadimgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                                idxroadimgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);
                            }
                        }

                        if (_maptype == AzmMapType.Normal)
                        {
                            if (!idxvectordict.ContainsKey(key))
                            {
                                LoadTileVectors(level, x, y);
                            }
                        }
                    }
                }

                if (_maptype == AzmMapType.Satellite)
                {
                    List<Image> dellist = new List<Image>();

                    foreach (var img in satimgidxdict)
                    {
                        if (img.Value.Item1 != level) dellist.Add(img.Key);
                        else
                        {
                            if (img.Value.Item2 < vpminx - 1 || img.Value.Item2 > vpminx + mapwidth / tilesize + 1)
                            {
                                dellist.Add(img.Key);
                            }
                            else if (img.Value.Item3 > vpmaxy + 1 || img.Value.Item3 < vpmaxy - mapheight / tilesize - 2)
                            {
                                dellist.Add(img.Key);
                            }
                        }
                    }

                    foreach (var img in dellist)
                    {
                        var t = satimgidxdict[img];
                        string key = t.Item1.ToString() + "_" + t.Item2.ToString() + "_" + t.Item3.ToString();

                        idxsatimgdict.Remove(key);
                        satimgidxdict.Remove(img);
                        tile.Children.Remove(img);
                    }
                }

                if (_maptype== AzmMapType.Normal) can.InvalidateSurface();
            }
        }

        private void zoomin_Clicked(object sender, EventArgs e)
        {
            if (level == 18) return;
            AzmCoord cc = GetCoordFromXY(level, this.center);
            level++;
            center = GetXYFromCoord(level, cc);
            UpdateRanges();
            backupx = center.X;
            backupy = center.Y;
            backupxrange = new Tuple<int, int>(xrange.Item1, xrange.Item2);
            backupyrange = new Tuple<int, int>(yrange.Item1, yrange.Item2);

        }

        private void zoomout_Clicked(object sender, EventArgs e)
        {
            if (level == 4) return;
            AzmCoord cc = GetCoordFromXY(level, this.center);
            level--;
            center = GetXYFromCoord(level, cc);
            UpdateRanges();
            backupx = center.X;
            backupy = center.Y;
            backupxrange = new Tuple<int, int>(xrange.Item1, xrange.Item2);
            backupyrange = new Tuple<int, int>(yrange.Item1, yrange.Item2);

        }

        private void sat_Clicked(object sender, EventArgs e)
        {
            MapType = AzmMapType.Satellite;
        }

        private void normal_Clicked(object sender, EventArgs e)
        {
            MapType = AzmMapType.Normal;
        }

        private void hybrid_Clicked(object sender, EventArgs e)
        {
            MapType = AzmMapType.Hybrid;
        }

        ObservableCollection<AzmOverlayView> overlays = null;
        public ObservableCollection<AzmOverlayView> Overlays
        {
            get
            {
                return overlays;
            }
            set
            {
                if (value != null)
                {
                    value.CollectionChanged -= Overlay_CollectionChanged;
                }
                overlays = value;
                if (value != null)
                {
                    value.CollectionChanged += Overlay_CollectionChanged;
                }
            }
        }

        private void Overlay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AzmOverlayView o in e.NewItems)
                {
                    o.MapView = this;
                    Point p = GetXYFromCoord(level, o.Coord);

                    o.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((p.X - vpminx) * tilesize - o.Anchor.X, (vpmaxy - p.Y) * tilesize - o.Anchor.Y, o.Size.Width, o.Size.Height));
                    o.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                    overlayviews.Children.Add(o);
                }
            }
            if (e.OldItems != null)
            {
                foreach (AzmOverlayView o in e.OldItems)
                {
                    overlayviews.Children.Remove(o);
                }
            }
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            double xf = info.Width / overlayviews.Width;
            double yf = info.Height / overlayviews.Height;

            canvas.Clear(Color.FromHex("#f7f5f5").ToSKColor());
            //canvas.Clear();


            for (var y = yrange.Item1; y <= yrange.Item2; y++)
            {
                for (var x = xrange.Item1; x <= xrange.Item2; x++)
                {
                    Rectangle r = new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize);

                    string idx = level.ToString() + "_" + x.ToString() + "_" + y.ToString();
                    if (idxvectordict.ContainsKey(idx))
                    {
                        TileVector tp = idxvectordict[idx];
                        tp.PaintShapes(canvas, (float)r.Left * (float)xf, (float)r.Top * (float)yf, (float)xf / 256.0f * tilesize, (float)yf / 256.0f * tilesize);
                    }

                }
            }
            for (var y = yrange.Item1; y <= yrange.Item2; y++)
            {
                for (var x = xrange.Item1; x <= xrange.Item2; x++)
                {
                    Rectangle r = new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize);

                    string idx = level.ToString() + "_" + x.ToString() + "_" + y.ToString();
                    if (idxvectordict.ContainsKey(idx))
                    {
                        TileVector tp = idxvectordict[idx];
                        tp.PaintPaths(canvas, (float)r.Left * (float)xf, (float)r.Top * (float)yf, (float)xf / 256.0f * tilesize, (float)yf / 256.0f * tilesize);
                    }
                }
            }
            for (var y = yrange.Item1; y <= yrange.Item2; y++)
            {
                for (var x = xrange.Item1; x <= xrange.Item2; x++)
                {
                    Rectangle r = new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize);

                    string idx = level.ToString() + "_" + x.ToString() + "_" + y.ToString();
                    if (idxvectordict.ContainsKey(idx))
                    {
                        TileVector tp = idxvectordict[idx];
                        tp.PaintLabels(canvas, (float)r.Left * (float)xf, (float)r.Top * (float)yf, (float)xf / 256.0f * tilesize, (float)yf / 256.0f * tilesize);
                    }
                }
            }

            // for debug only
            //TileVector.textpaint.Color = SKColors.Black;
            //TileVector.textpaint.TextSize = 15.0f * (float)yf;

            //SKPaint paint = new SKPaint
            //{
            //    Style = SKPaintStyle.Stroke,
            //    Color = Color.FromHex("#40000000").ToSKColor(),
            //    StrokeWidth = 2
            //};

            //for (var y = yrange.Item1; y <= yrange.Item2; y++)
            //{
            //    for (var x = xrange.Item1; x <= xrange.Item2; x++)
            //    {
            //        Rectangle r = new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize);
            //        canvas.DrawRect(new SKRect((float)r.Left * (float)xf, (float)r.Top * (float)yf, (float)r.Right * (float)xf, (float)r.Bottom * (float)yf), paint);
            //        canvas.DrawText(level.ToString()+", "+x.ToString()+", "+y.ToString(), (float)r.Left * (float)xf, (float)r.Top * (float)yf + 15.0f*(float)yf, TileVector.textpaint);
            //    }
            //}

        }
    }

    public class TileText
    {
        public int key { get; set; }
        public List<SKPoint> points = new List<SKPoint>();
        public List<string> strings = new List<string>();
        public int rotdeg { get; set; }
    }

    public class TileVector
    {
        public int level { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public bool _loaded = false;
        public bool loaded
        {
            get { return _loaded; }
            set { _loaded = value; }
        }

        public List<Tuple<int, SKPath>> _paths = new List<Tuple<int, SKPath>>();

        public List<Tuple<int, SKPath>> paths { get { return _paths; } }

        public List<TileText> _texts = new List<TileText>();

        public List<TileText> texts { get { return _texts; } }


        public static SKPaint textpaint = null;

        static TileVector()
        {
            if (textpaint == null)
            {
                textpaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Black,
                    TextEncoding = SKTextEncoding.Utf8,
                    TextSize = 10,
                    IsAntialias = true,
                    LcdRenderText = true
                };
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AepApp.iOS.msyh.ttc");
                    textpaint.Typeface = SKTypeface.FromStream(stream);
                }
                else
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AepApp.Droid.msyh.ttc");
                    textpaint.Typeface = SKTypeface.FromStream(stream);
                }
            }
        }

        public void PaintShapes(SKCanvas can,float x, float y, float sx, float sy)
        {
            if (!loaded) return;
            can.Translate(new SKPoint(x, y));
            can.Scale(new SKPoint(sx, sy));

            foreach (var p in _paths)
            {
                SKPaint paint = null;

                if (p.Item1 < 200000 && p.Item1 > 196000)   // fills
                {
                    SKColor col = new SKColor();

                    switch (p.Item1)
                    {
                        case 196610: col = Color.FromHex("#96bffd").ToSKColor(); break;
                        case 196612: col = Color.FromHex("#f9f9f9").ToSKColor(); break;
                        case 196614: col = Color.FromHex("#bce9af").ToSKColor(); break;
                        case 196615: col = Color.FromHex("#bce9af").ToSKColor(); break;
                        case 196616: col = Color.FromHex("#bce9af").ToSKColor(); break;
                        case 196617: col = Color.FromHex("#f3f1ea").ToSKColor(); break;
                        case 196618: col = Color.FromHex("#96bffd").ToSKColor(); break;
                        case 196619: col = Color.FromHex("#f8f6f3").ToSKColor(); break;
                        case 196621: col = Color.FromHex("#efebe8").ToSKColor(); break;
                        case 196622: col = Color.FromHex("#f3f1ea").ToSKColor(); break;
                        case 196627: col = Color.FromHex("#e6ede2").ToSKColor(); break;
                        case 196630: col = Color.FromHex("#edebdd").ToSKColor(); break;
                        case 196632: col = Color.FromHex("#eaf1f5").ToSKColor(); break;

                        case 196713: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196715: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196717: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196718: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196719: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196721: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196722: col = Color.FromHex("#d1577c").ToSKColor(); break;
                        case 196723: col = Color.FromHex("#d1577c").ToSKColor(); break;



                        case 196734: col = Color.FromHex("#569dd2").ToSKColor(); break;
                        case 196744: col = Color.FromHex("#569cd1").ToSKColor(); break;
                        case 196769: col = Color.FromHex("#efece7").ToSKColor(); break;
                        case 196770: col = Color.FromHex("#f8eaeb").ToSKColor(); break;
                        case 196771: col = Color.FromHex("#faf7f2").ToSKColor(); break;
                        default:
                            col = Color.FromHex("#f00").ToSKColor();
                            Console.WriteLine("&&&&&&& " + p.Item1.ToString());
                            break;
                    }

                    paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = col,
                    };
                    can.DrawPath(p.Item2, paint);
                }
            }

            can.ResetMatrix();
        }

        Dictionary<int, SKBitmap> iconcache = new Dictionary<int, SKBitmap>();

        public void PaintLabels(SKCanvas can, float x, float y, float sx, float sy)
        {
            if (!loaded) return;
            can.Translate(new SKPoint(x, y));
            can.Scale(new SKPoint(sx, sy));

            var m = can.TotalMatrix;
            foreach (var t in _texts)
            {
                if (t.strings.Count <= t.points.Count)
                {
                    SKColor col = new SKColor();
                    SKColor backcol = SKColors.Transparent;
                    float textsize = 10.0f;
                    int icon = -1;

                    string suf = ""; // for debug only

                    switch (t.key)
                    {
                        case 65537: col = Color.FromHex("#c0b6a6").ToSKColor(); icon = -2; textsize = 20.0f; break;
                        case 65538: col = Color.FromHex("#2b4dac").ToSKColor(); icon = -2; textsize = 20.0f; break;
                        case 65539: col = Color.FromHex("#797979").ToSKColor(); icon = -2; break;
                        case 65540: col = Color.FromHex("#4776ba").ToSKColor(); icon = -2; break;
                        case 65542: col = Color.FromHex("#5d5953").ToSKColor(); icon = -2; break;
                        case 65543: col = Color.FromHex("#797979").ToSKColor(); icon = 2; break;
                        case 65544: col = Color.FromHex("#333333").ToSKColor(); icon = -2; break;
                        case 65545: col = Color.FromHex("#989898").ToSKColor(); icon = -2; break;
                        case 65546: col = Color.FromHex("#746c53").ToSKColor(); icon = -2; break;
                        case 65547: col = Color.FromHex("#65431e").ToSKColor(); icon = -2; 
                            backcol = Color.FromHex("#fffbf2").ToSKColor();  break;
                        case 65549: col = Color.FromHex("#4776ba").ToSKColor(); icon = -2; break;
                        case 65550: col = Color.FromHex("#a4b8c2").ToSKColor(); textsize = 15; break;
                        case 65552: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65553: col = Color.FromHex("#989898").ToSKColor(); textsize = 8; icon = -2; break;
                        case 65554: col = Color.FromHex("#93c185").ToSKColor(); textsize = 15; break;
                        case 65555: col = Color.FromHex("#797979").ToSKColor(); icon = 0; break;
                        case 65556: col = Color.FromHex("#b83725").ToSKColor(); icon = -2; textsize = 20.0f; break;
                        case 65557: col = Color.FromHex("#797979").ToSKColor(); icon = 1; break;
                        case 65558: col = Color.FromHex("#797979").ToSKColor(); icon = 2; break;
                        case 65559: col = Color.FromHex("#333333").ToSKColor(); icon = 3; break;
                        case 65560: col = Color.FromHex("#417dcc").ToSKColor(); icon = 4; break;
                        case 65561: col = Color.FromHex("#797979").ToSKColor(); icon = -2; continue; break;
                        case 65562: col = Color.FromHex("#a17b60").ToSKColor(); icon = 6; break;
                        case 65563: col = Color.FromHex("#1d9740").ToSKColor(); icon = 7; break;
                        case 65564: col = Color.FromHex("#1d9740").ToSKColor(); icon = 8; break;
                        case 65565: col = Color.FromHex("#1d9740").ToSKColor(); icon = 9; break;
                        case 65567: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 11; break;
                        case 65568: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65569: col = Color.FromHex("#797979").ToSKColor(); icon = 13; break;
                        case 65570: col = Color.FromHex("#417dcc").ToSKColor(); icon = 14; break;
                        case 65571: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 15; break;
                        case 65572: col = Color.FromHex("#4e86cf").ToSKColor(); icon = 16; break;
                        case 65573: col = Color.FromHex("#797979").ToSKColor(); icon = 17; break;
                        case 65574: col = Color.FromHex("#797979").ToSKColor(); icon = 18; break;
                        case 65576: col = Color.FromHex("#417dcc").ToSKColor(); icon = 20; break;
                        case 65578: col = Color.FromHex("#797979").ToSKColor(); icon = 22; break;
                        case 65579: col = Color.FromHex("#797979").ToSKColor(); icon = 23; break;
                        case 65580: col = Color.FromHex("#989898").ToSKColor(); icon = 24; break;
                        case 65581: col = Color.FromHex("#af4d47").ToSKColor(); icon = 25; break;
                        case 65582: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 26; break;
                        case 65583: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65584: col = Color.FromHex("#797979").ToSKColor(); icon = 28; break;
                        case 65586: col = Color.FromHex("#797979").ToSKColor(); icon = 30; break;
                        case 65587: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 31; break;
                        case 65588: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 32; break;
                        case 65589: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 33; break;
                        case 65591: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 35; break;
                        case 65592: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65593: col = Color.FromHex("#8b7de7").ToSKColor(); icon = 37; break;
                        case 65594: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65595: col = Color.FromHex("#8a8a88").ToSKColor(); break;
                        case 65598: col = Color.FromHex("#797979").ToSKColor(); icon = 42; break;
                        case 65600: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65601: col = Color.FromHex("#8a8a88").ToSKColor(); break;
                        case 65604: col = Color.FromHex("#797979").ToSKColor(); icon = 48; break;
                        case 65605: col = Color.FromHex("#797979").ToSKColor(); icon = 14; break;
                        case 65606: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65607: col = Color.FromHex("#797979").ToSKColor(); icon = 51; break;
                        case 65608: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65609: col = Color.FromHex("#fff").ToSKColor();
                            backcol = Color.FromHex("#009a00").ToSKColor(); textsize = 8; break;
                        case 65610: col = Color.FromHex("#fff").ToSKColor();
                            backcol = Color.FromHex("#009a00").ToSKColor(); textsize = 8; break;
                        case 65611: col = Color.FromHex("#797979").ToSKColor(); icon = 55; break;
                        case 65614: col = Color.FromHex("#000").ToSKColor(); icon = 59; break;
                        case 65615: col = Color.FromHex("#000").ToSKColor(); icon = 60; break;
                        case 65616: col = Color.FromHex("#000").ToSKColor(); icon = 61; break;
                        case 65617: col = Color.FromHex("#797979").ToSKColor(); icon = 62; break;
                        case 65620: col = Color.FromHex("#797979").ToSKColor(); icon = 65; break;
                        case 65622: col = Color.FromHex("#797979").ToSKColor(); icon = 66; break;
                        case 65623: col = Color.FromHex("#797979").ToSKColor(); icon = 67; break;
                        case 65627: col = Color.FromHex("#797979").ToSKColor(); icon = 70; break;     // subway station name
                        case 65631: col = Color.FromHex("#000").ToSKColor(); icon = 73; break;     // atm
                        case 65632: col = Color.FromHex("#797979").ToSKColor(); break;     // subway station name
                        case 65668: col = Color.FromHex("#333333").ToSKColor(); icon = 113; break;     // hong kong subway station name
                        case 65669: col = Color.FromHex("#333333").ToSKColor(); icon = 113; break;     // hong kong subway station name
                        case 65680: col = Color.FromHex("#333333").ToSKColor(); icon = 121; break;     // ningbo subway station name
                        case 65681: col = Color.FromHex("#0f0").ToSKColor(); break;
                        case 65684: col = Color.FromHex("#00f").ToSKColor(); textsize = 20; break;
                        case 65686: col = Color.FromHex("#8a8a88").ToSKColor(); textsize = 15; break;
                        case 65687: col = Color.FromHex("#a8bb9e").ToSKColor(); textsize = 15; break;
                        case 65688: col = Color.FromHex("#b9b7a0").ToSKColor(); textsize = 15; break;
                        case 65700: col = Color.FromHex("#000").ToSKColor(); icon = 165; break;
                        case 65701: col = Color.FromHex("#000").ToSKColor(); icon = 166; break;
                        case 65702: col = Color.FromHex("#000").ToSKColor(); icon = 167; break;
                        case 65703: col = Color.FromHex("#000").ToSKColor(); icon = 168; break;
                        case 65704: col = Color.FromHex("#797979").ToSKColor(); icon = 169; break;
                        case 65705: col = Color.FromHex("#797979").ToSKColor(); icon = 170; break;
                        case 65712: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65713: col = Color.FromHex("#797979").ToSKColor(); icon = 28; break;
                        case 65714: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65715: col = Color.FromHex("#797979").ToSKColor(); icon = 18; break;
                        case 65716: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 45; break;
                        case 65717: col = Color.FromHex("#b04e48").ToSKColor(); icon = 93; break;
                        case 65741: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 53; break;
                        case 65742: col = Color.FromHex("#5f6f97").ToSKColor(); break;
                        case 65743: col = Color.FromHex("#8b7de7").ToSKColor(); break;
                        case 65755: col = Color.FromHex("#b8a790").ToSKColor(); textsize = 15; break;
                        case 65756: col = Color.FromHex("#caa1a2").ToSKColor(); textsize = 15; break;
                        case 65757: col = Color.FromHex("#bdbcbc").ToSKColor(); textsize = 15; break;
                        case 65853: col = Color.FromHex("#bc023c").ToSKColor(); icon = 309; break;
                        case 65857: col = Color.FromHex("#006fc1").ToSKColor(); icon = 307; break;


                        case 65976: col = Color.FromHex("#fff").ToSKColor();            // hk disney line
                            backcol = Color.FromHex("#f7afd2").ToSKColor(); break;
                        case 65977: col = Color.FromHex("#fff").ToSKColor();            // hk east rail line
                            backcol = Color.FromHex("#8ac4df").ToSKColor(); break;
                        case 65978: col = Color.FromHex("#fff").ToSKColor();            // hk east rail line
                            backcol = Color.FromHex("#8ac4df").ToSKColor(); break;
                        case 65980: col = Color.FromHex("#fff").ToSKColor();            // hk island line		
                            backcol = Color.FromHex("#6da2d9").ToSKColor(); break;
                        case 65981: col = Color.FromHex("#fff").ToSKColor();            // hk kung tong line		
                            backcol = Color.FromHex("#70bd6f").ToSKColor(); break;
                        case 65983: col = Color.FromHex("#fff").ToSKColor();            // hk tko line
                            backcol = Color.FromHex("#a292bc").ToSKColor(); break;
                        case 65985: col = Color.FromHex("#fff").ToSKColor();            // hk ma on shan line	
                            backcol = Color.FromHex("#bea37e").ToSKColor(); break;
                        case 65986: col = Color.FromHex("#fff").ToSKColor();            // hk tsuen wan line		
                            backcol = Color.FromHex("#e17a7e").ToSKColor(); break;
                        case 65987: col = Color.FromHex("#fff").ToSKColor();            // hk west rail line		
                            backcol = Color.FromHex("#e08fc0").ToSKColor(); break;
                        case 65979: col = Color.FromHex("#fff").ToSKColor();            // hk tung chung line	
                            backcol = Color.FromHex("#f6a775").ToSKColor(); break;
                        case 65982: col = Color.FromHex("#fff").ToSKColor();            // hk airport express		
                            backcol = Color.FromHex("#70d1cf").ToSKColor(); break;
                        case 66035: col = Color.FromHex("#fff").ToSKColor();            // hk sound island line	
                            backcol = Color.FromHex("#a2c013").ToSKColor(); break;





                        case 65998: col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#6da2d9").ToSKColor(); break;
                        case 66008: col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#e17a7e").ToSKColor(); break;





                        case 22179:  col = Color.FromHex("#5a5a5a").ToSKColor(); break;    // road name in strange key range
                        case 262163: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262164: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262165: col = Color.FromHex("#fff").ToSKColor();           // highway number, S
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262169: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262170: col = Color.FromHex("#fff").ToSKColor();           // highway number, S
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262166: col = Color.FromHex("#955410").ToSKColor(); break;
                        case 262167: col = Color.FromHex("#000").ToSKColor();
                            backcol = Color.FromHex("#f8c144").ToSKColor(); break;
                        case 262173: col = Color.FromHex("#704d10").ToSKColor(); break;
                        case 262178: col = Color.FromHex("#927745").ToSKColor(); break;
                        case 262179: col = Color.FromHex("#393939").ToSKColor(); break;
                        case 262180: col = Color.FromHex("#777776").ToSKColor(); break;
                        case 262181: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 262182: col = Color.FromHex("#858483").ToSKColor(); break;
                        case 262415: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262417: col = Color.FromHex("#77561a").ToSKColor(); textsize = 8; break;
                        case 262418: col = Color.FromHex("#77561a").ToSKColor(); textsize = 8; break;
                        case 262419: col = Color.FromHex("#605f5d").ToSKColor(); textsize = 8; break;
                        case 262420: col = Color.FromHex("#4f4f4e").ToSKColor(); break;
                        case 262421: col = Color.FromHex("#4a4a49").ToSKColor(); break;
                        case 262424: col = Color.FromHex("#f88").ToSKColor(); textsize = 20; break;
                        case 262425: col = Color.FromHex("#a77137").ToSKColor(); break;
                        case 262705: col = Color.FromHex("#8ebfe4").ToSKColor(); break;
                        default: col = Color.Magenta.ToSKColor(); suf = " " + t.key.ToString();
                            Console.WriteLine("%%%%%%% " + t.key);
                            break;
                    }

                    if (icon==-1 && t.key < 200000 && t.key >65535 && !backcol.Equals(SKColors.Transparent) && textsize<=10)
                        suf = " " + t.key.ToString();

                    textpaint.Color = col;
                    textpaint.TextSize = textsize;

                    if (!(t.points.Count == 1 && icon >= 0))
                    {
                        for (int i = 0; i < t.strings.Count; i++)
                        {
                            can.SetMatrix(m);
                            float tw = textpaint.MeasureText(t.strings[i]);
                            can.Translate(t.points[i + t.points.Count - t.strings.Count]);
                            can.RotateDegrees(t.rotdeg);

                            if (!backcol.Equals( SKColors.Transparent))
                            {
                                SKPaint backp = new SKPaint()
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = backcol,
                                };
                                can.DrawRect(-tw / 2 - 1, -textpaint.TextSize / 2 - 1, tw + 2, textpaint.TextSize + 3, backp);
                            }

                            if (!col.Equals(SKColors.White) && backcol.Equals( SKColors.Transparent))
                            {
                                textpaint.Style = SKPaintStyle.Stroke;
                                textpaint.StrokeWidth = 3.0f;
                                textpaint.Color = SKColors.White;
                                can.DrawText(t.strings[i] + suf, -tw / 2, textpaint.TextSize / 2, textpaint);
                            }

                            textpaint.Style = SKPaintStyle.Fill;
                            textpaint.Color = col;
                            can.DrawText(t.strings[i] + suf, -tw / 2, textpaint.TextSize / 2, textpaint);
                        }
                    }

                    if (icon >= 0)
                    {
                        can.SetMatrix(m);
                        can.Translate(t.points[0]);

                        if (iconcache.ContainsKey(icon))
                        {
                            //if (iconcache[icon] != null)
                            //{
                                SKPaint bp = new SKPaint();
                                bp.IsAntialias = true;
                                SKBitmap bm = iconcache[icon];
                                can.DrawBitmap(bm, new SKRect(- 9, - 9, 9, 9), bp);
                            //}
                        }
                        else
                        {
                            //iconcache.Add(icon, null);
                            SKBitmap webBitmap = null;
                            string iurl = string.Format("http://rt3.map.gtimg.com/icons/2d/{0}.png", icon);
                            Uri uri = new Uri(iurl);
                            WebRequest request = WebRequest.Create(uri);
                            request.BeginGetResponse((IAsyncResult arg) =>
                            {
                                try
                                {
                                    using (Stream stream = request.EndGetResponse(arg).GetResponseStream())
                                    using (MemoryStream memStream = new MemoryStream())
                                    {
                                        stream.CopyTo(memStream);
                                        memStream.Seek(0, SeekOrigin.Begin);

                                        using (SKManagedStream skStream = new SKManagedStream(memStream))
                                        {
                                            webBitmap = SKBitmap.Decode(skStream);
                                            //iconcache.Remove(icon);
                                            iconcache.Add(icon, webBitmap);
                                        }
                                    }
                                }
                                catch
                                {
                                }

                                Device.BeginInvokeOnMainThread(() => AzMapView.ActiveMap.InvalidateSurface());

                            }, null);
                        }
                    }
                }
            }
            can.ResetMatrix();
        }

        public void PaintPaths(SKCanvas can, float x, float y, float sx, float sy)
        {
            if (!loaded) return;

            can.Translate(new SKPoint(x, y));
            can.Scale(new SKPoint(sx, sy));

            foreach (var p in _paths)
            {
                SKPaint paint = null;
                if (p.Item1 < 133000 && p.Item1 > 131000)   // paths
                {
                    SKColor col = new SKColor();
                    float width = 3;
                    bool points = true;
                    SKPathEffect effect = null;

                    switch (p.Item1)
                    {
                        case 131073: col = Color.FromHex("#edb0b2").ToSKColor(); width = 3.0f; break;
                        case 131074: col = Color.FromHex("#edb0b2").ToSKColor(); width = 3.0f; break;
                        case 131078: col = Color.FromHex("#a0a0a0").ToSKColor(); width = 2.0f; break;
                        case 131079: col = Color.FromHex("#b5bab7").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131080: col = Color.FromHex("#b5bab7").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131081: col = Color.FromHex("#b8d0f5").ToSKColor(); width = 3.0f; break;
                        case 131082: col = Color.FromHex("#ffd08e").ToSKColor(); width = 3.0f; break;
                        case 131083: col = Color.FromHex("#f8e08b").ToSKColor(); width = 3.0f; break;
                        case 131084: col = Color.FromHex("#f8e08b").ToSKColor(); width = 3.0f; break;
                        case 131085: col = Color.FromHex("#e9e7df").ToSKColor(); width = 1.0f; break;
                        case 131086: col = Color.FromHex("#c9c9c9").ToSKColor(); width = 3.0f; break;
                        case 131087: col = Color.FromHex("#c9c9c9").ToSKColor(); width = 5.0f; break; // railway?
                        case 131088: col = Color.FromHex("#ffd08e").ToSKColor(); width = 6.0f; break;
                        case 131089: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;
                        case 131090: col = Color.FromHex("#ffd08e").ToSKColor(); width = 5.0f; break;
                        case 131091: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;
                        case 131092: col = Color.FromHex("#ff0").ToSKColor(); break;
                        case 131094: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131097: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131099: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131101: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131103: col = Color.FromHex("#ffffff").ToSKColor(); width = 4.0f; break;
                        case 131105: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131107: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131109: col = Color.FromHex("#f8e08b").ToSKColor(); width = 2.0f; break;
                        case 131111: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;
                        case 131115: col = Color.FromHex("#80ffffff").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;

                        // hong kong subway lines
                        case 131194: col = Color.FromHex("#ab748f").ToSKColor(); width = 2.0f; break;
                        case 131195: col = Color.FromHex("#6d9bb1").ToSKColor(); width = 2.0f; break;
                        case 131196: col = Color.FromHex("#6d9bb1").ToSKColor(); width = 2.0f; break;
                        case 131197: col = Color.FromHex("#ff0").ToSKColor(); width = 2.0f; break;
                        case 131198: col = Color.FromHex("#1e5b9d").ToSKColor(); width = 2.0f; break;
                        case 131199: col = Color.FromHex("#1c761d").ToSKColor(); width = 2.0f; break;
                        case 131200: col = Color.FromHex("#229c9b").ToSKColor(); width = 2.0f; break;
                        case 131201: col = Color.FromHex("#fff").ToSKColor(); width = 2.0f; break;
                        case 131202: col = Color.FromHex("#5d4c7d").ToSKColor(); width = 2.0f; break;
                        case 131203: col = Color.FromHex("#846742").ToSKColor(); width = 2.0f; break;
                        case 131204: col = Color.FromHex("#a83138").ToSKColor(); width = 2.0f; break;
                        case 131205: col = Color.FromHex("#99216d").ToSKColor(); width = 2.0f; break;
                        case 131377: col = Color.FromHex("#879b2e").ToSKColor(); width = 2.0f; break;


                        case 131329: col = Color.FromHex("#f8e08b").ToSKColor(); width = 4.0f; break;
                        case 131366: col = Color.FromHex("#0f0").ToSKColor(); width = 2.0f; break;
                        case 131397: col = Color.FromHex("#f8e08b").ToSKColor(); width = 2.0f; break;


                        // ningbo subway lines
                        case 131217: col = Color.FromHex("#0c4e95").ToSKColor(); width = 2.0f; break;
                        case 131327: col = Color.FromHex("#970810").ToSKColor(); width = 2.0f; break;


                        case 131331: col = Color.FromHex("#0ff").ToSKColor(); break;
                        case 131332: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131333: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 131334: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;
                        case 131335: col = Color.FromHex("#ffd08e").ToSKColor(); width = 6.0f; break;
                        case 131336: col = Color.FromHex("#ffffff").ToSKColor(); width = 2.0f; break;
                        case 131337: col = Color.FromHex("#ffd08e").ToSKColor(); width = 6.0f; break;
                        case 131338: col = Color.FromHex("#f88").ToSKColor(); width = 2.0f; break;
                        case 131339: col = Color.FromHex("#8f8").ToSKColor(); width = 2.0f; break;
                        case 131340: col = Color.FromHex("#88f").ToSKColor(); width = 2.0f; break;
                        case 131341: col = Color.FromHex("#ffffff").ToSKColor(); width = 4.0f; break;
                        case 131342: col = Color.FromHex("#ffffff").ToSKColor(); width = 4.0f; break;
                        case 131344: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;
                        case 131345:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131352: col = Color.FromHex("#f8e08b").ToSKColor(); width = 5.0f; break;

                        case 131353: col = Color.FromHex("#d4cfc6").ToSKColor(); width = 5.0f; break;
                        case 131354: col = Color.FromHex("#e6e6e6").ToSKColor(); width = 5.0f; break;
                        case 131355: col = Color.FromHex("#e6e6e6").ToSKColor(); width = 5.0f; break;


                        case 131396: col = Color.FromHex("#f8e08b").ToSKColor(); width = 2.0f; break;
                        case 131398: col = Color.FromHex("#ffffff").ToSKColor(); width = 2.0f; break;
                        case 131399: col = Color.FromHex("#ffffff").ToSKColor(); break;

                        case 131400:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131401:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131407: col = Color.FromHex("#0f0").ToSKColor(); break;
                        case 131408:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131409:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131410:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;
                        case 131411:
                            col = Color.FromHex("#d9d3c3").ToSKColor(); width = 1.0f;
                            effect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4); break;

                        default:
                            col = Color.Magenta.ToSKColor(); points = true;
                            Console.WriteLine("++++++ " + p.Item1.ToString());
                            break;
                    }

                    paint = new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = col,
                        StrokeWidth = width,
                        PathEffect = effect,
                        StrokeCap = SKStrokeCap.Round,
                        StrokeJoin = SKStrokeJoin.Round,
                    };
                    can.DrawPath(p.Item2, paint);

                    // railway specual handling
                    if (p.Item1== 131087)
                    {
                        SKPaint paint1 = new SKPaint
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = SKColors.White,
                            StrokeWidth = width-2,
                            PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0),
                            StrokeCap = SKStrokeCap.Round,
                            StrokeJoin = SKStrokeJoin.Round,
                        };
                        can.DrawPath(p.Item2, paint1);
                    }
                }
            }
            can.ResetMatrix();
        }

    }

    public class AzmCoord
    {
        public AzmCoord(double _lng, double _lat)
        {
            lng = _lng;
            lat = _lat;
        }
        public double lng { get; set; }
        public double lat { get; set; }
    }

    public class AzmOverlayView : Xamarin.Forms.TemplatedView
    {
        public AzMapView MapView { get; set; }
        public AzmCoord Coord { get; set; }
        public Size Size { get; set; }
        public Point Anchor { get; set; }

        public AzmOverlayView()
        {

        }
    }

    public class AzmLabelView : AzmOverlayView
    {
        public delegate void OnTappedEventHandler(object sender, EventArgs e);
        public event OnTappedEventHandler OnTapped;

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(AzmLabelView),
            defaultValue: default(string)
        );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(AzmLabelView),
            defaultValue: Color.White
        );

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public AzmLabelView(string text, AzmCoord coord = null, double maxwidthrequest = 100.0)
        {
            if (text == null) text = "";
            BackgroundColor = Color.FromHex("#002060");
            Text = text.Trim();
            WidthRequest = maxwidthrequest;
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);
            ControlTemplate dt = AzMapView.ActiveMap.Resources["azmlabeltemp"] as ControlTemplate;
            ControlTemplate = dt;

            this.BindingContext = new { name = Text };
            double height = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextHeightGivenMaxWidth(Text, WidthRequest - 6, 14));
            double width = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextWidthGivenExactHeight(Text, height, 14));
            width = Math.Min(width, WidthRequest - 6);
            Size = new Size(width + 6, height + 6);
            Anchor = new Point((width + 6) / 2, height + 6 + 7);

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            this.GestureRecognizers.Add(tap);
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            if (OnTapped != null) OnTapped(this, e);
        }
    }

    public class AzmMarkerView : AzmOverlayView
    {
        public delegate void OnPopupTappedEventHandler(object sender, EventArgs e);
        public event OnPopupTappedEventHandler OnPopupTapped;

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(AzmMarkerView),
            defaultValue: default(string)
        );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(AzmMarkerView),
            defaultValue: Color.White
        );

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: nameof(Source),
            returnType: typeof(ImageSource),
            declaringType: typeof(AzmMarkerView),
            defaultValue: null
        );

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public AzmMarkerView(ImageSource source, Size size, AzmCoord coord = null, double popupmaxwidthrequest = 100.0)
        {
            Source = source;
            Size = size;
            Anchor = new Point(size.Width / 2, size.Height);
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);
            ControlTemplate dt = AzMapView.ActiveMap.Resources["azmmarkertemp"] as ControlTemplate;
            ControlTemplate = dt;

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Marker_Tapped;
            this.GestureRecognizers.Add(tap);
        }

        private void Marker_Tapped(object sender, EventArgs e)
        {
            if (MapView.MarkerPopupLabel != null)
            {
                MapView.Overlays.Remove(MapView.MarkerPopupLabel);
                MapView.TappedMarker = null;
            }

            AzmLabelView lv = new AzmLabelView(Text, Coord)
            {
                BackgroundColor = Color.FromHex("#ccc"),
                TextColor = Color.FromHex("#444"),
            };
            lv.Anchor = new Point(lv.Anchor.X, lv.Anchor.Y + Size.Height);
            lv.OnTapped += Popup_OnTapped;
            MapView.Overlays.Add(lv);

            MapView.MarkerPopupLabel = lv;
            MapView.TappedMarker = this;
        }

        private void Popup_OnTapped(object sender, EventArgs e)
        {
            if (OnPopupTapped != null) OnPopupTapped(sender, e);
        }
    }


    public interface ITextMeter
    {
        double MeasureTextHeightGivenMaxWidth(string text, double width, double fontSize, string fontName = null);
        double MeasureTextWidthGivenExactHeight(string text, double height, double fontSize, string fontName = null);
    }

    public enum AzmMapType
    {
        Normal,
        Satellite,
        Hybrid,
    }
}