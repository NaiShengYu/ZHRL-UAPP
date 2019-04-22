using AepApp.MaterialForms.TreeViews;
using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    //指派网格员
    public partial class GridTreeWithUserPage : ContentPage
    {
        ObservableCollection<TestTreeModel> gridList = new ObservableCollection<TestTreeModel>();
        GridTaskInfoModel _taskModel;
        private List<GridTreeNode> checkModelList = new List<GridTreeNode>();


        private void SetAssignment(TestTreeModel parentModel, ObservableCollection<TestTreeModel> parentChildren, string baseName)
        {
            if (parentModel == null && parentChildren == null)
            {
                return;
            }
            ObservableCollection<TestTreeModel> childern = new ObservableCollection<TestTreeModel>();
            if (parentModel == null)
            {
                parentModel = new TestTreeModel
                {
                    id = Guid.NewGuid(),
                };
                childern = parentChildren;
            }
            else
            {
                childern = parentModel.children;
            }
            foreach (var child in childern)
            {
                if (child.isChecked)
                {
                    Assignments s1 = new Assignments
                    {
                        id = Guid.NewGuid(),
                        rowState = "add",
                        type = child.type.Value,
                    };
                    if (child.type == 0)//网格员
                    {
                        s1.staff = child.id;
                        s1.grid = child.parentId;
                    }
                    else if (child.type == 1)//网格
                    {
                        s1.grid = child.id;
                    }
                    _taskModel.assignments.Add(s1);
                    if (string.IsNullOrWhiteSpace(baseName))
                    {
                        _taskModel.AssignName = _taskModel.AssignName + "  " + parentModel.name + child.name;
                    }
                    else
                    {
                        _taskModel.AssignName = _taskModel.AssignName + "  " + baseName + parentModel.name + child.name;
                    }
                }
                SetAssignment(child, child.children, parentModel.name);
            }
        }

        public GridTreeWithUserPage(GridTaskInfoModel taskModel)
        {
            InitializeComponent();
            checkModelList.Clear();
            getGridAndUser(App.gridUser.grid, gridList);
            _taskModel = taskModel;
            ToolbarItems.Add(new ToolbarItem("确定", "", () =>
            {
                _taskModel.assignments.Clear();
                SetAssignment(null, gridList, "");
                Navigation.PopAsync();
            }));


            MessagingCenter.Unsubscribe<ContentView, TestTreeModel>(this, "ExapndChanged");

            MessagingCenter.Subscribe<ContentView, TestTreeModel>(this, "ExapndChanged", async (arg1, arg2) =>
            {
                var testModel = arg2 as TestTreeModel;
                testModel.isExpanded = !testModel.isChecked;
                if (testModel.children.Count == 0) getGridAndUser(testModel.id, testModel.children);
            });
            MessagingCenter.Unsubscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK);
            MessagingCenter.Subscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK, async (arg1, arg2) =>
            {
                var node = arg2 as GridTreeNode;
                CheckNodeMultiple(node);
            });
        }

        void creatView()
        {
            layoutTree.Children.Clear();
            for (int i = 0; i < gridList.Count; i++)
            {
                GridTreeView tree = new GridTreeView(true)
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                TestTreeModel modelRoot = gridList[i];
                tree.SetDataSource(modelRoot);
                layoutTree.Children.Add(tree);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentView, TestTreeModel>(this, "ExapndChanged");
            MessagingCenter.Unsubscribe<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK);
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

        //加载某网格下的所有子网格，另外选择性加载同一网格下的网格员
        private async void getGridAndUser(Guid grid, ObservableCollection<TestTreeModel> children)
        {

            string url = App.EP360Module.url + "/api/gbm/GetGridCellUnder";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("grid", grid);
            param.Add("includeStaff", "true");
            string pa = JsonConvert.SerializeObject(param);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(url, pa, "POST", App.FrameworkToken);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var resultList = JsonConvert.DeserializeObject<ObservableCollection<GridCellUnder>>(hTTPResponse.Results);

                    for (int i = 0; i < resultList.Count; i++)
                    {
                        GridCellUnder under = resultList[i];
                        under.parentId = grid;
                        TestTreeModel testTreeModel = new TestTreeModel
                        {
                            children = new ObservableCollection<TestTreeModel>(),
                            parentId = under.parentId,
                            id = under.id,
                            name = under.name,
                            count = under.count,
                            type = under.type,

                        };
                        testTreeModel.isLeaf = false;
                        if (testTreeModel.count == null) testTreeModel.count = 0;
                        if (testTreeModel.type == null) testTreeModel.type = 0;
                        if (testTreeModel.count == 0) testTreeModel.isLeaf = true;
                        if (testTreeModel.type == 0) testTreeModel.isLeaf = true;

                        foreach (var s1 in _taskModel.assignments)
                        {
                            if (s1.staff == testTreeModel.id || s1.grid == testTreeModel.id) testTreeModel.isChecked = true;
                        }



                        children.Add(testTreeModel);

                    }

                    creatView();
                }
                catch (Exception ex)
                {

                }

            }

        }


        private class GridCellUnder
        {
            public Guid parentId { get; set; }//父级id（及所属网格id）
            public Guid id { get; set; }
            public string name { get; set; }
            public int? count { get; set; }
            public int? type { get; set; }
        }


    }
}
