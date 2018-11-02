using AepApp.Models;
using AepApp.View;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo;
using Xamarin.Forms;

namespace AepApp.AuxiliaryExtension
{
    public class AddSiteUtil
    {
        private SelectSitePage selectSitePage;
        private String result;
        public AddSiteUtil(SelectSitePage selectSitePage)
        {
            this.selectSitePage = selectSitePage;
        }

        internal async void reqSiteInfo(string siteName, string siteUrl, ObservableCollection<TodoItem> dataList)
        {

            // Console.WriteLine("添加站点");                
            string[] s = siteUrl.Split(new char[] { ':' });
            string siteU = "";
            try
            {
                siteU = s[1];
            }
            catch (Exception ex)
            {

            }
            string uri = siteUrl + "/api/mod/GetWebInfo";
            //string uri = "https://" + siteUrl + "/api/login/getstationName?stationurl=" + s[0];
            Console.WriteLine("站点接口:" + uri);
            HTTPResponse respones = await EasyWebRequest.SendHTTPRequestAsync(uri, "", "GET", "", "");

            if (respones.StatusCode != System.Net.HttpStatusCode.OK)
            {
                await selectSitePage.DisplayAlert("提示", "站点添加失败", "确定");
                return;
            }

            bool isContainSite = false;
            try
            {
                AddSitePageModel model = JsonConvert.DeserializeObject<AddSitePageModel>(respones.Results);
                if (model != null)
                {
                    TodoItem todoItem = new TodoItem();
                    //todoItem.Name = model.name;
                    todoItem.Name = siteName;
                    todoItem.customerName = model.customerName;
                    todoItem.appTitle = model.appTitle;
                    todoItem.majorVersion = model.majorVersion;
                    todoItem.minorVersion = model.minorVersion;
                    todoItem.revision = model.revision;
                    todoItem.date = model.date;

                    todoItem.SiteAddr = siteUrl;
                    todoItem.isCurrent = true;
                    if (dataList.Count != 0)
                    {
                        foreach (var item in dataList) //遍历站点数据
                        {
                            if (item.SiteAddr.Equals(todoItem.SiteAddr))
                            {
                                isContainSite = true;
                                break;
                            }
                        }
                    }

                    if (!isContainSite)
                    {
                        selectSitePage.SaveData(todoItem);
                        //CrossHud.Current.Dismiss();
                    }
                    else
                    {
                        selectSitePage.CloseAddSitePage(true);
                        //CrossHud.Current.Dismiss();
                        //Navigation.PopAsync();
                    }
                    //Console.WriteLine("ex:" + model);
                    //添加站点
                }
                else
                {
                    selectSitePage.HideCrossHud();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                selectSitePage.HideCrossHud();
            }

        }


        public static async Task<TodoItem> getCurrentSite()
        {
            //获取数据库的数据
            ((App)App.Current).ResumeAtTodoId = -1;
            List<TodoItem> todoItems = await App.Database.GetItemsAsync();

            TodoItem todoItem = null;
            if (todoItems != null && todoItems.Count != 0)
            {

                for (int i = 0; i < todoItems.Count; i++)
                {
                    var item = todoItems[i];
                    if (item.isCurrent == true)
                    {
                        todoItem = item;
                    }
                }
            }
            return todoItem;
        }
    }
}