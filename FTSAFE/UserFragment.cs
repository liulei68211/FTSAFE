using Android.Content;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using System;

namespace FTSAFE
{
    public class UserFragment : Fragment
    {
        //要显示的页面
        private int FragmentPage;
        public static UserFragment NewInstance(int iFragmentPage)
        {
            UserFragment myFragment = new UserFragment();
            myFragment.FragmentPage = iFragmentPage;
            return myFragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
           
            base.OnCreate(savedInstanceState);

           
            // Create your fragment here
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
           // this.Activity.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            View view = inflater.Inflate(FragmentPage, container, false);
            Android.Support.V7.Widget.Toolbar toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "我的信息";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this.Activity, Resource.Style.Toolbar_TitleText);

            //get Arguments 属性值
            Bundle bundle = Arguments;
            XmlDBClass.userID = Convert.ToInt32(bundle.GetString("userID"));
            XmlDBClass.userName = bundle.GetString("userName");
            XmlDBClass.userCode = bundle.GetString("userCode");
            XmlDBClass.departID = Convert.ToInt32(bundle.GetString("departID"));
            XmlDBClass.departCode = bundle.GetString("departCode");
            XmlDBClass.departName = bundle.GetString("departName");
            XmlDBClass.workArea = bundle.GetString("workArea");
            XmlDBClass.stationID = Convert.ToInt32(bundle.GetString("stationID"));
            XmlDBClass.accID = Convert.ToInt32(bundle.GetString("accID"));
            XmlDBClass.companyName = bundle.GetString("companyName");


            TextView txt_user_1 = view.FindViewById<TextView>(Resource.Id.loginName);
            TextView txt_user_2 = view.FindViewById<TextView>(Resource.Id.userName);
            TextView txt_mobile = view.FindViewById<TextView>(Resource.Id.loginMobile);
            TextView txt_depart = view.FindViewById<TextView>(Resource.Id.userDeapart);

            txt_user_1.Text = XmlDBClass.userName;
            txt_user_2.Text = XmlDBClass.userName;
            txt_depart.Text = XmlDBClass.departName;
            txt_mobile.Text = XmlDBClass.mobile;
            //退出按钮
            Button bt_exit = view.FindViewById<Button>(Resource.Id.btExit);
            bt_exit.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(LoginActivity));
                intent.PutExtra("exitCode", "true");
                StartActivity(intent);
                this.Activity.Finish();
            };

            // this.Activity.SetTheme(Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);
            TextView txt_pass_modify = view.FindViewById<TextView>(Resource.Id.txt_passModify);
            txt_pass_modify.Click += delegate
            {
                Intent intent_pass = new Intent(this.Activity,typeof(PassModifyActivity));
                intent_pass.PutExtra("userID", XmlDBClass.userID.ToString());
                intent_pass.PutExtra("departID", XmlDBClass.departID.ToString());
                intent_pass.PutExtra("departName", XmlDBClass.departName);
                intent_pass.PutExtra("userName", XmlDBClass.userName);
                intent_pass.PutExtra("accID", XmlDBClass.accID.ToString());
                intent_pass.PutExtra("userCode", XmlDBClass.userCode);
            
                StartActivity(intent_pass);
               // this.Activity.Finish();
            };
           
            //版本更新
            TextView txt_version = view.FindViewById<TextView>(Resource.Id.txt_version);
            txt_version.Click += delegate
            {
                var dialog = new Android.App.AlertDialog.Builder(this.Activity);
                dialog.SetTitle("版本消息");
                dialog.SetMessage("已是最新版本");
                dialog.SetPositiveButton("关闭", delegate
                {

                });
                dialog.Show();
            };
            return view;
        }
    }
}