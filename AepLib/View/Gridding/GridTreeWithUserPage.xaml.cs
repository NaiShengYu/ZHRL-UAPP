using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AepApp.MaterialForms.TreeViews;
using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    public partial class GridTreeWithUserPage : ContentPage
    {
        ObservableCollection<TestTreeModel> gridList = new ObservableCollection<TestTreeModel>();
        GridTaskInfoModel _taskModel;
        public GridTreeWithUserPage(GridTaskInfoModel taskModel)
        {
            InitializeComponent();
            getGridAndUser(Guid.Parse("72a38f57-1939-40e6-8cca-2960e0d994ea"), gridList);
            _taskModel = taskModel;
            ToolbarItems.Add(new ToolbarItem("确定", "", () => {
                _taskModel.assignments.Clear();
                foreach (var rootModel in gridList)
                {
                    if (rootModel.isChecked)
                    {
                        Assignments s1 = new Assignments
                        {
                            id = Guid.NewGuid(),
                            rowState = "add",
                            type = rootModel.type.Value,
                        };
                        if (rootModel.type == 0) s1.staff = rootModel.id;
                        if (rootModel.type == 1) s1.grid = rootModel.id;
                        taskModel.assignments.Add(s1);
                        taskModel.AssignName = taskModel.AssignName + "  " + rootModel.name;
                    }
                    else
                    {
                        foreach (var children1 in rootModel.children)
                        {
                            if (children1.isChecked)
                            {
                                Assignments s1 = new Assignments
                                {
                                    id = Guid.NewGuid(),
                                    rowState = "add",
                                    type = children1.type.Value,
                                };
                                if (children1.type == 0) {
                                    s1.staff = children1.id;
                                    s1.grid = rootModel.id;
                                }
                                if (children1.type == 1) s1.grid = children1.id;
                                taskModel.assignments.Add(s1);
                                taskModel.AssignName = taskModel.AssignName + "  " + rootModel.name + children1.name;

                            }
                            else
                            {
                                foreach (var children2 in children1.children)
                                {
                                    if (children2.isChecked)
                                    {
                                        Assignments s1 = new Assignments
                                        {
                                            id = Guid.NewGuid(),
                                            rowState = "add",
                                            type = children1.type.Value,
                                        };
                                        if (children2.type == 0) {
                                            s1.staff = children2.id;
                                            s1.grid = children1.id;
                                        }
                                        if (children2.type == 1) s1.grid = children2.id;
                                        taskModel.assignments.Add(s1);
                                        taskModel.AssignName = taskModel.AssignName + "  " + rootModel.name + children1.name + children2.name;
                                    }
                                    else
                                    {


                                    }
                                }


                            }
                        }

                    }

                }
                Navigation.PopAsync();
            }));


            MessagingCenter.Unsubscribe<ContentView, TestTreeModel>(this, "ExapndChanged");

            MessagingCenter.Subscribe<ContentView, TestTreeModel>(this, "ExapndChanged", async (arg1, arg2) =>
            {
                var testModel = arg2 as TestTreeModel;
                testModel.isExpanded = !testModel.isChecked;
                if (testModel.children.Count == 0) getGridAndUser(testModel.id, testModel.children);
            });

        }

        void creatView()
        {
            layoutTree.Children.Clear();
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentView, TestTreeModel>(this, "ExapndChanged");

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
                        TestTreeModel testTreeModel = new TestTreeModel
                        {
                            children = new ObservableCollection<TestTreeModel>(),
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
            public Guid id { get; set; }
            public string name { get; set; }
            public int? count { get; set; }
            public int? type { get; set; }
        }


    }
}
