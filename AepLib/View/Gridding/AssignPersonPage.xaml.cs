using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class AssignPersonPage : ContentPage
    {

        void AssignPerson(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AssignPersonInfoPage());
        }
        public AssignPersonPage()
        {
            InitializeComponent();


            ToolbarItems.Add(new ToolbarItem("确定","",() => {
                Navigation.PopAsync();

            }));


        }
    }
}
