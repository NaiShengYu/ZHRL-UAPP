using AepApp.Models;
using AepApp.Tools;
using AepApp.View.EnvironmentalEmergency;
using AepApp.ViewModel;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using Plugin.Media;
using Reactive.Bindings;
using Rg.Plugins.Popup.Services;
using Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class TastInfoPage : ContentPage
    {
        private ObservableCollection<SampleInfoModel> samplingBottleList { get; set; } = new ObservableCollection<SampleInfoModel>();
        private ObservableCollection<ImageModel> photoList { get; set; } = new ObservableCollection<ImageModel>();
        private ObservableCollection<MultiSelectDataType> itemsFixer = new ObservableCollection<MultiSelectDataType>();//固定剂
        private ObservableCollection<MultiSelectDataType> itemsExamine = new ObservableCollection<MultiSelectDataType>();//检测项目

        private MySamplePlanItems _currentPlan;
        private TasksList _currentTask;
        private string _taskId = "";
        private SampleInfoModel _currentSample;//当前显示的样本
        private SampleInfoModel _lastCheckSample;

        public TastInfoPage(MySamplePlanItems _samplePlanItems, TasksList task)
        {
            InitializeComponent();
            _currentPlan = _samplePlanItems;
            _currentTask = task;
            if (task != null)
            {
                Title = task.taskname;
                _taskId = task.taskid;
            }
            BindCommonData();
            GetSampleListOfTask();
        }

        //设置计划相关内容
        private void BindCommonData()
        {
            if (_currentPlan != null)
            {
                LabLocation.Text = _currentPlan.lng + "E," + _currentPlan.lat + "N";
            }
            if (_currentTask != null)
            {
                LabType.Text = _currentTask.tasktypeName;
            }
        }

        /// <summary>
        /// 设置固定剂选项
        /// </summary>
        private void BindFixData()
        {
            itemsFixer = ConstConvertUtils.GetFixer();
            List<MultiSelectDataType> selected = new List<MultiSelectDataType>();
            if (_currentSample != null)
            {
                foreach (var item in itemsFixer)
                {
                    string fix = _currentSample.Fixative;
                    if (!string.IsNullOrWhiteSpace(fix) && fix.Contains(item.Name))
                    {
                        selected.Add(item);
                    }
                }
            }

            MultiSelectViewModel fixVm = new MultiSelectViewModel
            {
                AvailableItems = itemsFixer,
                SelectedItems = new ObservableCollection<MultiSelectDataType>(selected),
            };
            pickerF.BindingContext = fixVm;
        }

        /// <summary>
        /// 设置检测项目
        /// </summary>
        private void BindExamineItem()
        {
            itemsExamine.Clear();
            if (_currentTask != null && _currentTask.taskAnas != null)
            {
                string contents = "";
                foreach (var item in _currentTask.taskAnas)
                {
                    if (!string.IsNullOrWhiteSpace(item.atname))
                    {
                        contents += item.atname + " ";
                        itemsExamine.Add(new MultiSelectDataType
                        {
                            Id = item.atid,
                            Name = item.atname,
                            SampleAttype = item.attype,
                        });
                    }

                }
                LabExamineItems.Text = contents;
            }

            List<MultiSelectDataType> selected = new List<MultiSelectDataType>();
            if (_currentSample != null)
            {
                foreach (var item in itemsExamine)
                {
                    string fix = _currentSample.Anatype;
                    if (!string.IsNullOrWhiteSpace(fix) && fix.Contains(item.Name))
                    {

                        selected.Add(item);
                    }
                }
            }

            MultiSelectViewModel fixVm = new MultiSelectViewModel
            {
                AvailableItems = itemsExamine,
                SelectedItems = new ObservableCollection<MultiSelectDataType>(selected)
            };
            pickerExamine.BindingContext = fixVm;
        }


        /// <summary>
        /// 本地添加一个新样本
        /// </summary>
        private void AddEmptySample()
        {
            string taskTag = "";
            if (!string.IsNullOrWhiteSpace(_taskId) && _taskId.Length >= 4)
            {
                taskTag = _taskId.Substring(_taskId.Length - 4);
            }
            _currentSample = new SampleInfoModel
            {
                Number = "S" + taskTag + TimeUtils.DateTime2YMDHMSNowrap(DateTime.Now),
                Sampletime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds),
            };

            samplingBottleList.Add(_currentSample);
            ChangeUI();
        }

        private void ChangeToolbar()
        {
            ToolbarItem bar = ToolbarItems[0];
            if (_currentSample != null)
            {
                if (!string.IsNullOrWhiteSpace(_currentSample.id))
                {
                    bar.Text = "更新样本";
                }
                else
                {
                    bar.Text = "上传样本";
                }
            }
            else
            {
                bar.Text = "";
            }
        }


        /// <summary>
        /// 修改UI
        /// </summary>
        private void ChangeUI()
        {
            lvSample.ItemsSource = samplingBottleList;
            sclv.BindingContext = _currentSample;
            LabNumSample.Text = samplingBottleList.Count + "";

            UpdateTime();
            BindExamineItem();
            BindFixData();
            ChangeToolbar();

            if (_lastCheckSample != null)
            {
                _lastCheckSample.SetColors(false);
            }
            if (_currentSample != null)
            {
                _currentSample.SetColors(true);
            }
            _lastCheckSample = _currentSample;
        }

        /// <summary>
        /// 删除数据后修改UI
        /// </summary>
        /// <param name="checkIndex">删除item后默认选择删除项的前一项</param>
        private void ChangeUIAfterDelete(int checkIndex)
        {
            if (samplingBottleList.Count > 0)
            {
                if (checkIndex < 0)
                {
                    checkIndex = 0;
                }
                samplingBottleList[checkIndex].SetColors(true);
                _currentSample = samplingBottleList[checkIndex];
            }
            else
            {
                _currentSample = null;
            }
            ChangeUI();
        }

        private async void DeleteSample()
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择需要删除的样本");
                return;
            }
            string sampleId = _currentSample.id;
            int layoutIndex = samplingBottleList.IndexOf(_currentSample);
            int checkIndex = layoutIndex - 1;
            bool sure = await DisplayAlert("友情提示", "样本删除后不可恢复，确定继续删除吗？", "确定", "取消");
            if (sure)
            {
                if (string.IsNullOrWhiteSpace(sampleId))//本地删除
                {
                    samplingBottleList.Remove(_currentSample);
                    ChangeUIAfterDelete(checkIndex);
                }
                else
                {
                    bool success = await DeleteNetSample(sampleId);
                    if (success)
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除成功");
                        samplingBottleList.Remove(_currentSample);
                        ChangeUIAfterDelete(checkIndex);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除失败，请重试~");
                    }
                }
            }

        }

        /// <summary>
        /// 获取样本信息列表
        /// </summary>
        private async void GetSampleListOfTask()
        {
            //string url = App.EP360Module.url + "/Api/WaterData/GetListByTid";
            string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/GetListByTid?taskid=" + _taskId;
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<SampleInfoModel> list = JsonConvert.DeserializeObject<List<SampleInfoModel>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        foreach (SampleInfoModel item in list)
                        {
                            samplingBottleList.Add(item);
                        }
                        _currentSample = samplingBottleList[0];
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    ChangeUI();
                }
            }
        }


        /// <summary>
        /// 添加/修改样本
        /// </summary>
        private async Task<bool> EditSample()
        {

            bool isEdit = false;
            if (!string.IsNullOrWhiteSpace(_currentSample.id))
            {
                isEdit = true;
            }
            CrossHud.Current.Show("样本数据上传中...");
            //string url = App.EP360Module.url + "/Api/WaterData/Add";
            string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/Add";
            string urlUpdate = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/Update";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("appearance", _currentSample.Appearance);
            map.Add("depth", _currentSample.Depth);
            map.Add("direction", _currentSample.Direction);
            map.Add("DO", _currentSample.DoDyn);
            map.Add("number", _currentSample.Number);
            map.Add("PH", _currentSample.PhDyn);
            map.Add("sampletime", _currentSample.Sampletime);
            map.Add("taskid", _taskId);
            map.Add("tide", _currentSample.Tide);
            map.Add("waterlevel", _currentSample.Waterlevel);
            map.Add("width", _currentSample.Width);
            map.Add("qrcode", _currentSample.Qrcode);
            map.Add("anatype", _currentSample.Anatype);//检测项目
            map.Add("fixative", _currentSample.Fixative);
            if (isEdit)
            {
                map.Add("id", _currentSample.id);
            }

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(isEdit ? urlUpdate : url, JsonConvert.SerializeObject(map), "POST");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    if (!isEdit)
                    {
                        string result = JsonConvert.DeserializeObject<string>(res.Results);
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            _currentSample.id = result;
                            _currentSample.Status = "1";
                        }
                        return true;
                    }
                    else
                    {
                        bool result = JsonConvert.DeserializeObject<bool>(res.Results);
                        return result;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    CrossHud.Current.Dismiss();
                }
            }
            CrossHud.Current.Dismiss();
            return false;
        }


        /// <summary>
        /// 删除样本
        /// </summary>
        private async Task<bool> DeleteNetSample(string sampleId)
        {

            //string url = App.EP360Module.url + "/Api/Delete?id=" + sampleId;
            string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/Delete?id=" + sampleId;
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "", "POST");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    bool result = JsonConvert.DeserializeObject<bool>(res.Results);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        //拍照
        async void TakePhoto(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                CompressionQuality = 50,
                Directory = "Sample",
                Name = System.DateTime.Now + ".jpg"
            });

            if (file == null)
            {
                return;
            }
            photoList.Add(new ImageModel
            {
                Url = file.Path,
            });

            creatPhotoView();

        }

        /// <summary>
        /// 进入扫描界面
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Scanning(object sender, System.EventArgs e)
        {
            ScanQr();
        }

        /// <summary>
        /// 扫描二维码
        /// </summary>
        private void ScanQr()
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择需要采样的样本~");
                return;
            }
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "ScanningResult");

            MessagingCenter.Subscribe<ContentPage, string>(this, "ScanningResult", async (arg1, arg2) =>
            {
                Console.WriteLine("采样瓶二维码结果：" + arg2);
                _currentSample.Qrcode = arg2;
                LabelQr.Text = arg2;
            });

            Navigation.PushAsync(new ScanningPage
            {
                Title = "扫描采样瓶",
            });
        }

        /// <summary>
        /// 创建图片
        /// </summary>
        void creatPhotoView()
        {
            LabNumPhoto.Text = photoList.Count + "";
            lvPhoto.ItemsSource = photoList;
            lvPhoto.ItemLongTapCommand = new Command((obj) =>
            {
                ImageModel img = obj as ImageModel;
                if (img != null)
                {
                    if (photoList[photoList.IndexOf(img)].Status == 2)
                    {
                        photoList[photoList.IndexOf(img)].Status = 0;
                    }
                    else
                    {
                        photoList[photoList.IndexOf(img)].Status = 2;
                    }
                }
            });
        }

        private void UpdateTime()
        {
            if (_currentSample != null)
            {
                if (_currentSample.Sampletime != null)
                {
                    DatePickerStart.Date = _currentSample.Sampletime;
                    TimePickerStart.Time = new TimeSpan(_currentSample.Sampletime.TimeOfDay.Hours, _currentSample.Sampletime.TimeOfDay.Minutes, _currentSample.Sampletime.TimeOfDay.Seconds);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ContentPage, string>(this, "ScanningResult");
            MessagingCenter.Unsubscribe<ContentPage, ObservableCollection<MultiSelectDataType>>(this, "SelectData");
        }


        private void BtnAdd_Clicked(object sender, EventArgs e)
        {
            AddEmptySample();
        }

        private void BtnMinus_Clicked(object sender, EventArgs e)
        {
            DeleteSample();
        }

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnTrash_Clicked(object sender, EventArgs e)
        {

            bool sure = await DisplayAlert("提示", "确定删除所选照片吗？", "确定", "取消");
            if (sure)
            {
                for (int i = 0; i < photoList.Count; i++)
                {
                    if (photoList[i].Status == 2)
                    {
                        photoList.RemoveAt(i);
                        i--;
                    }
                }
                LabNumPhoto.Text = photoList.Count + "";
            }
        }

        private void GridLocation_Tapped(object sender, EventArgs e)
        {
            if (_currentPlan == null)
            {
                return;
            }
            Navigation.PushAsync(new RescueSiteMapPage("任务位置", _currentPlan.address, _currentPlan.lat, _currentPlan.lng));
        }

        private void FilterTrans_Tapped(object sender, EventArgs e)
        {
            if (_currentSample == null)
            {
                return;
            }
            _currentSample.FilterTrans = !_currentSample.FilterTrans;
            sclv.BindingContext = _currentSample;
        }

        private void FilterAccept_Tapped(object sender, EventArgs e)
        {
            if (_currentSample == null)
            {
                return;
            }
            _currentSample.FilterAccept = !_currentSample.FilterAccept;
            sclv.BindingContext = _currentSample;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择样本");
                return;
            }
            bool isEdit = false;
            if (!string.IsNullOrWhiteSpace(_currentSample.id))
            {
                isEdit = true;
            }
            bool success = await EditSample();
            if (success)
            {
                ChangeToolbar();
                int index = samplingBottleList.IndexOf(_currentSample);
                samplingBottleList[index].Status = "1";
                ChangeUI();
                DependencyService.Get<IToast>().ShortAlert(isEdit ? "样本信息更新成功" : "样本上传成功");
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert(isEdit ? "样本信息更新失败" : "样本上传失败");

            }
        }

        private void lvSample_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (_lastCheckSample != null)
            {
                _lastCheckSample.SetColors(false);
            }
            SampleInfoModel sample = e.SelectedItem as SampleInfoModel;
            _currentSample = sample;
            _currentSample.SetColors(true);
            ChangeUI();
        }

        /// <summary>
        /// 多选
        /// </summary>
        /// <param name="type"></param>
        private void SelectMultiData(int type)
        {
            MessagingCenter.Unsubscribe<ContentPage, ObservableCollection<MultiSelectDataType>>(this, "SelectData");
            MessagingCenter.Subscribe<ContentPage, ObservableCollection<MultiSelectDataType>>(this, "SelectData", async (arg1, arg2) =>
            {
                if (_currentSample != null && arg2 != null)
                {
                    ObservableCollection<MultiSelectDataType> s = arg2 as ObservableCollection<MultiSelectDataType>;
                    string contents = "";
                    foreach (var item in s)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Name))
                        {
                            contents += item.Name + ", ";
                        }
                    }
                    if (type == 1)
                    {
                        _currentSample.Anatype = contents;
                    }
                    else if (type == 2)
                    {
                        _currentSample.Fixative = contents;
                    }
                }
            });
            if (type == 1)
            {
                //PopupNavigation.Instance.PushAsync(pickerExamine.PopupPage);
            }
            else if (type == 2)
            {
                //PopupNavigation.Instance.PushAsync(pickerF.PopupPage);
            }
        }

        /// <summary>
        /// 选择固定剂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickerF_Clicked(object sender, EventArgs e)
        {
            SelectMultiData(2);
        }

        /// <summary>
        /// 选择检测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickerExamine_Clicked(object sender, EventArgs e)
        {
            SelectMultiData(1);
        }

        /// <summary>
        /// 查看检测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            string type = LabExamineItems.Text;
            if (!string.IsNullOrWhiteSpace(type))
            {
                Navigation.PushAsync(new ViewContentPage(null, type));
            }
        }

        /// <summary>
        /// 扫描二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_GridQr(object sender, EventArgs e)
        {
            ScanQr();
        }

        /// <summary>
        /// 选择日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {

            if (_currentSample != null)
            {
                DateTime t = new DateTime(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day,TimePickerStart.Time.Hours, TimePickerStart.Time.Minutes, TimePickerStart.Time.Seconds);
                _currentSample.Sampletime = t;
            }
        }

        /// <summary>
        /// 选择时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimePickerStart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            {
                TimePicker tp = sender as TimePicker;
                if (_currentSample != null)
                {
                    DateTime t = new DateTime(_currentSample.Sampletime.Year, _currentSample.Sampletime.Month, _currentSample.Sampletime.Day,
                        tp.Time.Hours, tp.Time.Minutes, tp.Time.Seconds);
                    _currentSample.Sampletime = t;
                }
            }
        }
    }
}
