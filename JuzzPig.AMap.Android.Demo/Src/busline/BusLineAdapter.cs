
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Services.Busline;
using JuzzPig.AMap.AndroidDemo;
using Object = Java.Lang.Object;

namespace JuzzPig.AMap.AndroidDemo
{
    public class BusLineAdapter:BaseAdapter
{
    private List<BusLineItem> busLineItems;
    private LayoutInflater layoutInflater;

    public BusLineAdapter(Context context, List<BusLineItem> busLineItems)
    {
        this.busLineItems = busLineItems;
        layoutInflater = LayoutInflater.From(context);
    }

        public override Object GetItem(int position)
        {
        return position;
        }

        public override long GetItemId(int position)
        {
        return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;
            if (convertView == null)
            {
                convertView = layoutInflater.Inflate(Resource.Layout.busline_item, null);
                holder = new ViewHolder();
                holder.busName = (TextView)convertView.FindViewById(Resource.Id.busname);
                holder.busId = (TextView)convertView.FindViewById(Resource.Id.busid);
                convertView.Tag=(holder);
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            holder.busName.SetText(Convert.ToInt32("公交名 :"
                                                   + busLineItems[position].BusLineName));
            holder.busId.SetText(Convert.ToInt32("公交ID:"
                                                 + busLineItems[position].BusLineId));
            return convertView;
        }

        public override int Count
        {
            get
            {
                return busLineItems.Count;
            }
        }


        internal class ViewHolder:Java.Lang.Object
    {
        public TextView busName;
        public TextView busId;
    }

}
}