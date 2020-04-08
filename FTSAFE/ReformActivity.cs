using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using FTSAFE.Adapter;
using System.Data;
using System.Web.Services.Protocols;
using FTSAFE.CommonClass;
using Android.Support.V7.App;
using System.Threading.Tasks;

namespace FTSAFE
{
    [Activity(Label = "隐患信息列表", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class ReformActivity : AppCompatActivity
    {
        private ListView myList;
        private List<TableItem> data = new List<TableItem>();
        private CustomAdapter adapter;
        private int hidenFlag = 0;

        protected override void OnStart()
        {
            base.OnStart();
            //Console.WriteLine("调用OnStart");
            //bdHidneListInfo(hidenFlag);
        }
        //重载Resume()方法
        protected override void OnResume()
        {
            base.OnResume();
            //绑定隐患信息
           bdHidneListInfo(hidenFlag);
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
            SetContentView(Resource.Layout.activity_reform_new);

            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            switch (hidenFlag)
            {
                case 0:
                    toolbar.Title = "已上报隐患列表";
                    toolbar.Subtitle = "点击需要整改的隐患，下发整改通知单";
                    //修改toolbar标题字体大小
                    toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
                    //修改子标题大小
                    toolbar.SetSubtitleTextAppearance(this, Resource.Style.Toolbar_SubTitleText);
                    break;
                case 1:
                    toolbar.Title = "待整改隐患列表";
                    toolbar.Subtitle = "点击需要整改的隐患，录入整改信息";
                    //修改toolbar标题字体大小
                    toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
                    //修改子标题大小
                    toolbar.SetSubtitleTextAppearance(this, Resource.Style.Toolbar_SubTitleText);
                    break;
                case 2:
                    toolbar.Title = "待复查隐患列表";
                    toolbar.Subtitle = "点击需要复查的隐患，录入复查信息";
                    //修改toolbar标题字体大小
                    toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
                    //修改子标题大小
                    toolbar.SetSubtitleTextAppearance(this, Resource.Style.Toolbar_SubTitleText);
                    break;
                case 4:
                    toolbar.Title = "隐患信息列表";
                    toolbar.Subtitle = "点击需要整改、复查的隐患，录入相关信息";
                    //修改toolbar标题字体大小
                    toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
                    //修改子标题大小
                    toolbar.SetSubtitleTextAppearance(this, Resource.Style.Toolbar_SubTitleText);
                    break;
            }

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

            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
           // bdHidneListInfo( hidenFlag );
           
        }

        private  void bdHidneListInfo(int flag)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            //xml 转table 
            try
            {
                string revXml = safeWeb.hidenInfoList(XmlDBClass.departID, flag, XmlDBClass.accID);
                //绑定listv
                data.Clear();
                if (revXml != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data.Add(new TableItem(
                                Convert.ToInt32(dt.Rows[i]["hidenID"].ToString()),
                                dt.Rows[i]["hidenPersonName"].ToString(),
                                dt.Rows[i]["name"].ToString(),
                                dt.Rows[i]["hidenTime"].ToString(),
                                dt.Rows[i]["hidenInfo"].ToString(),
                                dt.Rows[i]["hidenFlag"].ToString()
                               ));
                        }
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
            myList = FindViewById<ListView>(Resource.Id.listView1);
            myList.ItemClick += OnListItemClick;
            adapter = new CustomAdapter(this, data);
          myList.Adapter = adapter;
        }
        #region 绑定隐患信息 异步
        //private async void bdHidneListInfo(int flag)
        //{
        //    await Task.Run(() =>
        //    {
        //        //for (int i = 0; i < 200000000; i++)
        //        //{
        //        //    string dd = "";
        //        //}
        //        SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
        //        XmlDBClass xmlDB = new XmlDBClass();
        //        //xml 转table 
        //        try
        //        {
        //            string revXml = safeWeb.hidenInfoList(XmlDBClass.departID, flag, XmlDBClass.accID);
        //            //绑定listv
        //            data.Clear();
        //            if (revXml != "")
        //            {
        //                //xml数据转table
        //                DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
        //                if (dt.Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dt.Rows.Count; i++)
        //                    {
        //                        data.Add(new TableItem(
        //                            Convert.ToInt32(dt.Rows[i]["hidenID"].ToString()),
        //                            dt.Rows[i]["hidenPersonName"].ToString(),
        //                            dt.Rows[i]["name"].ToString(),
        //                            dt.Rows[i]["hidenTime"].ToString(),
        //                            dt.Rows[i]["hidenInfo"].ToString(),
        //                            dt.Rows[i]["hidenFlag"].ToString()
        //                           ));
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Toast.MakeText(this, "未查到相关隐患信息", ToastLength.Short).Show();
        //                //CommonFunction.ShowMessage("未查到相关隐患信息",this,true);
        //            }                }
        //        catch (SoapException ex)
        //        {
        //            CommonFunction.ShowMessage(ex.Message, this, true);
        //        }
        //    });
        //    myList = FindViewById<ListView>(Resource.Id.listView1);
        //    myList.ItemClick += OnListItemClick;
        //    adapter = new CustomAdapter(this, data);
        //    myList.Adapter = adapter;
        //}
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
            XmlDBClass.departName = t.hidenDept;
            if (flagStr == "已上报")
            {
                hidenFlag = 0;
              
                Intent intent = new Intent(this, typeof(ReformAddActivity));
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
            else if (flagStr ==  "待整改")
            {
                hidenFlag = 1;
                Intent intent = new Intent(this, typeof(ReformCheckActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("autoID", XmlDBClass.autoID.ToString());
                intent.PutExtra("hidenFlag",hidenFlag.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("hidenPerson", XmlDBClass.hidenPerson);
                intent.PutExtra("departName", XmlDBClass.departName);
                intent.PutExtra("hidenInfo", XmlDBClass.hidenInfo);
                StartActivity(intent);
            }
            else if(flagStr == "待复查")
            {
                hidenFlag = 2;
                Intent intent = new Intent(this, typeof(ReformEndActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("autoID", XmlDBClass.autoID.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("hidenPerson", XmlDBClass.hidenPerson);
                intent.PutExtra("departName", XmlDBClass.departName);
                intent.PutExtra("hidenInfo", XmlDBClass.hidenInfo);
                StartActivity(intent);
            }
        }
        #endregion
    }
}