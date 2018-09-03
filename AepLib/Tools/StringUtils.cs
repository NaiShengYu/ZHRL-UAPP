using System;
using System.Collections.Generic;
using System.Text;

namespace AepApp.Tools
{
    public class StringUtils
    {
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        public static bool IsImg(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }
            if (fileName.LastIndexOf(".") < 0)
            {
                return false;
            }
            string ends = fileName.Substring(fileName.LastIndexOf("."), fileName.Length);
            if (string.IsNullOrWhiteSpace(ends))
            {
                return false;
            }
            if (ends.ToLower().Equals("png") || ends.ToLower().Equals("jpg") || ends.ToLower().Equals("jpeg")
                || ends.ToLower().Equals("gif"))
            {
                return true;
            }
            return false;

        }
    }
}
