using AepApp.MaterialForms.TreeViews;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridTreeViewPage : ContentPage
    {
        public GridTreeViewPage()
        {
            InitializeComponent();


            getEventInfo();
           
        }

        void creatView(){
            for (int i = 0; i < gridList.Count; i++)
            {
                GridTreeView tree = new GridTreeView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                TestTreeModel modelRoot = gridList[i];
                tree.SetDataSource(modelRoot);
                layoutTree.Children.Add(tree);
            }
        }

        private void Button_Clicked_CheckAll(object sender, EventArgs e)
        {
            CheckAll(true);
        }

        private void Button_Clicked_Clear(object sender, EventArgs e)
        {
            CheckAll(false);
        }

        void Button_Clicked_sure(object sender, EventArgs e){



        }

        private void CheckAll(bool check)
        {
            foreach (GridTreeView t in layoutTree.Children)
            {
                GridTreeNode root = t.ViewModel.MyTree;
                if (root.IsChecked != check)
                {
                    root.IsChecked = check;
                }
                foreach (GridTreeNode node in t.ViewModel.MyTree.Descendants)
                {
                    if (node.IsChecked == check)
                    {
                        continue;
                    }
                    node.IsChecked = check;
                }
            }
        }

        ObservableCollection<TestTreeModel> gridList = new ObservableCollection<TestTreeModel>();
        //获取事件详情
        private async void getEventInfo()
        {

            string url = App.EP360Module.url + "/api/gbm/GetGridList";
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("grid", "72a38f57-1939-40e6-8cca-2960e0d994ea");
            param.Add("searchKey", "");
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url,pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    gridList = JsonConvert.DeserializeObject<ObservableCollection<TestTreeModel>>(hTTPResponse.Results);
                    creatView();
                }
                catch (Exception ex)
                {
                }

            }

        }

 

    }
}