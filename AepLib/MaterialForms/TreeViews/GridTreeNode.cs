using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AepApp.MaterialForms.TreeViews
{
    public class GridTreeNode : TreeNode<GridTreeNode>
    {
        public ICommand ToggleIsExpandedCommand { protected set; get; }
        public ICommand ToggleIsCheckedCommand { protected set; get; }

        // normal view model properties provide the content as well as the visual state

        bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { Set("IsExpanded", ref _IsExpanded, value); }
        }

        bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                Set("IsChecked", ref _IsChecked, value);
            }
        }

        bool _IsLeaf;
        public bool IsLeaf
        {
            get { return _IsLeaf; }
            set { Set("IsLeaf", ref _IsLeaf, value); }
        }
        // we're 100% in control of the indentation level, if any, that we use in rendering our tree nodes
        public double IndentWidth { get { return (double)(Depth * 30); } }

        string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { Set("Title", ref _Title, value); }
        }

        double _Score = 0;
        public double Score
        {
            get { return _Score; }
            set { Set("Score", ref _Score, value); }
        }

        int _Total = 0;
        public int Total
        {
            get { return _Total; }
            set { Set("Total", ref _Total, value); }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "Depth")
            {
                Debug.WriteLine("DemoTreeNode.Depth property changed - triggering update for IndentWidth");
                base.OnPropertyChanged("IndentWidth");
            }
        }

        //选择或取消选择
        private void CheckEvent()
        {
            IsChecked = !IsChecked;
            foreach (GridTreeNode node in Descendants)
            {
                if(node.IsChecked == IsChecked)
                {
                    continue;
                }
                node.IsChecked = IsChecked;
            }

            GridTreeNode p = Parent as GridTreeNode;
            while (p != null)
            {
                int checkedCount = 0;
                foreach (GridTreeNode node in p.ChildNodes)
                {
                    if (!node.IsChecked)
                    {
                        p.IsChecked = false;
                        break;
                    }
                    checkedCount++;
                }
                if (checkedCount == p.Children.Count)
                {
                    p.IsChecked = true;
                }
                p = p.Parent as GridTreeNode;
            }


        }

        public GridTreeNode()
        {
            ToggleIsExpandedCommand = new Command(obj => IsExpanded = !IsExpanded);
            ToggleIsCheckedCommand = new Command(obj => CheckEvent());
        }

        public override string ToString()
        {
            return string.Format("DemoTreeNode: Title={3}, Score={4}, IsExpanded={1}, IndentWidth={2} " + base.ToString(), ToggleIsExpandedCommand, IsExpanded, IndentWidth, Title, Score);
        }
    }
}
