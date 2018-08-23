using AepApp.MaterialForms.TreeViews;
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
	public partial class TestTreeViewPage : ContentPage
	{
        public TestTreeViewPage()
        {
            InitializeComponent();
            foreach (GridTreeView t in layoutTree.Children)
            {
                t.SetDataSource(null);
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
                if(root.IsChecked != check)
                {
                    root.IsChecked = check;
                }
                foreach (GridTreeNode node in t.ViewModel.MyTree.Descendants)
                {
                    if(node.IsChecked == check)
                    {
                        continue;
                    }
                    node.IsChecked = check;
                }
            }
        }
    }
}