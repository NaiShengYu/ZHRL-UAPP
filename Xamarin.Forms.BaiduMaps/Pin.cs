using System;

namespace Xamarin.Forms.BaiduMaps
{
    public class Pin : Annotation
    {
        // Animate
        public static readonly BindableProperty AnimateProperty = BindableProperty.Create(
            propertyName: nameof(Animate),
            returnType: typeof(bool),
            declaringType: typeof(Pin),
            defaultValue: default(bool)
        );

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        // Draggable
        public static readonly BindableProperty DraggableProperty = BindableProperty.Create(
            propertyName: nameof(Draggable),
            returnType: typeof(bool),
            declaringType: typeof(Pin),
            defaultValue: default(bool)
        );

        public bool Draggable
        {
            get { return (bool)GetValue(DraggableProperty); }
            set { SetValue(DraggableProperty, value); }
        }

        // Enabled3D
        public static readonly BindableProperty Enabled3DProperty = BindableProperty.Create(
            propertyName: nameof(Enabled3D),
            returnType: typeof(bool),
            declaringType: typeof(Pin),
            defaultValue: default(bool)
        );

        public bool Enabled3D
        {
            get { return (bool)GetValue(Enabled3DProperty); }
            set { SetValue(Enabled3DProperty, value); }
        }

        // Image
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(
            propertyName: nameof(Image),
            returnType: typeof(XImage),
            declaringType: typeof(Pin),
            defaultValue: default(XImage)
        );

        public XImage Image
        {
            get { return (XImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }


        public static readonly BindableProperty AnchorXProperty = BindableProperty.Create(
            propertyName: nameof(AnchorX),
            returnType: typeof(float),
            declaringType: typeof(Pin),
            defaultValue: 0.0f
        );

        public float AnchorX
        {
            get { return (float)GetValue(AnchorXProperty); }
            set { SetValue(AnchorXProperty, value); }
        }

        public static readonly BindableProperty AnchorYProperty = BindableProperty.Create(
            propertyName: nameof(AnchorY),
            returnType: typeof(float),
            declaringType: typeof(Pin),
            defaultValue: 0.0f
        );

        public float AnchorY
        {
            get { return (float)GetValue(AnchorYProperty); }
            set { SetValue(AnchorYProperty, value); }
        }


    }
}

