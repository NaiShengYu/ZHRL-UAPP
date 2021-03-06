﻿using AepApp.View;
using System;

namespace AepApp.Tools
{
    public class StringUtils
    {
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            if (html == null) return "";
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
            string ends = fileName.Substring(fileName.LastIndexOf(".") + 1);
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


        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <returns>The log with file name.</returns>
        /// <param name="fileName">File name.</param>
        public static string fileLogWithFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "file";
            if (fileName.LastIndexOf(".") < 0) return "file";
            string ends = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (string.IsNullOrEmpty(ends)) return "file";
            return fileLog(ends);
        }


        public static string fileLog(string filestate)
        {
            string filelogName = "file";
            if (string.IsNullOrEmpty(filestate)) return filelogName;
            if (filestate.Equals("doc") || filestate.Equals("docx"))
                filelogName = "doc";
            if (filestate.Equals("mp3"))
                filelogName = "mp3";
            if (filestate.Equals("mp4"))
                filelogName = "mp4";
            if (filestate.Equals("pdf"))
                filelogName = "pdf";
            if (filestate.Equals("ppt"))
                filelogName = "ppt";
            if (filestate.Equals("png") || filestate.Equals("jpg") || filestate.Equals("jpeg") || filestate.Equals("gif"))
                filelogName = "png";
            if (filestate.Equals("rtf"))
                filelogName = "rtf";
            if (filestate.Equals("txt"))
                filelogName = "txt";
            if (filestate.Equals("xls"))
                filelogName = "xls";

            return filelogName;
        }


        /// <summary>
        /// 经纬度转azm坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static AzmCoord nullable2Coord(double? lng, double? lat)
        {
            AzmCoord coord;
            if (lng == null || lat == null)
            {
                coord = new AzmCoord(0, 0);
            }
            else
            {
                coord = new AzmCoord(lng.Value, lat.Value);
            }
            return coord;
        }

        /// <summary>
        /// 经纬度转azm坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static AzmCoord string2Coord(string lng, string lat)
        {
            AzmCoord coord;
            if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lng))
            {
                coord = new AzmCoord(0, 0);
            }
            else
            {
                coord = new AzmCoord(double.Parse(lng), double.Parse(lat));
            }
            return coord;
        }

        public static double string2Double(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0;
            }
            return Convert.ToDouble(s);
        }

    }
}
