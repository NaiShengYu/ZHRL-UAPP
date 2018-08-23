﻿using AepApp.View.Gridding;
using AepApp.ViewModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace AepApp.MaterialForms.TreeViews
{
    public class GridTreeView : TreeView
    {
        public GridTreeViewModel ViewModel;

        public GridTreeView()
        {
            // these properties have to be set in a specific order, letting us know that we're doing some dumb things with properties and will need to 
            // TODO: fix this later


            NodeCreationFactory =
                () => new TreeNodeView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    BackgroundColor = Color.Blue
                };

            HeaderCreationFactory =
                () =>
                {
                    var result = new GridTreeCardView
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Start
                    };
                    Debug.WriteLine("HeaderCreationFactory: new DemoTreeCardView");
                    return result;
                };
        }

        public void SetDataSource(GridTreeViewModel.TestTreeModel data)
        {
            ViewModel = new GridTreeViewModel(data);
            BindingContext = ViewModel.MyTree;
        }
    }
}
