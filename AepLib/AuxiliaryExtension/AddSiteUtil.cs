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

        internal void reqSiteInfo(string siteName, string siteUrl, ObservableCollection<TodoItem> dataList)
        {
            BackgroundWorker wrk = new BackgroundWorker();
            wrk.DoWork += (sender1, e1) =>
            {
                // Console.WriteLine("添加站点");                
                string[] s = siteUrl.Split(new char[] { ':' });
                string uri = "https://" + siteUrl + "/api/login/getstationName?stationurl=" + s[0];
                Console.WriteLine("站点接口:" + uri);
                result = EasyWebRequest.sendGetHttpWebRequestWithNoToken(uri);

            };
            wrk.RunWorkerCompleted += (sender1, e1) =>
            {
                bool isContainSite = false;
                try
                {
                    AddSitePageModel model = JsonConvert.DeserializeObject<AddSitePageModel>(result);


                    if (model != null)
                    {

                        TodoItem todoItem = new TodoItem();
                        todoItem.SiteId = model.id;
                        todoItem.Name = model.name;
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
                            //saveData(todoItem);
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
            };
            wrk.RunWorkerAsync();
        }
    }
}