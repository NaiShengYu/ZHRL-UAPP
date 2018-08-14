using AepApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AepApp.View.Gridding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventHandleProcessPage : ContentPage
    {
        //private ObservableCollection<GridTaskModel> taskList = new ObservableCollection<GridTaskModel>();
        //private GridEventModel eventModel;

        public EventHandleProcessPage(GridEventModel eventM)
        {
            InitializeComponent();
            if (eventM == null)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(eventM.addTime))
            {
                registLayout.IsVisible = true;
            }
            if (!string.IsNullOrWhiteSpace(eventM.townHandleTime))
            {
                townLayout.IsVisible = true;
            }
            if (!string.IsNullOrWhiteSpace(eventM.countryHandleTime))
            {
                countryLayout.IsVisible = true;
            }
            if (!string.IsNullOrWhiteSpace(eventM.finishTime))
            {
                finishLayout.IsVisible = true;
            }

            if (eventM.taskList == null || eventM.taskList.Count == 0)
            {
                taskLayout.IsVisible = false;
                gridRoot.RowDefinitions[3].Height = 0;
            }
            else
            {
                taskLayout.IsVisible = true;
                taskListView.ItemsSource = eventM.taskList;
            }

        }

        public void Handle_TaskItem(Object sender, SelectedItemChangedEventArgs e)
        {
            GridTaskModel taskM = e.SelectedItem as GridTaskModel;
            if (taskM == null)
            {
                return;
            }
            taskListView.SelectedItem = null;
        }


    }
}