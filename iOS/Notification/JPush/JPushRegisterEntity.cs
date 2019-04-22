using Foundation;
using JPush.Binding.iOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.iOS.Notification.JPush
{
    public class JPushRegisterEntity : JPUSHRegisterEntity
    {
        public override NSSet Categories
        {
            get
            {
                return base.Categories;
            }
            set
            {
                base.Categories = value;
            }
        }

        public override nint Types
        {
            get
            {
                return base.Types;
            }
            set
            {
                base.Types = value;
            }
        }
    }
}
