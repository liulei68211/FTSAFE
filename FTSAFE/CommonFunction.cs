﻿using System;
using Android.App;
using Android.Content;

namespace FTSAFE
{
    class CommonFunction
    {
        /// <summary>
        /// 提示消息对话框D:\丰田安全项目\app\手机号登录\1009\FTSAFE\FTSAFE\CommonFunction.cs
        /// </summary>
        static AlertDialog dlg;

        /// <summary>
        /// 读卡模式- NFC/SerialPort
        /// </summary>
        public static string mode;

        #region 十六进制字符尾部添加空格
        /// <summary>
        /// 十六进制字符尾部添加空格
        /// </summary>
        public static string Add20ToUserName(string name, int len)
        {
            try
            {
                while (name.Length < len)
                {
                    name += "20";
                }
                return name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region 消息框
        /// <summary>
        /// 消息框
        /// </summary>
        public static void ShowMessage(String message, Activity activity, bool create)
        {
            try
            {
                if (dlg != null) dlg.Dismiss();  //新消息接收到后，消除上一次消息
                if (!create) return;

                AlertDialog dialog = new AlertDialog.Builder(activity).Create();
                dlg = dialog;

                dialog.SetCancelable(false); // This blocks the ‘BACK‘ button  
                dialog.SetMessage(message);
                dialog.SetButton("确定", new EventHandler<DialogClickEventArgs>((obj, args) => { dialog.Dismiss(); }));
                dialog.Show();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}