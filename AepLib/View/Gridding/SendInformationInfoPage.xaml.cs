using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AepApp.View.Gridding
{
    public partial class SendInformationInfoPage : ContentPage
    {
        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {

        }

        private ObservableCollection<informationFile> dataList = new ObservableCollection<informationFile>();

        public SendInformationInfoPage()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                informationFile file = new informationFile
                {
                    name = "2017年国家环境保护局",
                    lenth = "1.5M",
                    type = i % 2,
                };
                dataList.Add(file);
            }

            listV.ItemsSource = dataList;


        }


        private class informationFile{
            public string name
            {
                get;
                set;
            }

            public string lenth
            {
                get;
                set;
            }

            public int type
            {
                get;
                set;
            }
        }
    }
}
