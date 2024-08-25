using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using System.Net;
namespace myNewTools
{
    public partial class Form1 : Form
    {
        public TreeNode TN;
        public Form1()
        {
            InitializeComponent();
            InitializeAsync();
            treeView1.Nodes.Clear();
            TN = treeView1.Nodes.Add("庆余年");
        }
        async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.WebMessageReceived += UpdateAddressBar;
            webView21.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            //await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            //await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }
        private void CoreWebView2_NewWindowRequested(object sender,CoreWebView2NewWindowRequestedEventArgs e)
        {
            webView21.Source = new Uri(e.Uri.ToString());
            e.Handled = true;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.Navigate(toolStripTextBox1.Text);
            }
        }


        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            toolStripTextBox1.Text = uri;
            webView21.CoreWebView2.PostWebMessageAsString(uri);
        }
        public static HtmlAgilityPack.HtmlDocument LoadHtmlByUrl(string url)
        {
            HtmlAgilityPack.HtmlDocument htmldoc = new HtmlAgilityPack.HtmlDocument();
            HttpWebRequest req;
            req = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            req.Method = "GET";
            WebResponse rs = req.GetResponse();
            Stream rss = rs.GetResponseStream();
            htmldoc.Load(rss);
            string str = htmldoc.DocumentNode.InnerHtml;
            return htmldoc;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlAgilityPack.HtmlWeb hw = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlWeb.PreRequestHandler handler = delegate (HttpWebRequest request)
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, br, zstd";
                request.Headers[HttpRequestHeader.AcceptLanguage] = "en,zh-CN;q=0.9,zh;q=0.8,en-GB;q=0.7,en-US;q=0.6";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.CookieContainer = new System.Net.CookieContainer();
                return true;
            };
            hw.PreRequest += handler;
            hw.OverrideEncoding = Encoding.GetEncoding("gb2312");
            hw.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36 Edg/127.0.0.0";
       
            //hw.Timeout = 80000;
            
            doc = hw.LoadFromWebAsync("https://69shuba.cx/book/51840/").Result;
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"catalog\"]/ul/li");
            foreach (var node in nodes)
            {
                TN.Nodes.Add(node.InnerText);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
