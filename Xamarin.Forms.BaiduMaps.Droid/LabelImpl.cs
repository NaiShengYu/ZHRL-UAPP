using System;
using System.Collections.Generic;
using System.ComponentModel;

using Com.Baidu.Mapapi.Map;
using Xamarin.Forms.Platform.Android;
using BMap = Com.Baidu.Mapapi.Map;


namespace Xamarin.Forms.BaiduMaps.Droid
{
    internal class LabelImpl : BaseItemImpl<Label, BMap.MapView, Text>
    {
        protected override IList<Label> GetItems(Map map) => map.Labels;

        protected override Text CreateNativeItem(Label item)
        {
            TextOptions options = new TextOptions()
                .InvokePosition(item.Coordinate.ToNative())
                .InvokeBgColor(item.BackgroundColor.ToAndroid())
                .InvokeFontColor(item.FontColor.ToAndroid())
                .InvokeFontSize(item.FontSize)
                .InvokeText(item.Title);
            Text text = (Text)NativeMap.Map.AddOverlay(options);
            
            item.NativeObject = text;

            return text;
        }

        protected override void UpdateNativeItem(Label item)
        {
            Text native = (Text)item?.NativeObject;
            if (null == native)
            {
                return;
            }

            item.SetValueSilent(Label.CoordinateProperty, native.Position.ToUnity());
            native.SetText(item.Title);
       }

        protected override void RemoveNativeItem(Label item)
        {
            NativeMap.Map.HideInfoWindow();
            ((Text)item.NativeObject).Remove();
        }

        protected override void RemoveNativeItems(IList<Label> items)
        {
            foreach (Label item in items)
            {
                RemoveNativeItem(item);
            }
        }

        internal override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Label item = (Label)sender;
            Text native = (Text)item?.NativeObject;
            if (null == native)
            {
                return;
            }

            if (Label.CoordinateProperty.PropertyName == e.PropertyName)
            {
                native.Position = item.Coordinate.ToNative();
                return;
            }

            if (Label.TitleProperty.PropertyName == e.PropertyName)
            {
                native.SetText(item.Title);
                return;
            }

            if (Label.BackgroundColorProperty.PropertyName == e.PropertyName)
            {
                native.BgColor = item.BackgroundColor.ToAndroid();
                return;
            }

            if (Label.FontColorProperty.PropertyName == e.PropertyName)
            {
                native.BgColor = item.FontColor.ToAndroid();
                return;
            }

        }
    }
}