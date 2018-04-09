using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AepApp.Models
{
	public class LoginPageModels 
	{
        internal class haveToken {
            public string token { get; set; }
            public string success { get; set; }
            public string userid { get; set; }
            public string roles { get; set; }
        }

        internal class loginParameter
        {
            public string Password { get; set; }
            public string UserName { get; set; }
            public bool rememberStatus { get; set; }
            public string sid { get; set; }
            public string sname { get; set; }
            public int userdel { get; set; }
        }


    }
}