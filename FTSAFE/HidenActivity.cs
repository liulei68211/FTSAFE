using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace FTSAFE
{
    [Activity(Label = "HidenActivity")]
    public class HidenActivity : AppCompatActivity
    {
        private int userID = 0;
        private string userCode = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hiden);

            userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            userCode = Intent.GetStringExtra("userCode");
            //隐患录入
            ImageButton imgBt_hiden = FindViewById<ImageButton>(Resource.Id.img_hiden_add);
            imgBt_hiden.Click += delegate
            {
                Intent intent = new Intent(this, typeof(HidenAddActivity));
                intent.PutExtra("userID", userID.ToString());
                intent.PutExtra("userCode", userCode);
                StartActivity(intent);
                Finish();
            };
            //隐患整改下发
            ImageButton imgBt_reform = FindViewById<ImageButton>(Resource.Id.img_hiden_reform);
            imgBt_reform.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ReformActivity));
                intent.PutExtra("userID", userID.ToString());
                intent.PutExtra("userCode", userCode);
                StartActivity(intent);
                Finish();
            };
            //整改信息复查
            ImageButton imgBt_reform_sure = FindViewById<ImageButton>(Resource.Id.img_preform_sure);
            imgBt_reform_sure.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ReformCheckActivity));
                StartActivity(intent);
                Finish();
            };
            //整改复查完成
            ImageButton imgBt_check = FindViewById<ImageButton>(Resource.Id.img_hiden_check);
            imgBt_check.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ReformEndActivity));
                StartActivity(intent);
                Finish();
            };
            // imgBt_hiden.Click += hiden_Click;
            //隐患查询
            ImageButton imgBt_search = FindViewById<ImageButton>(Resource.Id.img_hiden_seasrch);
            imgBt_search.Click += delegate
            {
                Intent intent = new Intent(this, typeof(HidenSearchActivity));
                StartActivity(intent);
                Finish();
            };
            //隐患统计img_hiden_statistics
            ImageButton imgBt_statistics = FindViewById<ImageButton>(Resource.Id.img_hiden_statistics);
            imgBt_search.Click += delegate
            {
                Intent intent = new Intent(this, typeof(HidenStatisActivity));
                StartActivity(intent);
                Finish();
            };
        }
    }
}