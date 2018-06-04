using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.View.SecondaryFunction
{
    public class ZXingOverLayout : Grid
    {
        private int height;
        public ZXingOverLayout()
        {
            initView();
        }
        public void initView()
        {
            initGride();
            initChildView();
        }

        private void initChildView()
        {
            var verStack = new StackLayout { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };

            for (int i = 0; i < 5; i++)
            {
                var horStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
                for (int j = 0; j < 6; j++)
                {
                    //var tapGestureRecognizer = new TapGestureRecognizer();
                    //tapGestureRecognizer.NumberOfTapsRequired = 1;
                    //tapGestureRecognizer.Tapped += (s, e) => {
                    //    // handle the tap
                    //};
                    var image = new Image() { Source = "icon.png" };
                    horStack.Children.Add(image);
                    //image.GestureRecognizers.Add(tapGestureRecognizer);                   
                }
                var horScrollView = new ScrollView { Orientation = ScrollOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand, Content = horStack };

                verStack.Children.Add(horScrollView);
            }
            var verScrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand, Content = verStack };
            this.Children.Add(verScrollView, 0, 0);
        }

        private void initGride()
        {
            height = App.ScreenHeight / 2;
            this.HeightRequest = height;
            this.RowDefinitions.Add(new RowDefinition { Height = height });
            this.RowDefinitions.Add(new RowDefinition { Height = height });
            int statusBarHeight = DependencyService.Get<Interface.IStatusBar>().GetHeight();
            this.RowSpacing = 0;
            this.Margin = new Thickness(0, height - statusBarHeight, 0, -height + statusBarHeight);
        }

        internal void setMessage(string text)
        {
            Console.WriteLine("二维码" + text);
        }
    }
}