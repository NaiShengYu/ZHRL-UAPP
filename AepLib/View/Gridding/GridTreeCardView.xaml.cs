using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AepApp.MaterialForms.TreeViews;
using static AepApp.ViewModel.GridTreeViewModel;

namespace AepApp.View.Gridding
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridTreeCardView : ContentView
    {
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
    }
}