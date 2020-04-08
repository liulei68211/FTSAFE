using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FTSAFE
{
    public class FragmentMsgNews : Fragment
    {
        //要显示的页面
        private int FragmentPage;
        public static FragmentMsgNews NewInstance(int iFragmentPage)
        {
            FragmentMsgNews myFragment = new FragmentMsgNews();
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
          
            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}