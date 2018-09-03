using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AepApp.MaterialForms.TreeViews;
using static AepApp.ViewModel.GridTreeViewModel;
using AepApp.Tools;

namespace AepApp.View.Gridding
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridTreeCardView : ContentView
    {
        /// <summary>
        /// 展开/收缩节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Button button = sender as Button;
            GridTreeNode note = button.BindingContext as GridTreeNode;
            MessagingCenter.Send<ContentView, TestTreeModel>(this, "ExapndChanged", note.testTreeModel);
        }

        public GridTreeCardView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择/取消选择节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCheck_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            GridTreeNode node = button.BindingContext as GridTreeNode;
            MessagingCenter.Send<ContentView, GridTreeNode>(this, SubcriberConst.MSG_TREEVIEW_NODE_CHECK, node);
        }
    }
}