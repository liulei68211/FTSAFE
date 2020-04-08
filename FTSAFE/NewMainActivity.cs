using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Java.Net;

namespace FTSAFE
{
    [Activity(Label = "国泰安全双控平台")]
    public class NewMainActivity : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private MessageFragment msgFragment;
        //private FragmentMsgNews msgFragment;
        private HomeFragment homeFragment;
        private CountFragment countFragment;
        private UserFragment userFragment;
        private Android.Support.V7.Widget.Toolbar toolbar;
        private BottomNavigationView bottomNavigationView;
        private bool mIsExit = false;
        private static int DOWNLOAD_FILE_CODE = 100001;
        private static int DOWNLOAD_FILE_FAILE_CODE = 100002;
        private Button StartDownload;
        private TextView percent;
        private TextView loadMemo;
        private ProgressBar progressBar;
        private Handler handler;
        private bool isLoad = false;
        /*
         * 数据类型? 表示参数的值可以为null空
         * 此时这个参数可调用属性hasvalue来判断，此参数是否有除了null以外的值;进而进行其它的工作  
         * 必须要加?才可用hasvalue属性
         */
        private DateTime ? lastBackKeyDownTime;
        //重载方法
        protected override void OnStart()
        {
            base.OnStart();
            //Console.WriteLine("调用OnStart");
        }
        //重载Resume()方法
        protected override void OnResume()
        {
            base.OnResume();
            if (isLoad == false )
            {
                //判断版本号更新
                bool result = select_versionInfo();
                if (result)
                {
                    try
                    {
                        isLoad = true;
                        LayoutInflater inflater = LayoutInflater.From(this);
                        View imgEntryView = inflater.Inflate(Resource.Layout.activity_update, null); // 加载自定义的布局文件
                        Button btLoad = imgEntryView.FindViewById<Button>(Resource.Id.button1);
                        percent = imgEntryView.FindViewById<TextView>(Resource.Id.tv_download_progress);
                        loadMemo = imgEntryView.FindViewById<TextView>(Resource.Id.title);
                       //  percents = imgEntryView.FindViewById<TextView>(Resource.Id.tv_download_progress1);
                        progressBar = imgEntryView.FindViewById<ProgressBar>(Resource.Id.tv_download_progressBar);
                        var dialog = new Android.App.AlertDialog.Builder(this);

                        btLoad.Click += delegate
                        //dialog.SetNeutralButton("更新", delegate
                        {
                            loadMemo.Text = "正在下载，请等待....";
                            progressBar.Visibility = ViewStates.Visible;
                            percent.Visibility = ViewStates.Visible;
                            Thread t = new Thread(run);
                            t.Start();
                            Toast.MakeText(this, "开始更新", ToastLength.Short).Show();
                        };
                        dialog.SetTitle("程序更新");
                        dialog.SetView(imgEntryView); // 自定义dialog
                        dialog.Show();
                    }
                    catch (Exception ex)
                    {

                    }
                    //提示更新程序
                    // CommonFunction.ShowMessage("程序更新，请扫描二维码重新下载安装app", this, true);
                }
            } 
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
            //隐藏标题栏
            // this.RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_new_main);

            string[] PERMISSIONS_STORAGE = {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                             Manifest.Permission.Camera
                            };

            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, PERMISSIONS_STORAGE, 1);
            }



            handler = new Handler(HandleMessage);
            //SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
           FragmentTransaction transaction = FragmentManager.BeginTransaction();
            hideAllFragment(transaction);
            //接收返回按钮返回数据 显示相应的fragment
            if (homeFragment == null)
            {
                homeFragment = HomeFragment.NewInstance(Resource.Layout.fragment_home_new);
                transaction.Add(Resource.Id.FramePage, homeFragment);
                //传递数据到Fragment
                sendFieldInfo(homeFragment);
            }
            else
            {
                transaction.Show(homeFragment);
            }
            transaction.Commit();
            //接收用户信息
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.userName = Intent.GetStringExtra("userName");
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.companyName = Intent.GetStringExtra("companyName");
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.departCode = Intent.GetStringExtra("departCode");
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            XmlDBClass.workArea = Intent.GetStringExtra("workArea");
            XmlDBClass.stationID = Convert.ToInt32(Intent.GetStringExtra("stationID"));
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.navigation);
           //解决当item大于三个时，非平均布局问题
            initBottomNavigationView();
            bottomNavigationView.SetOnNavigationItemSelectedListener(this);          
        }
        #region 暂时不用
        private void ExitWidthTimmer()
        {
            bool isExit = false;
           System.Timers.Timer time_timer = null;//使用.net的Timer对象
            if (!isExit)
            {
                isExit = true;
                Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                time_timer = new System.Timers.Timer();
                time_timer.Interval = 2000;
                time_timer.Enabled = true;

                //定时的2000毫秒到了 isExis 为true 退出app
                time_timer.Elapsed += delegate
                {
                    isExit = false;
                };
            }
            else
            {
                Finish();
            }
        }
        #endregion

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
                        Message message = handler.ObtainMessage();
                        message.What = DOWNLOAD_FILE_CODE;

                        //decimal ii = 0.00m;
                        //ii = receivedBytes * 100 / totalBytes ;
                        message.Obj = receivedBytes * 100.0 / totalBytes;
                        //message.Obj = receivedBytes * 100 / totalBytes;

                        // message.Arg1 = receivedBytes / totalBytes;

                        message.Arg1 = Convert.ToInt32(receivedBytes * 100.0 / totalBytes);

                        handler.SendMessage(message);
                    }
                    while (true);
                }
                //调用下载的文件进行安装
                installApk(this, filePath);
            }
            catch (Exception ex)
            {
                downloadfail();
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

        public void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case 100001:
                    //更新进度条
                    progressBar.Max = 100;
                    progressBar.Progress = Convert.ToInt32(msg.Obj);
                    // percent.SetText(String.v(msg.arg1) + "%");

                    percent.Text = msg.Arg1 + "%";
                    // percents.Text = (msg.Obj).ToString();
                    //if (progressBar.Progress == 100)
                    //{
                    //    Toast.MakeText(this, "下载完成", ToastLength.Short).Show();
                    //}
                    break;
                case 100002:
                    Toast.MakeText(this, "下载失败", ToastLength.Short).Show();
                    break;
            }
        }

        private void downloadfail()
        {
            Message message = handler.ObtainMessage();
            message.What = DOWNLOAD_FILE_FAILE_CODE;
            handler.SendMessage(message);
        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                //TimeSpan(0,0,2) 标识一个时间间隔
                if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0,0,2))
                {
                    Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                    lastBackKeyDownTime = DateTime.Now;
                }
                else
                {
                    Finish();
                }
                //ExitWidthTimmer();
                return true; 
            }
            return base.OnKeyDown(keyCode, e);
        }
        #region 传递变量
        private void sendFieldInfo(Fragment fragment)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("userID", XmlDBClass.userID.ToString());
            bundle.PutString("departID", XmlDBClass.departID.ToString());
            bundle.PutString("departCode", XmlDBClass.departCode.ToString());
            bundle.PutString("departName", XmlDBClass.departName);
            bundle.PutString("userName", XmlDBClass.userName);
            bundle.PutString("accID", XmlDBClass.accID.ToString());
            bundle.PutString("userCode", XmlDBClass.userCode);
            bundle.PutString("companyName", XmlDBClass.companyName);
            //set Arguments 属性值
            fragment.Arguments = bundle;
        }
        #endregion
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            hideAllFragment(transaction);
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    //toolbar.Title = "首页";
                   // toolbar.Visibility = ViewStates.Visible;
                    if (homeFragment == null)
                    {
                        homeFragment = HomeFragment.NewInstance(Resource.Layout.fragment_home_new);
                        transaction.Add(Resource.Id.FramePage, homeFragment);
                        //传递数据到Fragment
                        sendFieldInfo(homeFragment);
                    }
                    else
                    {
                        transaction.Show(homeFragment);
                    }

                    transaction.Commit();
                    return true;
                case Resource.Id.navigation_count:
                    //toolbar.Title = "统计分析";
                   // toolbar.Visibility = ViewStates.Visible;
                    if (countFragment == null)
                    {
                        countFragment = CountFragment.NewInstance(Resource.Layout.fragment_count_new);
                        transaction.Add(Resource.Id.FramePage, countFragment);
                        //传递数据到Fragment
                        sendFieldInfo(countFragment);
                    }
                    else
                    {
                        transaction.Show(countFragment);
                    }

                    transaction.Commit();
                    return true;
                case Resource.Id.navigation_message:
                  //  toolbar.Title = "消息提醒";
                   // toolbar.Visibility = ViewStates.Visible;
                    if (msgFragment == null)
                    {
                       // msgFragment = new MessageFragment();
                        msgFragment = new MessageFragment();
                        msgFragment = MessageFragment.NewInstance(Resource.Layout.fragment_msg_new);
                       // msgFragment = MessageFragment.NewInstance(Resource.Layout.fragment_msg_new);
                        transaction.Add(Resource.Id.FramePage, msgFragment);
                        //传递数据到Fragment
                        sendFieldInfo(msgFragment);
                    }
                    else
                    {
                        transaction.Show(msgFragment);
                    }
                    transaction.Commit();
                    return true;
                case Resource.Id.navigation_user:
                    //toolbar.Visibility = ViewStates.Gone;
                  //  toolbar.Title = "我的信息";
                    if (userFragment == null)
                    {
                        userFragment = UserFragment.NewInstance(Resource.Layout.activity_mine_new);
                        transaction.Add(Resource.Id.FramePage, userFragment);
                        //传递数据到Fragment
                        sendFieldInfo(userFragment);
                    }
                    else
                    {
                        transaction.Show(userFragment);
                    }
                    transaction.Commit();
                    return true;
            }

            return false;
        }
        private void initBottomNavigationView()
        {
            BottomNavigationViewHelper.disableShiftMode(bottomNavigationView);//解决当item大于三个时，非平均布局问题
        }
        //隐藏所有Fragment
        public void hideAllFragment(FragmentTransaction transaction)
        {
            if (msgFragment != null)
            {
                transaction.Hide(msgFragment);
            }
            if (homeFragment != null)
            {
                transaction.Hide(homeFragment);
            }
            if (countFragment != null)
            {
                transaction.Hide(countFragment);
            }
            if (userFragment != null)
            {
                transaction.Hide(userFragment);
            }
        }

        #region 查询版本号信息
        private bool select_versionInfo()
        {
            bool result = false;
            //获取本地 版本号
            PackageManager packageManager = this.PackageManager;
            PackageInfo packInfo = PackageManager.GetPackageInfo(this.PackageName, 0);
            string versionName = packInfo.VersionName;
            int versionCode = packInfo.VersionCode;//用于更新
            //查询服务器版本号
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            try
            {
                string revXML = safeWeb.searchVersion(XmlDBClass.accID);
                //xml转table
                if (revXML != "")
                {
                    DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXML);
                    int version_Code = Convert.ToInt32(dt.Rows[0]["versionCode"].ToString());
                    XmlDBClass.versionName = dt.Rows[0]["versionName"].ToString();
                    if (version_Code > versionCode)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this, true);
            }
            return result;
        }
        #endregion
    }
}