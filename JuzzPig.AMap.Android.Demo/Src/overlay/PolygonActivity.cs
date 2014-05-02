
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
using Java.Util;
using JuzzPig.AMap.Demo.util;
using Math = Java.Lang.Math;
using Object = Java.Lang.Object;

namespace JuzzPig.AMap.AndroidDemo
{


    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]

/**
 * AMapV1地图中简单介绍一些Polygon的用法.
 */
    public class PolygonActivity: Activity ,
    SeekBar.IOnSeekBarChangeListener
{
    private static int WIDTH_MAX = 50;
    private static int HUE_MAX = 255;
    private static  int ALPHA_MAX = 255;
    private Com.Amap.Api.Maps2d.AMap aMap;
    private MapView mapView;
    private Polygon polygon;
    private SeekBar mColorBar;
    private SeekBar mAlphaBar;
    private SeekBar mWidthBar;


    protected override void OnCreate(Bundle savedInstanceState)
    {
       base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.polygon_activity);
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
        aMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(Constants.BEIJING, 5)); // 设置指定的可视区域地图
        // 绘制一个长方形
        aMap.AddPolygon(new PolygonOptions()
            .AddAll(createRectangle(Constants.SHANGHAI, 1, 1))
            .SetFillColor(Color.LightGray).SetStrokeColor(Color.Red).SetStrokeWidth(1));
        PolygonOptions options = new PolygonOptions();
        int numPoints = 400;
        float semiHorizontalAxis = 5f;
        float semiVerticalAxis = 2.5f;
        double phase = 2*Math.Pi/numPoints;
        for (int i = 0; i <= numPoints; i++)
        {
            options.Add(new LatLng(Constants.BEIJING.Latitude
                                   + semiVerticalAxis*Math.Sin(i*phase),
                Constants.BEIJING.Longitude + semiHorizontalAxis
                *Math.Cos(i*phase)));
        }
        // 绘制一个椭圆
        polygon = aMap.AddPolygon(options.SetStrokeWidth(25)
            .SetStrokeColor(Color.Argb(50, 1, 1, 1))
            .SetFillColor(Color.Argb(50, 1, 1, 1)));
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

    /**
	 * 生成一个长方形的四个坐标点
	 */

    private Java.Lang.IIterable createRectangle(LatLng center, double halfWidth,
        double halfHeight)
    {
        return (Java.Lang.IIterable)Arrays.AsList(new LatLng(center.Latitude - halfHeight,
            center.Longitude - halfWidth), new LatLng(center.Latitude
                                                      - halfHeight, center.Longitude + halfWidth), new LatLng(
                                                          center.Latitude + halfHeight, center.Longitude + halfWidth),
            new LatLng(center.Latitude + halfHeight, center.Longitude
                                                     - halfWidth));
    }



    public void OnStartTrackingTouch(SeekBar seekBar)
    {
    }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
          
        }

        /**
	 * Polygon中对填充颜色，透明度，画笔宽度设置响应事件
	 */


    public void OnProgressChanged(SeekBar seekBar, int progress,
        bool fromUser)
    {
        if (polygon == null)
        {
            return;
        }
        if (seekBar == mColorBar)
        {
            polygon.FillColor=(Color.Argb(progress, 1, 1, 1));

        }
        else if (seekBar == mAlphaBar)
        {
            polygon.StrokeColor=(Color.Argb(progress, 1, 1, 1));
        }
        else if (seekBar == mWidthBar)
        {
            polygon.StrokeWidth=(progress);
        }
        aMap.Invalidate(); //刷新地图
    }
}
}