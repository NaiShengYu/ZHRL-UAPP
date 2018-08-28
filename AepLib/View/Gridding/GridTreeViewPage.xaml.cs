using AepApp.MaterialForms.TreeViews;
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
        public GridTreeViewPage()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                GridTreeView tree = new GridTreeView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                TestTreeModel modelRoot = new TestTreeModel();
                modelRoot.name = i + "高桥镇";
                ObservableCollection<TestTreeModel> rootChildren = new ObservableCollection<TestTreeModel>();
                for (int j = 0; j < 3; j++)
                {
                    TestTreeModel modelBranch = new TestTreeModel();
                    modelBranch.name = i + "" + j + "小牙山村";
                    ObservableCollection<TestTreeModel> branchChildren = new ObservableCollection<TestTreeModel>();
                    for (int k = 0; k < 2; k++)
                    {
                        TestTreeModel modelLeaf = new TestTreeModel();
                        modelLeaf.name = i + "" + j + "" + k + " 王麻子";
                        modelLeaf.isLeaf = false;
                        branchChildren.Add(modelLeaf);
                    }

                    modelBranch.children = branchChildren;
                    modelBranch.isExpanded = j == 1 ? true : false;
                    modelBranch.isChecked = j == 0 ? true : false;
                    rootChildren.Add(modelBranch);
                }
                modelRoot.children = rootChildren;

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
    }
}