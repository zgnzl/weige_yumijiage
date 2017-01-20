using System;
using System.Collections.Generic;
using System.Data;
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
        private static WebClient webc = new WebClient();
        static void Main(string[] args)
        {
            webc.Encoding = Encoding.GetEncoding("UTF-8");
            string priceUrl = GetModPriceUrl();
            Console.WriteLine("原粮价格_玉米价格地址："+ priceUrl);
            GetTableByHtml("http://"+ priceUrl);
            Console.Read();
        }

        /// <summary>
        /// 获取数据抓取地址
        /// </summary>
        /// <returns>返回地址链接</returns>
        private static string GetModPriceUrl()
        {
            Uri uri = new Uri(dominAddress);
            string loadstr= webc.DownloadString(uri);
            string title = currentdate + "国内玉米价格汇总";
            string modPriceUrl = loadstr.Substring(loadstr.Substring(0, loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("href") + 6, loadstr.Substring(0, loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("target") - loadstr.Substring(0, loadstr.LastIndexOf("\"" + title + "\"")).LastIndexOf("href") - 8);
            return uri.Host + modPriceUrl;
            //string pattern = "<a.*title=\"" + title + "\"";
            //Regex reg = new Regex(pattern);
            //Match match = reg.Match(loadstr);
            //return uri.Host + new Regex("html.*.html").Match(match.Value).Value;
        }

        /// <summary>
        /// 数据保存在table中
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTableByHtml(string priceurl)
        {
            Uri uri = new Uri(priceurl);
            string htmlstr = webc.DownloadString(uri);
            string htmlTablestr = htmlstr.Substring(htmlstr.IndexOf("table"), htmlstr.LastIndexOf("table") - htmlstr.IndexOf("table"));
            string pattern = @"(<tr>[\s\S]*?</tr>)";//"(<p.class=\".*</p>)";
            Regex reg = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Console.WriteLine(reg.Matches(htmlTablestr).Count);
            int mintd = 1000;
            MatchCollection matchtds;
            foreach (Match match in reg.Matches(htmlTablestr))
            {
                
                StringBuilder sb =new  StringBuilder();
                bool flag = true;
                matchtds = new Regex("(<.*</span>)").Matches(match.Value);
                if (matchtds.Count < mintd)
                {
                    mintd = matchtds.Count;
                }
                    foreach (Match matchtd in matchtds)
                {
                    if (flag&&matchtds.Count == mintd)
                    {
                        flag = false;
                        sb.Append("  ↑  " + "->");
                    }
                    sb.Append((new Regex(">(.*)</span>").Match(matchtd.Value).Value.Replace(">", "").Substring(0, (new Regex(">(.*)</span>").Match(matchtd.Value).Value.Replace(">", "").IndexOf("<")))).Trim()+"->");
                }
                if (sb.ToString().Contains("与昨日相比"))
                {
                    sb.Insert(0, "\r\n");
                }
                Console.WriteLine(sb.ToString().TrimEnd('>').TrimEnd('-'));
            }
            return null;
        }
    }
}
