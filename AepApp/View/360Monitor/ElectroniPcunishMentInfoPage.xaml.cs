using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif


namespace AepApp.View.Monitor
{
    public partial class ElectroniPcunishMentInfoPage : ContentPage
    {
        public ElectroniPcunishMentInfoPage(ElectroniPcunishMentList electroniPcunish)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = electroniPcunish.title;


            listV.ItemsSource = electroniPcunish.items;



        }
    }
}
