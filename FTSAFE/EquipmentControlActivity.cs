using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;

using Android.Widget;
using FTSAFE.CommonClass;
using System.Data;
using Android.Provider;
using Android.Graphics;
using Android.Content.PM;
using System.IO;
using Android;
using Android.Support.V4.App;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using System.Web.Services.Protocols;

namespace FTSAFE
{
    [Activity(Label = "EquipmentControlActivity")]
    public class EquipmentControlActivity : Activity
    {
        private SafeWeb.JGNP safeWeb = null;
        private List<string> equipmentControl_list = new List<string>();
        private ImageView _imageView = null;
        private Java.IO.File _file;
        private Java.IO.File _dir;
        private Bitmap bitmap;
        private string fileAdress ="";

        private TextView txt_1 = null;
        private TextView txt_2 = null;
        private TextView txt_3 = null;
        private TextView txt_4 = null;
        private Button bt_normal = null;
        private Button bt_anormal = null;
        private string equipmentControlName = "";
        private int isBtPress = 0;//判断是哪个按钮按下
        private bool isBtTrue = false;
        private Button imgbt_1 = null;
        private Button imgbt_2 = null;
        private Button imgbt_3 = null;
        private Button imgbt_4 = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_equipment_control);
            string controlNameStr = "";
            string[] equipmentArr = Intent.GetStringExtra("equipmentName").Split('|'); 

            safeWeb = new SafeWeb.JGNP();
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.stationID = Convert.ToInt32(Intent.GetStringExtra("stationID"));
            XmlDBClass.equipmentName = equipmentArr[0];
            XmlDBClass.equipmentCode = equipmentArr[1];

            XmlDBClass.userName = Intent.GetStringExtra("userName");
            string stationName = searchStationInfo(XmlDBClass.stationID);

            //巡查异常按钮
            bt_anormal = FindViewById<Button>(Resource.Id.btAnnormal);
            bt_anormal.Click += delegate
            {
                if (isBtPress == 1)
                {
                    imgbt_1.Text = "巡检异常";
                   
                }
                else if (isBtPress == 2)
                {
                    imgbt_2.Text = "巡检异常";
                }
                else if (isBtPress == 2)
                {
                    imgbt_3.Text = "巡检异常";
                }
                else
                {
                    imgbt_4.Text = "巡检异常";
                }

                //跳转到隐患填报页面
                Intent intent = new Intent(this, typeof(HidenAddActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("autoID", XmlDBClass.autoID.ToString());
                intent.PutExtra("userCode", XmlDBClass.userCode);

                StartActivity(intent);
            };
            //巡检正常按钮
            bt_normal = FindViewById<Button>(Resource.Id.btNormal);
            bt_normal.Click += delegate
            {
                if (isBtPress == 1 )
                {
                    imgbt_1.Text = "巡检正常";
                    imgbt_1.SetTextColor(Android.Graphics.Color.Green);
                }
                else if (isBtPress == 2)
                {
                    imgbt_2.Text = "巡检正常";
                    imgbt_1.SetTextColor(Android.Graphics.Color.Green);
                }
                else if (isBtPress == 2)
                {
                    imgbt_3.Text = "巡检正常";
                    imgbt_1.SetTextColor(Android.Graphics.Color.Green);
                }
                else
                {
                    imgbt_4.Text = "巡检正常";
                    imgbt_1.SetTextColor(Android.Graphics.Color.Green);
                }
                //上传照片到服务器 巡查人 巡查结果 巡查人编号 巡查部门 巡查结果
                //图片转字节流
                string strImg = ImageToString(fileAdress);
                //arr 0 巡查人名称  1 设备名称 2 图片 3 岗位名称 4 图片名称  5 设备编码
                string[] arrPartol = { XmlDBClass.userName, XmlDBClass.equipmentName, strImg, searchStationInfo(XmlDBClass.stationID), equipmentControlName + ".jpg",XmlDBClass.equipmentCode };
                try
                {
                    bool result = safeWeb.insertEquipmentPartolData(arrPartol);
                    if (result)
                    {
                        CommonFunction.ShowMessage("操作成功", this, true);
                    }
                    else
                    {
                        CommonFunction.ShowMessage("已经提交过巡检记录", this, true);
                    }
                }
                catch (SoapException ex)
                {
                    CommonFunction.ShowMessage(ex.Message, this, true);
                }
            };
            txt_1 = FindViewById<TextView>(Resource.Id.txt_1);
            txt_2 = FindViewById<TextView>(Resource.Id.txt_2);
            txt_3 = FindViewById<TextView>(Resource.Id.txt_3);
            txt_4 = FindViewById<TextView>(Resource.Id.txt_4);

            imgbt_1 = FindViewById<Button>(Resource.Id.imgbt_1);
            imgbt_1.Click += delegate
            {
                equipmentControlName = txt_1.Text;
                isBtPress = 1;
                isBtTrue = true;
                cameraPress();
            };
            imgbt_2 = FindViewById<Button>(Resource.Id.imgbt_2);
            imgbt_2.Click += delegate
            {
                equipmentControlName = txt_2.Text;
                isBtPress = 2;
                isBtTrue = true;
                cameraPress();
            };
            imgbt_3 = FindViewById<Button>(Resource.Id.imgbt_3);
            imgbt_3.Click += delegate
            {
                isBtPress = 3;
                isBtTrue = true;
                equipmentControlName = txt_3.Text;
                cameraPress();
            };
            imgbt_4 = FindViewById<Button>(Resource.Id.imgbt_4);
            imgbt_4.Click += delegate
            {
                isBtPress = 4;
                isBtTrue = true;
                equipmentControlName = txt_4.Text;
                cameraPress();
            };
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
              //  Button btCamera = FindViewById<Button>(Resource.Id.btCamera);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                //imgbt_1.Click += cameraClick;
                //imgbt_2.Click += cameraClick;
                //imgbt_3.Click += cameraClick;
                //imgbt_4.Click += cameraClick;
            }

            //根据设备名称 查询设备控制点
            try
            {
                string revXml = safeWeb.searchEquipmentControlData(XmlDBClass.equipmentName);
                //转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                if (dt.Rows.Count > 0)
                {
                    equipmentControl_list.Clear();
                    for (int i=0;i<dt.Rows.Count;i++)
                    {
                        controlNameStr = dt.Rows[i]["equipmentControlName"].ToString();
                    }

                    string[]  controlNameStrArray = controlNameStr.Split(',');
                    txt_1.Text = controlNameStrArray[0].ToString();
                    txt_2.Text = controlNameStrArray[1].ToString();
                    txt_3.Text = controlNameStrArray[2].ToString();
                    txt_4.Text = controlNameStrArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
            }
        }

        #region
        private void cameraPress()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //保存路径
             _file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
             fileAdress = _file.ToString();
            // fileAdress = _file.ToString();
            // 保存图片
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
         }
        #endregion
        #region 拍照按钮
        private void cameraClick(object sender,EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //保存路径
            _file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            fileAdress = _file.ToString();
            // fileAdress = _file.ToString();
            // 保存图片
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
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
                 Android.OS.Environment.GetExternalStoragePublicDirectory(
                 Android.OS.Environment.DirectoryPictures), "CameraAppDemo");//CameraAppDemo
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

        #region 查询岗位信息 (分厂 + 车间 + 岗位)
        private string searchStationInfo(int stationID)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            string stationStr = safeWeb.searchStationData(stationID);
            return stationStr;
        }
        #endregion
    }
}