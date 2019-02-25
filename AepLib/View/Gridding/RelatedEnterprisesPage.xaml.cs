using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using AepApp.Models;
using AepApp.Tools;
using Sample;

namespace AepApp.View.Gridding
{
    public partial class RelatedEnterprisesPage : ContentPage
    {
       

        public delegate void addEnterprise();
        public event addEnterprise addEnter;
        string searchKey = "";
        int pageIndex = 0;
        bool hasMore = true;
        private ObservableCollection<GridEnterpriseModel> dataList = new ObservableCollection<GridEnterpriseModel>();
        GridEventInfoModel _infoModel = null;
        ObservableCollection<Enterprise> _enterprise = null;
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            pageIndex = 0;
            hasMore = true;
            GetAllEnterprise();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            GridEnterpriseModel model = e.SelectedItem as GridEnterpriseModel;
            if (model == null) return;
            if(_infoModel !=null){
                _infoModel.enterprise = model.id;
                _infoModel.EnterpriseName = model.name;
            }
            if(_enterprise != null){
                var ent = new Enterprise
                {
                    id = Guid.NewGuid(),
                    enterpriseName = model.name,
                    enterprise = Guid.Parse(model.id),
                    rowState = "add",
                };

                _enterprise.Add(ent);
                addEnter();
            }
            listView.SelectedItem = null;
            Navigation.PopAsync();

        }

        void Handle_ItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
            GridEnterpriseModel model = e.Item as GridEnterpriseModel;
            if(model !=null && model == dataList[dataList.Count-1])
                GetAllEnterprise();
        }

        public RelatedEnterprisesPage(){
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            GetAllEnterprise();
            listView.ItemsSource = dataList;
        }
       
        public RelatedEnterprisesPage(GridEventInfoModel infoModel):this()
        {
            _infoModel = infoModel;
        }

        public RelatedEnterprisesPage(ObservableCollection<Enterprise> enterprise) : this()
        {
            _enterprise = enterprise;
        }

        //获取企业列表
        private async void GetAllEnterprise()
        {
            if (hasMore == false) return;

            string url = App.BasicDataModule.url + "/api/mod/GetAllEnterprise";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("keys", searchKey);
            param.Add("pageIndex", pageIndex);
            param.Add("pageSize", ConstantUtils.PAGE_SIZE);

            string Pa = JsonConvert.SerializeObject(param);

            
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, Pa, "POST", App.FrameworkToken);
            //DependencyService.Get<IToast>().ShortAlert("请求url:" + url + "\n请求参数：" + Pa + "\n请求结果："+hTTPResponse.Results);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    if (pageIndex == 0) dataList.Clear();
                    var result = JsonConvert.DeserializeObject<resultModel>(hTTPResponse.Results);
                    foreach (var item in result.items)
                    {
                        dataList.Add(item);
                    }
                    if (dataList.Count < result.count)
                    {
                        pageIndex += 1;
                        hasMore = true;
                    }
                    else
                    {
                        hasMore = false;
                    }
                }
                catch (Exception ex)
                {

                }
             
            }

        }

        private class resultModel{
            public ObservableCollection<GridEnterpriseModel> items { get; set; }
            public int? count { get; set; }
        }

    }
}
