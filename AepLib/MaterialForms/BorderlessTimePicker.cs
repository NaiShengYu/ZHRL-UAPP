using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class BorderlessTimePicker : TimePicker
    {
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create("HasBorder", typeof(bool), typeof(BorderlessTimePicker), false);

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

        public static readonly BindableProperty XAlignProperty =
           BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(BorderlessTimePicker),
           TextAlignment.Start);

        public TextAlignment XAlign
        {
            get { return (TextAlignment)GetValue(XAlignProperty); }
            set { SetValue(XAlignProperty, value); }
        }


        public static readonly BindableProperty TextSizeProperty =
            BindableProperty.Create("TSize", typeof(float), typeof(BorderlessTimePicker), 17.0f);

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
