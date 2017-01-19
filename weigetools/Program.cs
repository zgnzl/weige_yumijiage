using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace weigetools
{
    class Program
    {
        private static string dominAddress = "http://www.yumi.com.cn/yumijiage/index.html";
        private static string currentdate = DateTime.Now.Date.ToString("yyyy年M月d日");
        static void Main(string[] args)
        {
            string priceUrl = GetModPriceUrl();
            Console.WriteLine("原粮价格_玉米价格地址："+ priceUrl);
            Console.Read();
        }

        private static string GetModPriceUrl()
        {
            WebClient webc = new WebClient();
            webc.Encoding = Encoding.GetEncoding("UTF-8");
            Uri uri = new Uri(dominAddress);
            string loadstr= webc.DownloadString(uri);
            string title = currentdate + "国内玉米价格汇总";
            //string modPriceUrl = loadstr.Substring(loadstr.Substring(0,loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("href")+6, loadstr.Substring(0, loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("target")- loadstr.Substring(0, loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("href")-8);
            // return uri.Host +modPriceUrl;
            string pattern = "<a.*title=\"" + title + "\"";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(loadstr);
            return uri.Host + new Regex("html.*.html").Match(match.Value).Value;
        }
    }
}
