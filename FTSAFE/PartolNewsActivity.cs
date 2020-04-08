using System;
using System.Collections.Generic;

using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FTSAFE.CommonClass;
using ZXing;
using ZXing.Mobile;
namespace FTSAFE
{
    [Activity(Label = "风险巡查")]
    public class PartolNewsActivity : AppCompatActivity
    {
        private EditText edit_time = null;
        private EditText editDept = null;
        private EditText editUser = null;
        private Button btSub = null;
        private int partDepID = 0;
        private string partolFlag = "0";//是否签到
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_partol_news);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "风险巡查";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
            SetSupportActionBar(toolbar);
            //设置返回按钮
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //响应返回按钮
            toolbar.NavigationClick += (s, e) =>
            {
                //Intent i = new Intent(this, typeof(NewMainActivity));
                //StartActivity(i);
                Finish();
            };

            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            XmlDBClass.departID = Convert.ToInt32(Intent.GetStringExtra("departID"));
            XmlDBClass.userName = Intent.GetStringExtra("userName");
            XmlDBClass.userCode = Intent.GetStringExtra("userCode");
            XmlDBClass.workArea = Intent.GetStringExtra("workArea");
            XmlDBClass.departName = Intent.GetStringExtra("departName");
            //初始化扫描仪 不然会报错空引用
            MobileBarcodeScanner.Initialize(Application);

            TextView txtSign = FindViewById<TextView>(Resource.Id.txtSign);
            editDept = FindViewById<EditText>(Resource.Id.textDeptValue);
            editDept.Text = XmlDBClass.departName;
            editUser = FindViewById<EditText>(Resource.Id.editName);
            //editUser.Text = XmlDBClass.userName;
            edit_time = FindViewById<EditText>(Resource.Id.txt_Time);
            edit_time.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

            EditText editName = FindViewById<EditText>(Resource.Id.editName);
            //巡查签到按钮 扫二维码
            Button btScan = FindViewById<Button>(Resource.Id.btSign);
            btScan.Click += bt_scannerClick;
            //提交按钮
            btSub = FindViewById<Button>(Resource.Id.btSub);
            btSub.Click += btSub_Click;
            //风险信息按钮
            Button btInfo = FindViewById<Button>(Resource.Id.btInfo);
            btInfo.Click += btInfo_Click;
        }
        #region 提交按钮
        private void btSub_Click(object sender, EventArgs e)
        {
            string partolResult = "0";
            string partolName = "";
            string messageStr = "";
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
          
            // EditText editName = FindViewById<EditText>(Resource.Id.editName);
            //巡查单位
            TextView editDept = FindViewById<TextView>(Resource.Id.textDeptValue);
            TextView txtSign = FindViewById<TextView>(Resource.Id.txtSign);
           
            var button = (Button)sender;

            var dd = button.Pressed;
            if (!button.Pressed)
            {
                button.Pressed = false;
                return;
            }

            //对话框
            var callDialog = new Android.App.AlertDialog.Builder(this);

            //对话框内容
            messageStr = "确定提交巡查记录吗?";
            callDialog.SetMessage(messageStr);
            callDialog.SetNeutralButton("确定", delegate
            {
                if (button.Text == "提交")
                {
                    if (editUser.Text != "")
                    {
                        partolName = editUser.Text;
                        if (txtSign.Text == "")
                        {
                            CommonFunction.ShowMessage("请先签到", this, true);
                        }
                        else
                        {
                            //组装xml数据
                            List<string> listData = new List<string>();
                            List<string> listName = new List<string>();
                            // hidenTime,hidenPtrStyleID,hidenTypeID,,hidenLevel,hidenInfo,hidenFlag
                            listName.Add("userID");
                            listName.Add("partolPersonName");
                            listName.Add("partolTime");
                            listName.Add("partolFlag");
                            listName.Add("partolStyleID");
                            listName.Add("ts");
                            listName.Add("accID");
                            listName.Add("departID");

                            listData.Add(XmlDBClass.userID.ToString());
                            listData.Add(editUser.Text);
                            listData.Add(edit_time.Text);
                            listData.Add("1");
                            listData.Add("0");
                            listData.Add(edit_time.Text);
                            listData.Add(XmlDBClass.accID.ToString());
                            listData.Add(XmlDBClass.departID.ToString());
                            //组装xml数据
                            XmlDocument xmldoc = new XmlDocument();
                            XmlNode node = xmldoc.CreateXmlDeclaration("1.0", "GBK", "");
                            xmldoc.AppendChild(node);
                            XmlNode root = xmldoc.CreateElement("xml_root");
                            xmldoc.AppendChild(root);
                            XmlNode node1 = xmldoc.CreateElement("out_infos");
                            root.AppendChild(node1);
                            string xml = xmldoc.OuterXml;

                            CreateXml crxml = new CreateXml();
                            xml = crxml.CreatCKXml(xml, listName, listData);
                            int result = safeWeb.insertPartolInfoDay(xml);
                            if (result == 1)
                            {
                                btSub.Enabled = false;
                                btSub.SetBackgroundColor(Color.Gray);
                                Toast.MakeText(this, "操作成功", ToastLength.Short).Show();
                                //CommonFunction.ShowMessage("操作成功", this, true);
                            }
                            if (result == 0)
                            {
                                CommonFunction.ShowMessage("已经提交过巡检记录", this, true);
                            }
                            if (result == 2)
                            {
                                CommonFunction.ShowMessage("操作失败，请检查网络", this, true);
                            }
                        }
                    }
                    else
                    {
                        CommonFunction.ShowMessage("请填写巡查人", this, true);
                    }
                }
            });
            //取消按钮
            callDialog.SetNegativeButton("取消", delegate {
                button.Pressed = false;
            });

            //显示对话框
            callDialog.Show();
        }
        #endregion

        #region 风险巡查信息
        private void btInfo_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(PartolInfoActivity));
            intent.PutExtra("userID", XmlDBClass.userID.ToString());
            intent.PutExtra("accID", XmlDBClass.accID.ToString());
            intent.PutExtra("userCode", XmlDBClass.userCode);
            intent.PutExtra("departID", XmlDBClass.departID.ToString());
            StartActivity(intent);
        }
        #endregion

        #region 二维码扫描按钮
        private async void bt_scannerClick(object sender, EventArgs e)
        {
            try
            {
                var opts = new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.QR_CODE
                    }
                };

                opts.CharacterSet = "";
                var scanner = new MobileBarcodeScanner();
                //不适用自定义界面
                scanner.UseCustomOverlay = false;
                scanner.FlashButtonText = "识别";
                scanner.CancelButtonText = "取消";

                //设置上下提示文字
                // scanner.TopText = "请将二维码对准方框内";
                //scanner.BottomText = "确认后按下右下角识别按钮";
                scanner.BottomText = "请将二维码对准方框内";

                var result = await scanner.Scan(opts);
                if (!string.IsNullOrEmpty(result.Text))
                {
                    string xxx = result.Text;
                    //先求出最后出现这个字符的下标
                    int index = xxx.LastIndexOf('=');

                    //从下一个索引开始截取
                    partDepID = Convert.ToInt32(xxx.Substring(index + 1));
                    ScanResultHandle(result);
                    //if (XmlDBClass.departID != partDepID)
                    //{
                    //    CommonFunction.ShowMessage("您扫描的是" + XmlDBClass.departName + "岗位的二维码,和你的岗位不符，请扫描正确的二维码", this, true);
                    //}
                    //else
                    //{
                      
                    //    ScanResultHandle(result);
                    //}
                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 二维码扫描结果
        private void ScanResultHandle(ZXing.Result result)
        {
            string nowTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            //巡查人
            EditText editName = FindViewById<EditText>(Resource.Id.editName);
            //是否签到
            TextView txtSign = FindViewById<TextView>(Resource.Id.txtSign);
            //巡查时间
            TextView txtTimePartol = FindViewById<TextView>(Resource.Id.txt_Time);
            txtTimePartol.Text = nowTime;
            

            if (!string.IsNullOrEmpty(result.Text))
            {
                editName.Enabled = true;
                editName.Text = XmlDBClass.userName;
                partolFlag = "1";
                txtSign.Text = "已签到";
                txtSign.SetTextColor(Android.Graphics.Color.Green);
            }
          
        }
        #endregion

        #region 操作成功
        private void WriteDataOK(bool ok)
        {
            if (ok)
            {
                CommonFunction.ShowMessage("操作成功！", this, true);
            }
            else CommonFunction.ShowMessage("提交失败！", this, true);
        }
        #endregion
    }
}