using System;
using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class BorderlessEntry : Entry
    {
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(BorderlessEntry), false);

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

    }
}
