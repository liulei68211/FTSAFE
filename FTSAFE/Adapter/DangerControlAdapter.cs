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
    /// 应急处置卡 适配器
    /// </summary>
    public class DangerControlItem
    {
        public int itemOrder { get; set; }//序号
        public string dangerType { get; set; }//事故类型
        public string dangerControl { get; set; }//处置要点
        public DangerControlItem(int itemorder, string dangertype, string dangercontrol)
        {
            this.itemOrder = itemorder;
            this.dangerType = dangertype;
            this.dangerControl = dangercontrol;
        }

    }
    class DangerControlAdapter : BaseAdapter<DangerControlItem>
    {
        List<DangerControlItem> items;
        Activity context;

        public DangerControlAdapter(Activity context, List<DangerControlItem> items) : base()
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

        public override DangerControlItem this[int position]
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
            public TextView txt_type;
            public TextView txt_control;
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
            convertView = context.LayoutInflater.Inflate(Resource.Layout.danger_control_listview, null);
            DangerControlItem item = items[position];
            if (convertView != null)
            {
                holder = new ViewHolder();
                holder.text_order = convertView.FindViewById<TextView>(Resource.Id.txtOrder);
                holder.txt_type = convertView.FindViewById<TextView>(Resource.Id.txtType);
                holder.txt_control = convertView.FindViewById<TextView>(Resource.Id.txtControl);
                convertView.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            holder.text_order.Text = item.itemOrder.ToString();
            holder.txt_type.Text = item.dangerType;
            holder.txt_control.Text = item.dangerControl;

            if (currentItem == position)
            {
                holder.text_order.Selected = true;
                holder.txt_type.Selected = true;
                holder.txt_control.Selected = true;
            }
            return convertView;
        }
    }
}