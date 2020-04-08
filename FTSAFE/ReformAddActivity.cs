using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using FTSAFE.CommonClass;
using Android.Content.PM;
using Android.Provider;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android;
using Android.Support.V4.App;
using System.IO;
using System.Xml;
using System.Data;
using Android.Support.V7.App;
using Android.Views;
using Java.Net;

namespace FTSAFE
{
    [Activity(Label = "下发整改通知单",LaunchMode =Android.Content.PM.LaunchMode.SingleTask)]
    public class ReformAddActivity : AppCompatActivity
    {
        private int currentDepartID = 0;
        private int hidenFlag = 0;
        private EditText edit_request = null;
        private Button bt_date = null;
        private Spinner abarTypeSpinner = null;
        private Spinner spinner_people = null;

        private Button bt_up = null;//转上一级按钮
        private Button bt_change = null;//移交按钮
        private Button bt_sub = null;//提交按钮

        private int bBt_1 = 0;//提交按钮是否点击
        private int bBt_2 = 0;//转上一级按钮是否点击
        private int bBt_3 = 0;//移交按钮是否点击

        protected override void OnStart()
        {
            base.OnStart();
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
            try
            {
                SetContentView(Resource.Layout.activity_reformadd_new);
                //防止软键盘改变底部控件位置
                Window.SetSoftInputMode(SoftInput.AdjustPan);
                Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                toolbar.Title = "整改通知单";
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

                edit_request = FindViewById<EditText>(Resource.Id.editRequest);
                bt_date = FindViewById<Button>(Resource.Id.editTime);
                bt_date.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                bt_date.Click += delegate
                {
                    DatePickerClass frag = DatePickerClass.NewInstance(delegate (DateTime time)
                    {
                        bt_date.Text = time.ToLongDateString();
                    });
                    frag.Show(FragmentManager, DatePickerClass.TAG);
                };
                //查询整改单位信息
                List<string> abarDepartList = selectAbarDepartInfo();
                //绑定spinner
                Spinner departSpinner = FindViewById<Spinner>(Resource.Id.spinerFac);
                abarTypeSpinner = FindViewById<Spinner>(Resource.Id.spinerAbarType);

                //绑定xml文件数据 
                // ArrayAdapter adapter_fac = ArrayAdapter.CreateFromResource(this, Resource.Array.facArrr, Resource.Layout.spinner_item);
                ArrayAdapter<string> adapter_depart = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, abarDepartList);
                adapter_depart.SetDropDownViewResource(Resource.Layout.dropdown_style);
                departSpinner.Adapter = adapter_depart;
                departSpinner.ItemSelected += departSpinner_ItemSelected;
                //绑定xml文件数据 整改类型
                ArrayAdapter adapter_abar_type = ArrayAdapter.CreateFromResource(this, Resource.Array.abarType, Resource.Layout.spinner_item);
                adapter_abar_type.SetDropDownViewResource(Resource.Layout.dropdown_style);
                abarTypeSpinner.Adapter = adapter_abar_type;

                //提交按钮
                 bt_sub = FindViewById<Button>(Resource.Id.btSub);
                bt_sub.Click += btSub_Click;
                //转上一级按钮
                 bt_up = FindViewById<Button>(Resource.Id.btUp);
                bt_up.Click += btUp_Click;
                //隐患移交按钮
                 bt_change = FindViewById<Button>(Resource.Id.btChange);
                bt_change.Click += btChange_Click;
                Bitmap bitmap =  hidenImagAdress();
                ImageView img_view = FindViewById<ImageView>(Resource.Id.imageHiden);
                img_view.Click += imgviewBig_Click;
                img_view.SetImageBitmap(bitmap);
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
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

        #region 转上一级按钮
        private void btUp_Click(object sender, EventArgs e)
        {
            //更新danger_hiden表中 deptID 为当前用户上级部门id deptIDOld 为原部门id
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
          
            try
            {
                //对话框
                var callDialog = new Android.App.AlertDialog.Builder(this);
                callDialog.SetMessage("确定该隐患要转给上一级部门吗?");
                callDialog.SetNeutralButton("确定", delegate
                {
                    int result = safeWeb.updateHidenUpDept(XmlDBClass.autoID, XmlDBClass.departID);
                    if (result == 2)
                    {
                        Toast.MakeText(this, "你已是最高部门，不能转上一级", ToastLength.Short).Show();
                        //CommonFunction.ShowMessage("你已是最高部门，不能转上一级",this,true);
                    }
                    else if (result == 1)
                    {
                        Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                        bt_sub.Enabled = false;
                        bt_sub.SetBackgroundColor(Color.Gray);
                        bt_change.Enabled = false;
                        bt_change.SetBackgroundColor(Color.Gray);
                    }
                    else
                    {
                        Toast.MakeText(this, "操作失败", ToastLength.Short).Show();
                    }
                });
                //取消按钮
                callDialog.SetNegativeButton("取消", delegate {

                });

                //显示对话框
                callDialog.Show();

            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion

        #region 隐患移交按钮
        private void btChange_Click(object sender, EventArgs e)
        {
           
            //对话框
            var callDialog = new Android.App.AlertDialog.Builder(this);
            callDialog.SetMessage("确定该隐患要跨部门下发整改通知单吗?");
            callDialog.SetNeutralButton("确定", delegate
            {
                bt_sub.Enabled = false;
                bt_sub.SetBackgroundColor(Color.Gray);
                bt_up.Enabled = false;
                bt_up.SetBackgroundColor(Color.Gray);
                //跳转到隐患移交页面
                Intent intent = new Intent(this, typeof(HidenChangeActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("userName", XmlDBClass.userName.ToString());
                intent.PutExtra("hidenFlag", hidenFlag.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);
                intent.PutExtra("departID", XmlDBClass.departID.ToString());
                intent.PutExtra("departCode", XmlDBClass.departCode);
                intent.PutExtra("departName", XmlDBClass.departName);
                intent.PutExtra("accID", XmlDBClass.accID.ToString());
                intent.PutExtra("hidenPerson", XmlDBClass.hidenPerson);
                intent.PutExtra("hidenInfo", XmlDBClass.hidenInfo);
                intent.PutExtra("companyName", XmlDBClass.companyName.ToString());
                intent.PutExtra("autoID", XmlDBClass.autoID.ToString());

                StartActivity(intent);
            });
            //取消按钮
            callDialog.SetNegativeButton("取消", delegate {

            });

            //显示对话框
            callDialog.Show();
           
        }
        #endregion

        #region 整改单位选择事件
        private void departSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            List<string> peopleList = new List<string>();
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //根据车号查询 list路线
            var CurSpinner = (Spinner)sender;
            string revXML = safeWeb.selectAbarDepartID(CurSpinner.SelectedItem.ToString(),XmlDBClass.accID);
            if (revXML != "")
            {
                //xml 转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);
                currentDepartID = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    peopleList.Add(dt.Rows[i]["user_nickname"].ToString());
                }
            }
            //currentDepartID = safeWeb.selectAbarDepartID(CurSpinner.SelectedItem.ToString());
            //绑定整改人
             spinner_people = FindViewById<Spinner>(Resource.Id.spinerResponseName);
            //绑定xml文件数据 
            // ArrayAdapter adapter_fac = ArrayAdapter.CreateFromResource(this, Resource.Array.facArrr, Resource.Layout.spinner_item);
            ArrayAdapter<string> adapter_response = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, peopleList);
            adapter_response.SetDropDownViewResource(Resource.Layout.dropdown_style);
            spinner_people.Adapter = adapter_response;
        }
        #endregion

        #region 绑定整改单位
        private List<string> selectAbarDepartInfo()
        {
            string revXML = "";
            List<string> abarDepartList = new List<string>();
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            revXML = safeWeb.selectChileDepartInfo(XmlDBClass.departID);
            if (revXML != "" )
            {
                if (revXML == "false")
                {
                    abarDepartList.Add(XmlDBClass.departName);
                }
                else
                {
                    //xml转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        abarDepartList.Add(dt.Rows[i]["name"].ToString());
                    }
                }
            }
            else
            {
                abarDepartList.Add("");
            }
            return abarDepartList;
        }
        #endregion

        #region 提交按钮
        private void btSub_Click(object sender,EventArgs e)
        {
           
            bBt_1 = 1;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //提交分厂 车间 整改类型 整改期限 整改要求
            //组装XML数据
            List<string> listData = new List<string>();
            List<string> listName = new List<string>();
            // hidenTime,hidenPtrStyleID,hidenTypeID,,hidenLevel,hidenInfo,hidenFlag
            DateTime abarTm = Convert.ToDateTime(bt_date.Text);
            string abartm = abarTm.ToString("yyyy-MM-dd");

            if (edit_request.Text != "")
            {
                try
                {
                    listName.Add("abarDepartID");
                    listName.Add("abarStyle");
                    listName.Add("abarbeitungTime");//整改期限
                    listName.Add("abarRequest");
                    listName.Add("abarTS");//整改期限ts
                    listName.Add("abarbeitungPerson");//整改责任人

                    listData.Add(currentDepartID.ToString());
                    listData.Add(abarTypeSpinner.SelectedItem.ToString());
                    listData.Add(string.Format("{0:yyyy-MM-dd}", DateTime.Now));
                    listData.Add(edit_request.Text);
                    listData.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                    listData.Add(spinner_people.SelectedItem.ToString());
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
                    try
                    {
                        bool result = safeWeb.updateReformInfo(XmlDBClass.autoID, xml);
                        if (result)
                        {
                            Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                            bt_up.Enabled = false;
                            bt_up.SetBackgroundResource(Resource.Drawable.btUnEnable);
                            //  bt_up.SetBackgroundColor(Color.Gray);
                            bt_sub.Enabled = false;
                            bt_sub.SetBackgroundResource(Resource.Drawable.btUnEnable);
                            // bt_sub.SetBackgroundColor(Color.Gray);
                            bt_change.Enabled = false;
                            bt_change.SetBackgroundResource(Resource.Drawable.btUnEnable);
                            // bt_change.SetBackgroundColor(Color.Gray);
                            //CommonFunction.ShowMessage("操作成功", this, true);
                        }
                        else
                        {
                            Toast.MakeText(this, "操作失败,请检查网络连接", ToastLength.Short).Show();
                            // CommonFunction.ShowMessage("操作失败", this, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunction.ShowMessage(ex.Message, this, true);
                    }
                }
                catch (Exception ex)
                {
                    CommonFunction.ShowMessage(ex.Message, this, true);
                }
            }
            else
            {
                CommonFunction.ShowMessage("请填写整改要求后再提交",this,true);
            }
           
        }
        #endregion

        #region 查询隐患上报图片地址
        private Bitmap hidenImagAdress()
        {
            Bitmap bitmap = null;
            string imgAdress = "";
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
            string revXml = safeWeb.selecthidenInfo(XmlDBClass.autoID, 0, XmlDBClass.accID);
            //从网上下载隐患图片
            if (revXml != "")
            {
                //xml数据转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                if (dt.Rows.Count > 0)
                {
                    imgAdress = dt.Rows[0]["hidenImg"].ToString();

                  
                    //截取获取最后一个/后的内容
                    string url = imgAdress.Replace("D:/cloudsafe/public/", "http://safe.guotaiyun.cn/");
                    //下载图片
                    URL myUrl = new URL(url);
                    URLConnection uConn = myUrl.OpenConnection();
                    System.IO.Stream intput = uConn.InputStream;
                    bitmap = BitmapFactory.DecodeStream(intput);
                  
                }
            }
            else
            {
                Toast.MakeText(this, "未查到相关隐患信息", ToastLength.Short).Show();

                //CommonFunction.ShowMessage("未查到相关隐患信息",this,true);
            }
            return bitmap;
        }
        #endregion

        #region 操作成功
        private void WriteDataOK(bool ok)
        {
            if (ok)
            {
                CommonFunction.ShowMessage("操作成功！", this, true);
            }
            else CommonFunction.ShowMessage("提交失败！", this, true);
        }
        #endregion
    }
}