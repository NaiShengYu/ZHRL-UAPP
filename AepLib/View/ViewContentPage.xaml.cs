
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewContentPage : ContentPage
    {
        public ViewContentPage(string title, string content)
        {
            InitializeComponent();
            Title = string.IsNullOrWhiteSpace(title) ? "查看" : title;
            if (!string.IsNullOrWhiteSpace(content))
            {
                LabContent.Text = content;
            }

        }
    }
}