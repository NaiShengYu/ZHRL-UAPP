﻿using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskTemplatePage : ContentPage
    {
        private bool hasMore = true;
        private int pageIndex;
        private string mSearchKey;
        private ObservableCollection<TaskTemplateModel> dataList = new ObservableCollection<TaskTemplateModel>();

        public delegate void SelectTaskTemplate(object sender, EventArgs args);
        public SelectTaskTemplate selectTemplateResult;
        public TaskTemplatePage()
        {
            InitializeComponent();
            SearchData();
        }

        void UsingThitTemplate (object sender ,EventArgs args){

            Button button = sender as Button;
            TaskTemplateModel model = button.BindingContext as TaskTemplateModel;
            selectTemplateResult(model, null);
            Navigation.PopAsync();
        }

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            TaskTemplateModel v = e.SelectedItem as TaskTemplateModel;
            if (v == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskTemplateInfoPage(v.id.ToString()));
            listView.SelectedItem = null;
        }

        public void Handle_TextChanged(Object sender, TextChangedEventArgs e)
        {
            mSearchKey = e.NewTextValue;
            SearchData();
        }

        public void Handle_Search(Object sender, EventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            pageIndex = 0;
            hasMore = true;
            dataList.Clear();
            ReqTaskTemplateList();
        }

        private async void ReqTaskTemplateList()
        {
            string url = App.EP360Module.url + "/api/gbm/GetTemplateByKey";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("pageIndex", pageIndex);
            map.Add("pageSize", ApiUtils.PAGE_SIZE);
            map.Add("searchKey", mSearchKey);
            string param = JsonConvert.SerializeObject(map);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<TaskTemplateModel> list = Tools.JsonUtils.DeserializeObject<List<TaskTemplateModel>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            dataList.Add(item);
                        }
                        pageIndex++;
                    }
                    else
                    {
                        hasMore = false;
                    }
                }
                catch (Exception e)
                {

                }
            }
            listView.ItemsSource = dataList;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {
            TaskTemplateModel item = e.Item as TaskTemplateModel;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (hasMore && dataList.Count >= ApiUtils.PAGE_SIZE)
                {
                    ReqTaskTemplateList();
                }
            }
        }

    }
}