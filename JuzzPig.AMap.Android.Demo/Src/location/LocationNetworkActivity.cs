
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
    

[Activity(Label = "JuzzPig.AMap.Android.Demo",  Icon = "@drawable/icon")]
/**
 * AMapV1中简单介绍混合定位
 */

    public class LocationNetworkActivity:Activity ,
    IAMapLocationListener, IRunnable
{
    private LocationManagerProxy aMapLocManager = null;
    private TextView myLocation;
    private AMapLocation aMapLocation; // 用于判断定位超时
    private Handler handler = new Handler();


    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.locationnetwork_activity);
        myLocation = (TextView) FindViewById(Resource.Id.myLocation);
        aMapLocManager = LocationManagerProxy.GetInstance(this);
        /*
		 * mAMapLocManager.setGpsEnable(false);//
		 * 1.0.2版本新增方法，设置true表示混合定位中包含gps定位，false表示纯网络定位，默认是true Location
		 * API定位采用GPS和网络混合定位方式
		 * ，第一个参数是定位provider，第二个参数时间最短是2000毫秒，第三个参数距离间隔单位是米，第四个参数是定位监听者
		 */
        aMapLocManager.RequestLocationUpdates(
            LocationProviderProxy.AMapNetwork, 2000, 10, this);
        handler.PostDelayed(this, 12000); // 设置超过12秒还没有定位到就停止定位
    }

    protected override void OnPause()
    {
        base.OnPause();
        stopLocation(); // 停止定位
    }

    /**
	 * 销毁定位
	 */

    private void stopLocation()
    {
        if (aMapLocManager != null)
        {
            aMapLocManager.RemoveUpdates(this);
            aMapLocManager.Destory();
        }
        aMapLocManager = null;
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
	 * 混合定位回调函数
	 */

    public void OnLocationChanged(AMapLocation location)
    {
        if (location != null)
        {
            this.aMapLocation = location; // 判断超时机制
            var geoLat = location.Latitude;
            var geoLng = location.Longitude;
            var cityCode = "";
            var desc = "";
            Bundle locBundle = location.Extras;
            if (locBundle != null)
            {
                cityCode = locBundle.GetString("citycode");
                desc = locBundle.GetString("desc");
            }
            string str = ("定位成功:(" + geoLng + "," + geoLat + ")"
                          + "\n精    度    :" + location.Accuracy + "米"
                          + "\n定位方式:" + location.Provider + "\n定位时间:"
                          + AMapUtil.convertToTime(location.Time) + "\n城市编码:"
                          + cityCode + "\n位置描述:" + desc + "\n省:"
                          + location.Province + "\n市:" + location.City
                          + "\n区(县):" + location.District + "\n区域编码:" + location
                              .AdCode);
            myLocation.Text=(str);
        }
    }


    public void Run()
    {
        if (aMapLocation == null)
        {
            ToastUtil.show(this, "12秒内还没有定位成功，停止定位");
            myLocation.Text=("12秒内还没有定位成功，停止定位");
            stopLocation(); // 销毁掉定位
        }
    }

}
}