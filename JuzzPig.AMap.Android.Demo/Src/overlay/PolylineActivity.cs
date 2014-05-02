
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
 * AMapV1地图中简单介绍一些Polyline的用法.
 */
    public class PolylineActivity:Activity ,
   SeekBar.IOnSeekBarChangeListener
{
    private static   int WIDTH_MAX = 50;
    private static  int HUE_MAX = 255;
    private static int ALPHA_MAX = 255;

    private Com.Amap.Api.Maps2d.AMap aMap;
    private MapView mapView;
    private Polyline polyline;
    private SeekBar mColorBar;
    private SeekBar mAlphaBar;
    private SeekBar mWidthBar;

    protected override void OnCreate(Bundle savedInstanceState)
    {
       base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.polyline_activity);
        mapView = (MapView) FindViewById(Resource.Id.map);
        mapView.OnCreate(savedInstanceState); // 此方法必须重写
        init();
    }

    /**
	 * 初始化AMap对象
	 */

    private void init()
    {
        mColorBar = (SeekBar) FindViewById(Resource.Id.hueSeekBar);
        mColorBar.Max=(HUE_MAX);
        mColorBar.Progress=(50);

        mAlphaBar = (SeekBar) FindViewById(Resource.Id.alphaSeekBar);
        mAlphaBar.Max=(ALPHA_MAX);
        mAlphaBar.Progress=(255);

        mWidthBar = (SeekBar) FindViewById(Resource.Id.widthSeekBar);
        mWidthBar.Max=(WIDTH_MAX);
        mWidthBar.Progress=(10);
        if (aMap == null)
        {
            aMap = mapView.Map;
            setUpMap();
        }
    }

    private void setUpMap()
    {
        mColorBar.SetOnSeekBarChangeListener(this);
        mAlphaBar.SetOnSeekBarChangeListener(this);
        mWidthBar.SetOnSeekBarChangeListener(this);
        aMap.MoveCamera(CameraUpdateFactory.ZoomTo(4));
        // 绘制一个三角形
        polyline = aMap.AddPolyline((new PolylineOptions())
            .Add(Constants.SHANGHAI, Constants.BEIJING, Constants.CHENGDU).SetWidth(10)
           .SetColor(Color.Argb(255, 1, 1, 1)));
        // 绘制一个乌鲁木齐到哈尔滨的线
        aMap.AddPolyline((new PolylineOptions()).Add(
            new LatLng(43.828, 87.621), new LatLng(45.808, 126.55)).SetColor(
                Color.Red));
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


    public void OnStopTrackingTouch(SeekBar seekBar)
    {
    }


    public void OnStartTrackingTouch(SeekBar seekBar)
    {
    }

    /**
	 * Polyline中对填充颜色，透明度，画笔宽度设置响应事件
	 */

    public void OnProgressChanged(SeekBar seekBar, int progress,
        bool fromUser)
    {
        if (polyline == null)
        {
            return;
        }
        if (seekBar == mColorBar)
        {
            polyline.Color=(Color.Argb(progress, 1, 1, 1));
        }
        else if (seekBar == mAlphaBar)
        {
            float[] prevHSV = new float[3];
            Color.ColorToHSV( new Color(polyline.Color), prevHSV);
            polyline.Color=(Color.HSVToColor(progress, prevHSV));
        }
        else if (seekBar == mWidthBar)
        {
            polyline.Width=(progress);
        }
        aMap.Invalidate(); //刷新地图
    }
}

}