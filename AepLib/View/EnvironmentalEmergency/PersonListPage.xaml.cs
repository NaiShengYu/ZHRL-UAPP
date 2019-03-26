using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
namespace AepApp.View.EnvironmentalEmergency
{
    public partial class PersonListPage : ContentPage
    {
        private int Index;
        private int total;
        public event EventHandler<EventArgs> SamplePerson;
        private ObservableCollection<Staff> dataList = new ObservableCollection<Staff>();


        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Staff item = e.SelectedItem as Staff;
            if (item == null)
                return;
           
            //布点选择人员
                AddPlacement_Staff equipment = new AddPlacement_Staff
                {
                    staffid = item.id,
                    staffname = item.username,
                };
                SamplePerson.Invoke(equipment, new EventArgs());
                Navigation.PopAsync();


            listView.SelectedItem = null;
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            searchKey = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                dataList.Clear();
                Index = 0;
                ReqEquipmentList("", Index, 10);
            }
        }
        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            dataList.Clear();
            Index = 0;
            ReqEquipmentList("", Index, 10);
        }

        string searchKey = "";


        public PersonListPage()
        {
            InitializeComponent();
            ReqEquipmentList("", Index, 10);
        }

        private async void ReqEquipmentList(string keyword, int pagrIndex, int pageSize)
        {
            string url = App.FrameworkURL + DetailUrl.PersonList;
            Dictionary<string, object> parameter = new Dictionary<string, object>();
            parameter.Add("keyword", searchKey);
            parameter.Add("pageindex", Index);
            parameter.Add("pageSize", 10);
            string param = JsonConvert.SerializeObject(parameter);
            //string param = "keyword=" + "" + "&pageIndex=" + pagrIndex + "&pageSize=" + pageSize;
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, param, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(hTTPResponse.Results);
                Index += 1;
               var staffList = JsonConvert.DeserializeObject<StaffList>(hTTPResponse.Results);
                total = staffList.count;
                List<Staff> list = staffList.items;
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    dataList.Add(list[i]);
                }
                if (total != 0) Title = "人员（" + total + "）";
                else Title = "人员";
                listView.ItemsSource = dataList;
            }
        }

        private void listView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            Staff item = e.Item as Staff;
            if (item == dataList[dataList.Count - 1] && item != null)
            {
                if (dataList.Count < total)
                {
                    ReqEquipmentList("", Index, 10);
                }
            }
        }


        private class Staff
        {
            public string id { get; set; }
            public string userid { get; set; }
            public string username { get; set; }
            public string telephone { get; set; }
            public string date { get; set; }
        }

        private class StaffList
        {
            public int count { get; set; }
            public List<Staff> items { get; set; }

        }

    }
}
