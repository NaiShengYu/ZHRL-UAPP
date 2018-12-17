using System;
using Xamarin.Essentials;

namespace AepApp.AuxiliaryExtension
{
    public class ElementMapping
    {

        public static D Mapper<D, S>(D d,S s)
        {
            try
            {
                var Types = s.GetType();//获得类型
                var Typed = d.GetType();
                int i = 1;
                foreach (var sp in Types.GetProperties())//获得类型的属性字段
                {
                    foreach (var dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name)//判断属性名是否相同
                        {
                            Console.WriteLine(dp.Name);
                            dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性
                            break;
                        }

                        Console.WriteLine(i++);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        public ElementMapping()
        {
            // Register for connectivity changes, be sure to unsubscribe when finished
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }


        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.Profiles;
        }

    }
}
