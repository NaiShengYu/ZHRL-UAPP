using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzMapView : ContentView
    {
        public static AzMapView ActiveMap { get; set; }

        int level = 17;
        Tuple<int, int> xrange = new Tuple<int, int>(109824, 109829);
        Tuple<int, int> yrange = new Tuple<int, int>(76940, 76947);
        Dictionary<string, Image> idximgdict = new Dictionary<string, Image>();
        Dictionary<Image, Tuple<int, int, int>> imgidxdict = new Dictionary<Image, Tuple<int, int, int>>();

        int mapwidth = 1;
        int mapheight = 1;

        const int tilesize = 128;

        Point center = new Point(109824, 76940);

        double vpminx = 0;
        double vpmaxy = 0;

        AzmCoord sw = new AzmCoord(0, 0);
        AzmCoord ne = new AzmCoord(0, 0);

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

            foreach (object v in map.Children)
            {
                if (v is AzmOverlayView)
                {
                    AzmOverlayView ov = v as AzmOverlayView;
                    Point p = GetXYFromCoord(level, ov.Coord);
                    ov.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((p.X - vpminx) * tilesize - ov.Anchor.X, (vpmaxy - p.Y) * tilesize - ov.Anchor.Y, ov.Size.Width, ov.Size.Height));
                }
            }
        }

        private void AzMapView_SizeChanged(object sender, EventArgs e)
        {
            mapwidth = (int)Math.Ceiling(map.Width);
            mapheight = (int)Math.Ceiling(map.Height);
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

        //public Tuple<AzmCoord,AzmCoord> GetRangeTileBound()
        //{
        //    return new Tuple<AzmCoord, AzmCoord>(
        //        GetCoordFromXY(level, new Point(xrange.Item1, yrange.Item1)),
        //        GetCoordFromXY(level, new Point(xrange.Item2+1, yrange.Item2+1)));
        //}

        private void ReloadTile()
        {
            // remove all tile
            List<Image> dellist = new List<Image>();
            foreach (var img in imgidxdict)
            {
                dellist.Add(img.Key);
            }
            foreach (var img in dellist)
            {
                var t = imgidxdict[img];
                string key = t.Item1.ToString() + "_" + t.Item2.ToString() + "_" + t.Item3.ToString();

                idximgdict.Remove(key);
                imgidxdict.Remove(img);
                map.Children.Remove(img);
            }
            imgidxdict.Clear();
            idximgdict.Clear();

            // add tile
            for (var y = yrange.Item1; y <= yrange.Item2; y++)
            {
                for (var x = xrange.Item1; x <= xrange.Item2; x++)
                {
                    int xb = x >> 4;
                    int yb = y >> 4;
                    string url = string.Format("http://p2.map.gtimg.com/sateTiles/{0}/{1}/{2}/{3}_{4}.jpg?version=229", level, xb, yb, x, y);
                    Image img = new Image();
                    img.Source = new UriImageSource
                    {
                        Uri = new Uri(url),
                        CachingEnabled = true,
                        CacheValidity = new TimeSpan(5, 0, 0, 0)
                    };
                    img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                    img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                    map.Children.Insert(0, img);

                    imgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                    idximgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);
                }
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

                foreach (object v in map.Children)
                {
                    if (v is Image)
                    {
                        Image img = v as Image;
                        if (imgidxdict.ContainsKey(img))
                        {
                            Tuple<int, int, int> idx = imgidxdict[img];
                            img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((idx.Item2 - vpminx) * tilesize, (vpmaxy - idx.Item3 - 1) * tilesize, tilesize, tilesize));
                        }
                    }
                    else if (v is AzmOverlayView)
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
                        string key = level.ToString() + "_" + x.ToString() + "_" + y.ToString();

                        if (!idximgdict.ContainsKey(key))
                        {

                            int xb = x >> 4;
                            int yb = y >> 4;
                            string url = string.Format("http://p2.map.gtimg.com/sateTiles/{0}/{1}/{2}/{3}_{4}.jpg?version=229", level, xb, yb, x, y);
                            Image img = new Image();
                            img.Source = new UriImageSource
                            {
                                Uri = new Uri(url),
                                CachingEnabled = true,
                                CacheValidity = new TimeSpan(5, 0, 0, 0)
                            };
                            img.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rectangle((x - vpminx) * tilesize, (vpmaxy - y - 1) * tilesize, tilesize, tilesize));
                            img.SetValue(AbsoluteLayout.LayoutFlagsProperty, AbsoluteLayoutFlags.None);
                            map.Children.Insert(0, img);

                            imgidxdict.Add(img, new Tuple<int, int, int>(level, x, y));
                            idximgdict.Add(level.ToString() + "_" + x.ToString() + "_" + y.ToString(), img);

                        }
                    }
                }

                List<Image> dellist = new List<Image>();

                foreach (var img in imgidxdict)
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
                    var t = imgidxdict[img];
                    string key = t.Item1.ToString() + "_" + t.Item2.ToString() + "_" + t.Item3.ToString();

                    idximgdict.Remove(key);
                    imgidxdict.Remove(img);
                    map.Children.Remove(img);
                }
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
                    map.Children.Add(o);
                }
            }
            if (e.OldItems != null)
            {
                foreach (AzmOverlayView o in e.OldItems)
                {
                    map.Children.Remove(o);
                }
            }
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
}