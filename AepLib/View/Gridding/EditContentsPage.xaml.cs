using System;
using System.Collections.Generic;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class EditContentsPage : ContentPage
    {
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            
            if (_type == "EditResult") 
                _infoModel.Results = e.NewTextValue;
            if (_type == "EditContent")
                _infoModel.Content = e.NewTextValue;        
        }

        GridEventInfoModel _infoModel = null;
        string _type = "";
            public EditContentsPage(GridEventInfoModel infoModel,string type)
        {
            InitializeComponent();
            _infoModel = infoModel;
            BindingContext = _infoModel;
            _type = type;
            if (_type == "EditResult")
                EditText.Text = _infoModel.Results;
            if (_type == "EditContent")
                EditText.Text = _infoModel.Content;     
            ToolbarItems.Add(new ToolbarItem("确定","" , () =>
            {
                Navigation.PopAsync();
            }));

        }
    }
}
