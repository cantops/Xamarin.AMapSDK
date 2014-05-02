
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
 * AMapV1地图中简单介绍一些Circle的用法.
 */
    public class CircleActivity
    

: Activity , SeekBar.IOnSeekBarChangeListener
{
    private static int WIDTH_MAX = 50;
    private static  int HUE_MAX = 255;
    private static  int ALPHA_MAX = 255;
    private  Com.Amap.Api.Maps2d.AMap aMap;
    private MapView mapView;
    private Circle circle;
    private SeekBar mColorBar;
    private SeekBar mAlphaBar;
    private SeekBar mWidthBar;


    protected override void OnCreate(Bundle savedInstanceState)
    {
       base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.circle_activity);
        mapView =  FindViewById<MapView>(Resource.Id.map);
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
        mAlphaBar.Progress=(50);

        mWidthBar = (SeekBar) FindViewById(Resource.Id.widthSeekBar);
        mWidthBar.Max=(WIDTH_MAX);
        mWidthBar.Progress=(25);
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
        aMap.MoveCamera(CameraUpdateFactory
            .NewLatLngZoom(Constants.BEIJING, 12)); // 设置指定的可视区域地图
        // 绘制一个圆形
        circle = aMap.AddCircle(new CircleOptions().SetCenter(Constants.BEIJING)
            .SetRadius(4000).SetStrokeColor(Color.Argb(50, 1, 1, 1))
            .SetFillColor(Color.Argb(50, 1, 1, 1)).SetStrokeWidth(25));
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
	 * Circle中对填充颜色，透明度，画笔宽度设置响应事件
	 */

    public void OnProgressChanged(SeekBar seekBar, int progress,
        bool fromUser)
    {
        if (circle == null)
        {
            return;
        }
        if (seekBar == mColorBar)
        {
            circle.FillColor=(Color.Argb(progress, 1, 1, 1));
        }
        else if (seekBar == mAlphaBar)
        {
            circle.StrokeColor=(Color.Argb(progress, 1, 1, 1));
        }
        else if (seekBar == mWidthBar)
        {
            circle.StrokeWidth=(progress);
        }
        aMap.Invalidate(); // 刷新地图
    }
}
}