using Sample;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AepApp.View.EnvironmentalEmergency
{
    public partial class ShowFilePage : ContentPage
    {
        public ShowFilePage(string info) : this()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, info);
            if (File.Exists(filename))
            {
                DependencyService.Get<IToast>().LongAlert("存在");

                FileStream fs = File.Create(filename);
                if (fs != null)
                {
                    DependencyService.Get<IToast>().LongAlert("文件长度：" + fs.Length);
                    Console.WriteLine("---------------文件长度：" + fs.Length);
                }
                fs.Close();
            }
            else
            {
                DependencyService.Get<IToast>().LongAlert("不存在");
            }

            web.Source = filename;
        }

        public ShowFilePage(string url, bool isFromNet) : this()
        {
            if (isFromNet)
            {
                web.Source = url;
            }
            else
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filename = Path.Combine(path, url);
                web.Source = filename;
            }
        }

        public ShowFilePage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            Title = "附件";
        }
    }
}
