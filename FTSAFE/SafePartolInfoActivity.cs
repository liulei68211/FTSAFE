using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FTSAFE.Adapter;
using FTSAFE.CommonClass;

namespace FTSAFE
{
    [Activity(Label = "安全确认信息")]
    public class SafePartolInfoActivity : AppCompatActivity
    {
        private ListView myList;
        private List<SafeItem> data = new List<SafeItem>();
        private SafeAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_safe_partolinfo);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "安全确认信息";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);

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

            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));

            bdHidneListInfo();
        }
        #region 绑定安全确认信息
        private void bdHidneListInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();

            string roleName = safeWeb.roleSafe(XmlDBClass.userID);

            //xml 转table 
            try
            {
                string revXml = safeWeb.searchSafeSureInfo(XmlDBClass.departID);
                if (revXml != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                    if (dt.Rows.Count > 0)
                    {
                        //绑定listv
                        data.Clear();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data.Add(new SafeItem(
                            Convert.ToInt32(dt.Rows[i]["dangerID"].ToString()),
                                dt.Rows[i]["workArea"].ToString(),
                                dt.Rows[i]["dangerName"].ToString(),
                                dt.Rows[i]["accidentStand"].ToString(),
                                dt.Rows[i]["accidentMeasures"].ToString(),
                                dt.Rows[i]["dangerLevel"].ToString()
                               ));
                        }

                        myList = FindViewById<ListView>(Resource.Id.listView1);
                       // myList.ItemClick += OnListItemClick;
                        adapter = new SafeAdapter(this, data);
                        myList.Adapter = adapter;
                    }
                }
                else
                {
                    Toast.MakeText(this, "未查到相关风险信息", ToastLength.Short).Show();
                    //CommonFunction.ShowMessage("未查到相关风险信息", this, true);
                }

            }
            catch (SoapException ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion
    }
}