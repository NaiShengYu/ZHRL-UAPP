using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace AepApp.MaterialForms
{
    public class ToggleView : ContentView
    {
        private Image ToggleImage;

        public ToggleView()
        {
            ToggleImage = new Image
            {
                Source = UnCheckedImaage,
            };

            this.Content = ToggleImage;

            var ges = new TapGestureRecognizer();
            ges.Tapped += Ges_Tapped;
            this.GestureRecognizers.Add(ges);
        }

        private void Ges_Tapped(object sender, EventArgs e)
        {
            if (ToggleImage.Source == UnCheckedImaage)
            {
                ToggleImage.Source = CheckedImaage;
                Checked = true;
            }
            else
            {
                ToggleImage.Source = UnCheckedImaage;
                Checked = false;
            }
        }


        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create<ToggleView, bool>(p => p.Checked, false);

        public bool Checked
        {
            get { return (bool)base.GetValue(CheckedProperty); }
            set { base.SetValue(CheckedProperty, value); }
        }

        public static readonly BindableProperty CheckedImageProperty =
            BindableProperty.Create<ToggleView, ImageSource>(p => p.CheckedImaage, null);

        public ImageSource CheckedImaage
        {
            get { return (ImageSource)base.GetValue(CheckedImageProperty); }
            set { base.SetValue(CheckedImageProperty, value); }
        }

        public static readonly BindableProperty UnCheckedImageProperty =
            BindableProperty.Create<ToggleView, ImageSource>(p => p.UnCheckedImaage, null);

        public ImageSource UnCheckedImaage
        {
            get { return (ImageSource)base.GetValue(UnCheckedImageProperty); }
            set { base.SetValue(UnCheckedImageProperty, value); }
        }
    }
}
