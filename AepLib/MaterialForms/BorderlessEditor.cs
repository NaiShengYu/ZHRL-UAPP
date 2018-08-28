using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class BorderlessEditor : Editor
    {
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(BorderlessEditor), false);

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }
    }
}
