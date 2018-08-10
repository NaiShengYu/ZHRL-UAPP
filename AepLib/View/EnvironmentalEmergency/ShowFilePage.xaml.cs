﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowFilePage : ContentPage
    {
        public ShowFilePage(string info)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "报告";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, info);
            web.Source = filename;
        }
    }
}