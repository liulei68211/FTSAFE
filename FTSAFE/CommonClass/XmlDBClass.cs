using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FTSAFE.CommonClass
{
    public class XmlDBClass
    {
        public static int accID = 0;
        public static string accCode = "";
        public static int userID = 0;
        public static int departID = 0;
        public static string departCode = "";
        public static string departName = "";
        public static string userCode = "";
        public static string userName = "";
        public static string passWord = "";
        public static string facName = "";
        public static string deptName = "";
        public static string stationName = "";
        public static string workArea = "";
        public static int autoID = 0;
        public static int stationID = 0;
        public static string equipmentCode = "";
        public static string equipmentName = "";
        public static string mobile = "";
        public static int userStatus = 0;
        public static string companyName = "";
        public static string hidenPerson = "";//排查人
        public static string hidenInfo = "";
        public static string versionName = "";//版本号
        public static string hidenImgAdress = "";//隐患上报图片
        public static string abarImgAdress = "";//隐患整改图片
        public static int isBig = 0;//是否重大隐患
        //xml  转table
        #region xml数据转table
        public static DataTable ConvertXMLToDataTable(string xmlData)
        {
            TextReader sr = null;
            try
            {
                DataTable dt = new DataTable();
                sr = new StringReader(xmlData);
                dt.ReadXml(sr);
                return dt;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null) sr.Close();
            }
        }
        #endregion

        //查询角色
        public static string roleNameSearch(int userid)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            string roleName = safeWeb.roleSafe(userid);
            return roleName;
        }
        #region Activity与Activity发送数据
        public static void seneFieldInfo(Intent intent)
        {
            intent.PutExtra("accID", accID.ToString());
            intent.PutExtra("userID", userID.ToString());
            intent.PutExtra("userName", userName);
            intent.PutExtra("departCode", departCode);//岗位编号
            intent.PutExtra("workArea", workArea);
            intent.PutExtra("departID", departID.ToString());
            intent.PutExtra("departName", departName);
            intent.PutExtra("mobile", mobile);
            intent.PutExtra("companyName", companyName);
        }
        #endregion
    }
}