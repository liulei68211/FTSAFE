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
    public class PartolDataItem
    {
        public int itemOrder { get; set; }//序号
        public string partolPerson { get; set; }//巡查人
        public string partolFac { get; set; }//所属部门
        public string partolTime { get; set; }//巡查日期
        public string partolStatus { get; set; }//是否巡查
      //  public string partolResult { get; set; }//巡查结果 ,string partolresult
        public PartolDataItem(int itemorder, string partolperson, string partolfac,string partoltime,string partolstatus)
        {
            this.itemOrder = itemorder;
            this.partolPerson = partolperson;
            this.partolFac = partolfac;

            this.partolTime = partoltime;
            this.partolStatus = partolstatus;
            //this.partolResult = partolresult;
        }
    }
    class PatrolDataAdapter : BaseAdapter<PartolDataItem>
    {
        List<PartolDataItem> items;
        Activity context;

        public PatrolDataAdapter(Activity context, List<PartolDataItem> items) : base()
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

        public override PartolDataItem this[int position]
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
            public TextView txt_person;
            public TextView txt_fac;
            public TextView txt_time;
            public TextView txt_status;
          //  public TextView txt_result;
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
            convertView = context.LayoutInflater.Inflate(Resource.Layout.activity_partolData_liat, null);
            PartolDataItem item = items[position];
            if (convertView != null)
            {
                holder = new ViewHolder();
                holder.text_order = convertView.FindViewById<TextView>(Resource.Id.txtOrder);
                holder.txt_person = convertView.FindViewById<TextView>(Resource.Id.txtPartolPerson);
                holder.txt_fac = convertView.FindViewById<TextView>(Resource.Id.txtPartolFac);
                holder.txt_time = convertView.FindViewById<TextView>(Resource.Id.txtPartolTime);
                holder.txt_status = convertView.FindViewById<TextView>(Resource.Id.txtPartolStatus);
              //  holder.txt_result = convertView.FindViewById<TextView>(Resource.Id.txtPartolResult);
                convertView.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            holder.text_order.Text = item.itemOrder.ToString();
            holder.txt_person.Text = item.partolPerson;
            holder.txt_fac.Text = item.partolFac;
            holder.txt_time.Text = item.partolTime;
            holder.txt_status.Text = item.partolStatus;
          //  holder.txt_result.Text = item.partolResult;


            if (holder.txt_status.Text == "已巡查")
            {
                holder.txt_status.SetTextColor(Android.Graphics.Color.Green);
            }
            else
            {
                holder.txt_status.SetTextColor(Android.Graphics.Color.Red);
            }

            if (currentItem == position)
            {
                holder.text_order.Selected = true;
                holder.txt_person.Selected = true;
                holder.txt_fac.Selected = true;
                holder.txt_time.Selected = true;
                holder.txt_status.Selected = true;
             //   holder.txt_result.Selected = true;
            }
            else
            {
                holder.text_order.Selected = false;
                holder.txt_person.Selected = false;
                holder.txt_fac.Selected = false;
                holder.txt_time.Selected = false;
                holder.txt_status.Selected = false;
             //   holder.txt_result.Selected = false;
            }
            return convertView;
        }
    }
}