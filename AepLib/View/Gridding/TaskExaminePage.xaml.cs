using AepApp.Models;
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
    public partial class TaskExaminePage : ContentPage
    {

        TaskExamineModel model;
        public TaskExaminePage()
        {
            InitializeComponent();
            model = new TaskExamineModel
            {
                date = DateTime.Now,
                total = 239,
                finished = 200,
                lastRatio = "87.33%",
            };
            List<string> list = new List<string>();
            for (int i = 0; i < 15; i++)
            {
                list.Add(i + "");
            }
            ListView.ItemsSource = list;
            BindingContext = model;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TaskExamineModel taskExamine = e.SelectedItem as TaskExamineModel;
            if (taskExamine == null)
            {
                return;
            }
            Navigation.PushAsync(new TaskExamineStaffPage());
            ListView.SelectedItem = null;
        }


    }
}