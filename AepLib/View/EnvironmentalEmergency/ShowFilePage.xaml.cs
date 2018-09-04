using Sample;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowFilePage : ContentPage
    {
        public ShowFilePage(string info) : this()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, info);
            web.Source = filename;
        }

        public ShowFilePage(string url, bool isFromNet) : this()
        {
            if (isFromNet)
            {
                web.Source = url;
            }
            else
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filename = Path.Combine(path, url);
                web.Source = filename;
            }
        }

        public ShowFilePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "附件";
        }
    }
}
