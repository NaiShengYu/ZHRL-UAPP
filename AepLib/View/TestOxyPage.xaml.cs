using AepApp.Interface;
using AepApp.Models;
using AepApp.Tools;
using AepApp.ViewModels;
using CloudWTO.Services;
using Plugin.Media;
using SimpleAudioForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestOxyPage : ContentPage, INotifyPropertyChanged
    {

        public TestOxyPage()
        {
            InitializeComponent();
            Position position = new Position(App.currentLocation.Latitude,App.currentLocation.Longitude);
            map.MoveToRegion(new MapSpan(position,0.01,0.01));

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();


        }

    }

}