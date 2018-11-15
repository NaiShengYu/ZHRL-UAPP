using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AepApp.Tools
{
    public class NetWorkStateGrid : Grid
    {
        public NetWorkStateGrid()
        {
            BackgroundColor = Color.Yellow;
            HeightRequest = 25;
            //IsVisible = false;
            Label label = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Text = "网络无法接通，现处于离线状态",
                FontSize = 12,
            };
            this.Children.Add(label);

            NetworkAccess access = Connectivity.NetworkAccess;
            stateChange(access);
            MessagingCenter.Unsubscribe<Grid, NetworkAccess>(this, "NetworkChanged");
            MessagingCenter.Subscribe<Grid, NetworkAccess>(this, "NetworkChanged", (Grid arg1, NetworkAccess arg2) =>
            {
                stateChange(arg2);
            });
        }
        void stateChange(NetworkAccess access){
            if (access == NetworkAccess.None || access == NetworkAccess.ConstrainedInternet)
                IsVisible = true;
            else IsVisible = false;
        }
    }
}