
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

    public class RouteSearchPoiDialog : Dialog
    {

        private List<PoiItem> poiItems;
        private Context context;
        private RouteSearchAdapter adapter;
        //protected OnListItemClick mOnClickListener;

        public RouteSearchPoiDialog(Context context)
            : base(context, global::Android.Resource.Style.ThemeDialog)
        {

        }


        public RouteSearchPoiDialog(Context context, List<PoiItem> poiItems)
            : base(context, global::Android.Resource.Style.ThemeDialog)
        {

            this.poiItems = poiItems;
            this.context = context;
            adapter = new RouteSearchAdapter(context, poiItems);
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.routesearch_list_poi);
            ListView listView = (ListView)FindViewById(Resource.Id.ListView_nav_search_list_poi);
            listView.Adapter = (adapter);
            listView.ItemClick += (sender, args) =>
            {
                Dismiss();
              //  mOnClickListener.onListItemClick(this, poiItems[(args.Position)]);
            }

            ;

        }



    }
}