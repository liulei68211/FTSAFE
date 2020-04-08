using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using System.Data;
using FTSAFE.Adapter;
using System.Web.Services.Protocols;

namespace FTSAFE
{
    [Activity(Label = "隐患排查清单")]
    public class HidenListActivity : AppCompatActivity
    {
        private ListView myList;
        private List<HidenListItem> data = new List<HidenListItem>();
        private HidenListAdapter adapter;
        private int hidenFlag = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_hidenlist_view);

            // Create your application here
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            toolbar.Title = "岗位隐患排查清单";
            toolbar.Subtitle = "岗位名称：" + XmlDBClass.departName;
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
            //修改子标题大小
            toolbar.SetSubtitleTextAppearance(this, Resource.Style.Toolbar_SubTitleText);
            SetSupportActionBar(toolbar);
            //设置返回按钮
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //响应返回按钮
            toolbar.NavigationClick += (s, e) =>
            {
                //Intent i = new Intent(this, typeof(NewMainActivity));
                //i.PutExtra("backCode", "homeBack");
                //StartActivity(i);
                Finish();
            };
            bdDangerListInfo();
        }


        #region 隐患排查清单
        private void bdDangerListInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            //xml 转table 
            try
            {
                string revXml = safeWeb.select_hidenListInfo(XmlDBClass.accID, XmlDBClass.departID);
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
                            data.Add(new HidenListItem(
                                 Convert.ToInt32(dt.Rows[i]["hidenListID"].ToString()),
                                dt.Rows[i]["checkArea"].ToString(),
                                dt.Rows[i]["checkObj"].ToString(),
                                dt.Rows[i]["dangerYS"].ToString(),
                                dt.Rows[i]["dangerType"].ToString(),
                                dt.Rows[i]["dangerStandard"].ToString(),
                                dt.Rows[i]["dangerLevel"].ToString()
                               ));
                        }
                        myList = FindViewById<ListView>(Resource.Id.listView1);

                        adapter = new HidenListAdapter(this, data);
                        myList.Adapter = adapter;
                    }
                }
                else
                {
                    Toast.MakeText(this, "未查到相关隐患信息", ToastLength.Short).Show();

                    //CommonFunction.ShowMessage("未查到相关隐患信息",this,true);
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