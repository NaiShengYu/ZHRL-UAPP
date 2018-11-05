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
using Xamarin.Forms;

namespace AepApp.View.Samples
{
    public partial class TastInfoPage : ContentPage
    {

        private ObservableCollection<SampleInfoModel> samplingBottleList = new ObservableCollection<SampleInfoModel>();
        private SampleInfoModel _currentSample;
        private ObservableCollection<string> photoList = new ObservableCollection<string>();
        private string _taskId = "";
        private bool isEdit = false;

        public TastInfoPage(string title, string taskId)
        {
            InitializeComponent();
            Title = title;
            _taskId = taskId;
            getSampleListOfTask();
        }

        /// <summary>
        /// 获取样本信息列表
        /// </summary>
        private async void getSampleListOfTask()
        {
            string url = App.EP360Module.url + "/Api/WaterData/GetListByTid";
            //string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/GetListByTid";
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("taskid", _taskId);
            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    List<SampleInfoModel> list = JsonConvert.DeserializeObject<List<SampleInfoModel>>(res.Results);
                    if (list != null && list.Count > 0)
                    {
                        samplingBottleList = new ObservableCollection<SampleInfoModel>(list);
                        _currentSample = samplingBottleList[0];
                        BindingContext = _currentSample;
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
        private async void addSample()
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择需要上传的样本");
                return;
            }
            CrossHud.Current.Show("样本数据上传中...");
            string url = App.EP360Module.url + "/Api/WaterData/Add";
            //string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/WaterData/Add";
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
            if (!string.IsNullOrWhiteSpace(_currentSample.id))
            {
                map.Add("id", _currentSample.id);
            }

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, JsonConvert.SerializeObject(map), "POST", App.FrameworkToken);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    string result = JsonConvert.DeserializeObject<string>(res.Results);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        _currentSample.id = result;
                        _currentSample.Status = "1";
                        samplingBottleList.Add(_currentSample);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    CrossHud.Current.Dismiss();
                }
            }
            CrossHud.Current.Dismiss();
        }

        private async void deleteSample()
        {
            if (_currentSample == null)
            {
                DependencyService.Get<IToast>().ShortAlert("请先选择需要删除的样本");
                return;
            }
            string sampleId = _currentSample.id;
            bool sure = await DisplayAlert("友情提示", "样本删除后不可恢复，确定继续删除吗？", "确定", "取消");
            if (sure)
            {
                if (string.IsNullOrWhiteSpace(sampleId))//本地删除
                {
                    samplingBottleList.Remove(_currentSample);
                    if (samplingBottleList.Count > 0)
                    {
                        lvSample.SelectedItem = samplingBottleList[samplingBottleList.Count - 1];
                        _currentSample = lvSample.SelectedItem;
                    }
                    else
                    {
                        _currentSample = null;
                    }
                }
                else
                {
                    deleteNetSample(sampleId);
                }
            }



        }

        /// <summary>
        /// 删除样本
        /// </summary>
        private async void deleteNetSample(string sampleId)
        {

            string url = App.EP360Module.url + "/Api/Delete?id=" + sampleId;
            //string url = ConstantUtils.SAMPLE_TEST_URL + "/Api/Delete?id=" + sampleId;

            HTTPResponse res = await EasyWebRequest.SendHTTPRequestAsync(url, "");
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    bool result = JsonConvert.DeserializeObject<bool>(res.Results);
                    if (result)
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除成功");

                    }
                    else
                    {
                        DependencyService.Get<IToast>().ShortAlert("删除失败，稍后重试~");
                    }
                }
                catch (Exception)
                {
                    DependencyService.Get<IToast>().ShortAlert("删除失败，稍后重试~");
                }
            }
        }

        private void InitSampleInfo()
        {
            _currentSample = new SampleInfoModel
            {
                id = new Guid().ToString(),
                Number = "S" + TimeUtils.DateTime2YMDHMS(DateTime.Now),
                Sampletime = DateTime.Now,
                Appearance = "",
            };
            BindingContext = _currentSample;
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

                photoList.Add(file.Path);

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
        /// 扫码创建采样瓶
        /// </summary>
        void creatSamplingBottle(string barCode)
        {

            //sampleBottleSK.Children.Clear();
            //foreach (string barCode in samplingBottleList)

            //{
            Button button = new Button
            {
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = new Thickness(0, 0, 0, 0),
                Image = ImageSource.FromFile("bottle") as FileImageSource,
                HorizontalOptions = LayoutOptions.Center,
            };

            Label lab3 = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 13,
                Text = barCode,
                Margin = new Thickness(0, 0, 0, 0),
                WidthRequest = 65,
                HorizontalTextAlignment = TextAlignment.Center,
            };


            StackLayout layout = new StackLayout
            {
                Spacing = 1,
                //BackgroundColor = Color.Black
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            layout.Children.Add(button);
            layout.Children.Add(lab3);

            Grid grid = new Grid
            {
                //BackgroundColor = Color.Blue,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,

            };
            grid.Children.Add(layout);

            Button button1 = new Button
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                //BackgroundColor = Color.Orange,
            };
            grid.Children.Add(button1);
            //sampleBottleSK.Children.Add(grid);
            //}


        }

        /// <summary>
        /// 根据拍照张数创建图片
        /// </summary>
        void creatPhotoView()
        {

            PickSK.Children.Clear();

            foreach (string path in photoList)
            {
                Grid grid = new Grid();
                PickSK.Children.Add(grid);
                Console.WriteLine("图片张数：" + photoList.Count);
                Image button = new Image
                {
                    Source = ImageSource.FromFile(path) as FileImageSource,
                    HeightRequest = 80,
                    WidthRequest = 80,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Aspect = Aspect.Fill,
                };
                grid.Children.Add(button);


                //Image = new Image
                //{
                //    VerticalOptions = LayoutOptions.Center,
                //    HorizontalOptions = LayoutOptions.Start,
                //    Aspect =Aspect.Fill,


                //};


            }



        }

        private void BtnAdd_Clicked(object sender, EventArgs e)
        {
            InitSampleInfo();
            samplingBottleList.Add(_currentSample);
            lvSample.SelectedItem = _currentSample;
        }

        private void BtnMinus_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnTrash_Clicked(object sender, EventArgs e)
        {

        }

        private void lvSample_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SampleInfoModel sample = e.SelectedItem as SampleInfoModel;
            if (sample == null)
            {
                return;
            }
            _currentSample = sample;
            BindingContext = _currentSample;

            //lvSample.SelectedItem = null;
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

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            addSample();
        }
    }
}
