using System;
using System.Collections.Generic;

using Xamarin.Forms;
using AepApp.View.EnvironmentalEmergency;
using AepApp.View.SecondaryFunction;
using Xamarin.Forms.PlatformConfiguration;

namespace AepApp.View
{
    public partial class MasterPage : ContentPage
    {
        Grid lastselecteditem = null;

        public MasterPage(MasterAndDetailPage masterdetailpage)
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                if (lastselecteditem != null) lastselecteditem.BackgroundColor = Color.White;
                lastselecteditem = s as Grid;
                lastselecteditem.BackgroundColor = Color.FromHex("#E7F3FF");

                if (lastselecteditem.Children[0].BindingContext != null)
                {
                    Type pagetype = Type.GetType(lastselecteditem.Children[0].BindingContext as string);
                    var page = new NavigationPage((Page)Activator.CreateInstance(pagetype));
                    masterdetailpage.Detail = page;
                }
                masterdetailpage.IsPresented = false;

            };

            StackLayout[] menus = new StackLayout[3] { menu1, menu2, menu3 };

            foreach (var menu in menus)
            {
                foreach (var child in menu.Children)
                {
                    if (child is Grid)
                    {
                        //if (child.StyleClass != null && child.StyleClass.Contains("menuitem"))
                        //{
                            child.GestureRecognizers.Add(tapGestureRecognizer);
                        //}
                    }
                }
            }

        }
    }
}
