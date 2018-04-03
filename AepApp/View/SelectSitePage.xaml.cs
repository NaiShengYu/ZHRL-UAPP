using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectSitePage : ContentPage
    {
        public SelectSitePage()
        {
            InitializeComponent();
            this.Title = "选择站点";          
            HeaderList.ItemsSource = new List<string>() {
                "List Item 1","List Item 2","List Item 3","List Item 4","List Item 5"
            };
            HeaderList.ItemSelected += (sender, e) => {
                var a = e.SelectedItem;            
                DependencyService.Get<Sample.IToast>().ShortAlert(a + "");
                HeaderList.SelectedItem = a;
            };
            HeaderList.ItemTapped += HeaderList_ItemTapped;
        }

        private void HeaderList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var a = e.SelectedItem;
            //DependencyService.Get<Sample.IToast>().ShortAlert(a + "");
            //HeaderList.SelectedItem = a;
           // throw new NotImplementedException();
        }

        private void Select_site(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddSite());
            //DependencyService.Get<Sample.IToast>().ShortAlert("选择站点");
            //Navigation.PushAsync(new SelectSitePage());
        }
    }
}