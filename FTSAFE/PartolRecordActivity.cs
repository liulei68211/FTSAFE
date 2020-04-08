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
    [Activity(Label = "巡查记录")]
    public class PartolRecordActivity : AppCompatActivity
    {
        private ListView myList;
        private List<PartolDataItem> data = new List<PartolDataItem>();
        private PatrolDataAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_parotlrecord_new);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "风险巡查记录";
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

            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            //日期
            //Button bt_date = FindViewById<Button>(Resource.Id.btDate);
            //bt_date.Click += delegate
            //{
            //    DatePickerClass frag = DatePickerClass.NewInstance(delegate (DateTime time)
            //    {
            //        bt_date.Text = time.ToLongDateString();
            //    });
            //    frag.Show(FragmentManager, DatePickerClass.TAG);
            //};
            //bt_date.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

            bdPartolDataInfo();
        }

        #region 绑定风险巡查信息
        private void bdPartolDataInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();

            //xml 转table 
            try
            {
                string revXml = safeWeb.searchPartolData(XmlDBClass.userID);
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
                            data.Add(new PartolDataItem(
                                Convert.ToInt32(dt.Rows[i]["partolID"].ToString()),
                                dt.Rows[i]["partolPersonName"].ToString(),
                                dt.Rows[i]["name"].ToString(),
                                dt.Rows[i]["partolTime"].ToString(),
                                dt.Rows[i]["partolFlag"].ToString()
                               // dt.Rows[i]["hidentype"].ToString()
                               ));
                        }
                        myList = FindViewById<ListView>(Resource.Id.listView1);
                        adapter = new PatrolDataAdapter(this, data);
                        myList.Adapter = adapter;
                    }
                }
                else
                {
                    Toast.MakeText(this, "未查到相关风险信息", ToastLength.Short).Show();
                    //CommonFunction.ShowMessage("未查到相关岗位风险信息", this, true);
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