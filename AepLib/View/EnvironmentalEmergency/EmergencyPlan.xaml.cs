using AepApp.Models;
using AepApp.Services;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class EmergencyPlan : ContentPage
    {
        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                ReqEmergencyPlan(searchKey, ""); //网络请求专家库，10条每次       
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            ReqEmergencyPlan(searchKey, ""); //网络请求专家库，10条每次       
        }
        string searchKey = "";
        private int totalNum = 0;
        private ObservableCollection<EmergencyPlanModels.ItemsBean> dataList = new ObservableCollection<EmergencyPlanModels.ItemsBean>();


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as EmergencyPlanModels.ItemsBean;
            if (item == null)
                return;
            List<EmergencyPlanModels.FilesBean> file = item.files;
            string fileName =file[0].id + "." + file[0].format;
            downloadPlan(item.files[0].storeUrl,fileName);
            listView.SelectedItem = null;
        }
       

        public EmergencyPlan()
        {
            InitializeComponent();
            ReqEmergencyPlan(searchKey, ""); //网络请求专家库，10条每次       
    
        }
        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            EmergencyPlanModels.ItemsBean item = e.Item as EmergencyPlanModels.ItemsBean;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < totalNum)
                {
                    ReqEmergencyPlan(searchKey, ""); //网络请求敏感源，10条每次
                }
            }
        }


        private async void downloadPlan(String filePath,string fileFormat){
            string url = App.EmergencyModule.url + filePath;
            string dirPath = DependencyService.Get<IFileService>().GetExtrnalStoragePath(Constants.STORAGE_TYPE_DOC);
            //存储文件名
            string filename = Path.Combine(dirPath, fileFormat);
            if (!File.Exists(filename))
            {
                HTTPResponse hTTPResponse = await EasyWebRequest.HTTPRequestDownloadAsync(url, fileFormat, App.EmergencyToken);
            }
            await Navigation.PushAsync(new ShowFilePage(filename));

        }


        private async void ReqEmergencyPlan(String Filter, String Sorting)
        {
            string url = App.EmergencyModule.url + DetailUrl.EmergencyPlan + "?Filter=" + Filter + "&Sorting=" + Sorting + "&MaxResultCount=10" +"&SkipCount=" + dataList.Count;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, "", "GET", App.EmergencyToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                EmergencyPlanModels.EmergencyPlanBean emergencyPlanBean = new EmergencyPlanModels.EmergencyPlanBean();
                emergencyPlanBean = JsonConvert.DeserializeObject<EmergencyPlanModels.EmergencyPlanBean>(hTTPResponse.Results);
                totalNum = emergencyPlanBean.result.preplans.totalCount;
                List<EmergencyPlanModels.ItemsBean> list = emergencyPlanBean.result.preplans.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                listView.ItemsSource = dataList;
            }
        }

       
        
    }
}
