using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo;
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
                TodoItem item = (TodoItem)e.SelectedItem;

                for (int i = 0; i < App.todoItemList.Count; i++){
                    TodoItem todoitem = App.todoItemList[i];
                  
                    if (todoitem.isCurrent == true)//找到上一个选中的站点
                    {
                        if (todoitem.SiteAddr == item.SiteAddr)//如果选中的站点和原来的相同退出循环
                            return;
                        
                        todoitem.ID = 1;
                        todoitem.isCurrent = false;
                        App.Database.SaveItemAsync(todoitem);
                    }
                }

                item.isCurrent = true;
                App.Database.SaveItemAsync(item);
                HeaderList.ItemsSource = App.todoItemList;

                //DependencyService.Get<Sample.IToast>().ShortAlert(a + "");
                //HeaderList.SelectedItem = a;
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
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();
            HeaderList.ItemsSource = todoItems;
            App.todoItemList = todoItems;
            //Console.WriteLine("数据库内容:" + todoItems);
        }
    }
}