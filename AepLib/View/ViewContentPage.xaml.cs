
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewContentPage : ContentPage
    {
        public ViewContentPage(string content)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(content))
            {
                LabContent.Text = content;
            }

        }
    }
}