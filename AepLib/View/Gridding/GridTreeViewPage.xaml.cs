using AepApp.MaterialForms.TreeViews;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridTreeViewPage : ContentPage
    {

        private bool showChildrenCount = false;//是否显示子节点数目
        private bool isSingleCheck = false;//是否单选
        ObservableCollection<TestTreeModel> gridList = new ObservableCollection<TestTreeModel>();
        private GridTreeNode lastCheckNode;
        private List<GridTreeNode> checkModelList = new List<GridTreeNode>();

        public GridTreeViewPage(bool isSingle, bool showCount)
        {
            InitializeComponent();
            showChildrenCount = showCount;
            isSingleCheck = isSingle;
            ButtonAll.IsVisible = !isSingleCheck;
            lastCheckNode = null;
            checkModelList.Clear();
            MessagingCenter.Unsubscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK);
            MessagingCenter.Subscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK, async (arg1, arg2) =>
            {
                var node = arg2 as GridTreeNode;
                if (isSingleCheck)
                {
                    CheckNodeSingle(node);
                }
                else
                {
                    CheckNodeMultiple(node);
                }
            });

            //creatView();
            getGridInfo();

        }

        public void creatView()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                GridTreeView tree = new GridTreeView(showChildrenCount)
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                TestTreeModel modelRoot = gridList[i];
                modelRoot.isLeaf = modelRoot.children.Count == 0;


                //TestTreeModel modelRoot = new TestTreeModel();
                //modelRoot.id = Guid.NewGuid();
                //modelRoot.name = i + "高桥镇";
                //ObservableCollection<TestTreeModel> rootChildren = new ObservableCollection<TestTreeModel>();
                //for (int j = 0; j < 3; j++)
                //{
                //    TestTreeModel modelBranch = new TestTreeModel();
                //    modelBranch.id = Guid.NewGuid();
                //    modelBranch.name = i + "" + j + "小牙山村";
                //    ObservableCollection<TestTreeModel> branchChildren = new ObservableCollection<TestTreeModel>();
                //    for (int k = 0; k < 2; k++)
                //    {
                //        TestTreeModel modelLeaf = new TestTreeModel();
                //        modelLeaf.id = Guid.NewGuid();
                //        modelLeaf.name = i + "" + j + "" + k + " 王麻子";
                //        modelLeaf.isLeaf = false;
                //        branchChildren.Add(modelLeaf);
                //    }

                //    modelBranch.children = branchChildren;
                //    modelBranch.isExpanded = j == 1 ? true : false;
                //    modelBranch.isChecked = false;
                //    rootChildren.Add(modelBranch);
                //}
                //modelRoot.children = rootChildren;

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

        void Button_Clicked_Confirm(object sender, EventArgs e)
        {
            if (isSingleCheck)
            {
                //DisplayAlert("选择", lastCheckNode != null ? lastCheckNode.Title : "无", "确定");
                MessagingCenter.Send<ContentPage, TestTreeModel>(this, 
                    SubcriberConst.MSG_SELECT_GRIDER, 
                    lastCheckNode == null ? null : lastCheckNode.testTreeModel);
            }
            else
            {
                //DisplayAlert("选择", "共：" + checkModelList.Count, "确定");
            }
            Navigation.PopAsync();
        }

        //选择或取消选择 -- 单选
        public void CheckNodeSingle(GridTreeNode nodes)
        {
            if (nodes == null)
            {
                return;
            }
            bool oldState = nodes.IsChecked;
            if (oldState)//本来已选中
            {
                nodes.IsChecked = !oldState;
                lastCheckNode = null;
            }
            else//未选中
            {
                if (lastCheckNode != null)
                {
                    if (lastCheckNode.testTreeModel.id != nodes.testTreeModel.id)//当前节点 不是上次选中节点
                    {
                        DependencyService.Get<IToast>().ShortAlert("最多选择一个");
                    }
                    else
                    {
                        nodes.IsChecked = !oldState;
                    }
                }
                else
                {
                    nodes.IsChecked = !oldState;
                    lastCheckNode = nodes;
                }
            }


        }

        //选择或取消选择 -- 多选
        public void CheckNodeMultiple(GridTreeNode nodes)
        {
            nodes.IsChecked = !nodes.IsChecked;
            nodes.testTreeModel.isChecked = nodes.IsChecked;
            foreach (GridTreeNode node in nodes.Descendants)
            {
                node.IsChecked = !node.IsChecked;
                node.testTreeModel.isChecked = node.IsChecked;
                if (node.IsChecked)
                {
                    checkModelList.Add(node);
                }
                else
                {
                    checkModelList.Remove(node);
                }
            }

            GridTreeNode p = nodes.Parent as GridTreeNode;
            while (p != null)
            {
                int checkedCount = 0;
                foreach (GridTreeNode node in p.ChildNodes)
                {
                    if (!node.IsChecked)
                    {
                        p.IsChecked = false;
                        p.testTreeModel.isChecked = p.IsChecked;
                        checkModelList.Remove(p);
                        break;
                    }
                    checkedCount++;
                }
                if (checkedCount == p.Children.Count)
                {
                    p.IsChecked = true;
                    p.testTreeModel.isChecked = p.IsChecked;
                    checkModelList.Add(p);
                }
                p = p.Parent as GridTreeNode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="check">是否全选</param>
        private void CheckAll(bool check)
        {
            checkModelList.Clear();
            foreach (GridTreeView t in layoutTree.Children)
            {
                GridTreeNode root = t.ViewModel.MyTree;
                if (root.IsChecked != check)
                {
                    root.IsChecked = check;
                }
                if (check)
                {
                    checkModelList.Add(root);
                }
                foreach (GridTreeNode node in t.ViewModel.MyTree.Descendants)
                {
                    if (check)
                    {
                        checkModelList.Add(node);
                    }
                    if (node.IsChecked == check)
                    {
                        continue;
                    }
                    node.IsChecked = check;
                }
            }
        }

        //获取网格
        private async void getGridInfo()
        {

            string url = App.EP360Module.url + "/api/gbm/GetGridList";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("grid", App.gridUser.grid);
            param.Add("searchKey", "");
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK);
        }

    }
}