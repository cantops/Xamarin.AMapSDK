
/**
 * AMapV1地图中简单介绍OnMapClickListener, OnMapLongClickListener,
 * OnCameraChangeListener三种监听器用法
 */

using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{
    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class EventsActivity : Activity , Com.Amap.Api.Maps2d.AMap.IOnMapClickListener, Com.Amap.Api.Maps2d.AMap.IOnMapLongClickListener, Com.Amap.Api.Maps2d.AMap.IOnCameraChangeListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;
        private TextView mTapTextView;
        private TextView mCameraTextView;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.events_activity);
            mapView = (MapView) FindViewById(Resource.Id.map);
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
            mTapTextView = (TextView) FindViewById(Resource.Id.tap_text);
            mCameraTextView = (TextView) FindViewById(Resource.Id.camera_text);
        }

        /**
	 * amap添加一些事件监听器
	 */

        private void setUpMap()
        {
            aMap.SetOnMapClickListener(this); // 对amap添加单击地图事件监听器
            aMap.SetOnMapLongClickListener(this); // 对amap添加长按地图事件监听器
            aMap.SetOnCameraChangeListener(this); // 对amap添加移动地图事件监听器
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
	 * 对单击地图事件回调
	 */


        public void OnMapClick(LatLng point)
        {
            mTapTextView.SetText(Convert.ToInt32("tapped, point=" + point));
        }

        /**
	 * 对长按地图事件回调
	 */

        public void OnMapLongClick(LatLng point)
        {
            mTapTextView.SetText(Convert.ToInt32("long pressed, point=" + point));
        }

        /**
	 * 对正在移动地图事件回调
	 */

        public void OnCameraChange(CameraPosition cameraPosition)
        {
            mCameraTextView.SetText(Convert.ToInt32("onCameraChange:" + cameraPosition.ToString()));
        }

        /**
	 * 对移动地图结束事件回调
	 */

        public void OnCameraChangeFinish(CameraPosition cameraPosition)
        {
            mCameraTextView.SetText(Convert.ToInt32("onCameraChangeFinish:"
                                                    + cameraPosition.ToString()));
            VisibleRegion visibleRegion = aMap.Projection.VisibleRegion; // 获取可视区域、

            LatLngBounds latLngBounds = visibleRegion.LatLngBounds; // 获取可视区域的Bounds
            bool isContain = latLngBounds.Contains(Constants.SHANGHAI); // 判断上海经纬度是否包括在当前地图可见区域
            if (isContain)
            {
                ToastUtil.show(
                    this,
                    "上海市在地图当前可见区域内")
                    ;
            }
            else
            {
                ToastUtil.show(
                    this,
                    "上海市超出地图当前可见区域")
                    ;
            }
        }
    }
}