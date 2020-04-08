using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Webkit;

namespace FTSAFE
{
    [Activity(Label = "统计分析")]
    public class CountActivity : AppCompatActivity
    {
        private WebView webview;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_count_new);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "公司隐患统计";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this,Resource.Style.Toolbar_TitleText);
            //修改子标题大小
            toolbar.SetSubtitleTextAppearance(this,Resource.Style.Toolbar_SubTitleText);
            SetSupportActionBar(toolbar);
            //设置返回按钮
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //响应返回按钮
            toolbar.NavigationClick += (s, e) =>
            {
                //Intent i = new Intent(this, typeof(NewMainActivity));
                //StartActivity(i);
                Finish();
            };

            // Create your application here
            webview = FindViewById<WebView>(Resource.Id.webview1);
            //设置webserver支持js
            webview.Settings.JavaScriptEnabled = true;
            //添加js接口
            webview.AddJavascriptInterface(this, "Test");
            //加载html的地址
            // webview.LoadUrl("file:///android_asset/Test.html");
            webview.LoadUrl("file:///android_asset/partol_Statis.html");
        }
    }
}