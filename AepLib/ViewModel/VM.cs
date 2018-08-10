﻿using AepApp.Models;
using CloudWTO.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AepApp.ViewModel
{
    public class VM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string FORMAT_YMD = "yyyy-MM-dd";
        private DatabaseContext _dbContext;


        public Dictionary<string, ObservableCollection<MySamplePlanItems>> planMap = new Dictionary<string, ObservableCollection<MySamplePlanItems>>();
        public Dictionary<string, ObservableCollection<MySamplePlanItems>> PlanMap
        {
            get
            {
                return planMap;
            }
            set
            {
                planMap = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<MySamplePlanItems> plans = new ObservableCollection<MySamplePlanItems>();
        public ObservableCollection<MySamplePlanItems> Plans
        {
            get
            {
                return plans;
            }
            set
            {
                plans = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime currentDay;
        public DateTime CurrentDay
        {
            get
            {
                return currentDay;
            }
            set
            {
                currentDay = value;
                NotifyPropertyChanged();
            }
        }
        private MySamplePlanItems selectPlan;
        public MySamplePlanItems SelectPlan
        {
            get
            {
                return selectPlan;
            }
            set
            {
                selectPlan = value;
                NotifyPropertyChanged();
            }
        }


        private string CurrentDay2String()
        {
            return CurrentDay.ToString(FORMAT_YMD);
        }


        public async void requestSamplePlanList()
        {
            _dbContext = new DatabaseContext();


            samplePlanRequestDic parameter = new samplePlanRequestDic
            {
                pageIndex = -1,
                searchKey = "",
                planTime = CurrentDay.ToString("yyyy-MM-dd"),
            };
            string param = JsonConvert.SerializeObject(parameter);
            HTTPResponse hTTPResponse = await EasyWebRequest.SendHTTPRequestAsync(App.SampleURL + "/Api/SamplePlan/PagedListForPhone", param, "POST", App.EmergencyToken);
            Console.WriteLine(hTTPResponse);
            if (hTTPResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                App.mySamplePlanResult = JsonConvert.DeserializeObject<MySamplePlanResult>(hTTPResponse.Results);
                //Console.WriteLine("结果是：" + App.mySamplePlanResult);
                if (PlanMap.ContainsKey(CurrentDay2String()))
                {
                    PlanMap.Remove(CurrentDay2String());
                }
                PlanMap.Add(CurrentDay2String(), App.mySamplePlanResult.Items);
                Plans = PlanMap[CurrentDay2String()];


                //_dbContext.Samples.AddRange(App.mySamplePlanResult.Items);
                //_dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine(hTTPResponse);
            }
            //List<MySamplePlanItems> plist = _dbContext.Samples
            //    .Where(b => b.plantime.ToString(FORMAT_YMD).Contains(CurrentDay.ToString()))
            //    .ToList();
            //Plans = new ObservableCollection<MySamplePlanItems>(plist);

        }


        internal class samplePlanRequestDic
        {
            public string searchKey { get; set; }
            public string planTime { get; set; }
            public int pageIndex { get; set; }
        }

    }
}