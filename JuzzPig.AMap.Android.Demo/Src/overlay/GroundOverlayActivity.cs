
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Com.Amap.Api.Location;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Core;
using Com.Amap.Api.Services.Poisearch;
using Com.Amap.Api.Services.Route;
using Java.Lang;
using Java.Net;
using Java.Util;
using JuzzPig.AMap.Demo.util;
using Math = Java.Lang.Math;
using Object = Java.Lang.Object;

namespace JuzzPig.AMap.AndroidDemo
{


    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]


/**
 * AMapV1地图中简单介绍一些GroundOverlay的用法.
 */
    public class GroundOverlayActivity: Activity
{

    private Com.Amap.Api.Maps2d.AMap amap;
    private MapView mapview;
    private GroundOverlay groundoverlay;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.groundoverlay_activity);
        mapview = (MapView) FindViewById(Resource.Id.map);
        mapview.OnCreate(savedInstanceState); // 此方法必须重写
        init();
    }

    /**
	 * 初始化AMap对象
	 */

    private void init()
    {
        if (amap == null)
        {
            amap = mapview.Map;
            addOverlayToMap();
        }
    }

    /**
	 * 往地图上添加一个groundoverlay覆盖物
	 */

    private void addOverlayToMap()
    {
        amap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(39.936713,
            116.386475), 18)); // 设置当前地图显示为北京市恭王府
        LatLngBounds bounds = new LatLngBounds.Builder()
           .Include(new LatLng(39.935029, 116.384377))
           .Include(new LatLng(39.939577, 116.388331)).Build();

        groundoverlay = amap.AddGroundOverlay(new GroundOverlayOptions()
            .Anchor(0.5f, 0.5f)
            .SetTransparency(0.1f)
            .SetImage(BitmapDescriptorFactory
                .FromResource(Resource.Drawable.groundoverlay))
            .PositionFromBounds(bounds));
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnResume()
    {
        base.OnResume();
        mapview.OnResume();
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnPause()
    {
        base.OnPause();
        mapview.OnPause();
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnSaveInstanceState(Bundle outState)
    {
        base.OnSaveInstanceState(outState);
        mapview.OnSaveInstanceState(outState);
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mapview.OnDestroy();
    }
}
}