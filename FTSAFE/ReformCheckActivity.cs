using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Java.Net;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
namespace FTSAFE
{
    /// <summary>
    /// 只有整改单位可以编辑
    /// </summary>
    [Activity(Label = "隐患确认", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class ReformCheckActivity : AppCompatActivity
    {  
        private ImageView _imageView;
        private Java.IO.File _file;
        private Java.IO.File _dir;
        private Bitmap bitmap;
        private string fileAdress;
        private int hidenFlag = 0;
        private Button bt_date = null;
        private Button bt_sub = null;
        private int isCamera = 0;//判断是否拍照

        private EditText edit_person_2 = null;
        private  EditText edit_result = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_reformcheck_new);

            //防止软键盘改变底部控件位置
            Window.SetSoftInputMode(SoftInput.AdjustPan);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "整改信息录入";
            toolbar.Subtitle = "只允许整改单位录入";
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
   
            EditText hiden_person = FindViewById<EditText>(Resource.Id.hidenPerson);
            EditText hiden_info = FindViewById<EditText>(Resource.Id.hidenInfo);
            hiden_info.Text = XmlDBClass.hidenInfo;
            hiden_person.Text = XmlDBClass.hidenPerson;
             edit_person_2 = FindViewById<EditText>(Resource.Id.editCheckPerson2);
             edit_result = FindViewById<EditText>(Resource.Id.editResult);

          

            #region 不添加此段代码 无法打开照相机
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
            {
                StrictMode.VmPolicy.Builder bulider = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(bulider.Build());
            }
            #endregion

            if (IsThereAnAppToTakePictures())   //判断本设备是否存在拍照功能
            {
                CreateDirectoryForPictures();
                //拍照按钮  保存图片到Picture文件夹下
                ImageButton btCamera = FindViewById<ImageButton>(Resource.Id.btCamera);
                _imageView = FindViewById<ImageView>(Resource.Id.imageReform);
                _imageView.Click += imgviewBig_Click;
                btCamera.Click += delegate
                {
                    isCamera = 1;
                    Intent intent = new Intent(MediaStore.ActionImageCapture);
                    //保存路径
                    _file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
                    fileAdress = _file.ToString();
                    // 保存图片
                    intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
                    StartActivityForResult(intent, 0);
                };
            }

            try
            {
                //从网上下载图片
                ImageView img_view = FindViewById<ImageView>(Resource.Id.imageHiden);
                img_view.Click += imgviewBig_Click_Reform;
                Bitmap bitmap = hidenImagAdress();
                img_view.SetImageBitmap(bitmap);

            }
            catch (Exception ex)
            {

            }

            //整改日期
            bt_date = FindViewById<Button>(Resource.Id.btCheckTime);
          
            bt_date.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
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
            selectReformInfo();
           
        }
        #region 点击图片放大
        private void imgviewBig_Click(object sender, EventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View imgEntryView = inflater.Inflate(Resource.Layout.imageBig, null); // 加载自定义的布局文件
            Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(this).Create();
            // ImageView img = (ImageView)imgEntryView.FindViewById(Resource.Id.large_image);
            ImageView img = imgEntryView.FindViewById<ImageView>(Resource.Id.large_image);
            int height = Resources.DisplayMetrics.HeightPixels;
            int width = height / 2;
            try
            {
                //获取拍照的位图
                bitmap = _file.Path.LoadAndResizeBitmap(width, height);
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

        #region 点击隐患上报图片放大
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
                Bitmap bitmapReform = hidenImagAdress();
              
                //获取拍照的位图
                //  bitmap = _file.Path.LoadAndResizeBitmap(width, height);
                if (bitmapReform != null)
                {
                    img.SetImageBitmap(bitmapReform);
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

        #region 照相机返回结果
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //用户操作完 结果码返回 -1 
            base.OnActivityResult(requestCode, resultCode, data);

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(_file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;

            //获取拍照的位图
            bitmap = _file.Path.LoadAndResizeBitmap(width, height);

            if (bitmap != null)
            {
                //将图片绑定到控件上
                _imageView.SetImageBitmap(bitmap);

                //清空bitmap 否则会出现oom问题
                bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }
        #endregion

        #region 创建目录图片
        private void CreateDirectoryForPictures()
        {
            string[] PERMISSIONS_STORAGE = {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                             Manifest.Permission.Camera
                            };

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, PERMISSIONS_STORAGE, 1);
            }

            //在外部存储装置中建立图片保存文件夹
            _dir = new Java.IO.File(
                  Environment.GetExternalStoragePublicDirectory(
                  Environment.DirectoryPictures), "CameraAppDemo");//CameraAppDemo
            if (!_dir.Exists())
            {
                bool result = _dir.Mkdirs();
            }
        }
        #endregion

        #region 判断是否有拍照功能
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
             PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        #endregion

        #region 图片转为string流
        public string ImageToString(string file_path)
        {
            //待上传图片路径
            //string uploadFile = file_path;

            //转化成文件  
            //System.IO.FileInfo imgFile = new System.IO.FileInfo(uploadFile);

            ////文件转化成字节
            //byte[] imgByte = new byte[imgFile.Length];

            //////读文件               
            //System.IO.FileStream imgStream = imgFile.OpenRead();

            //////文件写入到字节数组
            //imgStream.Read(imgByte, 0, Convert.ToInt32(imgFile.Length));

            //////字节数组转换成String类型
            //string by = Convert.ToBase64String(imgByte);

            ////上传到服务器 后面是文件名 
            ////fileUp.UpdateFile(imgByte, Guid.NewGuid() + ".png");

            //return imgByte;  

            //将图片文件转换成bitmap 格式：/storage/emulated/0/DCIM/Camera/IMG_20180425_105725.jpg
            Bitmap bitmap = BitmapFactory.DecodeFile(file_path);
            string bitstring = BitmapToString(bitmap);
            bitmap = null;              //一定要清空，否则会导致OOM问题
            GC.Collect();
            return bitstring;
        }
        #endregion

        #region 图片压缩处理
        public Bitmap zoomImage(Bitmap bgimage, double newWidth, double newHeight)
        {
            // 获取这个图片的宽和高
            float width = bgimage.Width;
            float height = bgimage.Height;
            // 创建操作图片用的matrix对象
            Matrix matrix = new Matrix();
            // 计算宽高缩放率
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;
            // 缩放图片动作
            matrix.PostScale(scaleWidth, scaleHeight);
            Bitmap bitmap = Bitmap.CreateBitmap(bgimage, 0, 0, (int)width,
                            (int)height, matrix, true);
            return bitmap;
        }
        #endregion
        public string BitmapToString(Bitmap bitmap)
        {
            Bitmap bit = zoomImage(bitmap, 750, 1000);//小图
                                                      //质量压缩
                                                      //MemoryStream stream = new MemoryStream();
                                                      //bit.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                                                      //byte[] bitmapData = stream.ToArray();
                                                      //Bitmap map = BitmapFactory.DecodeByteArray(bitmapData, 0, bitmapData.Length);
                                                      //btn_imagetwo.SetImageBitmap(map);
                                                      //Bitmap im = zoomImage(bitmap, 800, 900);//大图
            MemoryStream big_stream = new MemoryStream();
            bit.Compress(Bitmap.CompressFormat.Jpeg, 80, big_stream);
            byte[] big_bitmapData = big_stream.ToArray();
            return Convert.ToBase64String(big_bitmapData);
        }

        #region 查询隐患信息并赋值
        private void selectReformInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //整改单位
            EditText txt_fac = FindViewById<EditText>(Resource.Id.txtCheckFac);
            //整改类型
            EditText txt_type = FindViewById<EditText>(Resource.Id.txtCheckType);
            //整改期限
            EditText txt_time = FindViewById<EditText>(Resource.Id.txtTime);
            //整改要求
            EditText txt_request = FindViewById<EditText>(Resource.Id.txtCheckRequest);
            //整改责任人
            EditText edit_person = FindViewById<EditText>(Resource.Id.editCheckPerson);
            //隐患上报单位
            EditText hiden_depart = FindViewById<EditText>(Resource.Id.hidenDepart);
            
            try
            {
                string xmlStr = safeWeb.selecthidenInfo(XmlDBClass.autoID, hidenFlag, XmlDBClass.accID);
                if (xmlStr != "")
                {
                    //转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(xmlStr);
                    if (dt.Rows.Count > 0)
                    {
                        hiden_depart.Text = dt.Rows[0]["隐患上报部门"].ToString();
                        txt_fac.Text = dt.Rows[0]["隐患整改部门"].ToString();
                        txt_type.Text = dt.Rows[0]["abarStyle"].ToString();
                        txt_time.Text = dt.Rows[0]["abarbeitungTime"].ToString();
                        txt_request.Text = dt.Rows[0]["abarRequest"].ToString();
                        edit_person.Text = dt.Rows[0]["abarbeitungPerson"].ToString();

                        int abarID = Convert.ToInt32(dt.Rows[0]["abarDepartID"].ToString());
                        //判断当前岗位是否为整改单位
                        if (abarID != XmlDBClass.departID)
                        {
                            //不是整改单位 只许看不许操作
                            bt_sub.Enabled = false;
                            bt_sub.SetBackgroundResource(Resource.Drawable.btUnEnable);

                            //bt_sub.SetBackgroundColor(Color.Gray);
                            edit_person_2.Enabled = false;
                            edit_result.Enabled = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
            }          
        }
        #endregion

        #region 提交按钮
        private void btSub_Click(object sender, EventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
         
            //提交整改人 整改日期
            try
            {
                if (isCamera == 1 && edit_result.Text !="" && edit_person_2.Text !="")
                {
                    //图片转字节流
                    string strImg = ImageToString(fileAdress);
                    //组装XML数据
                    List<string> listData = new List<string>();
                    List<string> listName = new List<string>();
                    DateTime abarTm = Convert.ToDateTime(bt_date.Text);
                    string abartm = abarTm.ToString("yyyy-MM-dd");
                    try
                    {
                        listName.Add("abarbeitungPerson_2");
                        listName.Add("cImgFile");//图片文件夹名称
                        listName.Add("cImgName");//图片名称
                        listName.Add("cImgBytes");//图片字节流
                        listName.Add("abarTime");//整改日期
                        listName.Add("abarResult");
                        listName.Add("abarTS");

                        listData.Add(edit_person_2.Text);
                        listData.Add(abartm + '/' + XmlDBClass.companyName + '/' + XmlDBClass.departName + "/"); //图片文件夹名称
                        listData.Add(Guid.NewGuid().ToString() + ".jpg");
                        listData.Add(strImg);
                        listData.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                        listData.Add(edit_result.Text);
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

                        bool result = safeWeb.updateReformSureInfoNew(XmlDBClass.autoID, xml);
                        if (result)
                        {
                            Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                            bt_sub.Enabled = false;
                            bt_sub.SetBackgroundResource(Resource.Drawable.btUnEnable);
                           // bt_sub.SetBackgroundColor(Color.Gray);
                            //CommonFunction.ShowMessage("操作成功", this, true);
                        }
                        else
                        {
                            Toast.MakeText(this, "请检查网络连接", ToastLength.Short).Show();
                            //CommonFunction.ShowMessage("操作失败", this, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunction.ShowMessage(ex.Message, this, true);
                    }
                }
                else
                {
                    CommonFunction.ShowMessage("请拍照后提交并且填写整改人、整改结果后再提交", this, true);
                   // Toast.MakeText(this, "请拍照后再提交信息", ToastLength.Short).Show();
                }
               
            }
            catch (Exception ex )
            {

            }
          
        }
        #endregion

        #region 查询隐患上报图片地址
        private Bitmap hidenImagAdress()
        {
            string imgAdress = "";
            Bitmap bitmapReform = null;
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
         

            string revXml = safeWeb.selecthidenInfo(XmlDBClass.autoID, 1, XmlDBClass.accID);
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
                    bitmapReform = BitmapFactory.DecodeStream(intput);
                }
            }
            else
            {
                Toast.MakeText(this, "未查到相关隐患信息", ToastLength.Short).Show();

                //CommonFunction.ShowMessage("未查到相关隐患信息",this,true);
            }
            return bitmapReform;
        }
        #endregion

    }
}