using System;
using System.Collections.Generic;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Monitor
{
    public partial class MonitorPage : ContentPage
    {
        EnterpriseModel _preiseModel = null;
        //listView选中
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            
            var selectItem = e.SelectedItem as titleName;
            if (selectItem == null)
                return;
            
            switch (selectItem.id)
            {
                case 1:
                    Navigation.PushAsync(new ProjectApprovalOnePage(_preiseModel));
                    break;
                case 2:
                    Navigation.PushAsync(new EmissionPermitManagementPage(_preiseModel));
                    break;
                case 3:
                    Navigation.PushAsync(new LettersAndVisitsPage(_preiseModel));
                    break;
                case 4:
                    Navigation.PushAsync(new DailyRegulationPage(_preiseModel));
                    break;
                case 5:
                    Navigation.PushAsync(new ElectroniPcunishmentPage(_preiseModel));
                    break;
                case 6:
                    Navigation.PushAsync(new SupervisionAndInspectionPage(_preiseModel));
                    break;
                default:
                    break;
            }
            listV.SelectedItem = null;
        }
        public MonitorPage(EnterpriseModel preiseModel)
        {
            InitializeComponent();
            _preiseModel = preiseModel;
            Title = "360监控";
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            var title1 = new titleName();
            title1.title = "项目审批";
            title1.id = 1;
            var title2 = new titleName();
            title2.title = "排污许可证管理";
            title2.id = 2;
            var title3 = new titleName();
            title3.title = "信访管理";
            title3.id = 3;
            var title4 = new titleName();
            title4.title = "日常监管";
            title4.id = 4;
            var title5 = new titleName();
            title5.title = "电子处罚";
            title5.id = 5;
            var title6 = new titleName();
            title6.title = "监督检测";
            title6.id = 6;

            //String[] array1 = new String[] { "aaaa", "bbbbb", "cccc", "dddd" };
            titleName[] array = new titleName[] { title1, title2, title3, title4, title5, title6 };


            //List<titleName> list = new List<titleName>(){
            //    title1, title1, title1, title1

            //};
            listV.ItemsSource = array;
            //listV.ItemsSource = array1;
        }

        internal class titleName
        {
            public string title { get; set; }
            public int id { get; set; }

        }
    }
}
