using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Webkit;
using FTSAFE;
using FTSAFE.CommonClass;
using System;

namespace FTSAFE
{
    [Activity(Label = "相关制度")]
    public class RegimeActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_regime);

            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            //webview访问网页
            WebView webView = FindViewById<WebView>(Resource.Id.webview1);
            //指定处理时间的WebViewClient
            webView.SetWebViewClient(new MyWebClient());
            string url = "http://safe.guotaiyun.cn/demo/ressim/ressimlist?id="+XmlDBClass.accID+"";
            //打开网址
            webView.LoadUrl(url);
        }
    }
}