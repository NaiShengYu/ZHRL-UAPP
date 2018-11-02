using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class AddSitePageModel 
	{

        public string customerName { get; set; }
        public string appTitle { get; set; }
        public string majorVersion { get; set; }
        public string minorVersion { get; set; }
        public string revision { get; set; }
        public string date { get; set; }
    }
}