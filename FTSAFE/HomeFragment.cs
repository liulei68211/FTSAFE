using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;
using Q.Rorbin.Badgeview;

namespace FTSAFE
{
    public class HomeFragment : Fragment
    {
        //要显示的页面
        private int FragmentPage;
        private int hidenFlag = 0;
        private ImageButton safePartol_bt = null;//安全确认
        private ImageButton hidenAdd_bt = null;
        private ImageButton bigHidenAdd_bt = null;
        private ImageButton hidenReform_bt = null;
        private ImageButton hidenCheck_bt = null;
        private ImageButton hidenEcn_bt = null;
        private ImageButton hidenSearch_bt = null;
        private ImageButton partolAdd_bt = null;
        private ImageButton partolRecord_bt = null;
        private ImageButton hiden_auit_bt = null;//隐患审核
        private ImageButton ressim_bt = null;//制度
        private ImageButton hiden_danger_bt = null;//岗位风险告知卡
        private ImageButton hiden_list_bt = null;//岗位隐患排查清单
        private ImageButton hiden_control_bt = null;//岗位应急处置卡
        private TextView txt_0 = null;
        private TextView txt_1 = null;
        private TextView txt_2 = null;
        private TextView txt_3 = null;
        public static HomeFragment NewInstance(int iFragmentPage)
        {
            HomeFragment myFragment = new HomeFragment();
            myFragment.FragmentPage = iFragmentPage;
            return myFragment;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void OnStop()
        {
            base.OnStop();
        }
        public override void OnPause()
        {
            base.OnPause();
        }
        public override void OnResume()
        {
            base.OnResume();

            hidenAdd_bt.Enabled = true;//已上报
            hidenReform_bt.Enabled = true;//待整改
            hidenCheck_bt.Enabled = true;//待复查
            hidenEcn_bt.Enabled = true;//已完成
            hidenSearch_bt.Enabled = true;
            partolAdd_bt.Enabled = true;
            partolRecord_bt.Enabled = true;
            ressim_bt.Enabled = true;
            hiden_danger_bt.Enabled = true;
            hiden_list_bt.Enabled = true;
            hiden_control_bt.Enabled = true;
            safePartol_bt.Enabled = true;
            hiden_auit_bt.Enabled = true;//隐患审核
            txt_0.Enabled = true;
            txt_1.Enabled = true;
            txt_2.Enabled = true;
            txt_3.Enabled = true;

            QBadgeView bv_4 = new QBadgeView(this.Activity);
            //待审核隐患个数
            int checkUn = selectUnCheckHiden();
            //按钮上显示数字提示效果
            int count_4 = checkUn;
            bv_4.SetBadgeNumber(count_4);
            //bv_4.SetBadgeText(count_4.ToString());
            bv_4.SetBadgeTextSize(12, true);
            //相对于控件的位置
            bv_4.BindTarget(hiden_auit_bt);

            DataTable dtStatus = select_dangerCount();
            if (dtStatus.Rows.Count > 0)
            {
                int hidenStatu = -1;
                for (int i = 0; i < dtStatus.Rows.Count; i++)
                {
                    hidenStatu = Convert.ToInt32(dtStatus.Rows[i]["hidenFlag"].ToString());
                    if (Convert.ToInt32(dtStatus.Rows[i]["hidenFlag"].ToString()) == 0)
                    {
                        QBadgeView bv_1 = new QBadgeView(this.Activity);
                        txt_0.Text = dtStatus.Rows[i]["counts"].ToString();
                        //按钮上显示数字提示效果
                        int count_1 = Convert.ToInt32(txt_0.Text);
                        bv_1.SetBadgeNumber(count_1);
                        // bv_1.SetBadgeText(count_1.ToString());
                        bv_1.SetBadgeTextSize(12, true);
                        //相对于控件的位置
                        bv_1.BindTarget(hidenReform_bt);
                    }
                    else if (Convert.ToInt32(dtStatus.Rows[i]["hidenFlag"].ToString()) == 1)
                    {
                        QBadgeView bv_2 = new QBadgeView(this.Activity);
                        txt_1.Text = dtStatus.Rows[i]["counts"].ToString();
                        //按钮上显示数字提示效果
                        int count_2 = Convert.ToInt32(txt_1.Text);
                        bv_2.SetBadgeNumber(count_2);
                        //bv_2.SetBadgeText(count_2.ToString());
                        bv_2.SetBadgeTextSize(12, true);
                        bv_2.BindTarget(hidenCheck_bt);

                    }
                    else if (Convert.ToInt32(dtStatus.Rows[i]["hidenFlag"].ToString()) == 2)
                    {
                        QBadgeView bv_3 = new QBadgeView(this.Activity);
                      
                        txt_2.Text = dtStatus.Rows[i]["counts"].ToString();
                        //按钮上显示数字提示效果
                        int count_3 = Convert.ToInt32(txt_2.Text);
                        bv_3.SetBadgeNumber(count_3);
                        //bv_3.SetBadgeText(count_3.ToString());
                        bv_3.SetBadgeTextSize(12, true);
                        bv_3.BindTarget(hidenEcn_bt);
                    }
                    else
                    {
                        txt_3.Text = dtStatus.Rows[i]["counts"].ToString();
                    }
                }
            }
        }
       
        public override void OnStart()
        {
            base.OnStart();
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
         
          
            try
            {
                if (IsAdded) //判断是否依附Activity
                {
                    Android.Support.V7.Widget.Toolbar toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                    toolbar.Title = "首页";
                    //修改toolbar标题字体大小
                    toolbar.SetTitleTextAppearance(this.Activity, Resource.Style.Toolbar_TitleText);

                    DateTime dt = DateTime.Now;

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

                    //---------------日常工作按钮点击-------------------
                    //已上报
                    txt_0 = view.FindViewById<TextView>(Resource.Id.bt_0);
                    txt_0.Click += delegate
                    {
                        txt_0.Enabled = false;
                        hidenFlag = 0;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //待整改
                     txt_1 = view.FindViewById<TextView>(Resource.Id.bt_1);
                    txt_1.Click += delegate
                    {
                        txt_1.Enabled = false;
                        hidenFlag = 1;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //待复查
                     txt_2 = view.FindViewById<TextView>(Resource.Id.bt_2);
                    txt_2.Click += delegate
                    {
                        txt_2.Enabled = false;
                        hidenFlag = 2;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //已完成
                     txt_3 = view.FindViewById<TextView>(Resource.Id.bt_3);
                    //txt_3.Click += delegate
                    //{
                    //    txt_3.Enabled = false;
                    //    hidenFlag = 3;
                    //    Intent intent = new Intent(this.Activity, typeof(SafePartolActivity));
                    //    sendFieldInfo(intent);
                    //};
                    TextView txtMsg = view.FindViewById<TextView>(Resource.Id.txtMsg);
                    txtMsg.Text = XmlDBClass.companyName + "，" + XmlDBClass.userName + "，" + string.Format("{0:D}", dt);
        
                    //安全确认
                    safePartol_bt = view.FindViewById<ImageButton>(Resource.Id.img_safe_partol);
                    safePartol_bt.Click += delegate
                    {
                        safePartol_bt.Enabled = false;
                        Intent intent = new Intent(this.Activity, typeof(SafePartolActivity));
                        sendFieldInfo(intent);
                    };
                    //重大隐患录入
                    bigHidenAdd_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_big);
                    bigHidenAdd_bt.Click += delegate
                    {
                        bigHidenAdd_bt.Enabled = false;
                        hidenFlag = 0;
                        XmlDBClass.isBig = 1;
                        Intent intent = new Intent(this.Activity, typeof(HidenAddActivity));
                        intent.PutExtra("isBig", XmlDBClass.isBig.ToString());
                        sendFieldInfo(intent);
                    };
                    //隐患录入
                    hidenAdd_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_add);
                    hidenAdd_bt.Click += delegate
                    {
                        hidenAdd_bt.Enabled = false;
                        hidenFlag = 0;
                        Intent intent = new Intent(this.Activity, typeof(HidenAddActivity));
                        sendFieldInfo(intent);
                    };
                    //下发整改单
                    hidenReform_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_reform);
                    
                    hidenReform_bt.Click += delegate
                    {
                        hidenReform_bt.Enabled = false;
                        hidenFlag = 0;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //整改责任人录入
                    hidenCheck_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_check);
                   
                    hidenCheck_bt.Click += delegate
                    {
                        hidenCheck_bt.Enabled = false;
                        hidenFlag = 1;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //整改复查人录入
                    hidenEcn_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_end);
                   
                    hidenEcn_bt.Click += delegate
                    {
                        hidenEcn_bt.Enabled = false;
                        hidenFlag = 2;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //隐患查询
                    hidenSearch_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_search);
                    hidenSearch_bt.Click += delegate
                    {
                        hidenSearch_bt.Enabled = false;
                        hidenFlag = 4;
                        Intent intent = new Intent(this.Activity, typeof(ReformActivity));
                        sendFieldInfo(intent);
                    };
                    //隐患审核
                    hiden_auit_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_audit);
                    hiden_auit_bt.Click += delegate
                    {
                        hiden_auit_bt.Enabled = false;
                        hidenFlag = 1;
                        Intent intent = new Intent(this.Activity, typeof(AuitListActivity));
                        sendFieldInfo(intent);
                    };
                    //风险巡查
                    partolAdd_bt = view.FindViewById<ImageButton>(Resource.Id.img_partol_add);
                    partolAdd_bt.Click += delegate
                    {
                        partolAdd_bt.Enabled = false;
                        Intent intent = new Intent(this.Activity, typeof(PartolNewsActivity));
                        sendFieldInfo(intent);
                    };
                    //巡查记录
                    partolRecord_bt = view.FindViewById<ImageButton>(Resource.Id.img_partol_record);
                    partolRecord_bt.Click += delegate
                    {
                        partolRecord_bt.Enabled = false;
                        Intent intent = new Intent(this.Activity, typeof(PartolRecordActivity));
                        sendFieldInfo(intent);
                    };
                    //制度查阅
                    ressim_bt = view.FindViewById<ImageButton>(Resource.Id.img_ressim);
                    ressim_bt.Click += delegate
                    {
                        ressim_bt.Enabled = false;
                        Intent intent = new Intent(this.Activity, typeof(RegimeActivity));
                        sendFieldInfo(intent);
                    };
                    //隐患排查清单
                    hiden_list_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_list);
                    hiden_list_bt.Click += delegate
                    {
                        hiden_list_bt.Enabled = false;
                        hidenFlag = 4;
                        Intent intent = new Intent(this.Activity, typeof(HidenListActivity));
                        sendFieldInfo(intent);
                    };
                    //风险告知卡
                    hiden_danger_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_danger);
                    hiden_danger_bt.Click += delegate
                    {
                        hiden_danger_bt.Enabled = false;
                        hidenFlag = 4;
                        Intent intent = new Intent(this.Activity, typeof(DangerListActivity));
                        sendFieldInfo(intent);
                    };
                    //应急处置卡
                    hiden_control_bt = view.FindViewById<ImageButton>(Resource.Id.img_hiden_control);
                    hiden_control_bt.Click += delegate
                    {
                        hiden_control_bt.Enabled = false;
                        hidenFlag = 4;
                        Intent intent = new Intent(this.Activity, typeof(DangerControlActivity));
                        sendFieldInfo(intent);
                    };
                    
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message, this.Activity, true);
            }
            return view;
        }

        #region Fragment 传递数据到Activity
        private void sendFieldInfo(Intent intent )
        {
            intent.PutExtra("userID", XmlDBClass.userID.ToString());
            intent.PutExtra("userName", XmlDBClass.userName.ToString());
            intent.PutExtra("hidenFlag", hidenFlag.ToString());
            intent.PutExtra("userCode", XmlDBClass.userCode);
            intent.PutExtra("departID", XmlDBClass.departID.ToString());
            intent.PutExtra("departCode", XmlDBClass.departCode);
            intent.PutExtra("departName", XmlDBClass.departName);
            intent.PutExtra("accID", XmlDBClass.accID.ToString());
            intent.PutExtra("companyName", XmlDBClass.companyName.ToString());
          
            StartActivity(intent);
           //this.Activity.Finish();
        }
        #endregion

        #region 查询隐患状态个数
        private DataTable select_dangerCount()
        {
            DataTable dt = null;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            
            string revXML = safeWeb.select_hidenStatis(XmlDBClass.accID, XmlDBClass.departID);
            if (revXML != "")
            {
                //xml数据转table
                dt = XmlDBClass.ConvertXMLToDataTable(revXML);
            }
            return dt;
        }
        #endregion

        #region 查询待审核隐患个数
        private int selectUnCheckHiden()
        {
            DataTable dt = null;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();

            int checkCount = safeWeb.select_unCheckHiden(XmlDBClass.accID, XmlDBClass.departID);
           
            return checkCount;
        }
        #endregion
    }
}