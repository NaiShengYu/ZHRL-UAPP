using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;//使用ObservableCollection这个类需要导入的文件
using System.ComponentModel;
using Xamarin.Forms;
using AepApp.Models;
using CloudWTO.Services;
using Plugin.Hud;
using Sample;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif


namespace AepApp.View.Monitor
{
    public partial class ElectroniPcunishMentInfoPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        public ElectroniPcunishMentInfoPage(ElectroniPcunishMentList electroniPcunish, EnterpriseModel ent)
        {
            InitializeComponent();
            _ent = ent;
            this.BindingContext = Enterprise;
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = electroniPcunish.title;
            if(electroniPcunish.items == null || electroniPcunish.items.Count ==0)
                DependencyService.Get<IToast>().ShortAlert("无数据");
            listV.ItemsSource = electroniPcunish.items;
        }
    }
}
