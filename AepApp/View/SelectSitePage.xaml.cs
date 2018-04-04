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
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;
            //TodoItemDatabase database =  App.Database;          
            //List<TodoItem> todoItems = await App.Database.GetItemsAsync();
            HeaderList.ItemsSource = await App.Database.GetItemsAsync();
            //Console.WriteLine("数据库内容:" + todoItems);
        }
    }
}