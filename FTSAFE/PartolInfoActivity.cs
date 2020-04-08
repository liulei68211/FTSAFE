using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services.Protocols;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FTSAFE.Adapter;
using FTSAFE.CommonClass;

namespace FTSAFE
{
    [Activity(Label = "岗位风险信息")]
    public class PartolInfoActivity : AppCompatActivity
    {
        private ListView myList;
        private List<PartolItem> data = new List<PartolItem>();
        private PartolInfoAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_partolInfo_new);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "风险点信息";
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

        #region 绑定风险点信息
        private void bdHidneListInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();

            string roleName = safeWeb.roleSafe(XmlDBClass.userID);
            
            //xml 转table 
            try
            {
                string revXml = safeWeb.select_dangerInfo(XmlDBClass.accID,XmlDBClass.departID);
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
                            data.Add(new PartolItem(
                            Convert.ToInt32(dt.Rows[i]["dangerID"].ToString()),
                                dt.Rows[i]["dangerName"].ToString(),
                                dt.Rows[i]["dangerInfo"].ToString(),
                                dt.Rows[i]["accidentStand"].ToString(),
                                dt.Rows[i]["accidentMeasures"].ToString(),
                                dt.Rows[i]["dangerLevel"].ToString()
                               ));
                    }

                        myList = FindViewById<ListView>(Resource.Id.listView1);
                        myList.ItemClick += OnListItemClick;
                        adapter = new PartolInfoAdapter(this, data);
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

        #region listview 点击事件 填报隐患
        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //获取id 调整到隐患整改信息填报页面
            var listView = sender as ListView;
            adapter.setCurrentItem(e.Position);
            //通知ListView改变状态
            //notifyDataSetChanged调用后，listview会刷新所有已经显示出来的itemView

            adapter.NotifyDataSetChanged();

            var t = data[e.Position];
            XmlDBClass.autoID = t.itemOrder;

            //对话框
            var callDialog = new Android.App.AlertDialog.Builder(this);

            callDialog.SetMessage("确定提交隐患信息吗");
            callDialog.SetNeutralButton("确定", delegate
            {
                Intent intent = new Intent(this, typeof(HidenAddActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("autoID", XmlDBClass.autoID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);

                StartActivity(intent);
                Finish();
            });
            //取消按钮
            callDialog.SetNegativeButton("取消", delegate
            {
            });

            //显示对话框
            callDialog.Show();
        }
        #endregion
    }
}