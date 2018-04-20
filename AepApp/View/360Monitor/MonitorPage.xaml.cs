using System;
using System.Collections.Generic;
using System.Reflection;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Monitor
{
    public partial class MonitorPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }


        List<string> labels = new List<string> { "项目审批", "排污许可证管理", "信访管理", "日常监管", "电子处罚", "监督检测" };
        List<Type> types = new List<Type> { typeof(ProjectApprovalOnePage), typeof(EmissionPermitManagementPage), typeof(LettersAndVisitsPage), typeof(DailyRegulationPage), typeof(ElectroniPcunishmentPage), typeof(SupervisionAndInspectionPage) };

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            int i = labels.IndexOf(e.SelectedItem as string);
            listV.SelectedItem = null;
            if (i < 0) return;
            ConstructorInfo ctor=types[i].GetConstructor(new[] { typeof(EnterpriseModel) });
            object inst = ctor.Invoke(new object[] { _ent });
            Navigation.PushAsync(inst as ContentPage);
        }
        public MonitorPage(EnterpriseModel ent)
        {
            InitializeComponent();
            _ent = ent;
            this.BindingContext = Enterprise;
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字

            listV.ItemsSource = labels;
        }
    }
}
