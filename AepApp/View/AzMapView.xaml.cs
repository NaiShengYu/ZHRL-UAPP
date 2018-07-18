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
using System.ComponentModel;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzMapView : ContentView
    {
        public delegate void CenterCoordChangedEventHandler(object sender, CenterCoordChangedEventArg e);
        public event CenterCoordChangedEventHandler CenterCoordChanged;

        public static AzMapView ActiveMap { get; set; }

        public static AzmCoord Beijing = new AzmCoord(116.4074, 39.9042);

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
        public AzmOverlayView MarkerPopupView { get; set; }

        public AzMapView()
        {
            InitializeComponent();

            Overlays = new ObservableCollection<AzmOverlayView>();
            ShapeOverlays = new ObservableCollection<AzmOverlayView>();
            //UpdateRanges();
            this.SizeChanged += AzMapView_SizeChanged;
            ActiveMap = this;
            TappedMarker = null;
            MarkerPopupView = null;

            MapType = AzmMapType.Normal;

            SetCenter(8, new AzmCoord(116.4074, 39.9042));  // beijing

            this.BindingContext = this;
        }

        private bool _iszoombuttonvisible = true;

        public bool IsZoomButtonVisible
        {
            get { return _iszoombuttonvisible; }
            set { _iszoombuttonvisible = value; OnPropertyChanged("IsZoomButtonVisible"); }
        }

        private bool _ismaptypebuttonvisible = false;

        public bool IsMapTypeButtonVisible
        {
            get { return _ismaptypebuttonvisible; }
            set { _ismaptypebuttonvisible = value; OnPropertyChanged("IsMapTypeButtonVisible"); }
        }

        private bool _isdebuglabelvisible = false;

        public bool IsDebugLabelVisible
        {
            get { return _isdebuglabelvisible; }
            set { _isdebuglabelvisible = value; OnPropertyChanged("IsDebugLabelVisible"); }
        }

        private AzmCoord _centercoord = new AzmCoord(116.4074, 39.9042);    // bejing

        public AzmCoord CenterCoord
        {
            get { return _centercoord; }
            set { _centercoord = value; OnPropertyChanged("CenterCoord");
                CenterCoordChanged?.Invoke(this, new CenterCoordChangedEventArg(_centercoord));
            }
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
                    Gps g = PositionUtil.gps84_To_Gcj02(ov.Coord.lat, ov.Coord.lng);
                    Point p = GetXYFromCoord(level, new AzmCoord(g.getWgLon(), g.getWgLat()));
                    //Point p = GetXYFromCoord(level, ov.Coord);
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

        private double GetUnitPerMeterFromCoord(int level, AzmCoord coord)
        {
            double n = Math.Pow(2.0, (double)level);
            double ia = (coord.lat + 90.0) / 360.0 * Math.PI;
            double udy = n / 720.0 / Math.Sin(ia) / Math.Cos(ia);
            double lr = 6371000.0 * 2.0 * Math.PI / 360.0; // * Math.Cos(coord.lat / 180.0 * Math.PI);
            return udy / lr;
        }

        public void SetCenter(int _level, AzmCoord _center)
        {
            if (_level < 4) { SetCenter(11, Beijing); return; }
            if (_level > 18) { SetCenter(11, Beijing); return; }
            if (double.IsNaN(_center.lng)) { SetCenter(11, Beijing); return; }
            if (double.IsNaN(_center.lat)) { SetCenter(11, Beijing); return; }
            if (_center.lng < -180.0) { SetCenter(11, Beijing); return; }
            if (_center.lng > 180.0) { SetCenter(11, Beijing); return; }
            if (_center.lat < -85.051128779806592377796715521925) { SetCenter(11, Beijing); return; }
            if (_center.lat > 85.051128779806592377796715521925) { SetCenter(11, Beijing); return; }
            level = _level;
            center = GetXYFromCoord(level, _center);
            CenterCoord = _center;
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

                CenterCoord = GetCoordFromXY(level, center);

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
                        Gps g = PositionUtil.gps84_To_Gcj02(ov.Coord.lat, ov.Coord.lng);
                        Point p = GetXYFromCoord(level, new AzmCoord(g.getWgLon(), g.getWgLat()));
                        //Point p = GetXYFromCoord(level, ov.Coord);
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

        public void ZoomIn()
        {
            zoomin_Clicked(null, null);
        }

        public void ZoomOut()
        {
            zoomout_Clicked(null, null);
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

        private ObservableCollection<AzmOverlayView> shapeoverlays = null;

        public ObservableCollection<AzmOverlayView> ShapeOverlays
        {
            get { return shapeoverlays; }
            set {
                shapeoverlays = value;
            }
        }


        private void Overlay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AzmOverlayView o in e.NewItems)
                {
                    o.Attached(this);

                    o.MapView = this;

                    Gps g = PositionUtil.gps84_To_Gcj02(o.Coord.lat, o.Coord.lng);
                    Point p = GetXYFromCoord(level, new AzmCoord(g.getWgLon(), g.getWgLat()));

                    o.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((p.X - vpminx) * tilesize - o.Anchor.X, (vpmaxy - p.Y) * tilesize - o.Anchor.Y, o.Size.Width, o.Size.Height));
                    o.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                    overlayviews.Children.Add(o);
                }
            }
            if (e.OldItems != null)
            {
                foreach (AzmOverlayView o in e.OldItems)
                {
                    o.Detached();
                    //if (o is AzmMarkerView)
                    //{
                    //    AzmMarkerView mv = o as AzmMarkerView;
                    //    if (mv.Label != null) overlayviews.Children.Remove(mv.Label);
                    //}
                    //overlayviews.Children.Remove(o);
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

            //canvas.Clear(Color.Transparent.ToSKColor());
            foreach(var so in ShapeOverlays)
            {
                Gps g = PositionUtil.gps84_To_Gcj02(so.Coord.lat, so.Coord.lng);
                Point p = GetXYFromCoord(level, new AzmCoord(g.getWgLon(), g.getWgLat()));
                //Point p = GetXYFromCoord(level, so.Coord);
                double upm = GetUnitPerMeterFromCoord(level, so.Coord);
                if (so is AzmEllipseView) {
                    AzmEllipseView ell = so as AzmEllipseView;



                    SKPaint fpaint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = ell.BackgroundColor.ToSKColor(),
                    };



                    canvas.DrawCircle(new SKPoint((float)(p.X - vpminx) * tilesize * (float)xf, (float)(vpmaxy - p.Y) * tilesize * (float)yf), (float)(ell.Radius * upm * tilesize * xf), fpaint);

                    SKPaint spaint = new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = ell.StrokeColor.ToSKColor(),
                        StrokeWidth = (float)ell.StrokeThickness
                    };

                    if (ell.DashArray != null)
                    {
                        spaint.PathEffect = SKPathEffect.CreateDash(ell.DashArray, 0);
                    }

                    canvas.DrawCircle(new SKPoint((float)(p.X - vpminx) * tilesize * (float)xf  , (float)(vpmaxy - p.Y) * tilesize * (float)yf ), (float)(ell.Radius * upm * tilesize * xf), spaint);
                    //canvas.DrawCircle(new SKPoint((float)(p.X - vpminx) * tilesize, (float)(vpmaxy - p.Y) * tilesize), 100, spaint);
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

    public class Gps
    {
        private double wgLat;
        private double wgLon;

        public Gps(double wgLat, double wgLon)
        {
            setWgLat(wgLat);
            setWgLon(wgLon);
        }

        public double getWgLat()
        {
            return wgLat;
        }

        public void setWgLat(double wgLat)
        {
            this.wgLat = wgLat;
        }

        public double getWgLon()
        {
            return wgLon;
        }

        public void setWgLon(double wgLon)
        {
            this.wgLon = wgLon;
        }

        public override string ToString()
        {
            return wgLat + "," + wgLon;
        }
    }


    /**
 * 各地图API坐标系统比较与转换;
 * WGS84坐标系：即地球坐标系，国际上通用的坐标系。设备一般包含GPS芯片或者北斗芯片获取的经纬度为WGS84地理坐标系,
 * 谷歌地图采用的是WGS84地理坐标系（中国范围除外）;
 * GCJ02坐标系：即火星坐标系，是由中国国家测绘局制订的地理信息系统的坐标系统。由WGS84坐标系经加密后的坐标系。
 * 谷歌中国地图和搜搜中国地图采用的是GCJ02地理坐标系; BD09坐标系：即百度坐标系，GCJ02坐标系经加密后的坐标系;
 * 搜狗坐标系、图吧坐标系等，估计也是在GCJ02基础上加密而成的。 chenhua
 */
    public class PositionUtil
    {

        public static string BAIDU_LBS_TYPE = "bd09ll";
	
	    public static double pi = 3.1415926535897932384626;
        public static double a = 6378245.0;
        public static double ee = 0.00669342162296594323;

        /**
         * 84 to 火星坐标系 (GCJ-02) World Geodetic System ==> Mars Geodetic System
         * 
         * @param lat
         * @param lon
         * @return
         */
        public static Gps gps84_To_Gcj02(double lat, double lon)
        {
            if (OutOfChina(lat, lon))
            {
                return null;
            }
            double dLat = transformLat(lon - 105.0, lat - 35.0);
            double dLon = transformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return new Gps(mgLat, mgLon);
        }

        /**
         * * 火星坐标系 (GCJ-02) to 84 * * @param lon * @param lat * @return
         * */
        public static Gps gcj_To_Gps84(double lat, double lon)
        {
            Gps gps = transform(lat, lon);
            double lontitude = lon * 2 - gps.getWgLon();
            double latitude = lat * 2 - gps.getWgLat();
            return new Gps(latitude, lontitude);
        }

        /**
         * 火星坐标系 (GCJ-02) 与百度坐标系 (BD-09) 的转换算法 将 GCJ-02 坐标转换成 BD-09 坐标
         * 
         * @param gg_lat
         * @param gg_lon
         */
        public static Gps gcj02_To_Bd09(double gg_lat, double gg_lon)
        {
            double x = gg_lon, y = gg_lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * pi);
            double bd_lon = z * Math.Cos(theta) + 0.0065;
            double bd_lat = z * Math.Sin(theta) + 0.006;
            return new Gps(bd_lat, bd_lon);
        }

        /**
         * * 火星坐标系 (GCJ-02) 与百度坐标系 (BD-09) 的转换算法 * * 将 BD-09 坐标转换成GCJ-02 坐标 * * @param
         * bd_lat * @param bd_lon * @return
         */
        public static Gps bd09_To_Gcj02(double bd_lat, double bd_lon)
        {
            double x = bd_lon - 0.0065, y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * pi);
            double gg_lon = z * Math.Cos(theta);
            double gg_lat = z * Math.Sin(theta);
            return new Gps(gg_lat, gg_lon);
        }

        /**
         * (BD-09)-->84
         * @param bd_lat
         * @param bd_lon
         * @return
         */
        public static Gps bd09_To_Gps84(double bd_lat, double bd_lon)
        {

            Gps gcj02 = PositionUtil.bd09_To_Gcj02(bd_lat, bd_lon);
            Gps map84 = PositionUtil.gcj_To_Gps84(gcj02.getWgLat(),
                    gcj02.getWgLon());
            return map84;

        }

        public static bool OutOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        public static Gps transform(double lat, double lon)
        {
            if (OutOfChina(lat, lon))
            {
                return new Gps(lat, lon);
            }
            double dLat = transformLat(lon - 105.0, lat - 35.0);
            double dLon = transformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return new Gps(mgLat, mgLon);
        }

        public static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        public static double transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }

        //public static void main(String[] args)
        //{

        //    // 北斗芯片获取的经纬度为WGS84地理坐标 31.426896,119.496145
        //    Gps gps = new Gps(31.426896, 119.496145);
        //    System.out.println("gps :" + gps);
        //    Gps gcj = gps84_To_Gcj02(gps.getWgLat(), gps.getWgLon());
        //    System.out.println("gcj :" + gcj);
        //    Gps star = gcj_To_Gps84(gcj.getWgLat(), gcj.getWgLon());
        //    System.out.println("star:" + star);
        //    Gps bd = gcj02_To_Bd09(gcj.getWgLat(), gcj.getWgLon());
        //    System.out.println("bd  :" + bd);
        //    Gps gcj2 = bd09_To_Gcj02(bd.getWgLat(), bd.getWgLon());
        //    System.out.println("gcj :" + gcj2);
        //}
    }





    public class CenterCoordChangedEventArg : EventArgs
    {
        public AzmCoord Center { get; set; }

        public CenterCoordChangedEventArg(AzmCoord _center)
        {
            Center = _center;
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
                            //北京地铁站 
                        case 196634: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196635: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196636: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196637: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196638: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196639: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196640: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196641: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196642: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196643: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196644: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196645: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196646: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196647: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196648: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196649: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196650: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196765: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196801: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196803: col = Color.FromHex("#84bcf8").ToSKColor(); break;
                        case 196804: col = Color.FromHex("#84bcf8").ToSKColor(); break;

                            //天津
                        case 196703: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196704: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196705: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196706: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196758: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //厦门
                        case 196809: col = Color.FromHex("#6089b5").ToSKColor(); break;
                            //贵阳
                        case 196782: col = Color.FromHex("#6089b5").ToSKColor(); break;

                            //石家庄
                        case 196781: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196780: col = Color.FromHex("#afd475").ToSKColor(); break;

                            //合肥
                        case 196790: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196773: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //南宁
                        case 196755: col = Color.FromHex("#f7715d").ToSKColor(); break;
                        case 196800: col = Color.FromHex("#f7715d").ToSKColor(); break;

                            //东莞
                        case 196753: col = Color.FromHex("#afd475").ToSKColor(); break;

                            //福州
                        case 196756: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //南昌
                        case 196787: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196748: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //长春
                        case 196724: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196783: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196725: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //大连
                        case 196743: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196745: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196687: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196746: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196686: col = Color.FromHex("#6a77aa").ToSKColor(); break;

                            //无锡
                        case 196735: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196740: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //长沙
                        case 196759: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196757: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196732: col = Color.FromHex("#6a77aa").ToSKColor(); break;

                            //郑州
                        case 196761: col = Color.FromHex("#bbb378").ToSKColor(); break;
                        case 196726: col = Color.FromHex("#bbb378").ToSKColor(); break;
                        case 196776: col = Color.FromHex("#bbb378").ToSKColor(); break;

                            //青岛
                        case 196747: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196793: col = Color.FromHex("#afd475").ToSKColor(); break;

                            //哈尔滨
                        case 196691: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196767: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //杭州
                        case 196692: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196739: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196741: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //昆明
                        case 196733: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196788: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196694: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196775: col = Color.FromHex("#afd475").ToSKColor(); break;
                        case 196695: col = Color.FromHex("#afd475").ToSKColor(); break;

                            //苏州
                        case 196778: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196779: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196702: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196701: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //西安
                        case 196762: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196710: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196711: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //重庆
                        case 196729: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196728: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196727: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196730: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196807: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196806: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196731: col = Color.FromHex("#60a656").ToSKColor(); break;

                            //沈阳
                        case 196700: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196699: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //成都
                        case 196684: col = Color.FromHex("#5fa1c8").ToSKColor(); break;
                        case 196685: col = Color.FromHex("#5fa1c8").ToSKColor(); break;
                        case 196749: col = Color.FromHex("#5fa1c8").ToSKColor(); break;
                        case 196760: col = Color.FromHex("#5fa1c8").ToSKColor(); break;
                        case 196789: col = Color.FromHex("#5fa1c8").ToSKColor(); break;
                        case 196792: col = Color.FromHex("#5fa1c8").ToSKColor(); break;

                            //南京
                        case 196738: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196768: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196697: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196698: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196742: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196696: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196791: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196736: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196805: col = Color.FromHex("#d75a5d").ToSKColor(); break;
                        case 196737: col = Color.FromHex("#d75a5d").ToSKColor(); break;

                            //深圳
                        case 196679: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196681: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196763: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196683: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196682: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196754: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196764: col = Color.FromHex("#60a656").ToSKColor(); break;
                        case 196680: col = Color.FromHex("#60a656").ToSKColor(); break;
                            
                            //武汉
                        case 196708: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196707: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196772: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196709: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196798: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196750: col = Color.FromHex("#6a77aa").ToSKColor(); break;
                        case 196799: col = Color.FromHex("#6a77aa").ToSKColor(); break;

                            //广州
                        case 196657: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196658: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196653: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196652: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196660: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196690: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196654: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196659: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196655: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196656: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196797: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196796: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196766: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196651: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196795: col = Color.FromHex("#d05755").ToSKColor(); break;

                            //上海背景色
                        case 196672: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196664: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196677: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196675: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196676: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196673: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196678: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196662: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196663: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196670: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196671: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196666: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196810: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196668: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196661: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196794: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196667: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196674: col = Color.FromHex("#d05755").ToSKColor(); break;
                        case 196669: col = Color.FromHex("#d05755").ToSKColor(); break;

                           


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
                        case 65566: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 10; break;
                        case 65567: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 11; break;
                        case 65568: col = Color.FromHex("#797979").ToSKColor(); icon = 12; break;
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
                        case 65596: col = Color.FromHex("#797979").ToSKColor(); icon = 40; break;
                        case 65597: col = Color.FromHex("#797979").ToSKColor(); icon = 41; break;
                        case 65598: col = Color.FromHex("#797979").ToSKColor(); icon = 42; break;
                        case 65599: col = Color.FromHex("#797979").ToSKColor(); icon = 43; break;
                        case 65600: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 65601: col = Color.FromHex("#8a8a88").ToSKColor(); break;
                        case 65602: col = Color.FromHex("#797979").ToSKColor(); icon = 46; break;
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
                        case 65613: col = Color.FromHex("#000").ToSKColor(); icon = 58; break;
                        case 65614: col = Color.FromHex("#000").ToSKColor(); icon = 59; break;
                        case 65615: col = Color.FromHex("#000").ToSKColor(); icon = 60; break;
                        case 65616: col = Color.FromHex("#000").ToSKColor(); icon = 61; break;
                        case 65617: col = Color.FromHex("#797979").ToSKColor(); icon = 62; break;
                        case 65618: col = Color.FromHex("#797979").ToSKColor(); icon = 63; break;
                        case 65619: col = Color.FromHex("#797979").ToSKColor(); icon = 64; break;
                        case 65620: col = Color.FromHex("#797979").ToSKColor(); icon = 65; break;
                        case 65622: col = Color.FromHex("#797979").ToSKColor(); icon = 66; break;
                        case 65623: col = Color.FromHex("#797979").ToSKColor(); icon = 67; break;
                        case 65624: col = Color.FromHex("#797979").ToSKColor(); icon = 68; break;
                        case 65625: col = Color.FromHex("#797979").ToSKColor(); icon = 160; break;
                        case 65626: col = Color.FromHex("#797979").ToSKColor(); icon = 69; break;
                        case 65627: col = Color.FromHex("#797979").ToSKColor(); icon = 70; break;     // subway station name
                        case 65628: col = Color.FromHex("#797979").ToSKColor(); icon = 71; break;
                        case 65629: col = Color.FromHex("#797979").ToSKColor(); icon = 72; break;
                        case 65630: col = Color.FromHex("#797979").ToSKColor(); icon = 74; break;
                        case 65631: col = Color.FromHex("#000").ToSKColor(); icon = 73; break;     // atm
                        case 65632: col = Color.FromHex("#797979").ToSKColor(); break;     // subway station name
                        case 65869: col = Color.FromHex("#797979").ToSKColor();textsize = 8; break;   
                        case 66064: col = Color.FromHex("#797979").ToSKColor();icon = 341;textsize = 8; break;   
                        case 66111: col = Color.FromHex("#C2C2C2").ToSKColor(); textsize = 8; break;
                        case 66114: col = Color.FromHex("#C2C2C2").ToSKColor(); textsize = 8; break;
                        case 66115: col = Color.FromHex("#C2C2C2").ToSKColor(); textsize = 12; break;

                            // 北京站点名称字体颜色
                        case 65634: col = Color.FromHex("#000000").ToSKColor(); break;     
                        case 65635: col = Color.FromHex("#000000").ToSKColor(); break; 
                            // 天津站点名称字体颜色    
                        case 65664: col = Color.FromHex("#000000").ToSKColor(); break;    
                        case 65665: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //厦门站点名称字体颜色
                        case 66102: col = Color.FromHex("#000000").ToSKColor(); break;   
                            //贵阳
                        case 66045: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //石家庄
                        case 66038: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 66039: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //合肥
                        case 65896: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65895: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //南宁
                        case 65696: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65697: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //东莞
                        case 65695: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //福州
                        case 65699: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //南昌
                        case 65692: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65691: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //长春
                        case 65646: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65647: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65871: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //大连
                        case 65650: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65651: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //无锡
                        case 65682: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65683: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //长沙
                        case 65676: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65677: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //郑州站点
                        //case 65672: col = Color.FromHex("#000000").ToSKColor();icon = 94; break;   
                        //case 65673: col = Color.FromHex("#000000").ToSKColor();icon = 94; break;   
                        //case 65883: col = Color.FromHex("#000000").ToSKColor();icon = 94; break;   
                        case 65672: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65673: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65883: col = Color.FromHex("#000000").ToSKColor(); break;   


                            //青岛
                        case 65690: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65689: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //哈尔滨
                        case 65674: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65675: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //杭州
                        case 65658: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65659: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //昆明
                        case 65656: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65657: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //苏州
                        case 65662: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65663: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //西安
                        case 65652: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65653: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //重庆
                        case 65666: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65667: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //沈阳
                        case 65648: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65649: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //成都
                        case 65654: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65655: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //高雄
                        case 66065: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //南京
                        case 65636: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65637: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //深圳
                        case 65638: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65639: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //武汉
                        case 65644: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65645: col = Color.FromHex("#000000").ToSKColor(); break;   

                            //广州站点
                        case 65640: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65641: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65642: col = Color.FromHex("#000000").ToSKColor(); break;   
                        case 65643: col = Color.FromHex("#000000").ToSKColor(); break;  

                            //上海
                        case 65660: col = Color.FromHex("#000000").ToSKColor(); break;  
                        case 65661: col = Color.FromHex("#000000").ToSKColor(); break;  



                        case 65668: col = Color.FromHex("#333333").ToSKColor(); icon = 113; break;     // hong kong subway station name
                        case 65669: col = Color.FromHex("#333333").ToSKColor(); icon = 113; break;     // hong kong subway station name
                        case 65680: col = Color.FromHex("#333333").ToSKColor(); icon = 121; break;     // ningbo subway station name
                        case 65681: col = Color.FromHex("#0f0").ToSKColor(); break;
                        case 65684: col = Color.FromHex("#00f").ToSKColor(); textsize = 20; break;
                        case 65686: col = Color.FromHex("#8a8a88").ToSKColor(); textsize = 15; break;
                        case 65687: col = Color.FromHex("#a8bb9e").ToSKColor(); textsize = 15; break;
                        case 65688: col = Color.FromHex("#b9b7a0").ToSKColor(); textsize = 15; break;
                        case 65693: col = Color.FromHex("#8a8a88").ToSKColor(); textsize = 10; break;
                        case 65700: col = Color.FromHex("#000").ToSKColor(); icon = 165; break;
                        case 65701: col = Color.FromHex("#000").ToSKColor(); icon = 166; break;
                        case 65702: col = Color.FromHex("#000").ToSKColor(); icon = 167; break;
                        case 65703: col = Color.FromHex("#000").ToSKColor(); icon = 168; break;
                        case 65704: col = Color.FromHex("#797979").ToSKColor(); icon = 169; break;
                        case 65705: col = Color.FromHex("#797979").ToSKColor(); icon = 170; break;
                        case 65706: col = Color.FromHex("#797979").ToSKColor(); icon = 171; break;
                        case 65707: col = Color.FromHex("#797979").ToSKColor(); icon = 172; break;
                        case 65708: col = Color.FromHex("#000").ToSKColor(); icon = 61; break;
                        case 65709: col = Color.FromHex("#000").ToSKColor(); icon = 160; break;
                        case 65710: col = Color.FromHex("#797979").ToSKColor(); icon = 32; break;
                        case 65711: col = Color.FromHex("#797979").ToSKColor(); icon = 176; break;
                        case 65712: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65713: col = Color.FromHex("#797979").ToSKColor(); icon = 28; break;
                        case 65714: col = Color.FromHex("#797979").ToSKColor(); icon = 5; break;
                        case 65715: col = Color.FromHex("#797979").ToSKColor(); icon = 18; break;
                        case 65716: col = Color.FromHex("#8a8a88").ToSKColor(); icon = 45; break;
                        case 65717: col = Color.FromHex("#b04e48").ToSKColor(); icon = 93; break;
                        case 65741: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 53; break;
                        case 65742: col = Color.FromHex("#5f6f97").ToSKColor(); break;
                        case 65743: col = Color.FromHex("#8b7de7").ToSKColor(); break;
                        case 65744: col = Color.FromHex("#5f6f97").ToSKColor(); icon = 124; break;

                        case 65755: col = Color.FromHex("#b8a790").ToSKColor(); textsize = 15; break;
                        case 65756: col = Color.FromHex("#caa1a2").ToSKColor(); textsize = 15; break;
                        case 65757: col = Color.FromHex("#bdbcbc").ToSKColor(); textsize = 15; break;
                        case 65877: col = Color.FromHex("#bdbcbc").ToSKColor(); textsize = 8; break;

                     
                        case 65836: col = Color.FromHex("#84bcf8").ToSKColor(); icon = 300; break;//北京出站口
                        case 65851: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//天津出站口
                        case 65864: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//天津出站口
                        case 65853: col = Color.FromHex("#bc023c").ToSKColor(); icon = 309; break;//香港
                        case 65857: col = Color.FromHex("#006fc1").ToSKColor(); icon = 307; break;//宁波
                        case 66104: col = Color.FromHex("#6089b5").ToSKColor(); icon = 307; break;//厦门
                        case 66047: col = Color.FromHex("#6a77aa").ToSKColor(); icon = 304; break;//贵阳 
                        case 66040: col = Color.FromHex("#afd475").ToSKColor(); icon = 303; break;//石家庄
                            
                        case 65893: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//合肥
                       
                        case 65862: col = Color.FromHex("#f7715d").ToSKColor(); icon = 301; break;//南宁

                        case 65861: col = Color.FromHex("#afd475").ToSKColor(); icon = 303; break;//东莞出站口

                        case 65863: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//福州出站口

                        case 65860: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//南昌出站口

                        case 65842: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//长春出站口

                        case 65844: col = Color.FromHex("#6a77aa").ToSKColor(); icon = 304; break;//大连出站口

                        case 65858: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//无锡出站口

                        case 65856: col = Color.FromHex("#6a77aa").ToSKColor(); icon = 304; break;//长沙出站口

                        case 65854: col = Color.FromHex("#bbb378").ToSKColor(); icon = 305; break;//郑州出站口

                        case 65859: col = Color.FromHex("#afd475").ToSKColor(); icon = 303; break;//青岛出站口
                       
                        case 65855: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//哈尔滨出站口

                        case 65848: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//杭州出站口

                        case 65847: col = Color.FromHex("#afd475").ToSKColor(); icon = 303; break;//昆明出站口

                        case 65850: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//苏州出站口

                        case 65845: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//西安出站口

                        case 65852: col = Color.FromHex("#60a656").ToSKColor(); icon = 302; break;//重庆出站口
                      
                        case 65843: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//沈阳出站口

                        case 65846: col = Color.FromHex("#5fa1c8").ToSKColor(); icon = 307; break;//成都出站口

                        case 65837: col = Color.FromHex("#d75a5d").ToSKColor(); icon = 306; break;//南京出站口

                        case 65838: col = Color.FromHex("#60a656").ToSKColor(); icon = 302; break;//深圳出站口

                        case 65841: col = Color.FromHex("#6a77aa").ToSKColor(); icon = 304; break;//武汉出站口

                        case 65839: col = Color.FromHex("#d05755").ToSKColor(); icon = 306; break;//广州出站口
                        case 65840: col = Color.FromHex("#d05755").ToSKColor(); icon = 306; break;//广州出站口

                        case 65849: col = Color.FromHex("#d05755").ToSKColor(); icon = 306; break;//上海出站口


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




                            //宁波铁路线lab颜色
                        case 65998: col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#6da2d9").ToSKColor(); break;
                        case 66008: col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#e17a7e").ToSKColor(); break;
                                                       
                            //北京地铁路线lab颜色
                        case 65901:
                            col = Color.FromHex("#fff").ToSKColor();            // 北京地铁1号线（1971年01月15日）三轨 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65902:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 65903:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#77d0ce").ToSKColor(); break;
                        case 65904:
                            col = Color.FromHex("#fff").ToSKColor();            // 5 line label
                            backcol = Color.FromHex("#bb6d9c").ToSKColor(); break;
                        case 65905:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 65913:
                            col = Color.FromHex("#fff").ToSKColor();            // 7 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 65906:
                            col = Color.FromHex("#fff").ToSKColor();            // 8 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 65907:
                            col = Color.FromHex("#fff").ToSKColor();            // 9 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65897:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 65898:
                            col = Color.FromHex("#fff").ToSKColor();            // 13 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 65899:
                            col = Color.FromHex("#fff").ToSKColor();            // 14 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;
                        case 65900:
                            col = Color.FromHex("#fff").ToSKColor();            // 15 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                        case 65914:
                            col = Color.FromHex("#fff").ToSKColor();            // 16 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65912:
                            col = Color.FromHex("#fff").ToSKColor();            // 机场 line label
                            backcol = Color.FromHex("#a193bb").ToSKColor(); break;
                        case 65911:
                            col = Color.FromHex("#fff").ToSKColor();            // 亦庄 line label
                            backcol = Color.FromHex("#dd91bf").ToSKColor(); break;
                        case 65910:
                            col = Color.FromHex("#fff").ToSKColor();            //房山 line label
                            backcol = Color.FromHex("#ea9378").ToSKColor(); break;
                        case 65909:
                            col = Color.FromHex("#fff").ToSKColor();            //昌平 line label
                            backcol = Color.FromHex("#f4b1d2").ToSKColor(); break;
                        case 66093:
                            col = Color.FromHex("#fff").ToSKColor();            // 燕房line label
                            backcol = Color.FromHex("#e8855a").ToSKColor(); break;
                        case 66096:
                            col = Color.FromHex("#fff").ToSKColor();            // 西郊 line label
                            backcol = Color.FromHex("#e77275").ToSKColor(); break;
                        case 66095:
                            col = Color.FromHex("#fff").ToSKColor();            // s1 line label
                            backcol = Color.FromHex("#e66b72").ToSKColor(); break;
                      
                            //天津
                        case 65967:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65968:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65969:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#77d0ce").ToSKColor(); break;
                        case 66020:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#bb6d9c").ToSKColor(); break;
                        case 65970:
                            col = Color.FromHex("#fff").ToSKColor();            // 9 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;

                            //台北
                        case 66075:
                            col = Color.FromHex("#fff").ToSKColor();            // 文湖线 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;
                        case 66072:
                            col = Color.FromHex("#fff").ToSKColor();            // 淡水信义线（1997年） line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66073:
                            col = Color.FromHex("#fff").ToSKColor();            //松山新店线（2000年）line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 66074:
                            col = Color.FromHex("#fff").ToSKColor();            //松山新店线（2000年）line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 66078:
                            col = Color.FromHex("#fff").ToSKColor();            // 中和新芦线（2002年） line label
                            backcol = Color.FromHex("#f3c36d").ToSKColor(); break;
                        case 66079:
                            col = Color.FromHex("#fff").ToSKColor();            // 中和新芦线（2002年） line label
                            backcol = Color.FromHex("#f3c36d").ToSKColor(); break;
                        case 66071:
                            col = Color.FromHex("#fff").ToSKColor();            // 板南线（2005年） line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 66080:
                            col = Color.FromHex("#fff").ToSKColor();            // 板南线（2005年） line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 66077:
                            col = Color.FromHex("#fff").ToSKColor();            //新北投线（2010年） line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 66076:
                            col = Color.FromHex("#fff").ToSKColor();            // 小碧潭线（2004年）line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 66070:
                            col = Color.FromHex("#fff").ToSKColor();            // 猫空缆车 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;

                            //上海
                        case 65948:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65949:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65950:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break; 
                        case 65951:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 65952:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                        case 65953:
                            col = Color.FromHex("#fff").ToSKColor();            // 5 line label
                            backcol = Color.FromHex("#a193bb").ToSKColor(); break;
                        case 65954:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65955:
                            col = Color.FromHex("#fff").ToSKColor();            // 7 line label
                            backcol = Color.FromHex("#ea9378").ToSKColor(); break;
                        case 65956:
                            col = Color.FromHex("#fff").ToSKColor();            // 8 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 65957:
                            col = Color.FromHex("#fff").ToSKColor();            // 9 line label
                            backcol = Color.FromHex("#8dc4de").ToSKColor(); break;
                        case 65941:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;
                        case 65942:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;
                        case 65944:
                            col = Color.FromHex("#fff").ToSKColor();            // 11 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65943:
                            col = Color.FromHex("#fff").ToSKColor();            // 11 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65945:
                            col = Color.FromHex("#fff").ToSKColor();            // 12 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 65946:
                            col = Color.FromHex("#fff").ToSKColor();            // 13 line label
                            backcol = Color.FromHex("#f4b1d2").ToSKColor(); break;
                        case 65947:
                            col = Color.FromHex("#fff").ToSKColor();            // 16 line label
                            backcol = Color.FromHex("#a0d7c5").ToSKColor(); break;
                        case 66086:
                            col = Color.FromHex("#fff").ToSKColor();            // 17 line label
                            backcol = Color.FromHex("#e8855a").ToSKColor(); break;
                        case 66106:
                            col = Color.FromHex("#fff").ToSKColor();            // 浦江线 line label
                            backcol = Color.FromHex("#74998e").ToSKColor(); break;
                        case 65940:
                            col = Color.FromHex("#fff").ToSKColor();            // 磁悬浮 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                    
                            //广州
                        case 65923:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;
                        case 65924:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 65925:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#f1aa7f").ToSKColor(); break;
                        case 65926:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#f1aa7f").ToSKColor(); break;
                        case 65927:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 65928:
                            col = Color.FromHex("#fff").ToSKColor();            // 5 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65929:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                        case 66027:
                            col = Color.FromHex("#fff").ToSKColor();            // 7 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65930:
                            col = Color.FromHex("#fff").ToSKColor();            // 8 line label
                            backcol = Color.FromHex("#77d0ce").ToSKColor(); break;
                        case 66087:
                            col = Color.FromHex("#fff").ToSKColor();            // 9 line label
                            backcol = Color.FromHex("#31c3c2").ToSKColor(); break;
                        case 66089:
                            col = Color.FromHex("#fff").ToSKColor();            // 13 line label
                            backcol = Color.FromHex("#a4be2c").ToSKColor(); break;
                        case 66088:
                            col = Color.FromHex("#fff").ToSKColor();            // 14 line label
                            backcol = Color.FromHex("#e66b72").ToSKColor(); break;
                        case 65922:
                            col = Color.FromHex("#fff").ToSKColor();            // APM线 line label
                            backcol = Color.FromHex("#79c2f6").ToSKColor(); break;
                        case 65931:
                            col = Color.FromHex("#fff").ToSKColor();            // 广佛线 line label
                            backcol = Color.FromHex("#bfcb73").ToSKColor(); break;
                        case 65921:
                            col = Color.FromHex("#fff").ToSKColor();            // 广佛线 line label
                            backcol = Color.FromHex("#bfcb73").ToSKColor(); break;


                            //武汉
                        case 65973:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 65971:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#f4b1d2").ToSKColor(); break;
                        case 66013:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#bda280").ToSKColor(); break;
                        case 65972:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 66030:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#a0d7c5").ToSKColor(); break;
                        case 66091:
                            col = Color.FromHex("#fff").ToSKColor();            // 8 line label
                            backcol = Color.FromHex("#a6d5ee").ToSKColor(); break;
                        case 66090:
                            col = Color.FromHex("#fff").ToSKColor();            // 21 line label
                            backcol = Color.FromHex("#cc4f9d").ToSKColor(); break;

                            //深圳
                        case 65961:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 65962:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#ea9378").ToSKColor(); break;
                        case 65959:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 65960:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65958:
                            col = Color.FromHex("#fff").ToSKColor();            // 5 line label
                            backcol = Color.FromHex("#a193bb").ToSKColor(); break;
                        case 66025:
                            col = Color.FromHex("#fff").ToSKColor();            // 7 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 66026:
                            col = Color.FromHex("#fff").ToSKColor();            // 9 line label
                            backcol = Color.FromHex("#d7a49d").ToSKColor(); break;
                        case 66016:
                            col = Color.FromHex("#fff").ToSKColor();            // 11 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                
                            //南京
                        case 65938:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 65939:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66006:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 66029:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#dd91bf").ToSKColor(); break;
                        case 66000:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#f3a779").ToSKColor(); break;
                        case 66001:
                            col = Color.FromHex("#fff").ToSKColor();            // s1 line label
                            backcol = Color.FromHex("#77d0ce").ToSKColor(); break;
                        case 66083:
                            col = Color.FromHex("#fff").ToSKColor();            // s3 line label
                            backcol = Color.FromHex("#cc4f9d").ToSKColor(); break;
                        case 66002:
                            col = Color.FromHex("#fff").ToSKColor();            // s8 line label
                            backcol = Color.FromHex("#ea9378").ToSKColor(); break;
                        case 66097:
                            col = Color.FromHex("#fff").ToSKColor();            // s9 line label
                            backcol = Color.FromHex("#f2be47").ToSKColor(); break;

                            //高雄
                        case 66067:
                            col = Color.FromHex("#fff").ToSKColor();            //红线（2008年）line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66068:
                            col = Color.FromHex("#fff").ToSKColor();            // 环状轻轨 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66069:
                            col = Color.FromHex("#fff").ToSKColor();            // 橙线（2015年） line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;

                            //成都
                        case 66116:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                        case 65916:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;
                        case 66022:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66014:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#86bf80").ToSKColor(); break;
                        case 66084:
                            col = Color.FromHex("#fff").ToSKColor();            //7 line label
                            backcol = Color.FromHex("#a6d5ee").ToSKColor(); break;
                        case 66063:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#4a8ed2").ToSKColor(); break;
                    
                            //沈阳
                        case 65963:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65964:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#f3a779").ToSKColor(); break;

                            //重庆
                        case 65991:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65992:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;
                        case 65993:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 66032:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 66098:
                            col = Color.FromHex("#fff").ToSKColor();            // 5 line label
                            backcol = Color.FromHex("#55b4fa").ToSKColor(); break;
                        case 65994:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#dd91bf").ToSKColor(); break;
                        case 65995:
                            col = Color.FromHex("#fff").ToSKColor();            // 国博 line label
                            backcol = Color.FromHex("#dd91bf").ToSKColor(); break;
                        case 66099:
                            col = Color.FromHex("#fff").ToSKColor();            // 10 line label
                            backcol = Color.FromHex("#7e478f").ToSKColor(); break;

                            //西安
                        case 65975:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#de8287").ToSKColor(); break;
                        case 65974:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;
                        case 66024:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#997e9e").ToSKColor(); break;

                            //苏州
                        case 65965:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#bcca6b").ToSKColor(); break;
                        case 65966:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66036:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#55b4fa").ToSKColor(); break;
                        case 66037:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#55b4fa").ToSKColor(); break;

                            //昆明
                        case 65935:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66033:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65997:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#6fa3d7").ToSKColor(); break;
                        case 66062:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#8b7aae").ToSKColor(); break;
                        case 65936:
                            col = Color.FromHex("#fff").ToSKColor();            // 6 line label
                            backcol = Color.FromHex("#77c1f6").ToSKColor(); break;

                            //杭州
                        case 65934:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 65933:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66003:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#f4aa7c").ToSKColor(); break;
                        case 66005:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#76bb72").ToSKColor(); break;

                            //哈尔滨
                        case 65932:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7b80").ToSKColor(); break;
                        case 66028:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#f4c36a").ToSKColor(); break;

                            //  青岛
                        case 66011:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#77d0ce").ToSKColor(); break;
                        case 66085:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#e66b72").ToSKColor(); break;
                        case 66117:
                            col = Color.FromHex("#fff").ToSKColor();            // 11 line label
                            backcol = Color.FromHex("#4a8ed2").ToSKColor(); break;

                            //郑州
                        case 65990:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                        case 66023:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#f3c56a").ToSKColor(); break;
                        case 66034:
                            col = Color.FromHex("#fff").ToSKColor();            // 城郊 line label
                            backcol = Color.FromHex("#cd469d").ToSKColor(); break;

                            //长沙
                        case 65996:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#7bc1f6").ToSKColor(); break;
                        case 66019:
                            col = Color.FromHex("#fff").ToSKColor();            // 磁悬浮 line label
                            backcol = Color.FromHex("#f5aed2").ToSKColor(); break;
                        case 66021:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;

                            //无锡
                        case 65999:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                        case 66004:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#73be71").ToSKColor(); break;

                            //大连
                        case 65919:
                            col = Color.FromHex("#fff").ToSKColor();            // 九里 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                        case 66010:
                            col = Color.FromHex("#fff").ToSKColor();            // 12 line label
                            backcol = Color.FromHex("#73a3d7").ToSKColor(); break;
                        case 66007:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#73a3d7").ToSKColor(); break;
                        case 66009:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#bccd6b").ToSKColor(); break;
                        case 65918:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                        case 65920:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                        case 65917:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;

                            //长春
                        case 65988:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#d7a59e").ToSKColor(); break;
                        case 65989:
                            col = Color.FromHex("#fff").ToSKColor();            // 4 line label
                            backcol = Color.FromHex("#74bf72").ToSKColor(); break;
                        case 66049:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#e66973").ToSKColor(); break;

                            //南昌
                        case 66012:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7a80").ToSKColor(); break;
                        case 66061:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#f1c146").ToSKColor(); break;

                            //福州
                        case 66018:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7a80").ToSKColor(); break;

                            //东莞
                        case 66015:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#de797f").ToSKColor(); break;
                            //南宁
                        case 66017:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#74bf72").ToSKColor(); break;
                        case 66092:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#f55f64").ToSKColor(); break;

                            //合肥
                        case 66031:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#de7a80").ToSKColor(); break;
                        case 66082:
                            col = Color.FromHex("#fff").ToSKColor();            // 2 line label
                            backcol = Color.FromHex("#518dd3").ToSKColor(); break;

                            //石家庄
                        case 66042:
                            col = Color.FromHex("#fff").ToSKColor();            // 1 line label
                            backcol = Color.FromHex("#e7855c").ToSKColor(); break;
                        case 66043:
                            col = Color.FromHex("#fff").ToSKColor();            // 3 line label
                            backcol = Color.FromHex("#5cb3fb").ToSKColor(); break;


                            //贵阳铁路线lab颜色
                        case 66048:
                            col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#a3c12a").ToSKColor(); break;

                        //厦门铁路线lab颜色
                        case 66105:
                            col = Color.FromHex("#fff").ToSKColor();            // subway line label
                            backcol = Color.FromHex("#e7855c").ToSKColor(); break;

                        case 22179:  col = Color.FromHex("#5a5a5a").ToSKColor(); break;    // road name in strange key range
                        case 262162: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262163: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262164: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262165: col = Color.FromHex("#fff").ToSKColor();           // highway number, S
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262168:
                            col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262169: col = Color.FromHex("#fff").ToSKColor();           // highway number, G
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262170: col = Color.FromHex("#fff").ToSKColor();           // highway number, S
                            backcol = Color.FromHex("#50bb25").ToSKColor(); break;
                        case 262166: col = Color.FromHex("#955410").ToSKColor(); break;
                        case 262167: col = Color.FromHex("#000").ToSKColor();
                            backcol = Color.FromHex("#f8c144").ToSKColor(); break;
                        case 262173: col = Color.FromHex("#704d10").ToSKColor(); break;
                        case 262176: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 262178: col = Color.FromHex("#927745").ToSKColor(); break;
                        case 262179: col = Color.FromHex("#393939").ToSKColor(); break;
                        case 262180: col = Color.FromHex("#777776").ToSKColor(); break;
                        case 262181: col = Color.FromHex("#797979").ToSKColor(); break;
                        case 262182: col = Color.FromHex("#858483").ToSKColor(); break;
                        case 262185: col = Color.FromHex("#858483").ToSKColor(); break;
                        case 262186: col = Color.FromHex("#858483").ToSKColor(); break;
                        case 262187: col = Color.FromHex("#858483").ToSKColor(); break;
                        case 262188: col = Color.FromHex("#ffffff").ToSKColor(); break;
                        case 262189: col = Color.FromHex("#000000").ToSKColor(); textsize = 10; break;

                        case 262412: col = Color.FromHex("#000000").ToSKColor(); textsize = 8; break;
                        case 262413: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262414: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262415: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262416: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262417: col = Color.FromHex("#77561a").ToSKColor(); textsize = 8; break;
                        case 262418: col = Color.FromHex("#77561a").ToSKColor(); textsize = 8; break;
                        case 262419: col = Color.FromHex("#605f5d").ToSKColor(); textsize = 8; break;
                        case 262420: col = Color.FromHex("#4f4f4e").ToSKColor(); break;
                        case 262421: col = Color.FromHex("#4a4a49").ToSKColor(); break;
                        case 262422: col = Color.FromHex("#595957").ToSKColor(); textsize = 8; break;
                        case 262423: col = Color.FromHex("#595957").ToSKColor(); textsize = 10; break;
                        case 262424: col = Color.FromHex("#f88").ToSKColor(); textsize = 20; break;
                        case 262425: col = Color.FromHex("#a77137").ToSKColor(); break;
                        case 262705: col = Color.FromHex("#8ebfe4").ToSKColor(); break;
                        case 262714: col = Color.FromHex("#595957").ToSKColor(); break;
                        case 262715: col = Color.FromHex("#595957").ToSKColor(); break;
                        case 262782: col = Color.FromHex("#a34b15").ToSKColor(); textsize = 10;break;
                        case 262789: col = Color.FromHex("#a34b15").ToSKColor(); textsize = 10; break;
                        case 262797: col = Color.FromHex("#8ebfe4").ToSKColor(); textsize = 8; break;

                            
                        default: col = Color.Magenta.ToSKColor(); suf = " " + t.key.ToString();
                            Console.WriteLine("%%%%%%% " + t.key);
                            break;
                    }

                    List<int> subwayexitids = new List<int>(){
                        65836,
                        65851,
                        65864,
                        65853,
                        65857,
                        66104,
                        66047,
                        66040,
                        65893,
                        65862,
                        65861,
                        65863,
                        65860,
                        65842,
                        65844,
                        65858,
                        65856,
                        65854,
                        65859,
                        65855,
                        65848,
                        65847,
                        65850,
                        65845,
                        65852,
                        65843,
                        65846,
                        65837,
                        65838,
                        65841,
                        65839,
                        65840,
                        65849,
                    };

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
                                
                            //设置出站口文字
                            if(subwayexitids.Contains(t.key)){
                                for (int i = 0; i < t.strings.Count; i++)
                                {
                                    float tw = textpaint.MeasureText(t.strings[i]);
                                    textpaint.Style = SKPaintStyle.Stroke;
                                    textpaint.StrokeWidth = 1.0f;
                                    can.DrawText(t.strings[i] + suf, -tw / 2, textpaint.TextSize / 2-1, textpaint);
                                }
                               


                            }
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


                        case 131397: col = Color.FromHex("#f8e08b").ToSKColor(); width = 2.0f; break;


                        // ningbo subway lines
                        case 131217: col = Color.FromHex("#0c4e95").ToSKColor(); width = 2.0f; break;
                        case 131327: col = Color.FromHex("#970810").ToSKColor(); width = 2.0f; break;

                 
                            //北京
                        case 131129: col = Color.FromHex("#aa7791").ToSKColor(); width = 2.0f; break;//昌平
                        case 131368: col = Color.FromHex("#909f42").ToSKColor(); width = 2.0f; break;//16
                        case 131119: col = Color.FromHex("#d7a49d").ToSKColor(); width = 2.0f; break;//14
                        case 131413: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//s1
                        case 131121: col = Color.FromHex("#ad454b").ToSKColor(); width = 2.0f; break;//1
                        case 131414: col = Color.FromHex("#941018").ToSKColor(); width = 2.0f; break;//西郊
                        case 131126: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//8
                        case 131118: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//13
                        case 131124: col = Color.FromHex("#782057").ToSKColor(); width = 2.0f; break;//5
                        case 131120: col = Color.FromHex("#42234c").ToSKColor(); width = 2.0f; break;//15
                        case 131132: col = Color.FromHex("#7a6e94").ToSKColor(); width = 2.0f; break;//机场
                        case 131125: col = Color.FromHex("#ba6f39").ToSKColor(); width = 2.0f; break;//6
                        case 131128: col = Color.FromHex("#ad454b").ToSKColor(); width = 2.0f; break;//1
                        case 131322: col = Color.FromHex("#ba6f39").ToSKColor(); width = 2.0f; break;//7
                        case 131131: col = Color.FromHex("#8d1860").ToSKColor(); width = 2.0f; break;//亦庄
                        case 131123: col = Color.FromHex("#2a9695").ToSKColor(); width = 2.0f; break;//4
                        case 131130: col = Color.FromHex("#ae5733").ToSKColor(); width = 2.0f; break;//房山
                        case 131117: col = Color.FromHex("#1c75b8").ToSKColor(); width = 2.0f; break;//10
                        case 131122: col = Color.FromHex("#105193").ToSKColor(); width = 2.0f; break;//2
                        case 131127: col = Color.FromHex("#8b9a3a").ToSKColor(); width = 2.0f; break;//9
                        case 131404: col = Color.FromHex("#a74822").ToSKColor(); width = 2.0f; break;//燕房
                            //天津
                        case 131187: col = Color.FromHex("#96141b").ToSKColor(); width = 2.0f; break;//1
                        case 131185: col = Color.FromHex("#8f9e40").ToSKColor(); width = 2.0f; break;//2
                        case 131186: col = Color.FromHex("#2a9695").ToSKColor(); width = 2.0f; break;//3
                        case 131361: col = Color.FromHex("#71164f").ToSKColor(); width = 2.0f; break;//6
                        case 131188: col = Color.FromHex("#105193").ToSKColor(); width = 2.0f; break;//9

                            //台北
                        case 131293: col = Color.FromHex("#2e81be").ToSKColor(); width = 2.0f; break;//板南线
                        case 131302: col = Color.FromHex("#2e81be").ToSKColor(); width = 2.0f; break;//板南线
                        case 131303: col = Color.FromHex("#2e81be").ToSKColor(); width = 2.0f; break;//板南线
                        case 131300: col = Color.FromHex("#d7aa8a").ToSKColor(); width = 2.0f; break;//中和新芦线（2002年）
                        case 131301: col = Color.FromHex("#d7aa8a").ToSKColor(); width = 2.0f; break;//中和新芦线（2002年）
                        case 131299: col = Color.FromHex("#ba6f39").ToSKColor(); width = 2.0f; break;//新北投线（2010年）
                        case 131294: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//淡水信义线（1997年）
                        case 131297: col = Color.FromHex("#9a6b67").ToSKColor(); width = 2.0f; break;//文湖线
                        case 131298: col = Color.FromHex("#c2a04f").ToSKColor(); width = 2.0f; break;//小碧潭线（2004年）
                        case 131292: col = Color.FromHex("#9a6b67").ToSKColor(); width = 2.0f; break;//猫空缆车
                        case 131295: col = Color.FromHex("#22701d").ToSKColor(); width = 2.0f; break;//松山新店线（2000年）
                        case 131296: col = Color.FromHex("#22701d").ToSKColor(); width = 2.0f; break;//松山新店线（2000年）

                            //上海
                        case 131174: col = Color.FromHex("#1c75b9").ToSKColor(); width = 2.0f; break;//8
                        case 131165: col = Color.FromHex("#74998e").ToSKColor(); width = 2.0f; break;//16
                        case 131161: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//11
                        case 131162: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//11
                        case 131158: col = Color.FromHex("#257bbc").ToSKColor(); width = 2.0f; break;//磁悬浮
                        case 131168: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//2
                        case 131167: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//2
                        case 131175: col = Color.FromHex("#749eb3").ToSKColor(); width = 2.0f; break;//9
                        case 131163: col = Color.FromHex("#3a8137").ToSKColor(); width = 2.0f; break;//12
                        case 131172: col = Color.FromHex("#9e242b").ToSKColor(); width = 2.0f; break;//6
                        case 131159: col = Color.FromHex("#ab8480").ToSKColor(); width = 2.0f; break;//10
                        case 131160: col = Color.FromHex("#ab8480").ToSKColor(); width = 2.0f; break;//10
                        case 131169: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//3
                        case 131166: col = Color.FromHex("#98181f").ToSKColor(); width = 2.0f; break;//1
                        case 131173: col = Color.FromHex("#a74821").ToSKColor(); width = 2.0f; break;//7
                        case 131164: col = Color.FromHex("#c9a8b9").ToSKColor(); width = 2.0f; break;//13
                        case 131171: col = Color.FromHex("#584979").ToSKColor(); width = 2.0f; break;//5
                        case 131170: col = Color.FromHex("#4c2f56").ToSKColor(); width = 2.0f; break;//4
                        case 131391: col = Color.FromHex("#aa4e28").ToSKColor(); width = 2.0f; break;//17
                        case 131420: col = Color.FromHex("#74998e").ToSKColor(); width = 2.0f; break;//浦江线

                            //广州
                        case 131392: col = Color.FromHex("#2a9695").ToSKColor(); width = 2.0f; break;//9
                        case 131144: col = Color.FromHex("#be7743").ToSKColor(); width = 2.0f; break;//3
                        case 131143: col = Color.FromHex("#be7743").ToSKColor(); width = 2.0f; break;//3
                        case 131147: col = Color.FromHex("#573c60").ToSKColor(); width = 2.0f; break;//6
                        case 131142: col = Color.FromHex("#195797").ToSKColor(); width = 2.0f; break;//2
                        case 131146: col = Color.FromHex("#96141c").ToSKColor(); width = 2.0f; break;//5
                        case 131141: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//1
                        case 131139: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//广佛
                        case 131149: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//广佛
                        case 131148: col = Color.FromHex("#2a9695").ToSKColor(); width = 2.0f; break;//8
                        case 131369: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//7
                        case 131145: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//4
                        case 131394: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//13
                        case 131393: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//14
                        case 131140: col = Color.FromHex("#1c75b8").ToSKColor(); width = 2.0f; break;//APM线

                            //武汉
                        case 131191: col = Color.FromHex("#3e73a8").ToSKColor(); width = 2.0f; break;//1
                        case 131189: col = Color.FromHex("#ad7d95").ToSKColor(); width = 2.0f; break;//2
                        case 131351: col = Color.FromHex("#704f28").ToSKColor(); width = 2.0f; break;//3
                        case 131190: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//4
                        case 131372: col = Color.FromHex("#74998e").ToSKColor(); width = 2.0f; break;//6
                        case 131402: col = Color.FromHex("#6795ac").ToSKColor(); width = 2.0f; break;//8
                        case 131395: col = Color.FromHex("#982c70").ToSKColor(); width = 2.0f; break;//21
                            //深圳
                        case 131179: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//1
                        case 131180: col = Color.FromHex("#b86a49").ToSKColor(); width = 2.0f; break;//2
                        case 131177: col = Color.FromHex("#247bbb").ToSKColor(); width = 2.0f; break;//3
                        case 131178: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//4
                        case 131176: col = Color.FromHex("#4f3f71").ToSKColor(); width = 2.0f; break;//5
                        case 131366: col = Color.FromHex("#28629e").ToSKColor(); width = 2.0f; break;//7
                        case 131367: col = Color.FromHex("#a77f7b").ToSKColor(); width = 2.0f; break;//9
                        case 131357: col = Color.FromHex("#42234c").ToSKColor(); width = 2.0f; break;//11
                            //南京
                        case 131155: col = Color.FromHex("#4e95c9").ToSKColor(); width = 2.0f; break;//1
                        case 131156: col = Color.FromHex("#4e95c9").ToSKColor(); width = 2.0f; break;//1
                        case 131157: col = Color.FromHex("#b86065").ToSKColor(); width = 2.0f; break;//2
                        case 131325: col = Color.FromHex("#639d62").ToSKColor(); width = 2.0f; break;//3
                        case 131371: col = Color.FromHex("#972a6e").ToSKColor(); width = 2.0f; break;//4
                        case 131219: col = Color.FromHex("#ba6f39").ToSKColor(); width = 2.0f; break;//10
                        case 131220: col = Color.FromHex("#389f9e").ToSKColor(); width = 2.0f; break;//s1
                        case 131388: col = Color.FromHex("#932268").ToSKColor(); width = 2.0f; break;//s3
                        case 131221: col = Color.FromHex("#a74821").ToSKColor(); width = 2.0f; break;//s8
                        case 131415: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//s9

                            //高雄
                        case 131289: col = Color.FromHex("#a6383e").ToSKColor(); width = 2.0f; break;//红线
                        case 131291: col = Color.FromHex("#ba6f39").ToSKColor(); width = 2.0f; break;//橙线（2015年）
                        case 131290: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//环状轻轨

                            //成都
                        case 131421: col = Color.FromHex("#5e4467").ToSKColor(); width = 2.0f; break;//1
                        case 131133: col = Color.FromHex("#5e4467").ToSKColor(); width = 2.0f; break;//1
                        case 131386: col = Color.FromHex("#115193").ToSKColor(); width = 2.0f; break;//10
                        case 131389: col = Color.FromHex("#7ca4b7").ToSKColor(); width = 2.0f; break;//7
                        case 131350: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//4
                        case 131134: col = Color.FromHex("#aa4f29").ToSKColor(); width = 2.0f; break;//2
                        case 131363: col = Color.FromHex("#98161e").ToSKColor(); width = 2.0f; break;//3
                           
                            //沈阳
                        case 131181: col = Color.FromHex("#a43138").ToSKColor(); width = 2.0f; break;//1
                        case 131182: col = Color.FromHex("#bd7541").ToSKColor(); width = 2.0f; break;//2

                            //重庆
                        case 131210: col = Color.FromHex("#a83a41").ToSKColor(); width = 2.0f; break;//1
                        case 131211: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//2
                        case 131212: col = Color.FromHex("#195797").ToSKColor(); width = 2.0f; break;//3
                        case 131374: col = Color.FromHex("#195797").ToSKColor(); width = 2.0f; break;//3
                        case 131416: col = Color.FromHex("#3082bf").ToSKColor(); width = 2.0f; break;//5
                        case 131213: col = Color.FromHex("#8d185f").ToSKColor(); width = 2.0f; break;//6
                        case 131214: col = Color.FromHex("#8d185f").ToSKColor(); width = 2.0f; break;//国博线
                        case 131417: col = Color.FromHex("#42234c").ToSKColor(); width = 2.0f; break;//10

                            //西安
                        case 131192: col = Color.FromHex("#2078ba").ToSKColor(); width = 2.0f; break;//1
                        case 131193: col = Color.FromHex("#96131b").ToSKColor(); width = 2.0f; break;//2
                        case 131365: col = Color.FromHex("#42234c").ToSKColor(); width = 2.0f; break;//3

                            //苏州
                        case 131183: col = Color.FromHex("#84942f").ToSKColor(); width = 2.0f; break;//1
                        case 131184: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//2
                        case 131378: col = Color.FromHex("#1c75b8").ToSKColor(); width = 2.0f; break;//4
                        case 131379: col = Color.FromHex("#1c75b8").ToSKColor(); width = 2.0f; break;//4
                            
                            //昆明
                        case 131153: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1
                        case 131375: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1
                        case 131216: col = Color.FromHex("#105193").ToSKColor(); width = 2.0f; break;//2
                        case 131385: col = Color.FromHex("#84799c").ToSKColor(); width = 2.0f; break;//3
                        case 131154: col = Color.FromHex("#1c75b8").ToSKColor(); width = 2.0f; break;//6

                            //杭州
                        case 131152: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1
                        case 131151: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1
                        case 131321: col = Color.FromHex("#bf7744").ToSKColor(); width = 2.0f; break;//2
                        case 131324: col = Color.FromHex("#277423").ToSKColor(); width = 2.0f; break;//4

                            //哈尔滨
                        case 131150: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1
                        case 131370: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//3

                            //青岛
                        case 131348: col = Color.FromHex("#2a9695").ToSKColor(); width = 2.0f; break;//3
                        case 131390: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//2
                        case 131422: col = Color.FromHex("#105193").ToSKColor(); width = 2.0f; break;//11

                            //郑州
                        case 131209: col = Color.FromHex("#af4a50").ToSKColor(); width = 2.0f; break;//1
                        case 131364: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//2
                        case 131376: col = Color.FromHex("#8d185f").ToSKColor(); width = 2.0f; break;//城郊

                            //长沙
                        case 131215: col = Color.FromHex("#1f77ba").ToSKColor(); width = 2.0f; break;//2
                        case 131360: col = Color.FromHex("#a8738e").ToSKColor(); width = 2.0f; break;//磁悬浮
                        case 131362: col = Color.FromHex("#9d222a").ToSKColor(); width = 2.0f; break;//1

                            //无锡
                        case 131218: col = Color.FromHex("#981820").ToSKColor(); width = 2.0f; break;//1
                        case 131323: col = Color.FromHex("#267322").ToSKColor(); width = 2.0f; break;//1

                            //大连
                        case 131347: col = Color.FromHex("#115193").ToSKColor(); width = 2.0f; break;//12
                        case 131326: col = Color.FromHex("#115193").ToSKColor(); width = 2.0f; break;//2
                        case 131346: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;//1
                        case 131135: col = Color.FromHex("#9c1e26").ToSKColor(); width = 2.0f; break;//3
                        case 131136: col = Color.FromHex("#9c1e26").ToSKColor(); width = 2.0f; break;//3
                        case 131137: col = Color.FromHex("#9c1e26").ToSKColor(); width = 2.0f; break;//3
                        case 131138: col = Color.FromHex("#9c1e26").ToSKColor(); width = 2.0f; break;//3

                            //长春
                        case 131206: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;//3
                        case 131207: col = Color.FromHex("#9b6c68").ToSKColor(); width = 2.0f; break;//4
                        case 131383: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;//1

                            //南昌
                        case 131384: col = Color.FromHex("#bb953d").ToSKColor(); width = 2.0f; break;//2
                        case 131349: col = Color.FromHex("#97141c").ToSKColor(); width = 2.0f; break;//1

                            // 贵阳 subway lines
                        case 131382: col = Color.FromHex("#82932d").ToSKColor(); width = 2.0f; break;

                        //厦门  subway lines 
                        case 131419: col = Color.FromHex("#a74821").ToSKColor(); width = 2.0f; break;

                        // 石家庄  subway lines 
                        case 131381: col = Color.FromHex("#1d75b8").ToSKColor(); width = 2.0f; break;
                        case 131380: col = Color.FromHex("#a74821").ToSKColor(); width = 2.0f; break;

                        // 合肥  subway lines 
                        case 131387: col = Color.FromHex("#1e5b99").ToSKColor(); width = 2.0f; break;
                        case 131373: col = Color.FromHex("#9b1d25").ToSKColor(); width = 2.0f; break;

                        // 南宁  subway lines 
                        case 131403: col = Color.FromHex("#930d15").ToSKColor(); width = 2.0f; break;
                        case 131358: col = Color.FromHex("#1e6d1a").ToSKColor(); width = 2.0f; break;

                        // 东莞  subway lines 
                        case 131356: col = Color.FromHex("#95121a").ToSKColor(); width = 2.0f; break;
                        // 福州  subway lines 
                        case 131359: col = Color.FromHex("#940f17").ToSKColor(); width = 2.0f; break;


                        case 131329: col = Color.FromHex("#f8e08b").ToSKColor(); width = 4.0f; break;
                        case 131330: col = Color.FromHex("#ffffff").ToSKColor(); break;
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
                        case 131406: col = Color.FromHex("#0f0").ToSKColor(); break;
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

                        case 131412:
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

        public override string ToString()
        {
            string ret= lng.ToString("0.0#####") + (lng != 0.0 ? (lng > 0.0 ? " E" : " W") : "") + ", " + lat.ToString("0.0#####") + (lat != 0.0 ? (lat > 0.0 ? " N" : " S") : "");
            return ret;
        }
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

        public virtual void Attached(AzMapView mapview)
        {
            MapView = mapview;
        }

        public virtual void Detached()
        {
            MapView = null;
        }
    }

    public class AzmEllipseView : AzmOverlayView
    {
        public delegate void OnTappedEventHandler(object sender, EventArgs e);
        public event OnTappedEventHandler OnTapped;


        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
            propertyName: nameof(StrokeColor),
            returnType: typeof(Color),
            declaringType: typeof(AzmEllipseView),
            defaultValue: Color.Black
        );

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(
            propertyName: nameof(StrokeThickness),
            returnType: typeof(double),
            declaringType: typeof(AzmEllipseView),
            defaultValue: 1.0
        );

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
            propertyName: nameof(Radius),
            returnType: typeof(double),
            declaringType: typeof(AzmEllipseView),
            defaultValue: 1000.0
        );

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly BindableProperty DashArrayProperty = BindableProperty.Create(
            propertyName: nameof(DashArray),
            returnType: typeof(float[]),
            declaringType: typeof(AzmEllipseView),
            defaultValue: null
        );

        public float[] DashArray
        {
            get { return (float[])GetValue(DashArrayProperty); }
            set { SetValue(DashArrayProperty, value); }
        }

        public AzmEllipseView(AzmCoord coord = null, double radius=1000.0)
        {
            //BackgroundColor = Color.Transparent;
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);
            Radius = radius;

            //ControlTemplate dt = AzMapView.ActiveMap.Resources["azmlabeltemp"] as ControlTemplate;
            //ControlTemplate = dt;

            //this.BindingContext = new { name = Text };
            //double height = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextHeightGivenMaxWidth(Text, WidthRequest - 6, 14));
            //double width = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextWidthGivenExactHeight(Text, height, 14));
            //width = Math.Min(width, WidthRequest - 6);
            //Size = new Size(width + 6, height + 6);
            //Anchor = new Point((width + 6) / 2, height + 6 + 7);

            //TapGestureRecognizer tap = new TapGestureRecognizer();
            //tap.Tapped += Tap_Tapped;
            //this.GestureRecognizers.Add(tap);
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            if (OnTapped != null) OnTapped(this, e);
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

        }

        public override void Attached(AzMapView mapview)
        {
            base.Attached(mapview);

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            this.GestureRecognizers.Add(tap);

        }

        public override void Detached()
        {
            base.Detached();
            this.GestureRecognizers.Clear();
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


        public bool AlwaysShowLabel
        {
            get { return (bool)GetValue(AlwaysShowLabelProperty); }
            set { SetValue(AlwaysShowLabelProperty, value); }
        }

        public static readonly BindableProperty AlwaysShowLabelProperty = BindableProperty.Create(
            propertyName: nameof(AlwaysShowLabel),
            returnType: typeof(bool),
            declaringType: typeof(AzmMarkerView),
            defaultValue: false
        );

        private AzmOverlayView _label = null;

        public AzmOverlayView Label
        {
            get { return _label; }
            set { _label = value; }
        }

        private Point _customviewanchor = new Point();

        private AzmOverlayView _customview = null;

        public AzmOverlayView CustomView
        {
            get { return _customview; }
            set {
                _customview = value;
                _customviewanchor = new Point(_customview.Anchor.X, _customview.Anchor.Y);
            }
        }

        //private ControlTemplate controlTemplate = null;

        //public ControlTemplate CustomViewTemplate
        //{
        //    get { return controlTemplate; }
        //    set { controlTemplate = value; }
        //}


        public AzmMarkerView(ImageSource source, Size size, AzmCoord coord = null, double popupmaxwidthrequest = 100.0)
        {
            Source = source;
            Size = size;
            Anchor = new Point(size.Width / 2, size.Height);
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);
            ControlTemplate dt = AzMapView.ActiveMap.Resources["azmmarkertemp"] as ControlTemplate;
            ControlTemplate = dt;
        }

        public override void Attached(AzMapView mapview)
        {
            base.Attached(mapview);

            if (!AlwaysShowLabel)
            {
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Marker_Tapped;
                this.GestureRecognizers.Add(tap);
            }
            else
            {
                if (CustomView == null)
                {
                    if (string.IsNullOrWhiteSpace(Text)) return;
                    AzmLabelView lv = new AzmLabelView(Text, Coord)
                    {
                        BackgroundColor = Color.FromHex("#ccc"),
                        TextColor = Color.FromHex("#444"),
                    };
                    lv.Anchor = new Point(lv.Anchor.X, lv.Anchor.Y + Size.Height);
                    lv.OnTapped += Popup_OnTapped;
                    MapView.Overlays.Add(lv);
                    Label = lv;
                } else
                {
                    _customview.Anchor = new Point(_customviewanchor.X, _customviewanchor.Y + Size.Height);
                    //_customview.OnTapped += Popup_OnTapped;
                    MapView.Overlays.Add(_customview);
                    Label = _customview;
                }
            }
        }

        public override void Detached()
        {
            base.Detached();
            this.GestureRecognizers.Clear();

            if (Label != null)
            {
                MapView.Overlays.Remove(Label);
                Label = null;
            }
        }

        private void Marker_Tapped(object sender, EventArgs e)
        {
            if (MapView.MarkerPopupView != null)
            {
                MapView.Overlays.Remove(MapView.MarkerPopupView);
                MapView.TappedMarker = null;
                Label = null;
            }

            if (CustomView == null)
            {
                if (string.IsNullOrWhiteSpace(Text)) return;
                AzmLabelView lv = new AzmLabelView(Text, Coord)
                {
                    BackgroundColor = Color.FromHex("#ccc"),
                    TextColor = Color.FromHex("#444"),
                };
                lv.Anchor = new Point(lv.Anchor.X, lv.Anchor.Y + Size.Height);
                lv.OnTapped += Popup_OnTapped;
                MapView.Overlays.Add(lv);
                Label = lv;
                MapView.MarkerPopupView = lv;
                MapView.TappedMarker = this;
            } else
            {
                _customview.Anchor = new Point(_customviewanchor.X, _customviewanchor.Y + Size.Height);
                //_customview.OnTapped += Popup_OnTapped;
                MapView.Overlays.Add(_customview);
                Label = _customview;
                MapView.MarkerPopupView = _customview;
                MapView.TappedMarker = this;
            }
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