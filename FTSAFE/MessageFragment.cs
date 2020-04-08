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

namespace FTSAFE
{
    public class MessageFragment : Fragment
    {
        //要显示的页面
        private int FragmentPage;
        TextView txt_msg_partol = null;
        TextView txt_msg_hiden = null;
        public static MessageFragment NewInstance(int iFragmentPage)
        {
            MessageFragment myFragment = new MessageFragment();
            myFragment.FragmentPage = iFragmentPage;
            return myFragment;
        }
        public override void OnResume()
        {
            base.OnResume();
         
           
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(FragmentPage, container, false);
            try
            {
                if (IsAdded)
                {
                    Android.Support.V7.Widget.Toolbar toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                    toolbar.Title = "消息提醒";
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

                     txt_msg_partol = view.FindViewById<TextView>(Resource.Id.partolMsg);
                     txt_msg_hiden = view.FindViewById<TextView>(Resource.Id.hidenMsg);

                    //未整改隐患
                    DataTable dt = hidenMsgSelect();
                    //风险未巡查
                    int partolCount = partolMsgSelect();
                    //查询岗位巡查规则
                    string revXML = searchPartolStandstr();

                    if (dt.Rows.Count > 0)
                    {
                        int flag_1 = Convert.ToInt32(dt.Rows[0]["counts"]);
                        int flag_2 = Convert.ToInt32(dt.Rows[1]["counts"]);

                        txt_msg_hiden.Text = XmlDBClass.departName + "有" + flag_1 + "个待整改隐患，有" + flag_2 + "个待复查隐患";
                    }
                    else
                    {
                        txt_msg_hiden.Text = XmlDBClass.departName + "没有未处理的隐患";

                    }

                    txt_msg_partol.Text = revXML + "，今天巡查" + partolCount + "次";
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this.Activity,true);
            }
            return view;
        }

        #region 查询未整改隐患
        private DataTable hidenMsgSelect()
        {
            DataTable dt = null;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            try
            {
                string revXml = safeWeb.searchMessageHiden(XmlDBClass.accID,XmlDBClass.departID);
                if (revXml != "")
                {
                    //xml数据转table
                    dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                   
                }
            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        #endregion

        #region 查询风险是否巡查
        private int partolMsgSelect()
        {
            int revID = 0;
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            try
            {
                revID = safeWeb.searchMessagePartol(XmlDBClass.accID,XmlDBClass.departID);
                
            }
            catch (Exception ex)
            {

            }
            return revID;
        }
        #endregion

        #region 查询岗位风险巡查规则
        private string searchPartolStandstr()
        {
            string revStr = "";
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            XmlDBClass xmlDB = new XmlDBClass();
            try
            {
                revStr = safeWeb.searchPartolStand(XmlDBClass.departID);

            }
            catch (Exception ex)
            {

            }
            return revStr;
        }
        #endregion
    }
}