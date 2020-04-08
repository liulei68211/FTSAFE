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
    /// 风险巡查信息
    /// </summary>
    public class PartolItem
    {
        public int itemOrder { get; set; }//序号
        public string dangerObj { get; set; }//辨识对象
        public string dangerInfo { get; set; }//危害因素
        public string dangerStand { get; set; }//事故类型
        public string dangerControl { get; set; }//控制措施
        public string dangerLevel { get; set; }//风险级别
        public PartolItem(int itemorder, string dangerobj, string dangerinfo, string dangerstand, string dangercontrol, string dangerlevel)
        {
            this.itemOrder = itemorder;
            this.dangerObj = dangerobj;
            this.dangerInfo = dangerinfo;
            this.dangerStand = dangerstand;
            this.dangerControl = dangercontrol;
            this.dangerLevel = dangerlevel;
        }
    }
    class PartolInfoAdapter : BaseAdapter<PartolItem>
    {
        List<PartolItem> items;
        Activity context;

        public PartolInfoAdapter(Activity context, List<PartolItem> items) : base()
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

        public override PartolItem this[int position]
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
            public TextView txt_obj;
            public TextView txt_danger;
            public TextView txt_stand;
            public TextView txt_control;
            public TextView txt_level;
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
            convertView = context.LayoutInflater.Inflate(Resource.Layout.activity_partol_list, null);
            PartolItem item = items[position];
            if (convertView != null)
            {
                holder = new ViewHolder();
                holder.text_order = convertView.FindViewById<TextView>(Resource.Id.txtOrder);
                holder.txt_obj = convertView.FindViewById<TextView>(Resource.Id.txtObj);
                holder.txt_danger = convertView.FindViewById<TextView>(Resource.Id.txtDanger);
                holder.txt_stand = convertView.FindViewById<TextView>(Resource.Id.txtStand);
                holder.txt_control = convertView.FindViewById<TextView>(Resource.Id.txtControl);
                holder.txt_level = convertView.FindViewById<TextView>(Resource.Id.txtLevel);
                convertView.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            holder.text_order.Text = item.itemOrder.ToString();
            
            holder.txt_obj.Text = item.dangerObj;
            holder.txt_danger.Text = item.dangerInfo;
            holder.txt_stand.Text = item.dangerStand;
            holder.txt_control.Text = item.dangerControl;
            holder.txt_level.Text = item.dangerLevel;  

            if (currentItem == position)
            {
                holder.text_order.Selected = true;
                holder.txt_obj.Selected = true;
                holder.txt_danger.Selected = true;
                holder.txt_stand.Selected = true;
                holder.txt_control.Selected = true;
                holder.txt_level.Selected = true;
            }
            //else
            //{
            //    holder.text_order.Selected = false;
            //    holder.txt_obj.Selected = false;
            //    holder.txt_danger.Selected = false;
            //    holder.txt_stand.Selected = false;
            //    holder.txt_control.Selected = false;
            //    holder.txt_level.Selected = false;
            //}
            return convertView;
        }
    }
}