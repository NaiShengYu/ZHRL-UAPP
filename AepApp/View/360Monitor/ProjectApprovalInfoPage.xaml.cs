
using System;
using System.Collections.Generic;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Monitor
{
    public partial class ProjectApprovalInfoPage : ContentPage
    {
        public ProjectApprovalInfoPage(ProjectApproval approval)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            this.Title = approval.PROJECT_NAME;
            listV.ItemsSource = approval.FileData;

        }
    }
}
