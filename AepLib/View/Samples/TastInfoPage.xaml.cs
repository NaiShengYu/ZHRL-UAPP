using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using Plugin.Media;
using Reactive.Bindings;
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

        private string _taskId = "";
        private SampleInfoModel _currentSample;//当前显示的样本
        private SampleInfoModel _lastCheckSample;

        public TastInfoPage(string title, string taskId)
        {
            InitializeComponent();
            Title = title;
            _taskId = taskId;
            GetSampleListOfTask();
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
                Sampletime = DateTime.Now,
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
            map.Add("qrcode", _currentSample.Qrcode);
            map.Add("anatype", _currentSample.Anatype);
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

        }

        private void GridExamineItem_Tapped(object sender, EventArgs e)
        {

        }

        private void GridFix_Tapped(object sender, EventArgs e)
        {

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
    }
}
