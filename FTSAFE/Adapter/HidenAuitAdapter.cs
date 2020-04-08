using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FTSAFE.Adapter
{
    /// <summary>
    /// 隐患审核adpter
    /// </summary>
    public class AuitItem
    {
        public int itemOrder { get; set; }//序号
        public string hidenPerson { get; set; }//排查人
        public string hidenDept { get; set; }//排查单位
        public string hidenTm { get; set; }//排查时间
        public string hidenInfo { get; set; }//隐患信息
        public string hidenStatus { get; set; }//隐患状态
        public AuitItem(int itemorder, string hidenperson, string hidendept, string hidentm, string hideninfo, string hidenstatus)
        {
            this.itemOrder = itemorder;
            this.hidenPerson = hidenperson;
            this.hidenDept = hidendept;
            this.hidenTm = hidentm;
            this.hidenInfo = hideninfo;
            this.hidenStatus = hidenstatus;
        }
    }
    class HidenAuitAdapter : BaseAdapter<AuitItem>
    {
        List<AuitItem> items;
        Activity context;

        public HidenAuitAdapter(Activity context, List<AuitItem> items) : base()
        {
            this.items = items;
            this.context = context;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override AuitItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public class ViewHolder : Java.Lang.Object
        {
            public TextView text_order;
            public TextView text_person;
            public TextView text_dept;
            public TextView text_time;
            public TextView text_info;
            public TextView text_status;
        }
        //当前Item被点击的位置
        private int currentItem = -1;
        public void setCurrentItem(int currentItem)
        {
            this.currentItem = currentItem;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;
            convertView = context.LayoutInflater.Inflate(Resource.Layout.list_auit, null);
            AuitItem item = items[position];
            if (convertView != null)
            {
                holder = new ViewHolder();
                holder.text_order = convertView.FindViewById<TextView>(Resource.Id.txtOrder);
                holder.text_person = convertView.FindViewById<TextView>(Resource.Id.txtPerson);
                holder.text_dept = convertView.FindViewById<TextView>(Resource.Id.txtDept);
                holder.text_time = convertView.FindViewById<TextView>(Resource.Id.txtTime);
                holder.text_info = convertView.FindViewById<TextView>(Resource.Id.txtInfo);
                holder.text_status = convertView.FindViewById<TextView>(Resource.Id.txtStatus);
                convertView.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            holder.text_order.Text = item.itemOrder.ToString();
            holder.text_person.Text = item.hidenPerson;
            holder.text_dept.Text = item.hidenDept;
            holder.text_time.Text = item.hidenTm;
            holder.text_info.Text = item.hidenInfo;
            holder.text_status.Text = item.hidenStatus;

            if (currentItem == position)
            {
                holder.text_order.Selected = true;
                holder.text_person.Selected = true;
                holder.text_dept.Selected = true;
                holder.text_time.Selected = true;
                holder.text_info.Selected = true;
                holder.text_status.Selected = true;
            }
            return convertView;
        }
    }
}