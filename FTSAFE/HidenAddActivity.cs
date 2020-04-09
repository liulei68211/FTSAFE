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

namespace FTSAFE
{
    [Activity(Label = "隐患录入")]
    public class HidenAddActivity : AppCompatActivity
    {
        private ImageView _imageView;
        private Java.IO.File _file;
        private Java.IO.File _dir;
        private Bitmap bitmap;
        private string fileAdress;
        private EditText edit_person;
        private EditText edit_time;
        private EditText edit_dept;
        private EditText edit_level;//隐患级别
        private int partolModeID = 0;
        private int hidenTypeID = 0;
        private int hidenAdressID = 0;
        private Button btSub = null;
        private int isCamera = 0;//判断是否拍照
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //隐藏标题栏
           // this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hidenadd_new);

            //防止软键盘改变底部控件位置
            Window.SetSoftInputMode(SoftInput.AdjustPan);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "隐患信息录入";
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
                //Intent i = new Intent(this, typeof(NewMainActivity));
                //StartActivity(i);
                //Finish();
            };

            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.userName = Intent.GetStringExtra("userName");
            XmlDBClass.isBig = Convert.ToInt32(Intent.GetStringExtra("isBig"));
           
             btSub = FindViewById<Button>(Resource.Id.btSub);
            btSub.Click += btSub_Click;

            #region 不添加此段代码 无法打开照相机
            //if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
            if(Build.VERSION.SdkInt >= BuildVersionCodes.N)
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
                _imageView = FindViewById<ImageView>(Resource.Id.imageHiden);
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
            //排查人
            edit_person = FindViewById<EditText>(Resource.Id.editName);
            edit_person.Text = XmlDBClass.userName;
            edit_person.ClearFocus();
            edit_person.Enabled = false;//排查人为默认 不可编辑
            edit_time = FindViewById<EditText>(Resource.Id.txtTime);
            edit_time.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //隐患级别
            edit_level = FindViewById<EditText>(Resource.Id.editLevel);
            if (XmlDBClass.isBig == 0)
            {
                edit_level.Text = "一般";
            }
            else
            {
                edit_level.Text = "重大";
            }
            //排查单位
            edit_dept = FindViewById<EditText>(Resource.Id.textDept);
            edit_dept.Text = XmlDBClass.departName;
            /* spinner
             * spinner_mode 排查方式(综合检查 专项检查等)
             * spinner_type 隐患类别(现场管理、基础管理)
             * spinner_level 隐患级别(一般、重大)
             */
            Spinner spinner_mode = FindViewById<Spinner>(Resource.Id.spinerMode);
            Spinner spinner_type = FindViewById<Spinner>(Resource.Id.spinerType);
            //隐患所属部门
            Spinner spinner_adress = FindViewById<Spinner>(Resource.Id.spinnerAdress);

            List<string> partolTypeList = selectPartolType();
            List<string> hidenTypeList = selectHidenType();
            List<string> hidenAdresList = selectHidenAdress();
            //查询隐患所属部门 只查找安全科以下的 不包含岗位
            List<string> hidenAdressList = selectHidenAdress();
            //spinner新样式
            //绑定xml文件数据 排查方式
            // ArrayAdapter adapter_mode = ArrayAdapter.CreateFromResource(this, Resource.Array.hidenMode, Resource.Layout.spinner_item);
            //绑定数据库查询数据
            ArrayAdapter<string> adapter_mode = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, partolTypeList);
            adapter_mode.SetDropDownViewResource(Resource.Layout.dropdown_style);
            spinner_mode.Adapter = adapter_mode;
            spinner_mode.ItemSelected += spinner_mode_ItemSelected;
            //绑定xml文件数据 隐患类别
            // ArrayAdapter adapter_type = ArrayAdapter.CreateFromResource(this, Resource.Array.hidenType, Resource.Layout.spinner_item);
            ArrayAdapter<string> adapter_type = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, hidenTypeList);
            adapter_type.SetDropDownViewResource(Resource.Layout.dropdown_style);
            spinner_type.Adapter = adapter_type;
            spinner_type.ItemSelected += spinner_type_ItemSelected;
            //绑定隐患所属部门 数据库数据
            ArrayAdapter<string> adapter_adress = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, hidenAdresList);
            adapter_adress.SetDropDownViewResource(Resource.Layout.dropdown_style);
            spinner_adress.Adapter = adapter_adress;
            spinner_adress.ItemSelected += spinner_adress_ItemSelected;
            #region 绑定数据库数据
            //ArrayAdapter<string> adapter_mode = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, carList);
            //adapter_mode.SetDropDownViewResource(Resource.Layout.dropdown_style);
            //spinner_mode.Adapter = adapter_mode;
            //spinner_mode.ItemSelected += spinnerMode_ItemSelected
            #endregion
        }

        #region 查询隐患所属部门
        private List<string> selectHidenAdress()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            string revXML = "";
            List<string> hidenAdressList = new List<string>();
            try
            {
                revXML = safeWeb.select_hidenAdress(XmlDBClass.accID, XmlDBClass.accCode);
                if (revXML != "")
                {
                    //xml转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        hidenAdressList.Add(dt.Rows[i]["name"].ToString());
                    }
                }
            }
            catch (Exception ex )
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
            }
            return hidenAdressList
        }
        #endregion

        #region 点击图片放大
        private void imgviewBig_Click(object sender,EventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View imgEntryView = inflater.Inflate(Resource.Layout.imageBig, null); // 加载自定义的布局文件
            Android.App.AlertDialog dialog = new Android.App.AlertDialog.Builder(this).Create();
            // ImageView img = (ImageView)imgEntryView.FindViewById(Resource.Id.large_image);
            ImageView img = imgEntryView.FindViewById<ImageView>(Resource.Id.large_image);
            int height = Resources.DisplayMetrics.HeightPixels;
            int width = height/2;
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

        #region 绑定排查方式
        private List<string> selectPartolType()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            List<string> partolTypeList = new List<string>();
            string receiveXML = safeWeb.selectHidenPartolType();
            if (receiveXML != "")
            {
                //xml转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(receiveXML);
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    partolTypeList.Add(dt.Rows[i]["partolStyleName"].ToString());
                }
            }
            else
            {
                partolTypeList.Add("无数据");
            }

            return partolTypeList;
        }
        #endregion

        #region 绑定隐患类别
        private List<string> selectHidenType()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            List<string> hidenTypeList = new List<string>();
            string receiveXML = safeWeb.selectHidenType();
            if (receiveXML != "")
            {
                //xml转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(receiveXML);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    hidenTypeList.Add(dt.Rows[i]["dangerTypeName"].ToString());
                }
            }
            else
            {
                hidenTypeList.Add("无数据");
            }

            return hidenTypeList;
        }
        #endregion

        #region 排查方式选择事件
        private void spinner_mode_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //根据车号查询 list路线
            var CurSpinner = (Spinner)sender;
            partolModeID = safeWeb.selectHidenPartolTypeID(CurSpinner.SelectedItem.ToString());
        }
        #endregion

        #region 隐患类别选择事件
        private void spinner_type_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //根据车号查询 list路线
            var CurSpinner = (Spinner)sender;
            hidenTypeID = safeWeb.selectHidenTypeID(CurSpinner.SelectedItem.ToString());
        }
        #endregion

        #region 隐患所属部门选择事件
        private void spinner_adress_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //根据车号查询 list路线
            var CurSpinner = (Spinner)sender;
            try
            {
                hidenAdressID = safeWeb.select_hidenAdressID(XmlDBClass.accID,CurSpinner.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
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

        #region 隐患提交按钮
        private void btSub_Click(object sender,EventArgs e)
        {
            string hidenFlag = "0";
        
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            EditText edit_info = FindViewById<EditText>(Resource.Id.editInfo);
        
            try
            {
                if (isCamera == 1 && edit_info.Text != "")
                {
                    //图片转字节流
                    string strImg = ImageToString(fileAdress);
                    //获取服务器时间
                    string web_time = safeWeb.webTimeMysql().Substring(0, 10);
                    DateTime webtm = Convert.ToDateTime(web_time);
                    web_time = webtm.ToString("yyyy-MM-dd");

                    List<string> listData = new List<string>();
                    List<string> listName = new List<string>();
                    // hidenTime,hidenPtrStyleID,hidenTypeID,,hidenLevel,hidenInfo,hidenFlag
                    listName.Add("hidenPersonID");
                    listName.Add("hidenPersonName");
                    listName.Add("hidenTime");
                    listName.Add("hidenPtrStyleID");
                    listName.Add("hidenTypeID");
                    listName.Add("hidenLevel");
                    listName.Add("hidenInfo");
                    listName.Add("accID");
                    listName.Add("cImgFile");//图片文件夹名称
                    listName.Add("cImgName");//图片名称
                    listName.Add("cImgBytes");//图片字节流
                    listName.Add("departID");//图片字节流
                    //listName.Add("iHidenAdress");//隐患所属部门

                    listData.Add(XmlDBClass.userID.ToString());
                    listData.Add(edit_person.Text);
                    listData.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                    listData.Add(partolModeID.ToString());
                    listData.Add(hidenTypeID.ToString());
                    listData.Add(edit_level.Text);
                    listData.Add(edit_info.Text);
                    listData.Add(XmlDBClass.accID.ToString());
                    listData.Add(web_time + '/' + XmlDBClass.companyName + '/' + XmlDBClass.departName + "/"); //图片文件夹名称
                    listData.Add(Guid.NewGuid().ToString() + ".jpg");
                    listData.Add(strImg);
                    listData.Add(XmlDBClass.departID.ToString());

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
                        bool result = safeWeb.insertHidenInfo(xml);
                        if (result)
                        {
                            Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                            btSub.Enabled = false;
                            btSub.SetBackgroundResource(Resource.Drawable.btUnEnable);
                            // btSub.SetBackgroundColor(Color.Gray);
                            //  btSub.SetBackgroundColor(Color.ParseColor("#efefef"));
                            //CommonFunction.ShowMessage("操作成功", this, true);
                        }
                        else
                        {
                            Toast.MakeText(this, "操作失败，请检查网络是否正确", ToastLength.Short).Show();
                            //CommonFunction.ShowMessage("操作失败，请检查网络是否正确", this, true);
                        }
                    }
                    catch (SoapException ex)
                    {
                        CommonFunction.ShowMessage(ex.Message, this, true);
                    }
                }
                else
                {
                    CommonFunction.ShowMessage("请拍照后并填写隐患描述后再提交", this,true);
                    //Toast.MakeText(this, "请拍照后提交", ToastLength.Short).Show();
                }
              
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
           
        }
        #endregion

        #region 图片转为string流
        public  string ImageToString(string file_path)
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
        public  Bitmap zoomImage(Bitmap bgimage, double newWidth, double newHeight)
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
    }
}