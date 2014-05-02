
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
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
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{


    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]

/**
 * TileOverlay功能介绍
 */
    public class TileOverlayActivity:Activity , View.IOnClickListener
{
    private MapView mapView;
    private Com.Amap.Api.Maps2d.AMap aMap;
    private TileOverlay tileOverlay;
    private Button firstFloor, secondFloor, thridFloor, openTile;
    public static string url = "http://106.3.73.18:8080/tileserver/Tile?x=%d&y=%d&z=%d&f=%d";


    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.tileoverlay_activity);
        mapView =  FindViewById<MapView>(Resource.Id.map);
        mapView.OnCreate(savedInstanceState); // 此方法必须重写
        init();
    }

    /**
	 * 初始化AMap对象
	 */

    private void init()
    {
        firstFloor = (Button) FindViewById(Resource.Id.firstfloor);
        firstFloor.SetOnClickListener(this);
        secondFloor = (Button) FindViewById(Resource.Id.secondfloor);
        secondFloor.SetOnClickListener(this);
        thridFloor = (Button) FindViewById(Resource.Id.thridfloor);
        thridFloor.SetOnClickListener(this);
        openTile = (Button) FindViewById(Resource.Id.opentile);
        openTile.SetOnClickListener(this);

        if (aMap == null)
        {
            aMap = mapView.Map;
            aMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(
                39.910695, 116.372830), 19));
            showTileOverlay(1);
        }
    }

        class MyTileProvider : UrlTileProvider
        {
            private int floor;
            public MyTileProvider(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public MyTileProvider(int p0, int p1,int floor) : base(p0, p1)
            {
                this.floor = floor;
            }

             public override URL GetTileUrl(int x,
            int y,
            int zoom) {
    try {
    return new URL(string.Format(url,
            x,
            y,
            zoom,
            floor));
        } catch
        (MalformedURLException
        e)
        {
            e.PrintStackTrace();
        }
        return null;
    }
        }
    /**
	 * 显示第几层的tileOverlay
	 */

    private void showTileOverlay( int floor)
    {
        if (tileOverlay != null)
        {
            tileOverlay.Remove();
        }
        var tileProvider = new MyTileProvider(256, 256,floor);
        if (tileProvider != null)
        {
            tileOverlay = aMap.AddTileOverlay(new TileOverlayOptions()
                .SetTileProvider(tileProvider)
                .SetDiskCacheDir("/storage/amap/cache").SetDiskCacheEnabled(true)
                .SetDiskCacheSize(100));
        }

    }


    public void OnClick(View v)
    {
        switch (v.Id)
        {
            case Resource.Id.firstfloor:
                showTileOverlay(1);
                break;
            case Resource.Id.secondfloor:
                showTileOverlay(2);
                break;
            case Resource.Id.thridfloor:
                showTileOverlay(3);
                break;
            case Resource.Id.opentile:
                if ("打开TileOverlay".Equals(openTile.Text.ToString()))
                {
                    openTile.Text=("关闭TileOverlay");
                    tileOverlay.Visible=(false);
                    aMap.Invalidate(); // 刷新地图
                }
                else if ("关闭TileOverlay".Equals(openTile.Text.ToString()))
                {
                    openTile.Text=("打开TileOverlay");
                    tileOverlay.Visible=(true);
                    aMap.Invalidate(); // 刷新地图
                }
                break;
            default:
                break;
        }
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnResume()
    {
        base.OnResume();
        mapView.OnResume();
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnPause()
    {
        base.OnPause();
        mapView.OnPause();
    }

    /**
	 * 方法必须重写
	 */


    protected override void OnSaveInstanceState(Bundle outState)
    {
        base.OnSaveInstanceState(outState);
        mapView.OnSaveInstanceState(outState);
    }

    /**
	 * 方法必须重写
	 */

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mapView.OnDestroy();
    }
}
}