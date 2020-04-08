using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Java.Net;

namespace FTSAFE
{
    [Activity(Label = "隐患审核")]
    public class HidenAuitActivity : AppCompatActivity
    {
       private Button bt_auit = null;
        private Button bt_auit_no = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hiden_auit);
            try
            {
                //防止软键盘改变底部控件位置
                Window.SetSoftInputMode(SoftInput.AdjustPan);

                Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                toolbar.Title = "跨部门整改通知单审核";
                //修改toolbar标题字体大小
                toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);

                SetSupportActionBar(toolbar);
                //设置返回按钮
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //响应返回按钮
                toolbar.NavigationClick += (s, e) =>
                {
                    Finish();
                };

                //调用类中的方法 普通方法 需要实例化类 静态方法 不需要实例化
                XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
                XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
                XmlDBClass.userCode = Intent.GetStringExtra("userCode");
                XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
                XmlDBClass.hidenPerson = Intent.GetStringExtra("hidenPerson");
                XmlDBClass.hidenInfo = Intent.GetStringExtra("hidenInfo");
                XmlDBClass.departName = Intent.GetStringExtra("departName");

                EditText hiden_person = FindViewById<EditText>(Resource.Id.hidenPerson);
                EditText hiden_depart = FindViewById<EditText>(Resource.Id.hidenDepart);
                hiden_depart.Text = XmlDBClass.departName;
                EditText hiden_info = FindViewById<EditText>(Resource.Id.hidenInfo);
                hiden_info.Text = XmlDBClass.hidenInfo;
                hiden_person.Text = XmlDBClass.hidenPerson;

                //从网上下载隐患图片
                ImageView img_view = FindViewById<ImageView>(Resource.Id.imageHiden);
                img_view.Click += imgviewBig_Click;
                Bitmap bitmapp = hidenImagAdress();
                img_view.SetImageBitmap(bitmapp);

                //审核按钮
                 bt_auit = FindViewById<Button>(Resource.Id.btAuit);
                bt_auit.Click += btAuit_Click;
                //弃审按钮
                 bt_auit_no = FindViewById<Button>(Resource.Id.btAuitNo);
                bt_auit_no.Click += btAuitNo_Click;
                selectAuitInfo();
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }

        #region 点击图片放大
        private void imgviewBig_Click(object sender, EventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View imgEntryView = inflater.Inflate(Resource.Layout.imageBig, null); // 加载自定义的布局文件
            Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(this).Create();
            // ImageView img = (ImageView)imgEntryView.FindViewById(Resource.Id.large_image);
            ImageView img = imgEntryView.FindViewById<ImageView>(Resource.Id.large_image);
            Bitmap bitmap = hidenImagAdress();
            try
            {
                //获取拍照的位图
                // bitmap = _file.Path.LoadAndResizeBitmap(width, height);
                if (bitmap != null)
                {
                    img.SetImageBitmap(bitmap);
                    //清空bitmap 否则会出现oom问题
                    bitmap = null;
                    //imageDownloader.download("图片地址", img); // 这个是加载网络图片的，可以是自己的图片设置方法
                    dialog.SetView(imgEntryView); // 自定义dialog
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 审核按钮
        private void btAuit_Click(object sender, EventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            try
            {

                int result = safeWeb.updateAuitInfo(XmlDBClass.autoID,XmlDBClass.userName);
                if (result == 1)
                {
                    bt_auit_no.Enabled = false;
                    bt_auit_no.SetBackgroundResource(Resource.Drawable.btUnEnable);
                   // bt_auit.SetBackgroundColor(Color.Gray);
                    bt_auit.Enabled = false;
                    bt_auit.SetBackgroundResource(Resource.Drawable.btUnEnable);
                   // bt_auit.SetBackgroundColor(Color.Gray);
                    Toast.MakeText(this, "审核成功", ToastLength.Short).Show();
                   // CommonFunction.ShowMessage("审核成功", this, true);
                }
                else
                {
                    Toast.MakeText(this, "审核失败,请检查网络连接", ToastLength.Short).Show();
                    //CommonFunction.ShowMessage("审核失败", this, true);
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion

        #region 弃审按钮
        private void btAuitNo_Click(object sender, EventArgs e)
        {
            EditText auit_memos = FindViewById<EditText>(Resource.Id.auitMemos);
            auit_memos.Enabled = true;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            try
            {
                string auitMemos = auit_memos.Text;
                if (auitMemos != "")
                {
                    int result = safeWeb.updateAuitInfoNO(auitMemos, XmlDBClass.autoID, XmlDBClass.userName);
                    if (result == 1)
                    {
                        bt_auit_no.Enabled = false;
                        bt_auit.SetBackgroundColor(Color.Gray);
                        bt_auit.Enabled = false;
                        bt_auit.SetBackgroundColor(Color.Gray);

                        Toast.MakeText(this, "审核成功", ToastLength.Short).Show();
                        //CommonFunction.ShowMessage("弃审成功", this, true);
                    }
                    else
                    {
                        Toast.MakeText(this, "弃审失败,请检查网络连接", ToastLength.Short).Show();
                       // CommonFunction.ShowMessage("弃审失败", this, true);
                    }
                }
                else
                {
                    CommonFunction.ShowMessage("请填写弃审原因",this,true);
                }
             
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion

        #region 查询隐患上报图片地址
        private Bitmap hidenImagAdress()
        {
            Bitmap bitmap = null;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
           
            //添加以下代码 防止程序抛出NetWorkOnMainThreadException 异常
            StrictMode.SetThreadPolicy(new StrictMode.ThreadPolicy.Builder()
                                                                    .DetectDiskReads()
                                                                    .DetectDiskReads()
                                                                    .DetectNetwork()
                                                                    .PenaltyLog()
                                                                    .Build());
            StrictMode.SetVmPolicy(new StrictMode.VmPolicy.Builder()
                                                                  .DetectLeakedSqlLiteObjects()
                                                                  .DetectLeakedClosableObjects()
                                                                  .PenaltyLog()
                                                                  .PenaltyDeath()
                                                                  .Build());


            try
            {
                string revXml = safeWeb.selecthidenInfo(XmlDBClass.autoID, 1, XmlDBClass.accID);
                if (revXml != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                    if (dt.Rows.Count > 0)
                    {
                       string imgAdress = dt.Rows[0]["hidenImg"].ToString();
                        if (imgAdress != "")
                        {
                            //截取获取最后一个/后的内容
                            string url = imgAdress.Replace("D:/cloudsafe/public/", "http://safe.guotaiyun.cn/");
                            //下载图片
                            URL myUrl = new URL(url);
                            URLConnection uConn = myUrl.OpenConnection();
                            System.IO.Stream intput = uConn.InputStream;
                            bitmap = BitmapFactory.DecodeStream(intput);
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, "未查到相关隐患信息", ToastLength.Short).Show();

                    //CommonFunction.ShowMessage("未查到相关隐患信息",this,true);
                }
            }
            catch (Exception ex)
            {

            }

            
            return bitmap;
        }
        #endregion

        #region 查询隐患审核信息 当前用户id 与 隐患表中的 iAuitDepartID 相等
        private void selectAuitInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //整改部门
            EditText txt_abardept = FindViewById<EditText>(Resource.Id.abarDepatName);
            //整改类型
            EditText txt_abartype = FindViewById<EditText>(Resource.Id.abarTypeName);
            //整改期限
            EditText txt_abartime = FindViewById<EditText>(Resource.Id.abarTime);
            //整改要求
            EditText txt_abarrequest = FindViewById<EditText>(Resource.Id.abarRequest);
            //审核部门
            EditText txt_auit = FindViewById<EditText>(Resource.Id.auitDepartName);
            //弃审原因
            EditText txt_memos = FindViewById<EditText>(Resource.Id.auitMemos);
            try
            {
                string xmlStr = safeWeb.selectHidenAuitData( XmlDBClass.accID,XmlDBClass.departID,XmlDBClass.autoID);
                if (xmlStr != "")
                {
                    //转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(xmlStr);
                    if (dt.Rows.Count > 0)
                    {
                        txt_abardept.Text = dt.Rows[0]["隐患整改部门"].ToString();
                        txt_abartype.Text = dt.Rows[0]["abarStyle"].ToString();
                        txt_abartime.Text = dt.Rows[0]["abarbeitungTime"].ToString();
                        txt_abarrequest.Text = dt.Rows[0]["abarRequest"].ToString();
                        txt_auit.Text = dt.Rows[0]["审核部门"].ToString();
                

                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion
    }
}