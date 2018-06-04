using System;
using Xamarin.Forms;
namespace AepApp.View
{
    public class ZoomImage : Image
    {
        public delegate void SwipeEventHandler(object sender, PanUpdatedEventArgs e);
        public event SwipeEventHandler swipeStart;
        public event SwipeEventHandler swipeRunning;
        public event SwipeEventHandler swipeStop;

        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double StartScale;
        private double LastX, LastY;

        public ZoomImage()
        {
            BackgroundColor = Color.Black;

            var pinch = new Xamarin.Forms.PinchGestureRecognizer();
            pinch.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinch);

            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(pan);

            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;


        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
            return base.OnMeasure(widthConstraint, heightConstraint);
        }


        private void OnTapped(object sender, EventArgs e)
        {
            Console.WriteLine("width=" + Width + "\nheight" + Height + "\nscale=" + Scale);

            if (Scale > MIN_SCALE)
            {
                this.ScaleTo(MIN_SCALE, 250, Easing.CubicInOut);
                this.TranslateTo(0, 0, 250, Easing.CubicInOut);
            }
            else
            {
                //图片中心点位置
                AnchorX = AnchorY = 0.5; //TODO tapped position
                this.ScaleTo(MAX_SCALE, 250, Easing.CubicInOut);
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            if (Scale >= MIN_SCALE)
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        LastX = TranslationX;
                        LastY = TranslationY;
                        if (swipeStart != null) swipeStart(this, e);
                        break;
                    case GestureStatus.Running:
                        Console.WriteLine("X=" + e.TotalX + "Y=" + e.TotalY);
                        //Console.WriteLine("width=" + Width * (Scale - 1) / 2 + "\nheight" + Height * (Scale - 1) / 2 + "\nscale=" + Scale);
                        TranslationX = Clamp(LastX + e.TotalX * Scale, -Width * (Scale - 1) / 2, Width * (Scale - 1) / 2);
                        TranslationY = Clamp(LastY + e.TotalY * Scale, -Height * (Scale - 1) / 2, Height * (Scale - 1) / 2);

                        //Console.WriteLine("\ntransLationX=" + TranslationX + "\ntranslationY=" + TranslationY);
                        if (LastX == TranslationX && Math.Abs(LastY - TranslationY) < 20)
                        {
                            if (swipeRunning != null) swipeRunning(this, e);
                        }

                        break;

                    case GestureStatus.Completed:
                        {
                            if (swipeStop != null) swipeStop(this, e);
                        }
                        break;
                }
        }

        private double lastScaleX, lastScaleY;

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    StartScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    lastScaleX = e.ScaleOrigin.X;
                    lastScaleY = e.ScaleOrigin.Y;
                    break;
                case GestureStatus.Running:
                    double current = Scale + (e.Scale - 1) * StartScale;
                    Scale = Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;
                case GestureStatus.Completed:

                    AnchorX = AnchorY = 0.5;

                    if (Scale > MAX_SCALE)
                    {
                        this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);

                    }
                    else if (Scale < MIN_SCALE)
                    {
                        this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                        TranslationX = TranslationY = 0;
                    }
                    else
                    {
                        TranslationX = Clamp((0.5 - lastScaleX) * Width * Scale, -Width * (Scale - 1) / 2, Width * (Scale - 1) / 2);
                        TranslationY = Clamp((0.5 - lastScaleY) * Height * Scale, -Height * (Scale - 1) / 2, Height * (Scale - 1) / 2);
                    }
                    break;
            }
        }

        private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            else if (value.CompareTo(maximum) > 0)
                return maximum;
            else
                return value;
        }
    }
}
