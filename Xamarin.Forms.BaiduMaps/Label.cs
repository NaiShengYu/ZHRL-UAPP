using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.BaiduMaps
{
    public class Label : Annotation
    {
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(BackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(Label),
            defaultValue: new Color(0.0, 0.2, 0.5)
        );

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly BindableProperty FontColorProperty = BindableProperty.Create(
            propertyName: nameof(FontColor),
            returnType: typeof(Color),
            declaringType: typeof(Label),
            defaultValue: new Color(1, 1, 1)
        );

        public Color FontColor
        {
            get { return (Color)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(int),
            declaringType: typeof(Label),
            defaultValue: 24
        );

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }


    }
}
