using AepApp.MaterialForms.TreeViews;
using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AepApp.ViewModel
{
    public class GridTreeViewModel : ObservableObject
    {
        GridTreeNode _MyTree;
        public GridTreeNode MyTree
        {
            get { return _MyTree; }
            set { Set("MyTree", ref _MyTree, value); }
        }

        public ICommand AddNodeCommand { protected set; get; }

        static Random random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Day);
        
        public GridTreeViewModel(TestTreeModel data)
        {
            if (data == null)
            {
                return;
            }

            MyTree = new GridTreeNode
            {
                Title = data.name,
            };
            AddChildren(data, MyTree);
        }

        private void AddChildren(TestTreeModel model, GridTreeNode treeNode)
        {
            if (model == null || model.children == null || model.children.Count == 0 || treeNode == null)
            {
                return;
            }
            foreach (TestTreeModel m in model.children)
            {
                GridTreeNode node = new GridTreeNode { Title = m.name, IsChecked = m.isChecked, IsExpanded = m.isExpanded, IsLeaf = m.isLeaf };
                treeNode.Children.Add(node);
                AddChildren(m, node);
            }
        }

        public class TestTreeModel : BaseModel
        {
            public string name { get; set; }
            private ObservableCollection<TestTreeModel> Children;
            public ObservableCollection<TestTreeModel> children { get { return Children; } set { Children = value; NotifyPropertyChanged(); } }
            public bool isLeaf { get; set; }
            public bool isChecked { get; set; }
            public bool isExpanded { get; set; }
        }
    }
}
