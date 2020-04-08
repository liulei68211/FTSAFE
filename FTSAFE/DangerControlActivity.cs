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
    [Activity(Label = "岗位应急处置卡信息")]
    public class DangerControlActivity : AppCompatActivity
    {
        private ListView myList;
        private List<DangerControlItem> data = new List<DangerControlItem>();
        private DangerControlAdapter adapter;
        private int hidenFlag = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_danger_control);

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
            select_dangerControlInfo();
            bdDangerListInfo();
        }
        #region 岗位应急处置卡
        private void bdDangerListInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            //xml 转table 
            try
            {
                string revXml = safeWeb.select_dangerControlInfo(XmlDBClass.accID, XmlDBClass.departID);
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
                            data.Add(new DangerControlItem(
                                 Convert.ToInt32(dt.Rows[i]["contolID"].ToString()),
                                dt.Rows[i]["dangerType"].ToString(),
                                dt.Rows[i]["dangerMain"].ToString()
                               ));
                        }
                        myList = FindViewById<ListView>(Resource.Id.listView1);

                        adapter = new DangerControlAdapter(this, data);
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

        #region 查询应急处置卡信息
        private void select_dangerControlInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            //应急值守电话
            TextView txt_phone = FindViewById<TextView>(Resource.Id.txtPhone);
            //单位负责人及电话
            TextView txt_phone_depart = FindViewById<TextView>(Resource.Id.txtDepartPhone);
            //火警电话
            TextView txt_phone_fire = FindViewById<TextView>(Resource.Id.txtFirePhone);
            //急救电话
            TextView txt_phone_emerg = FindViewById<TextView>(Resource.Id.txtEmergPhone);

            try
            {
                string revXml = safeWeb.select_dangerControlInfo(XmlDBClass.accID, XmlDBClass.departID);
                if (revXml != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                    if (dt.Rows.Count > 0)
                    {
                        txt_phone.Text = dt.Rows[0]["facIphone"].ToString();
                        txt_phone_depart.Text = dt.Rows[0]["deptIphone"].ToString();
                        txt_phone_fire.Text = dt.Rows[0]["fireIphone"].ToString();
                        txt_phone_emerg.Text = dt.Rows[0]["emergencyIphone"].ToString();
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