﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class BorderlessPicker : Picker
    {
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(BorderlessPicker), false);

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

        public static readonly BindableProperty XAlignProperty =
           BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(BorderlessPicker),
           TextAlignment.Start);

        public TextAlignment XAlign
        {
            get { return (TextAlignment)GetValue(XAlignProperty); }
            set { SetValue(XAlignProperty, value); }
        }
    }
}
