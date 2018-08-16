using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class BorderlessDatePicker : DatePicker
    {
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(BorderlessDatePicker), false);

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

        public static readonly BindableProperty XAlignProperty =
           BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(BorderlessDatePicker),
           TextAlignment.Start);

        /// <summary>
        /// 对齐方式
        /// </summary>
        public TextAlignment XAlign
        {
            get { return (TextAlignment)GetValue(XAlignProperty); }
            set { SetValue(XAlignProperty, value); }
        }

        public static readonly BindableProperty TextSizeProperty =
            BindableProperty.Create("TSize", typeof(float), typeof(BorderlessDatePicker), 17.0f);

        /// <summary>
        /// 字体大小
        /// </summary>
        public float TSize
        {
            get { return (float)GetValue(TextSizeProperty); }
            set { SetValue(TextSizeProperty, value); }
        }
    }
}
