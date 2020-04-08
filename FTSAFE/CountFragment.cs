using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace FTSAFE
{
    public class CountFragment : Fragment
    {
        //要显示的页面
        private int FragmentPage;
        private WebView webview;
        public static CountFragment NewInstance(int iFragmentPage)
        {
            CountFragment myFragment = new CountFragment();
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
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(FragmentPage, container, false);
            Android.Support.V7.Widget.Toolbar toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "统计分析";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this.Activity, Resource.Style.Toolbar_TitleText);
       
            TextView txt_1 = view.FindViewById<TextView>(Resource.Id.txt_1);
            TextView txt_2 = view.FindViewById<TextView>(Resource.Id.txt_2);
            TextView txt_3 = view.FindViewById<TextView>(Resource.Id.txt_3);
            TextView txt_4 = view.FindViewById<TextView>(Resource.Id.txt_4);
            TextView txt_5 = view.FindViewById<TextView>(Resource.Id.txt_5);
            TextView txt_6 = view.FindViewById<TextView>(Resource.Id.txt_6);
            TextView txt_7 = view.FindViewById<TextView>(Resource.Id.txt_7);
            TextView txt_8 = view.FindViewById<TextView>(Resource.Id.txt_8);
            TextView txt_9 = view.FindViewById<TextView>(Resource.Id.txt_9);

            txt_1.Click += delegate
             {
                 Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                 StartActivity(i_1);
                // this.Activity.Finish();
             };
            txt_2.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
                //this.Activity.Finish();
            };
            txt_3.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
                //this.Activity.Finish();
            };
            txt_4.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
               // this.Activity.Finish();
            };
            txt_5.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
                //this.Activity.Finish();
            };
            txt_6.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
               // this.Activity.Finish();
            };
            txt_7.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
               // this.Activity.Finish();
            };
            txt_8.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
                //this.Activity.Finish();
            };
            txt_9.Click += delegate
            {
                Intent i_1 = new Intent(this.Activity, typeof(CountActivity));
                StartActivity(i_1);
               // this.Activity.Finish();
            };
            return view;
        }
    }
}