using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FTSAFE.CommonClass;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FTSAFE
{
    [Activity(Label = "密码修改页面")]
    public class PassModifyActivity : AppCompatActivity
    {
        private EditText pass_old = null;
        private EditText pass_new = null;
        private EditText pass_sure = null;
        private string keys = "LM1KW44FaBMHnyJp88ELe3Bj0ZQB8pL3ZDmKxeIgqtp";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_pass_modify_new);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "密码修改";
            //修改toolbar标题字体大小
            toolbar.SetTitleTextAppearance(this, Resource.Style.Toolbar_TitleText);
          
            SetSupportActionBar(toolbar);
            //设置返回按钮
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //响应返回按钮
            toolbar.NavigationClick += (s, e) =>
            {
                //Intent i = new Intent(this, typeof(NewMainActivity));
                //i.PutExtra("backCode", "mineBack");
                //StartActivity(i);
                Finish();
            };
            XmlDBClass.accID = Convert.ToInt32(Intent.GetStringExtra("accID"));
            XmlDBClass.userID = Convert.ToInt32(Intent.GetStringExtra("userID"));
            //原密码
            pass_old = FindViewById<EditText>(Resource.Id.passOld);
            //新密码
            pass_new = FindViewById<EditText>(Resource.Id.passNew);
            //新密码确认
            pass_sure = FindViewById<EditText>(Resource.Id.editPassSure);
            //提交按钮
            Button bt_sub = FindViewById<Button>(Resource.Id.btSub);
            bt_sub.Click += btSubClick;
        }
        private void btSubClick(object sender, EventArgs e)
        {
            SafeWeb.JGNP safeWeb = new SafeWeb.JGNP();
            //密码加密
            if (pass_new.Text != pass_sure.Text)
            {
                //新密码两次输入的不正确，请重新输入
                CommonFunction.ShowMessage("新密码两次输入的不正确，请重新输入",this,true);
            }
            else
            {
                string passNew = Encrypt(pass_sure.Text,keys);//加密
                string passOld = Encrypt(pass_old.Text, keys);//加密
                //调用密码修改接口
                int result = safeWeb.modifyPassMySQL(XmlDBClass.accID,XmlDBClass.userID,passNew, passOld);
                if (result == 1)
                {
                    CommonFunction.ShowMessage("密码修改成功",this,true);
                }
                else if (result == 22)
                {
                    CommonFunction.ShowMessage("密码修改失败，请检查网络是否连接", this, true);
                }
                else
                {
                    CommonFunction.ShowMessage("原密码输入错误请重新输入", this, true);
                }
            }

        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string Encrypt(string encryptStr, string key)
        {
            string ddd = key.Substring(0, 31);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key.Substring(0, 32));
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
       
    }
}