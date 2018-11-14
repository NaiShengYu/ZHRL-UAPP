using AepApp.Models;
using AepApp.Tools;
using CloudWTO.Services;
using Newtonsoft.Json;
using Plugin.Hud;
using Plugin.Media;
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

        private ObservableCollection<SampleInfoModel> samplingBottleList = new ObservableCollection<SampleInfoModel>();
        private ObservableCollection<SamplePhotoModel> photoList = new ObservableCollection<SamplePhotoModel>();
        private string _taskId = "";
        private int checkSampleIndex = -1;//当前选中样本的index
        private int lastCheckSampleIndex = -1;//上一次选中样本的index
        private SampleInfoModel _currentSample;//当前显示的样本

        public TastInfoPage(string title, string taskId)
        {
            InitializeComponent();
            Title = title;
            _taskId = taskId;
            getSampleListOfTask();
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

            creatSampleItemUI(_currentSample);

            samplingBottleList.Add(_currentSample);
            checkSampleIndex = sampleBottleSK.Children.Count - 1;
            chageUI();
        }

        private void changeToolbar()
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

        private void changeSampleStatusIcon()
        {
            if (checkSampleIndex >= 0 && sampleBottleSK.Children.Count > checkSampleIndex)
            {
                Grid sl = sampleBottleSK.Children[checkSampleIndex] as Grid;
                if (sl != null)
                {
                    Image imgStatus = sl.Children[1] as Image;
                    if (imgStatus != null)
                    {
                        imgStatus.Source = ImageSource.FromFile("greentick") as FileImageSource;
                    }
                }
            }
        }

        /// <summary>
        /// 修改UI
        /// </summary>
        private void chageUI()
        {
            if(_currentSample == null)
            {
                return;
            }
            _currentSample.SampleCount = samplingBottleList.Count;
            _currentSample.PhotoCount = photoList.Count;
            _currentSample.HasSample = samplingBottleList.Count > 0;
            _currentSample.HasPhoto = photoList.Count > 0;
            BindingContext = _currentSample;
            changeToolbar();
            if (lastCheckSampleIndex >= 0 && sampleBottleSK.Children.Count > lastCheckSampleIndex)
            {
                Grid sl = sampleBottleSK.Children[lastCheckSampleIndex] as Grid;
                if (sl != null)
                {
                    sl.BackgroundColor = Color.White;
                }
            }
            if (checkSampleIndex >= 0 && sampleBottleSK.Children.Count > checkSampleIndex)
            {
                Grid sl = sampleBottleSK.Children[checkSampleIndex] as Grid;
                if (sl != null)
                {
                    sl.BackgroundColor = Color.FromHex("#E6E6E6");
                    lastCheckSampleIndex = checkSampleIndex;
                }
            }
        }

        private void creatSampleItemUI(SampleInfoModel s)
        {
            if (s == null)
            {
                return;
            }
            Image imgBottle = new Image
            {
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 40,
                HeightRequest = 45,
                Margin = new Thickness(0, 0, 0, 0),
                Source = ImageSource.FromFile("bottle") as FileImageSource,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label labNumber = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 10,
                Text = s.Number,
                Margin = new Thickness(0, 0, 0, 0),
                WidthRequest = 65,
                HorizontalTextAlignment = TextAlignment.Center,
            };


            StackLayout layout = new StackLayout
            {
                Spacing = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            layout.Children.Add(imgBottle);
            layout.Children.Add(labNumber);

            Grid gridItem = new Grid
            {

                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,

            };
            gridItem.Children.Add(layout);

            string source = "";
            if (s.Status == "1")
            {
                source = "greentick";
            }
            else if (s.Status == "2")
            {
                source = "bluetruck";
            }
            else if (s.Status == "3")
            {
                source = "bluemicroscope";
            }
            Image imgStatus = new Image
            {

                WidthRequest = 15,
                HeightRequest = 15,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Aspect = Aspect.AspectFit,
            };
            if (!string.IsNullOrWhiteSpace(source))
            {
                imgStatus.Source = ImageSource.FromFile(source) as FileImageSource;
            }
            gridItem.Children.Add(imgStatus);

            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += (a, e) =>
            {
                gridItem.BackgroundColor = Color.Gray;
                Grid g = a as Grid;
                SampleInfoModel sm = g.BindingContext as SampleInfoModel;
                checkSampleIndex = samplingBottleList.IndexOf(sm);

                _currentSample = sm;
                chageUI();
            };
            gridItem.GestureRecognizers.Add(tgr);
            gridItem.BindingContext = s;

            sampleBottleSK.Children.Add(gridItem);
        }

        private void createSampleAllItemsUI()
        {
            sampleBottleSK.Children.Clear();
            foreach (SampleInfoModel s in samplingBottleList)
            {
                creatSampleItemUI(s);
            }
        }

        private async void deleteSample()
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择需要删除的样本");
                return;
            }
            string sampleId = _currentSample.id;
            int layoutIndex = samplingBottleList.IndexOf(_currentSample);
            bool sure = await DisplayAlert("友情提示", "样本删除后不可恢复，确定继续删除吗？", "确定", "取消");
            if (sure)
            {
                if (string.IsNullOrWhiteSpace(sampleId))//本地删除
                {
                    samplingBottleList.Remove(_currentSample);
                    changeDataAfterDelete(layoutIndex);
                }
                else
                {
                    bool success = await deleteNetSample(sampleId);
                    if (success)
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除成功");
                        samplingBottleList.Remove(_currentSample);
                        changeDataAfterDelete(layoutIndex);
                    }
                    else
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除失败，请重试~");
                    }
                }
            }

        }

        /// <summary>
        /// 删除样本后修改界面数据
        /// </summary>
        /// <param name="layoutIndex"></param>
        private void changeDataAfterDelete(int layoutIndex)
        {
            if (layoutIndex >= 0 && sampleBottleSK.Children.Count > layoutIndex)
            {
                sampleBottleSK.Children.RemoveAt(layoutIndex);
            }
            if (samplingBottleList.Count > 0)
            {
                _currentSample = samplingBottleList[samplingBottleList.Count - 1];
            }
            else
            {
                _currentSample = null;
            }
            checkSampleIndex = samplingBottleList.Count - 1;
            chageUI();
        }




        /// <summary>
        /// 获取样本信息列表
        /// </summary>
        private async void getSampleListOfTask()
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
                        samplingBottleList = new ObservableCollection<SampleInfoModel>(list);
                        createSampleAllItemsUI();

                        _currentSample = samplingBottleList[0];
                        checkSampleIndex = 0;
                        chageUI();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// 添加/修改样本
        /// </summary>
        private async Task<bool> editSample()
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
        private async Task<bool> deleteNetSample(string sampleId)
        {

            //string url = App.EP360Module.url + "/Api/Delete?id=" + sampleId;
            string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/Delete?id=" + sampleId;
            //Dictionary<string, object> map = new Dictionary<string, object>();
            //map.Add("id", sampleId);
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
        async void takePhoto(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
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

            else
            {
                SamplePhotoModel samplePhotoModel = new SamplePhotoModel
                {
                    photoPath = file.Path,
                    isSelect = false,
                };

                photoList.Add(samplePhotoModel);

                creatPhotoView();


            }


        }

        /// <summary>
        /// 进入扫描界面
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void scanning(object sender, System.EventArgs e)
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
        /// 根据拍照张数创建图片
        /// </summary>
        void creatPhotoView()
        {

            PickSK.Children.Clear();

            foreach (SamplePhotoModel photoModel in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = ImageSource.FromFile(photoModel.photoPath) as FileImageSource,
                    HeightRequest = 80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(button);

                var selectImg = new Image
                {
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                    Aspect =Aspect.Fill,
                    WidthRequest = 25,
                    HeightRequest = 25,
                    Source = photoModel.isSelect==true?ImageSource.FromFile("graytick") as FileImageSource : ImageSource.FromFile("greentick") as FileImageSource,
                };

                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += (object sender, EventArgs e) => {
                    photoModel.isSelect = !photoModel.isSelect;
                    selectImg.Source = photoModel.isSelect == true ? ImageSource.FromFile("graytick") as FileImageSource : ImageSource.FromFile("greentick") as FileImageSource;
                };
                selectImg.GestureRecognizers.Add(tap);
            }
        }






        private void BtnAdd_Clicked(object sender, EventArgs e)
        {
            AddEmptySample();
        }

        private void BtnMinus_Clicked(object sender, EventArgs e)
        {
            deleteSample();
        }

        private void BtnTrash_Clicked(object sender, EventArgs e)
        {
            if (photoList.Count == 0) return;
            for (int i = photoList.Count; i >=0; i--)
            {
                var photoModel = photoList[i];
                if (photoModel.isSelect == true)
                    photoList.Remove(photoModel);
            }
            creatPhotoView();
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
            if(_currentSample == null)
            {
                return;
            }
            _currentSample.FilterTrans = !_currentSample.FilterTrans;
            BindingContext = _currentSample;
        }

        private void FilterAccept_Tapped(object sender, EventArgs e)
        {
            if (_currentSample == null)
            {
                return;
            }
            _currentSample.FilterAccept = !_currentSample.FilterAccept;
            BindingContext = _currentSample;
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
            bool success = await editSample();
            if (success)
            {
                changeToolbar();
                changeSampleStatusIcon();
                DependencyService.Get<IToast>().ShortAlert(isEdit ? "样本信息更新成功" : "样本上传成功");
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert(isEdit ? "样本信息更新失败" : "样本上传失败");
            }
        }
    }
}
