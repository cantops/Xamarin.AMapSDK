
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


[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    /**
     * AMapV1地图中简单介绍显示定位小蓝点
     */

    public class LocationSourceActivity : Activity, ILocationSource,
    IAMapLocationListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;
        private ILocationSourceOnLocationChangedListener mListener;
        private LocationManagerProxy mAMapLocationManager;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.locationsource_activity);
            mapView = FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState); // 此方法必须重写
            init();
        }

        /**
         * 初始化AMap对象
         */

        private void init()
        {
            if (aMap == null)
            {
                aMap = mapView.Map;
                setUpMap();
            }
        }

        /**
         * 设置一些amap的属性
         */

        private void setUpMap()
        {
            // 自定义系统定位小蓝点
            MyLocationStyle myLocationStyle = new MyLocationStyle();
            myLocationStyle.SetMyLocationIcon(BitmapDescriptorFactory
                .FromResource(Resource.Drawable.location_marker)); // 设置小蓝点的图标
            myLocationStyle.SetStrokeColor(Color.Black); // 设置圆形的边框颜色
            myLocationStyle.SetRadiusFillColor(Color.Argb(100, 0, 0, 180)); // 设置圆形的填充颜色
            // myLocationStyle.anchor(int,int)//设置小蓝点的锚点
            myLocationStyle.SetStrokeWidth(1.0f); // 设置圆形的边框粗细
            aMap.SetMyLocationStyle(myLocationStyle);
            aMap.SetLocationSource(this); // 设置定位监听
            aMap.UiSettings.MyLocationButtonEnabled = (true); // 设置默认定位按钮是否显示
            aMap.MyLocationEnabled = (true); // 设置为true表示显示定位层并可触发定位，false表示隐藏定位层并不可触发定位，默认是false
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
            Deactivate();
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
         * 定位成功后回调函数
         */

        public void OnLocationChanged(AMapLocation aLocation)
        {
            if (mListener != null && aLocation != null)
            {
                mListener.OnLocationChanged(aLocation); // 显示系统小蓝点
            }
        }

        /**
         * 激活定位
         */
        public void Activate(ILocationSourceOnLocationChangedListener listener)
        {
            mListener = listener;
            if (mAMapLocationManager == null)
            {
                mAMapLocationManager = LocationManagerProxy.GetInstance(this);
                /*
                 * mAMapLocManager.setGpsEnable(false);
                 * 1.0.2版本新增方法，设置true表示混合定位中包含gps定位，false表示纯网络定位，默认是true Location
                 * API定位采用GPS和网络混合定位方式
                 * ，第一个参数是定位provider，第二个参数时间最短是2000毫秒，第三个参数距离间隔单位是米，第四个参数是定位监听者
                 */
                mAMapLocationManager.RequestLocationUpdates(
                    LocationProviderProxy.AMapNetwork, 2000, 10, this);
            }
        }

        /**
         * 停止定位
         */

        public void Deactivate()
        {
            mListener = null;
            if (mAMapLocationManager != null)
            {
                mAMapLocationManager.RemoveUpdates(this);
                mAMapLocationManager.Destory();
            }
            mAMapLocationManager = null;
        }


    }
}