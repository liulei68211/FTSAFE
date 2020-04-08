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
    [Activity(Label = "隐患审核信息列表")]
    public class AuitListActivity : AppCompatActivity
    {
        private ListView myList;
        private List<AuitItem> data = new List<AuitItem>();
        private HidenAuitAdapter adapter;
        private int hidenFlag = 0;
        protected override void OnStart()
        {
            base.OnStart();
            //Console.WriteLine("调用OnStart");
        }
        //重载Resume()方法
        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.acitvity_auit_list);
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            toolbar.Title = "待审核整改通知单列表";
            toolbar.Subtitle = "点击需要审核的隐患，进入审核页面";
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

            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            bdHidneListInfo(hidenFlag);
        }

        #region 绑定隐患信息
        private void bdHidneListInfo(int flag)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();

            //xml 转table 
            try
            {
                string revXml = safeWeb.selectHidenAuitDataList(XmlDBClass.accID,XmlDBClass.departID);

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
                            data.Add(new AuitItem(
                                Convert.ToInt32(dt.Rows[i]["hidenID"].ToString()),
                                dt.Rows[i]["hidenPersonName"].ToString(),
                                dt.Rows[i]["上报部门"].ToString(),
                                dt.Rows[i]["hidenTime"].ToString(),
                                dt.Rows[i]["hidenInfo"].ToString(),
                                dt.Rows[i]["hidenFlag"].ToString()
                               ));
                        }
                        myList = FindViewById<ListView>(Resource.Id.listView1);
                        myList.ItemClick += OnListItemClick;
                        adapter = new HidenAuitAdapter(this, data);
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

        #region listview 点击事件
        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //获取id 调整到隐患整改信息填报页面
            var listView = sender as ListView;
            adapter.setCurrentItem(e.Position);
            //通知ListView改变状态
            adapter.NotifyDataSetChanged();

            var t = data[e.Position];
            XmlDBClass.autoID = t.itemOrder;
            string flagStr = t.hidenStatus;
            XmlDBClass.hidenInfo = t.hidenInfo;
            XmlDBClass.hidenPerson = t.hidenPerson;

            Intent intent = new Intent(this, typeof(HidenAuitActivity));
            intent.PutExtra("userID", XmlDBClass.userID.ToString());
            intent.PutExtra("userCode", XmlDBClass.userCode);
            intent.PutExtra("autoID", XmlDBClass.autoID.ToString());
            intent.PutExtra("departID", XmlDBClass.departID.ToString());
            intent.PutExtra("accID", XmlDBClass.accID.ToString());
            intent.PutExtra("hidenFlag", hidenFlag.ToString());

            intent.PutExtra("departName", XmlDBClass.departName);
            intent.PutExtra("hidenPerson", XmlDBClass.hidenPerson);
            intent.PutExtra("hidenInfo", XmlDBClass.hidenInfo);
            StartActivity(intent);
        }
        #endregion
    }
}