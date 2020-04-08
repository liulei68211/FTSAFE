using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Java.Net;

namespace FTSAFE
{
    [Activity(Label = "整改复查人信息录入", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class ReformEndActivity : AppCompatActivity
    {
        private int hidenFlag = 0;
        private Button bt_date = null;//复查时间
        private Bitmap bitmap;
        private Bitmap bitmapReform;
        private Button bt_sub = null;

        private EditText edit_person_review = null;
        //复查结果
        private EditText edit_result_review = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_reformend_new);

            //防止软键盘改变底部控件位置
            Window.SetSoftInputMode(SoftInput.AdjustPan);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "整改复查信息录入";
            toolbar.Subtitle = "只允许排查单位录入";
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
            XmlDBClass.autoID = Convert.ToInt32(Intent.GetStringExtra("autoID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            hidenFlag = Convert.ToInt32(Intent.GetStringExtra("hidenFlag"));
            XmlDBClass.hidenPerson = Intent.GetStringExtra("hidenPerson");
            XmlDBClass.hidenInfo = Intent.GetStringExtra("hidenInfo");
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            EditText hiden_depart = FindViewById<EditText>(Resource.Id.hidenDepart);
            hiden_depart.Text = XmlDBClass.departName;
            EditText hiden_person = FindViewById<EditText>(Resource.Id.hidenPerson);
            EditText hiden_info = FindViewById<EditText>(Resource.Id.hidenInfo);
            edit_person_review = FindViewById<EditText>(Resource.Id.editReviewPerson);
            edit_result_review = FindViewById<EditText>(Resource.Id.editReviewResult);
            hiden_info.Text = XmlDBClass.hidenInfo;
            hiden_person.Text = XmlDBClass.hidenPerson;

            bt_date = FindViewById<Button>(Resource.Id.editReviewTime);
            bt_date.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //复查时间
            bt_date.Click += delegate
            {
                DatePickerClass frag = DatePickerClass.NewInstance(delegate (DateTime time)
                {
                    bt_date.Text = time.ToLongDateString();
                });
                frag.Show(FragmentManager, DatePickerClass.TAG);
            };
            //提交按钮
             bt_sub = FindViewById<Button>(Resource.Id.btSub);
            bt_sub.Click += btSub_Click;

            try
            {
                Bitmap[] arr = hidenImagAdress();
                //从网上下载图片
                ImageView img_view = FindViewById<ImageView>(Resource.Id.imageHiden);
                img_view.Click += imgviewBig_Click_Hidenb;
                img_view.SetImageBitmap(arr[0]);
                ImageView img_view_1 = FindViewById<ImageView>(Resource.Id.imageReform);
                img_view_1.SetImageBitmap(arr[1]);
                img_view_1.Click += imgviewBig_Click_Reform;
            }
            catch (Exception ex)
            {

            }
            selectReformInfo();
        }

        #region 点击隐患上报图片放大
        private void imgviewBig_Click_Hidenb(object sender, EventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View imgEntryView = inflater.Inflate(Resource.Layout.imageBig, null); // 加载自定义的布局文件
            Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(this).Create();
            // ImageView img = (ImageView)imgEntryView.FindViewById(Resource.Id.large_image);
            ImageView img = imgEntryView.FindViewById<ImageView>(Resource.Id.large_image);
            //int height = Resources.DisplayMetrics.HeightPixels;
            //int width = height / 2;
            Bitmap[] arr = hidenImagAdress();
            try
            {
                //获取拍照的位图
                //  bitmap = _file.Path.LoadAndResizeBitmap(width, height);
                if (arr[0] != null)
                {
                    img.SetImageBitmap(arr[0]);
                    //清空bitmap 否则会出现oom问题
                    arr[0] = null;
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

        #region 点击整改图片放大
        private void imgviewBig_Click_Reform(object sender, EventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View imgEntryView = inflater.Inflate(Resource.Layout.imageBig, null); // 加载自定义的布局文件
            Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(this).Create();
            // ImageView img = (ImageView)imgEntryView.FindViewById(Resource.Id.large_image);
            ImageView img = imgEntryView.FindViewById<ImageView>(Resource.Id.large_image);
            //int height = Resources.DisplayMetrics.HeightPixels;
            //int width = height / 2;
            try
            {
                //获取拍照的位图
                //  bitmap = _file.Path.LoadAndResizeBitmap(width, height);
                Bitmap[] arr = hidenImagAdress();
                if (arr[1] != null)
                {
                    img.SetImageBitmap(arr[1]);
                    //清空bitmap 否则会出现oom问题
                    bitmapReform = null;
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

        #region 查询隐患信息并赋值
        private void selectReformInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //整改分厂
            EditText txt_fac = FindViewById<EditText>(Resource.Id.txtCheckFac);
            //整改类型
            EditText txt_type = FindViewById<EditText>(Resource.Id.txtCheckType);
            //整改期限
            EditText txt_time = FindViewById<EditText>(Resource.Id.txtTime);
            //整改要求
            EditText txt_request = FindViewById<EditText>(Resource.Id.txtCheckRequest);
            //整改责任人txtCheckPerson
            EditText txt_person = FindViewById<EditText>(Resource.Id.txtCheckPerson);
            //整改责任人txtCheckPerson_2
            EditText txt_person_2 = FindViewById<EditText>(Resource.Id.txtCheckPerson_2);
            //整改日期txtCheckTime
            EditText txt_check_time = FindViewById<EditText>(Resource.Id.txtCheckTime);
            //整改结果
            EditText edit_result = FindViewById<EditText>(Resource.Id.editResult);
            try
            {
                string xmlStr = safeWeb.selecthidenInfo(XmlDBClass.autoID, hidenFlag,XmlDBClass.accID);
                if (xmlStr != "")
                {
                    //转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(xmlStr);
                    if (dt.Rows.Count > 0)
                    {
                        txt_fac.Text = dt.Rows[0]["隐患整改部门"].ToString();
                        txt_type.Text = dt.Rows[0]["abarStyle"].ToString();
                        txt_time.Text = dt.Rows[0]["abarbeitungTime"].ToString();
                        txt_request.Text = dt.Rows[0]["abarRequest"].ToString();
                        txt_person.Text = dt.Rows[0]["abarbeitungPerson"].ToString();
                        txt_person_2.Text = dt.Rows[0]["abarbeitungPerson_2"].ToString();
                        txt_check_time.Text = dt.Rows[0]["abarTime"].ToString();
                        edit_result.Text = dt.Rows[0]["abarResult"].ToString();

                        int reviewID = Convert.ToInt32(dt.Rows[0]["departID"].ToString());
                        //判断当前岗位是否为整改单位
                        if (reviewID != XmlDBClass.departID)
                        {
                            //不是整改单位 只许看不许操作
                            bt_sub.Enabled = false;
                            // bt_sub.SetBackgroundColor(Color.Gray);
                            bt_sub.SetBackgroundResource(Resource.Drawable.btUnEnable);
                            edit_person_review.Enabled = false;
                            edit_result_review.Enabled = false;
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion

        #region 提交按钮
        private void btSub_Click(object sender, EventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //组装XML数据
            List<string> listData = new List<string>();
            List<string> listName = new List<string>();
            DateTime reviewTm = Convert.ToDateTime(bt_date.Text);
            string reviewtm = reviewTm.ToString("yyyy-MM-dd");//复查时间
            if (edit_person_review.Text != "" && edit_result_review.Text != "")
            {
                try
                {
                    listName.Add("reviewPerson");
                    listName.Add("reviewResult");
                    listName.Add("reviewTime");
                    listName.Add("reviewTS");

                    listData.Add(edit_person_review.Text);
                    listData.Add(edit_result_review.Text);
                    listData.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                    listData.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                    //组装xml数据
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNode node = xmldoc.CreateXmlDeclaration("1.0", "GBK", "");
                    xmldoc.AppendChild(node);
                    XmlNode root = xmldoc.CreateElement("xml_root");
                    xmldoc.AppendChild(root);
                    XmlNode node1 = xmldoc.CreateElement("out_infos");
                    root.AppendChild(node1);
                    string xml = xmldoc.OuterXml;

                    CreateXml crxml = new CreateXml();
                    xml = crxml.CreatCKXml(xml, listName, listData);

                    bool result = safeWeb.updateReformCheckInfo(XmlDBClass.autoID, xml);
                    if (result)
                    {
                        bt_sub.Enabled = false;
                        bt_sub.SetBackgroundResource(Resource.Drawable.btUnEnable);
                        //bt_sub.SetBackgroundColor(Color.Gray);
                        Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                        // CommonFunction.ShowMessage("操作成功", this, true);
                    }
                    else
                    {
                        Toast.MakeText(this, "操作失败，请检查网络连接", ToastLength.Short).Show();
                        // CommonFunction.ShowMessage("操作失败", this, true);
                    }
                }
                catch (Exception ex)
                {
                    CommonFunction.ShowMessage(ex.Message, this, true);
                }
            }
            else
            {
                CommonFunction.ShowMessage("请填写复查人、复查结果后再提交", this, true);
            }
            
        }
        #endregion

        #region 查询隐患上报图片地址
        private Bitmap[] hidenImagAdress()
        {
            string imgAdress = "";
            Bitmap[] bitmapArr = new Bitmap[2];
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
                string revXml = safeWeb.selecthidenInfo(XmlDBClass.autoID, 2, XmlDBClass.accID);
                if (revXml != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                    if (dt.Rows.Count > 0)
                    {
                        imgAdress = dt.Rows[0]["hidenImg"].ToString() + "|" + dt.Rows[0]["abarImg"].ToString();
                        string[] imgAarr = imgAdress.Split('|');
                        string hiden_url = imgAarr[0].ToString();
                        string reform_url = imgAarr[1].ToString();
                        //截取获取最后一个/后的内容
                        string url = hiden_url.Replace("D:/cloudsafe/public/", "http://safe.guotaiyun.cn/");
                        string url_1 = reform_url.Replace("D:/cloudsafe/public/", "http://safe.guotaiyun.cn/");
                        //下载图片
                        URL myUrl = new URL(url);
                        URLConnection uConn = myUrl.OpenConnection();
                        System.IO.Stream intput = uConn.InputStream;
                        bitmap = BitmapFactory.DecodeStream(intput);
                        bitmapArr[0] = bitmap;

                        URL myUrl_1 = new URL(url_1);
                        URLConnection uConn_1 = myUrl_1.OpenConnection();
                        System.IO.Stream intput_1 = uConn_1.InputStream;
                        bitmapReform = BitmapFactory.DecodeStream(intput_1);
                        bitmapArr[1] = bitmapReform;
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
           
            return bitmapArr;
        }
        #endregion
    }
}