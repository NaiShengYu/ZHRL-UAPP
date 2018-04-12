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
            Navigation.PushAsync(new ProjectApprovalOnePage(_preiseModel));
            listV.SelectedItem = null;
        }
        public MonitorPage(EnterpriseModel preiseModel)
        {
            InitializeComponent();
            _preiseModel = preiseModel;
            Title = "360监控";
            //NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            var title1 = new titleName();
            title1.title = "项目审批";
            var title2 = new titleName();
            title2.title = "排污许可证管理";
            var title3 = new titleName();
            title3.title = "信访管理";
            var title4 = new titleName();
            title4.title = "日常监管";
            var title5 = new titleName();
            title5.title = "电子处罚";
            var title6 = new titleName();
            title6.title = "监督检测";

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
        }
    }
}
