
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Core;
using Com.Amap.Api.Services.Poisearch;
using Com.Amap.Api.Services.Route;
using Java.Lang;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{
    public class RouteSearchAdapter:BaseAdapter
{
    private Context context;
    private List<PoiItem> poiItems = null;
    private LayoutInflater mInflater;

    public RouteSearchAdapter(Context context, List<PoiItem> poiItems)
    {
        this.context = context;
        this.poiItems = poiItems;
        mInflater = LayoutInflater.From(context);
    }


    public override int Count
    {
        get { return poiItems.Count; }
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
        if (convertView == null)
        {
            convertView = mInflater.Inflate(Resource.Layout.poi_result_list, null);
        }

        TextView PoiName = ((TextView) convertView.FindViewById(Resource.Id.poiName));
        TextView poiAddress = (TextView) convertView
            .FindViewById(Resource.Id.poiAddress);
        PoiName.Text=(poiItems[(position)].Title);
        string address = null;
        if (poiItems[(position)].Snippet != null)
        {
            address = poiItems[(position)].Snippet;
        }
        else
        {
            address = "中国";
        }
        poiAddress.Text=("地址:" + address);
        return convertView;
    }

}

}