
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
 * AMapV1地图中简单介绍一些Marker的用法.
 */
    public class MarkerActivity: Activity , Com.Amap.Api.Maps2d.AMap.IOnMarkerClickListener,
    Com.Amap.Api.Maps2d.AMap.IOnInfoWindowClickListener, Com.Amap.Api.Maps2d.AMap.IOnMarkerDragListener, Com.Amap.Api.Maps2d.AMap.IOnMapLoadedListener,
    View.IOnClickListener, Com.Amap.Api.Maps2d.AMap.IInfoWindowAdapter
{
    private MarkerOptions markerOption;
    private TextView markerText;
    private RadioGroup radioOption;
    private Com.Amap.Api.Maps2d.AMap aMap;
    private MapView mapView;
    private Marker marker2; // 有跳动效果的marker对象
    private LatLng latlng = new LatLng(36.061, 103.834);

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.marker_activity);
        mapView =  FindViewById<MapView>(Resource.Id.map);
        mapView.OnCreate(savedInstanceState); // 此方法必须重写
        init();
    }

    /**
	 * 初始化AMap对象
	 */

    private void init()
    {
        markerText = (TextView) FindViewById(Resource.Id.mark_listenter_text);
        radioOption = (RadioGroup) FindViewById(Resource.Id.custom_info_window_options);
        Button clearMap = (Button) FindViewById(Resource.Id.clearMap);
        clearMap.SetOnClickListener(this);
        Button resetMap = (Button) FindViewById(Resource.Id.resetMap);
        resetMap.SetOnClickListener(this);
        if (aMap == null)
        {
            aMap = mapView.Map;
            setUpMap();
        }
    }

    private void setUpMap()
    {
        aMap.SetOnMarkerDragListener(this); // 设置marker可拖拽事件监听器
        aMap.SetOnMapLoadedListener(this); // 设置amap加载成功事件监听器
        aMap.SetOnMarkerClickListener(this); // 设置点击marker事件监听器
        aMap.SetOnInfoWindowClickListener(this); // 设置点击infoWindow事件监听器
        aMap.SetInfoWindowAdapter(this); // 设置自定义InfoWindow样式
        addMarkersToMap(); // 往地图上添加marker
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
	 * 在地图上添加marker
	 */

    private void addMarkersToMap()
    {
        aMap.AddMarker(new MarkerOptions().Anchor(0.5f, 0.5f)
            .SetPosition(Constants.CHENGDU).SetTitle("成都市")
            .SetSnippet("成都市:30.679879, 104.064855").Draggable(true));

        markerOption = new MarkerOptions();
        markerOption.SetPosition(Constants.XIAN);
        markerOption.SetTitle("西安市").SetSnippet("西安市：34.341568, 108.940174");
        markerOption.Draggable(true);
        markerOption.SetIcon(BitmapDescriptorFactory
            .FromResource(Resource.Drawable.arrow));
        marker2 = aMap.AddMarker(markerOption);
        drawMarkers(); // 添加10个带有系统默认icon的marker
    }

    /**
	 * 绘制系统默认的1种marker背景图片
	 */

    public void drawMarkers()
    {
        Marker marker = aMap.AddMarker(new MarkerOptions()
            .SetPosition(latlng)
            .SetTitle("好好学习")
            .SetIcon(BitmapDescriptorFactory
                .DefaultMarker(BitmapDescriptorFactory.HueAzure))
            .Draggable(true));
        marker.ShowInfoWindow(); // 设置默认显示一个infowinfow
    }

    /**
	 * 对marker标注点点击响应事件
	 */

    public bool OnMarkerClick( Marker marker)
    {
        if (marker.Equals(marker2))
        {
            if (aMap != null)
            {
                jumpPoint(marker);
            }
        }
        markerText.Text=("你点击的是" + marker.Title);
        return false;
    }

    /**
	 * marker点击时跳动一下
	 */

    public void jumpPoint( Marker marker)
    {
        Handler handler = new Handler();
       
        long start = SystemClock.UptimeMillis();
        Projection proj = aMap.Projection;
        Point startPoint = proj.ToScreenLocation(Constants.XIAN);
        startPoint.Offset(0, -100);
       
        LatLng startLatLng = proj.FromScreenLocation(startPoint);
       
        long duration = 1500;

        var interpolator = new BounceInterpolator();
      
    long elapsed = SystemClock.UptimeMillis() - start;
    float t = interpolator.GetInterpolation((float) elapsed
    / duration);
    double lng = t * Constants.XIAN.Longitude + (1 - t)
    * startLatLng.Longitude;
    double lat = t * Constants.XIAN.Latitude + (1 - t)
    * startLatLng.Latitude;
    marker.Position=(new LatLng(lat,
            lng));
    aMap.Invalidate(); // 刷新地图
    if (t < 1.0) {
        /*
    handler.postDelayed(this,
            16);*/
        }
   

    }

    /**
	 * 监听点击infowindow窗口事件回调
	 */

    public void OnInfoWindowClick(Marker marker)
    {
        ToastUtil.show(this, "你点击了infoWindow窗口" + marker.Title);
    }

    /**
	 * 监听拖动marker时事件回调
	 */
    public void OnMarkerDrag(Marker marker)
    {
        string curDes = marker.Title + "拖动时当前位置:(lat,lng)\n("
                        + marker.Position.Latitude + ","
                        + marker.Position.Longitude + ")";
        markerText.Text=(curDes);
    }

    /**
	 * 监听拖动marker结束事件回调
	 */

    public void OnMarkerDragEnd(Marker marker)
    {
        markerText.Text=(marker.Title + "停止拖动");
    }

    /**
	 * 监听开始拖动marker事件回调
	 */

    public void OnMarkerDragStart(Marker marker)
    {
        markerText.Text=(marker.Title + "开始拖动");
    }

    /**
	 * 监听amap地图加载成功事件回调
	 */

    public void OnMapLoaded()
    {
        // 设置所有maker显示在当前可视区域地图中
        LatLngBounds bounds = new LatLngBounds.Builder()
            .Include(Constants.XIAN).Include(Constants.CHENGDU)
            .Include(latlng).Build();
        aMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 10));
    }

    /**
	 * 监听自定义infowindow窗口的infocontents事件回调
	 */

    public View GetInfoContents(Marker marker)
    {
        if (radioOption.CheckedRadioButtonId != Resource.Id.custom_info_contents)
        {
            return null;
        }
        View infoContent = LayoutInflater.Inflate(
            Resource.Layout.custom_info_contents, null);
        Render(marker, infoContent);
        return infoContent;
    }

    /**
	 * 监听自定义infowindow窗口的infowindow事件回调
	 */

    public View GetInfoWindow(Marker marker)
    {
        if (radioOption.CheckedRadioButtonId != Resource.Id.custom_info_window)
        {
            return null;
        }
        View infoWindow = LayoutInflater.Inflate(
            Resource.Layout.custom_info_window, null);

        Render(marker, infoWindow);
        return infoWindow;
    }

    /**
	 * 自定义infowinfow窗口
	 */

    public void Render(Marker marker, View view)
    {
        if (radioOption.CheckedRadioButtonId == Resource.Id.custom_info_contents)
        {
            ((ImageView) view.FindViewById(Resource.Id.badge))
                .SetImageResource(Resource.Drawable.badge_sa);
        }
        else if (radioOption.CheckedRadioButtonId == Resource.Id.custom_info_window)
        {
            ImageView imageView = (ImageView) view.FindViewById(Resource.Id.badge);
            imageView.SetImageResource(Resource.Drawable.badge_wa);
        }
        string title = marker.Title;
        TextView titleUi = ((TextView) view.FindViewById(Resource.Id.title));
        if (title != null)
        {
            SpannableString titleText = new SpannableString(title);
            titleText.SetSpan(new ForegroundColorSpan(Color.Red), 0,
                titleText.Length(), 0);
            titleUi.TextSize=(15);
            titleUi.SetText(titleText,TextView.BufferType.Normal);

        }
        else
        {
            titleUi.SetText("",TextView.BufferType.Normal);
        }
        string snippet = marker.Snippet;
        TextView snippetUi = ((TextView) view.FindViewById(Resource.Id.snippet));
        if (snippet != null)
        {
            SpannableString snippetText = new SpannableString(snippet);
            snippetText.SetSpan(new ForegroundColorSpan(Color.Green), 0,
                snippetText.Length(), 0);
            snippetUi.SetTextSize(ComplexUnitType.Dip, 20);
            snippetUi.SetText(snippetText,TextView.BufferType.Normal);
        }
        else
        {
            snippetUi.SetText("",TextView.BufferType.Normal);
        }
    }

    public void OnClick(View v)
    {
        switch (v.Id)
        {
                /**
		 * 清空地图上所有已经标注的marker
		 */
            case Resource.Id.clearMap:
                if (aMap != null)
                {
                    aMap.Clear();
                }
                break;
                /**
		 * 重新标注所有的marker
		 */
            case Resource.Id.resetMap:
                if (aMap != null)
                {
                    aMap.Clear();
                    addMarkersToMap();
                }
                break;
            default:
                break;
        }
    }

}
}