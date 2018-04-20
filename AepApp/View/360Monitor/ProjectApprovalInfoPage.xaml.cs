
using System;
using System.Collections.Generic;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Monitor
{
    public partial class ProjectApprovalInfoPage : ContentPage
    {
        private EnterpriseModel _ent;

        public EnterpriseModel Enterprise
        {
            get { return _ent; }
            set { _ent = value; }
        }

        public ProjectApprovalInfoPage(ProjectApproval approval, EnterpriseModel ent)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _ent = ent;
            this.BindingContext = Enterprise;
            this.Title = approval.PROJECT_NAME;
            listV.ItemsSource = approval.FileData;
        }
    }
}
