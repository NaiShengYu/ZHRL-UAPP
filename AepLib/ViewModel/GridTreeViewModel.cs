using AepApp.MaterialForms.TreeViews;
using System;
using System.Collections.Generic;
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
                MyTree = new GridTreeNode { Title = "Root1", Score = 0.5, IsExpanded = false }; ;

                var a = MyTree.Children.Add(new GridTreeNode { Title = "Branch A", Score = 0.75, IsExpanded = false });


                var b = a.Children.Add(new GridTreeNode { Title = "Leaf A1", Score = 0.85, IsExpanded = true, IsChecked = true, IsLeaf = true, });
                a.Children.Add(new GridTreeNode { Title = "Leaf A2", Score = 0.65, IsExpanded = true, IsChecked = false, IsLeaf = true, });
                var c = a.Children.Add(new GridTreeNode { Title = "Leaf A3", Score = 0.65, IsExpanded = true, IsChecked = true, IsLeaf = false, });
                c.Children.Add(new GridTreeNode { Title = "Leaf A31", Score = 0.65, IsExpanded = true, IsChecked = false, IsLeaf = true, });

                b = MyTree.Children.Add(new GridTreeNode { Title = "Branch B", Score = 0.25, IsExpanded = false });
                b.Children.Add(new GridTreeNode { Title = "Leaf B1", Score = 0.35, IsExpanded = true, IsLeaf = true, });
                b.Children.Add(new GridTreeNode { Title = "Leaf B2", Score = 0.15, IsExpanded = true, IsLeaf = true, });
            }
            else
            {
                //todo
                MyTree = new GridTreeNode
                {
                    Title = data.name,
                };
                TreeNodeList<GridTreeNode> children = MyTree.Children;
                List<TestTreeModel> l = data.children;
                while (l != null)
                {
                    foreach (TestTreeModel t in l)
                    {
                        ITreeNode<GridTreeNode> node = children.Add(new GridTreeNode { Title = t.name });
                    }

                }
            }
        }

        public class TestTreeModel
        {
            public string name { get; set; }
            public List<TestTreeModel> children { get; set; }
        }
    }
}
