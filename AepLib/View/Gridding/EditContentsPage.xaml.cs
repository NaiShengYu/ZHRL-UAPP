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
            if(_page ==1){
                if (_type == "EditResult") 
                    _EventInfoModel.Results = e.NewTextValue;
            if (_type == "EditContent")
                    _EventInfoModel.contents = e.NewTextValue;    
            }
            if(_page ==2){
                _taskInfoModel.Contents = e.NewTextValue;
            }
             
            if (_page == 3)
            {
                _followModel.Remarks = e.NewTextValue;
            }
        }

        int _page = 0;
        public EditContentsPage(){
            InitializeComponent();
            ToolbarItems.Add(new ToolbarItem("确定", "", () =>
            {
                Navigation.PopAsync();
            }));
        }

        GridEventInfoModel _EventInfoModel = null;
        string _type = "";
        public EditContentsPage(GridEventInfoModel infoModel,string type,int page):this()
        {
            _EventInfoModel = infoModel;
            BindingContext = _EventInfoModel;
            _page = page;
            _type = type;
            if (_type == "EditResult")
                EditText.Text = _EventInfoModel.Results;
            if (_type == "EditContent")
                EditText.Text = _EventInfoModel.contents;     
         
        }

        GridTaskInfoModel _taskInfoModel = null;
        public EditContentsPage(GridTaskInfoModel infoModel, string type, int page):this()
        {
            _taskInfoModel = infoModel;
            BindingContext = _taskInfoModel;
            _page = page;
            _type = type;
            EditText.Text = _taskInfoModel.Contents;     
        }

        GridEventFollowModel _followModel = null;
        public EditContentsPage(GridEventFollowModel followModel, string type, int page) : this()
        {
            _followModel = followModel;
            BindingContext = _followModel;
            _page = page;
            _type = type;
            EditText.Text = _followModel.Remarks;
        }



    }
}
