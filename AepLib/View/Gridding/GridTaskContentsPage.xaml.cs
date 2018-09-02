
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AepApp.Interface;
using AepApp.Models;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class GridTaskContentsPage : ContentPage
    {
        GridTaskInfoModel _infoModel = null;
        public GridTaskContentsPage(GridTaskInfoModel taskModel)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            _infoModel = taskModel;
            if (!taskModel.canEdit)
            {
                var source = new HtmlWebViewSource();
                source.Html = _infoModel.Contents;
                Webview.Source = source;
            }
            else
            {
                var source = new UrlWebViewSource();
                var rootPath = DependencyService.Get<IWebviewService>().Get();
                source.Url = System.IO.Path.Combine(rootPath, "EditorHome.html");
                Webview.Source = source;
            }


             ToolbarItems.Add(new ToolbarItem("确定", "",async ()  =>
            {
                try
                {
                    var result =await _infoModel.EvaluateJavascript("javascript:getEditorValue();");
                    string content = Regex.Unescape(result);                    
                    _infoModel.Contents = content;
                }
                catch (Exception ex)
                {

                }
                Navigation.PopAsync();
            }));

            BindingContext = _infoModel;

        }

        private async void Webview_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (_infoModel.canEdit && !string.IsNullOrWhiteSpace(_infoModel.Contents))
            {
                await _infoModel.EvaluateJavascript("javascript:setEditorValue('" + _infoModel.Contents + "');");
            }

        }

         

    }
}
