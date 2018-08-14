using AepApp.Models;
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
    public partial class SelectGridWorkerPage : ContentPage
    {
        private int totalNum;
        private string mSearchKey;
        private ObservableCollection<GridEventModel> dataList = new ObservableCollection<GridEventModel>();

        public SelectGridWorkerPage()
        {
            InitializeComponent();
            SearchData();
        }

        public async void OnMessageClicked(Object sender, EventArgs e)
        {
            await DisplayAlert("title", "message", "ok");
        }
        public async void OnPhoneClicked(Object sender, EventArgs e)
        {
            await DisplayAlert("title", "call", "ok");
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

        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {

        }
        private void SearchData()
        {
            dataList.Clear();
            ReqWorksList();
        }

        private void ReqWorksList()
        {
            List<string> datas = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                datas.Add(i.ToString());
            }
            listView.ItemsSource = datas;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {

        }
    }
}