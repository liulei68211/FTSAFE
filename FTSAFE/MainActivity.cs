using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using System;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Xml;

namespace FTSAFE
{
    [Activity(Label = "首页")]
    public class MainActivity : AppCompatActivity
    {
        BottomNavigationView navigation;
        private int hidenFlag = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //标题栏增加返回按钮
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
           
            XmlDBClass.userID =  Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.userName = Intent.GetStringExtra("userName");
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.departCode = Intent.GetStringExtra("departCode");
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.workArea = Intent.GetStringExtra("workArea"); 
            XmlDBClass.stationID = Convert.ToInt32(Intent.GetStringExtra("stationID"));
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));

            //隐患录入
            ImageButton hidenAdd_bt = FindViewById<ImageButton>(Resource.Id.img_hiden_add);
            hidenAdd_bt.Click += delegate
            {
                hidenFlag = 0;
                Intent intent = new Intent(this, typeof(HidenAddActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                intent.PutExtra("hideFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("departCode", XmlDBClass.departCode);
                intent.PutExtra("departName", XmlDBClass.departName);
                intent.PutExtra("userName", XmlDBClass.userName);
                StartActivity(intent);
            };
            //下发整改单
            ImageButton hidenReform_bt = FindViewById<ImageButton>(Resource.Id.img_hiden_reform);
            hidenReform_bt.Click += delegate
            {
                hidenFlag = 0;
                Intent intent = new Intent(this, typeof(ReformActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                StartActivity(intent);
            };
            //整改责任人录入
            ImageButton hidenCheck_bt = FindViewById<ImageButton>(Resource.Id.img_hiden_check);
            hidenCheck_bt.Click += delegate
            {
                hidenFlag = 1;
                Intent intent = new Intent(this, typeof(ReformActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                StartActivity(intent);
            };
            //整改复查人录入
            ImageButton hidenEcn_bt = FindViewById<ImageButton>(Resource.Id.img_hiden_end);
            hidenEcn_bt.Click += delegate
            {
                hidenFlag = 2;
                Intent intent = new Intent(this, typeof(ReformActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                StartActivity(intent);
            };
            //隐患查询
            ImageButton hidenSearch_bt = FindViewById<ImageButton>(Resource.Id.img_hiden_search);
            hidenSearch_bt.Click += delegate
            {
                hidenFlag = 4;
                Intent intent = new Intent(this, typeof(ReformActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                StartActivity(intent);
            };
            //风险巡查
            ImageButton partolAdd_bt = FindViewById<ImageButton>(Resource.Id.img_partol_add);
            partolAdd_bt.Click += delegate
            {
                Intent intent = new Intent(this, typeof(PartolActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("departName", XmlDBClass.departName);
                intent.PutExtra("userName", XmlDBClass.userName);
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("workArea", XmlDBClass.workArea);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());

                StartActivity(intent);
            };
            //巡查记录
            ImageButton partolRecord_bt = FindViewById<ImageButton>(Resource.Id.img_partol_record);
            partolRecord_bt.Click += delegate
            {
                Intent intent = new Intent(this, typeof(PartolRecordActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                StartActivity(intent);
            };
            //设备点巡检
            ImageButton equipment_bt = FindViewById<ImageButton>(Resource.Id.img_equipment);
            equipment_bt.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(EquipmentActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("userName", XmlDBClass.userName);
                intent.PutExtra("stationID", XmlDBClass.stationID.ToString());
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                StartActivity(intent);
            };
            //相关制度
            ImageButton regism_bt = FindViewById<ImageButton>(Resource.Id.img_resim);
            regism_bt.Click += delegate
            {
                Intent intent = new Intent(this, typeof(RegimeActivity));
                StartActivity(intent);
            };
        }

        #region 查询隐患统计
        //private void hidenCount()
        //{
        //    TextView txtSum = FindViewById<TextView>(Resource.Id.txt_sum);
        //    txtSum.SetTextColor(Android.Graphics.Color.Blue);
        //    TextView btSend = FindViewById<TextView>(Resource.Id.txt_send);
        //    // txtSend.SetTextColor(Android.Graphics.Color.Red);
        //    TextView btWait = FindViewById<TextView>(Resource.Id.txt_reform);
        //    // txtWait.SetTextColor(Android.Graphics.Color.Orange);
        //    TextView btCheck = FindViewById<TextView>(Resource.Id.txt_check);
        //    // txtCheck.SetTextColor(Android.Graphics.Color.Gray);
        //    TextView btEnd = FindViewById<TextView>(Resource.Id.txt_end);
        //   // txtEnd.SetTextColor(Android.Graphics.Color.Green);
        //    //隐患统计
        //    SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
        //    //调用隐患统计方法
        //    string revXml = safeWeb.hidenStatis(XmlDBClass.userCode);
        //    //xml数据转table
        //    DataTable dt = ConvertXMLToDataTable(revXml);
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            if (dt.Rows[i]["hidenFlag"].ToString() == "0")
        //            {
        //                btSend.Text = dt.Rows[i]["counts"].ToString();
        //            }
        //            if (dt.Rows[i]["hidenFlag"].ToString() == "1")
        //            {
        //                btWait.Text = dt.Rows[i]["counts"].ToString();
        //            }
        //            if (dt.Rows[i]["hidenFlag"].ToString() == "2")
        //            {
        //                btCheck.Text = dt.Rows[i]["counts"].ToString();
        //            }
        //            if (dt.Rows[i]["hidenFlag"].ToString() == "3")
        //            {
        //                btEnd.Text = dt.Rows[i]["counts"].ToString();
        //            }
        //            int sum = Convert.ToInt32(btSend.Text) + Convert.ToInt32(btWait.Text) + Convert.ToInt32(btSend.Text) + Convert.ToInt32(btWait.Text) + Convert.ToInt32(btCheck.Text) + Convert.ToInt32(btEnd.Text);
        //            txtSum.Text = sum.ToString();
        //        }
        //    }
        //}
        #endregion

        #region 查询是否巡查
        //private void partolSearch()
        //{
        //    SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
        //    TextView txtWaring = FindViewById<TextView>(Resource.Id.textPartol);

        //    string revStr = safeWeb.partolStation(XmlDBClass.userCode);
        //    if (revStr == "")
        //    {
        //        txtWaring.Text = XmlDBClass.userName + "请在今天24点前提交巡检记录";
        //        txtWaring.SetTextColor(Android.Graphics.Color.Red);
        //    }
        //    else
        //    {
        //        txtWaring.Text = XmlDBClass.userName + "今天已巡检完成";
        //        txtWaring.SetTextColor(Android.Graphics.Color.Green);
        //    }
        //}
        #endregion

        #region xml数据转table
        private static DataTable ConvertXMLToDataTable(string xmlData)
        {
            TextReader sr = null;
            try
            {
                DataTable dt = new DataTable();
                sr = new StringReader(xmlData);
                dt.ReadXml(sr);
                return dt;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null) sr.Close();
            }
        }
        #endregion
    }
}

