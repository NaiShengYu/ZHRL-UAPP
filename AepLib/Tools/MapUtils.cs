using AepApp.Interface;
using AepApp.View;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace AepApp.Tools
{
    public class MapUtils
    {
        private static MapUtils _instance = null;
        private static object MapUtils_Lock = new object();

        public static MapUtils GetInstance()
        {
            if (_instance == null)
            {
                lock (MapUtils_Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MapUtils();
                    }
                }
            }
            return _instance;
        }

   }

    public class NavLabelView : AzmOverlayView
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


        private ICommand _navcommand = null;

        public ICommand NavCommand
        {
            get { return _navcommand; }
            set { _navcommand = value; }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public NavLabelView(string text, AzmCoord coord = null, double maxwidthrequest = 120.0)
        {
            if (text == null) text = "";
            BackgroundColor = Color.FromHex("#002060");
            Text = text.Trim();
            WidthRequest = maxwidthrequest;
            if (coord != null) Coord = new AzmCoord(coord.lng, coord.lat);

            this.BindingContext = new { name = Text };
            double height = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextHeightGivenMaxWidth(Text, WidthRequest - 40, 14));
            double width = Math.Ceiling(DependencyService.Get<ITextMeter>().MeasureTextWidthGivenExactHeight(Text, height, 14));
            width = Math.Min(width, WidthRequest - 40);
            Size = new Size(width + 40, height + 6);
            Anchor = new Point((width + 40) / 2, height + 6 + 7);

            NavCommand = new Command(() =>
            {
                // navigation here
            });
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

}
