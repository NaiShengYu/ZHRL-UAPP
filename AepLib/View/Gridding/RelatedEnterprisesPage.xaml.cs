using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using AepApp.Models;
namespace AepApp.View.Gridding
{
    public partial class RelatedEnterprisesPage : ContentPage
    {
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            GetAllEnterprise();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            GridEnterpriseModel model = e.SelectedItem as GridEnterpriseModel;
            if (model == null) return;
            _infoModel.enterprise = model.id;
            _infoModel.EnterpriseName = model.name;

            listView.SelectedItem = null;
            Navigation.PopAsync();

        }

        string searchKey = "";
        private ObservableCollection<GridEnterpriseModel> dataList = new ObservableCollection<GridEnterpriseModel>();
        GridEventInfoModel _infoModel = null;
        public RelatedEnterprisesPage(GridEventInfoModel infoModel)
        {
            InitializeComponent();
            _infoModel = infoModel;
            NavigationPage.SetBackButtonTitle(this, "");
            GetAllEnterprise();
        }



        //获取事件详情
        private async void GetAllEnterprise()
        {

            string url = App.environmentalQualityModel.url + "/api/mod/GetAllEnterprise";
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("keys", searchKey);
            string Pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, Pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dataList.Clear();
                dataList = JsonConvert.DeserializeObject<ObservableCollection<GridEnterpriseModel>>(hTTPResponse.Results);
                listView.ItemsSource = dataList;
            }

        }

    }
}
