using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskTemplateInfoPage : ContentPage
    {
        public TaskTemplateInfoPage()
        {
            InitializeComponent();
            var htmSource = new HtmlWebViewSource();
            htmSource.Html = @"
                  <h1>Xamarin.Forms</h1>
                  <p>Welcome to WebView.<br/>
                    Welcome to WebView.<br/>
                    Welcome to WebView.<br/>
                    Welcome to WebView.<br/>
                    Welcome to WebView.<br/>
                    Welcome to WebView.<br/>
                    End.</p>";
            contentWebview.Source = htmSource;
            reportWebview.Source = htmSource;
        }

    }
}