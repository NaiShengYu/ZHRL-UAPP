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
        public delegate void addEnterprise();
        public event addEnterprise addEnter;
        string searchKey = "";
        private ObservableCollection<GridEnterpriseModel> dataList = new ObservableCollection<GridEnterpriseModel>();
        GridEventInfoModel _infoModel = null;
        ObservableCollection<Enterprise> _enterprise = null;
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
        public RelatedEnterprisesPage(){
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            GetAllEnterprise();

        }
       
        public RelatedEnterprisesPage(GridEventInfoModel infoModel):this()
        {
            _infoModel = infoModel;
        }

        public RelatedEnterprisesPage(ObservableCollection<Enterprise> enterprise) : this()
        {
            _enterprise = enterprise;
        }

        //获取事件详情
        private async void GetAllEnterprise()
        {

            string url = App.BasicDataModule.url + "/api/mod/GetAllEnterprise";
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
