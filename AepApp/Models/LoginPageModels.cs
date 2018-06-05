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
        internal class newToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string refresh_token { get; set; }
            public string userName { get; set; }
        }
        internal class convertTokenResult {
            public convertToken result;
        }
        internal class convertToken
        {
            public string accessToken { get; set; }
            public string encryptedAccessToken { get; set; }
            public string expireInSeconds { get; set; }
            public string waitingForActivation { get; set; }
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

        internal class newLoginParameter {
            public string password { get; set; }
            public string username { get; set; }
            public string grant_type { get; set; }
        }
        internal class newConvertTokenParameter {
            public string authProvider { get; set; }
            public string providerAccessCode { get; set; }
         
        }

    }
}