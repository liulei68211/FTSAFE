using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FTSAFE.CommonClass;

namespace FTSAFE
{
    [Activity(Label = "EquipmentActivity")]
    public class EquipmentActivity : Activity
    {
        private GridLayout gridContainer = null;
        private SafeWeb.JGNP safeWeb = null;
        private List<string> equipment_list = new List<string>();
        private List<string> equipmentCode_list = new List<string>();
        private List<int> equipmentID_list = new List<int>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hiden_equipment);

            gridContainer = FindViewById<GridLayout>(Resource.Id.gridLayout1);

            TextView txt_1 = FindViewById<TextView>(Resource.Id.txt_1);
            TextView txt_2 = FindViewById<TextView>(Resource.Id.txt_2);
            TextView txt_3 = FindViewById<TextView>(Resource.Id.txt_3);

            ImageButton imgbt_1 = FindViewById<ImageButton>(Resource.Id.imgbt_1);
            imgbt_1.Click += delegate
            {
                Intent intent = new Intent(this, typeof(EquipmentControlActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("stationID", XmlDBClass.stationID.ToString());
                intent.PutExtra("userName", XmlDBClass.userName);
                intent.PutExtra("equipmentName", txt_1.Text);

                StartActivity(intent);
            };
            ImageButton imgbt_2 = FindViewById<ImageButton>(Resource.Id.imgbt_2);
            imgbt_2.Click += delegate
            {
                Intent intent = new Intent(this, typeof(EquipmentControlActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("stationID", XmlDBClass.stationID.ToString());
                intent.PutExtra("userName", XmlDBClass.userName);
                intent.PutExtra("equipmentName", txt_2.Text);

                StartActivity(intent);
            };
            ImageButton imgbt_3 = FindViewById<ImageButton>(Resource.Id.imgbt_3);
            imgbt_3.Click += delegate
            {
                Intent intent = new Intent(this, typeof(EquipmentControlActivity));
                intent.PutExtra("userID", XmlDBClass.userID.ToString());
                intent.PutExtra("stationID", XmlDBClass.stationID.ToString());
                intent.PutExtra("userName", XmlDBClass.userName);
                intent.PutExtra("equipmentName", txt_3.Text);

                StartActivity(intent);
            };

            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.stationID = Convert.ToInt32(Intent.GetStringExtra("stationID"));
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.userName = Intent.GetStringExtra("userName");
            //根据岗位id 查询该岗位相关的设备
            safeWeb = new SafeWeb.JGNP();
            try
            {
                string revXml = safeWeb.searchEquipmentData(XmlDBClass.stationID);
                //xml转table
                DataTable dt = XmlDBClass.ConvertXMLToDataTable(revXml);
                equipmentID_list.Clear();
                equipment_list.Clear();
                equipmentCode_list.Clear();
                if (dt.Rows.Count > 0)
                {
                    string stationName = dt.Rows[0]["stationName"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        equipmentID_list.Add(Convert.ToInt32(dt.Rows[i]["equipmentID"].ToString()));
                        equipment_list.Add(dt.Rows[i]["equipmentName"].ToString());
                        equipmentCode_list.Add(dt.Rows[i]["equipmentCode"].ToString());                      
                    }

                    txt_1.Text = equipment_list[0].ToString() + "|" + equipmentCode_list[0].ToString();
                    txt_2.Text = equipment_list[1].ToString() + "|" + equipmentCode_list[1].ToString(); ;
                    txt_3.Text = equipment_list[2].ToString() + "|" + equipmentCode_list[2].ToString(); ;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.ShowMessage(ex.Message,this,true);
            }
          
        }
    }
}