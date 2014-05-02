
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
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
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{


/**
 * AMapV1地图中简单介绍GPS定位
 */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]

    public class LocationGPSActivity: Activity,
    IAMapLocationListener
{
    private LocationManagerProxy locationManager;
    private TextView myLocation;


    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.locationnetwork_activity);
        init();
    }

    private void init()
    {
        myLocation = (TextView) FindViewById(Resource.Id.myLocation);
        locationManager = LocationManagerProxy
            .GetInstance(
        this)
        ;
        // API定位采用GPS定位方式，第一个参数是定位provider，第二个参数时间最短是2000毫秒，第三个参数距离间隔单位是米，第四个参数是定位监听者
        locationManager.RequestLocationUpdates(
            LocationManagerProxy.GpsProvider, 2000, 10, this);
    }

    protected override void OnPause()
    {
        base.OnPause();
        if (locationManager != null)
        {
            locationManager.RemoveUpdates(this);
            locationManager.Destory();
        }
        locationManager = null;
    }


    protected override void OnDestroy()
    {
        if (locationManager != null)
        {
            locationManager.RemoveUpdates(this);
            locationManager.Destory();
        }
        locationManager = null;
        base.OnDestroy();
    }

    /**
	 * 此方法已经废弃
	 */

    public void OnLocationChanged(Location location)
    {

    }


    public void OnProviderDisabled(string provider)
    {

    }

    public void OnProviderEnabled(string provider)
    {

    }

    public void OnStatusChanged(string provider, Availability status, Bundle extras)
    {

    }

    /**
	 * gps定位回调方法
	 */

    public void OnLocationChanged(AMapLocation location)
    {
        if (location != null)
        {
            var geoLat = location.Latitude;
            var geoLng = location.Longitude;
            string str = ("定位成功:(" + geoLng + "," + geoLat + ")"
                          + "\n精    度    :" + location.Accuracy + "米"
                          + "\n定位方式:" + location.Provider + "\n定位时间:" + AMapUtil
                              .convertToTime(location.Time));
            myLocation.Text=(str);
        }
    }
}
}