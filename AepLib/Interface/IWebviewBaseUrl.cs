using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AepApp.Interface
{
    public interface IWebviewService
    {
        string Get();
        string SetEditorContent(WebView web, string html);
    }
}
