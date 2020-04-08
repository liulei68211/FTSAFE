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
using FTSAFE.Adapter;
using System.Data;
using System.Web.Services.Protocols;
namespace FTSAFE
{
    [Activity(Label = "DangerListActivity")]
    public class DangerListActivity : AppCompatActivity
    {
        private ListView myList;
        private List<DangerListItem> data = new List<DangerListItem>();
        private DangerListAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_danger_list);

            // Create your application here
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            toolbar.Title = "岗位风险告知卡";
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
                Finish();
            };
            bdHidneListInfo();
        }
        #region 绑定岗位风险信息
        private void bdHidneListInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            //xml 转table 
            try
            {
                string revXml = safeWeb.select_dangerInfo(XmlDBClass.accID, XmlDBClass.departID);
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
                            data.Add(new DangerListItem(

                            Convert.ToInt32(dt.Rows[i]["dangerID"].ToString()),
                                dt.Rows[i]["dangerName"].ToString(),
                                dt.Rows[i]["dangerInfo"].ToString(),
                                dt.Rows[i]["accidentStand"].ToString(),
                                dt.Rows[i]["accidentMeasures"].ToString(),
                                dt.Rows[i]["dangerLevel"].ToString()
                               ));
                        }
                        myList = FindViewById<ListView>(Resource.Id.listView1);

                        adapter = new DangerListAdapter(this, data);
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