using System;
using System.Collections.Generic;
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
        private int totalNum;
        private string mSearchKey;

        public TaskTemplatePage()
        {
            InitializeComponent();
            SearchData();
        }


        public void Handle_ItemSelected(Object sender, SelectedItemChangedEventArgs e)
        {
            var v = e.SelectedItem;
            if (v == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskTemplateInfoPage());
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
            ReqTaskTemplateList();
        }

        private void ReqTaskTemplateList()
        {
            List<string> list = new List<string>();
            list.Add("现场检查任务");
            list.Add("日常检查任务");
            list.Add("现场检查任务（2018修订）");
            listView.ItemsSource = list;
        }

        public void LoadMore(object sender, ItemVisibilityEventArgs e)
        {

        }

    }
}