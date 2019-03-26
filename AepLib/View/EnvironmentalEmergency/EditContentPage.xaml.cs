using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EditContentPage : ContentPage
    {
        string _content = "";
        public event EventHandler<EventArgs> result;
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            _content = e.NewTextValue;
        }
        public EditContentPage(string title,bool canEdit,string content)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");//去掉返回键文字
            _content = content;
            EditText.Text = content;
            EditText.IsEnabled = canEdit;
            Title = title;
            if (canEdit ==true)
            {
                ToolbarItems.Add(new ToolbarItem("确定", "", () =>
                {
                    result.Invoke(_content, new EventArgs());
                    Navigation.PopAsync();
                }));
            }

        }
    }
}
