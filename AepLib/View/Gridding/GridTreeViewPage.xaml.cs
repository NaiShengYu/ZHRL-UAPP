using AepApp.MaterialForms.TreeViews;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
//=======

//                TestTreeModel modelRoot = new TestTreeModel();
//                modelRoot.name = i + "高桥镇";
//                ObservableCollection<TestTreeModel> rootChildren = new ObservableCollection<TestTreeModel>();
//                for (int j = 0; j < 3; j++)
//                {
//                    TestTreeModel modelBranch = new TestTreeModel();
//                    modelBranch.name = i + "" + j + "小牙山村";
//                    ObservableCollection<TestTreeModel> branchChildren = new ObservableCollection<TestTreeModel>();
//                    for (int k = 0; k < 2; k++)
//                    {
//                        TestTreeModel modelLeaf = new TestTreeModel();
//                        modelLeaf.name = i + "" + j + "" + k + " 王麻子";
//                        modelLeaf.isLeaf = false;
//                        branchChildren.Add(modelLeaf);
//                    }

//                    modelBranch.children = branchChildren;
//                    modelBranch.isExpanded = j == 1 ? true : false;
//                    modelBranch.isChecked = j == 0 ? true : false;
//                    rootChildren.Add(modelBranch);
//                }
//                modelRoot.children = rootChildren;

//>>>>>>> 16414d6237b8147be3cba33bd713bc00681be839
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
            //param.Add("grid", "72a38f57-1939-40e6-8cca-2960e0d994ea");
            param.Add("grid", App.gridUser.gridcell.ToString());
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