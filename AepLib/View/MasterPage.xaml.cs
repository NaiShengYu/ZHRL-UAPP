using AepApp.Tools;
//using AepApp.View.SecondaryFunction;
using AepApp.View.Gridding;
using Sample;
using System;
using Xamarin.Forms;

namespace AepApp.View
{
    public partial class MasterPage : ContentPage
    {
        Grid lastselecteditem = null;

        public MasterPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            try
            {
                nameLab.Text = App.userInfo.userName;
                layoutSampling.IsVisible = App.moduleConfigSampling != null;
                layoutSampling.BindingContext = App.moduleConfigSampling;
                menu1.IsVisible = App.moduleConfigSampling != null;
                menu1.BindingContext = App.moduleConfigSampling;
                layoutEP.IsVisible = App.moduleConfigEP360 != null;
                layoutEP.BindingContext = App.moduleConfigEP360;
                //LayoutBasicData.IsVisible = App.

                try
                {
                    layoutGrid.IsVisible = App.moduleConfigEP360.menuGridTask;
                }
                catch (Exception ex)
                {
                }
                if (App.moduleConfigBasicData != null) LayoutBasicData.IsVisible = App.moduleConfigBasicData.showEmeSummary;
                LayoutBasicData.BindingContext = App.moduleConfigBasicData;
                layoutGrid.BindingContext = App.moduleConfigEP360;
                EmegencyLat.BindingContext = App.moduleConfigEmergency;
                menu2.BindingContext = App.moduleConfigENVQ;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    FileUtils.SaveLogFile("public MasterPage()错误：" + ex.ToString());
                }
                //DependencyService.Get<IToast>().ShortAlert("public MasterPage()错误：" + ex.Message);
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                if (lastselecteditem != null) lastselecteditem.BackgroundColor = Color.White;
                lastselecteditem = s as Grid;
                lastselecteditem.BackgroundColor = Color.FromHex("#E7F3FF");

                if (lastselecteditem.Children[0].BindingContext != null)
                {
                    string t = lastselecteditem.Children[0].BindingContext as string;
                    if (t == null)
                    {
                        return;
                    }
                    Type pagetype = Type.GetType(t);
                    var page = (Page)Activator.CreateInstance(pagetype);
                    NavigationPage.SetBackButtonTitle(page, "");
                    Navigation.PushAsync(page);

                }
                App.masterAndDetailPage.IsPresented = false;

            };

            StackLayout[] menus = new StackLayout[8] { menu1, menu2, menu3, layoutEP, layoutSampling, layoutGrid, EmegencyLat, LayoutBasicData };

            foreach (var menu in menus)
            {
                foreach (var child in menu.Children)
                {
                    if (child is Grid)
                    {
                        //if (child.StyleClass != null && child.StyleClass.Contains("menuitem"))
                        //{
                        child.GestureRecognizers.Add(tapGestureRecognizer);
                        //}
                    }
                }
            }

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (App.gridUser == null)
            {
                App.gridUser = await (App.Current as App).getStaffInfo(App.userInfo.id);
            }
            if (App.gridUser == null)
            {
                if (App.gridUser == null)
                {
                    DependencyService.Get<IToast>().ShortAlert("获取网格员信息失败，无法查看");
                    return;
                }
            }
            if (App.gridUser.gridLevel == App.GridMaxLevel)
            {
                //App.masterAndDetailPage.Detail = new NavigationPage(new TaskExamineStaffPage(App.gridUser.grid));
                await Navigation.PushAsync(new TaskExamineStaffPage(App.gridUser.grid));

            }
            else
            {
                //App.masterAndDetailPage.Detail = new NavigationPage(new TaskExaminePage(App.gridUser.grid));
                await Navigation.PushAsync(new TaskExaminePage(App.gridUser.grid));

            }
            App.masterAndDetailPage.IsPresented = false;
        }
        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void TapGestureRegistrationEventPage_Tapped(object sender, EventArgs e)
        {
            if (App.gridUser == null)
            {
                App.gridUser = await (App.Current as App).getStaffInfo(App.userInfo.id);
                //if (App.gridUser == null)
                //{
                //    DependencyService.Get<IToast>().ShortAlert("获取网格员信息失败，无法增加事件");
                //    return;
                //}
            }
            await Navigation.PushAsync(new RegistrationEventPage(""));
            App.masterAndDetailPage.IsPresented = false;
        }

    }
}
