using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Java.Net;

namespace FTSAFE
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class LoginActivity : Activity
    {
        private EditText editLoginCode = null;
        private EditText editPassWord = null;
        private string keys = "LM1KW44FaBMHnyJp88ELe3Bj0ZQB8pL3ZDmKxeIgqtp";
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        protected override void OnResume()
        {
            base.OnResume();
           
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public void run()
        {
            int receivedBytes = 0;
            int totalBytes = 0;
            //创建下载文件
            Java.IO.File dirPaths = new Java.IO.File(
                 Android.OS.Environment.GetExternalStoragePublicDirectory(
                 Android.OS.Environment.DirectoryPictures), "CameraAppDemo");//CameraAppDemo
            string dirPath = dirPaths.ToString();
            //string dirPath = "/sdcard/updateVersion/version";
            var filePath = Path.Combine(dirPath, "FTSAFE.FTSAFE.apk");
            URL url = new URL("http://safe.guotaiyun.cn/upload/FTSAFE.FTSAFE.apk");//urlToDownload 下载文件的url地址

            HttpURLConnection conn = (HttpURLConnection)url.OpenConnection();
            conn.Connect();

            Stream Ins = conn.InputStream;

            totalBytes = conn.ContentLength;

            try
            {
                //在外部存储装置中建立下载文件保存文件夹           
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                else
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                using (FileStream fos = new FileStream(filePath, FileMode.Create))
                {
                    byte[] buf = new byte[512];


                    do
                    {
                        int numread = Ins.Read(buf, 0, 512);
                        receivedBytes += numread;
                        if (numread <= 0)
                        {
                            break;
                        }
                        fos.Write(buf, 0, numread);
                        //将实时的下载长度传给UI线程
                        //Message message = handler.ObtainMessage();
                        //message.What = DOWNLOAD_FILE_CODE;

                        //decimal ii = 0.00m;
                        //ii = receivedBytes * 100 / totalBytes ;
                       // message.Obj = receivedBytes * 100.0 / totalBytes;
                        //message.Obj = receivedBytes * 100 / totalBytes;

                        // message.Arg1 = receivedBytes / totalBytes;

                        //message.Arg1 = Convert.ToInt32(receivedBytes * 100.0 / totalBytes);

                        //handler.SendMessage(message);
                    }
                    while (true);
                }
                //调用下载的文件进行安装
                installApk(this, filePath);
            }
            catch (Exception ex)
            {
                //downloadfail();
            }
        }
        private void installApk(Context context, string filePath)
        {
            //：Android N（对应sdk24）（版本7.0）及以上对访问文件权限收回，
            //按照Android N的要求，若要在应用间共享文件，
            // 您应发送一项 content://URI，并授予 URI 临时访问权限。 
            //判断Android sdk版本是否大于7.0即24
            try
            {
                Java.IO.File file = new Java.IO.File(filePath);
                Intent intent = new Intent(Intent.ActionView);

                Android.Net.Uri contentUri; //android 版本大于7 和 小于7 的 这个值不一样 否则 intent.SetDataAndType会不起作用
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                    var provider = context.PackageName + ".fileprovider";
                    contentUri = FileProvider.GetUriForFile(context, provider, file);
                    intent.SetDataAndType(contentUri, "application/vnd.android.package-archive");
                }
                else
                {
                    contentUri = Android.Net.Uri.FromFile(file);
                    intent.SetDataAndType(contentUri, "application/vnd.android.package-archive");
                    intent.SetFlags(ActivityFlags.NewTask);
                }
                context.StartActivity(intent);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OpenFile === " + e.Message);
            }

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_loginnew);

            //防止软键盘改变底部控件位置
            Window.SetSoftInputMode(SoftInput.AdjustPan);

            //Thread t = new Thread(run);
            //t.Start();

            string exitCode = "true";
            exitCode = Intent.GetStringExtra("exitCode");
            if (exitCode != "true")
            {
                //读取登录信息
                readLoginInfo();
            }
     
            editLoginCode = FindViewById<EditText>(Resource.Id.edit_user);
            editPassWord = FindViewById<EditText>(Resource.Id.edit_pass);
            //登录按钮
            Button bt_login = FindViewById<Button>(Resource.Id.bt_login);
            bt_login.Click += delegate
            {
                selectLoginInfo();
            };
        }

      
        #region 读取登录信息
        private void readLoginInfo()
        {
            editLoginCode = FindViewById<EditText>(Resource.Id.edit_user);
             editPassWord = FindViewById<EditText>(Resource.Id.edit_pass);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editLoginCode.Text = prefs.GetString("loginCode", null);
            editPassWord.Text = prefs.GetString("PassWord", null);

            if (editLoginCode.Text != "")
            {
                //selectLoginInfo();
                XmlDBClass.userID = Convert.ToInt32(prefs.GetString("userID", null));
                XmlDBClass.userName = prefs.GetString("userName", null);
                XmlDBClass.passWord = prefs.GetString("PassWord", null);
                XmlDBClass.departCode = prefs.GetString("departCode", null);
                XmlDBClass.departName = prefs.GetString("departName", null);
                XmlDBClass.workArea = prefs.GetString("workArea", null);
             
                XmlDBClass.mobile = prefs.GetString("mobile", null);
                XmlDBClass.accID = Convert.ToInt32(prefs.GetString("accID", null));
                XmlDBClass.companyName = prefs.GetString("companyName", null);

                //departID 需要重新查询
                //保存登录信息
                SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
                try
                {
                    string revXML = safeWeb.loginSafe(editLoginCode.Text,XmlDBClass.passWord);
                    if (revXML != "")
                    {
                        //xml数据转table
                        DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);
                        XmlDBClass.departID = Convert.ToInt32(dt.Rows[0]["departID"].ToString());
                    }
                      
                    Intent intent = new Intent(this, typeof(NewMainActivity));
                    XmlDBClass.seneFieldInfo(intent);
                    StartActivity(intent);
                    Finish();
                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion

        #region 查询登录信息
        private void selectLoginInfo()
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            try
            {
                string passWord = Encrypt(editPassWord.Text,keys);
                //保存登录信息
                string revXML = safeWeb.loginSafe(editLoginCode.Text, passWord);
                if (revXML == "1")
                {
                    CommonFunction.ShowMessage("用户还未审核，请联系管理员审核", this, true);
                }
                else if (revXML == "2")
                {
                    CommonFunction.ShowMessage("密码错误请重新输入密码", this, true);
                }
                else if (revXML != "")
                {
                    //xml数据转table
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);
                    XmlDBClass.userID = Convert.ToInt32(dt.Rows[0]["userID"].ToString());
                    XmlDBClass.userName = dt.Rows[0]["user_nickname"].ToString();
                    XmlDBClass.passWord = dt.Rows[0]["user_app_pass"].ToString();
                    XmlDBClass.departCode = dt.Rows[0]["departCode"].ToString();
                    XmlDBClass.departName = dt.Rows[0]["name"].ToString();
                    XmlDBClass.workArea = dt.Rows[0]["workArea"].ToString();
                    XmlDBClass.departID = Convert.ToInt32(dt.Rows[0]["departID"].ToString());
                    XmlDBClass.mobile = dt.Rows[0]["mobile"].ToString();
                    XmlDBClass.accID = Convert.ToInt32(dt.Rows[0]["accID"].ToString());
                    XmlDBClass.companyName = dt.Rows[0]["cCompanyName"].ToString();
                    //传递数据
                    Intent intent = new Intent(this, typeof(NewMainActivity));
                 
                    //保存登录数据
                    saveLoginInfo();
                    XmlDBClass.seneFieldInfo(intent);
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    CommonFunction.ShowMessage("未查到当前手机号注册的用户信息，请检查手机号是否输入正确", this, true);
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
        }
        #endregion

        #region 保存登录信息
        private void saveLoginInfo()
        {
            //保存登录信息
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            //editor.PutBoolean("CK_Status", true);
            editor.PutString("loginCode", editLoginCode.Text);
            editor.PutString("accID", XmlDBClass.accID.ToString());
            editor.PutString("userID", XmlDBClass.userID.ToString());
            editor.PutString("userName", XmlDBClass.userName);
            editor.PutString("PassWord", XmlDBClass.passWord);
            editor.PutString("departCode", XmlDBClass.departCode);
            editor.PutString("workArea", XmlDBClass.workArea);
            editor.PutString("departID", XmlDBClass.departID.ToString());
            editor.PutString("departName", XmlDBClass.departName);
            editor.PutString("mobile", XmlDBClass.mobile);
            editor.PutString("companyName", XmlDBClass.companyName);
            editor.Commit();
            editor.Apply();
        }
        #endregion

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="decryptStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string Decrypt(string decryptStr, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key.Substring(0, 32));
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string Encrypt(string encryptStr, string key)
        {
            string ddd = key.Substring(0, 31);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key.Substring(0, 32));
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}