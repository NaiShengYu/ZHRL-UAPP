using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.EnvironmentalEmergency
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GeneralMapPage : ContentPage
	{
        public GeneralMapPage()
        {
            InitializeComponent();
        }

        public GeneralMapPage(string title, AzmCoord singlecoord)
        {
            InitializeComponent();
            Title = title;

            AzmMarkerView m = new AzmMarkerView(ImageSource.FromFile("markerred"), new Size(30, 30), singlecoord);
            map.Overlays.Add(m);
            map.SetCenter(13, singlecoord);
        }

        public GeneralMapPage(string title, AzmMarkerView singlemarker)
        {
            InitializeComponent();
            Title = title;

            map.Overlays.Add(singlemarker);
            map.SetCenter(13, singlemarker.Coord);
        }


    }
}