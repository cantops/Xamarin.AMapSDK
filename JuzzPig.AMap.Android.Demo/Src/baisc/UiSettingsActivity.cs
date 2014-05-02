using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Location;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Java.IO;
using Java.Lang;
using Java.Text;
using Java.Util;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{

    /**
     * UI settings一些选项设置响应事件
     */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class UiSettingsActivity : Activity,
    RadioGroup.IOnCheckedChangeListener, View.IOnClickListener, ILocationSource,
    IAMapLocationListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;
        private UiSettings mUiSettings;
        private ILocationSourceOnLocationChangedListener mListener;
        private LocationManagerProxy aMapManager;

        protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ui_settings_activity);
            mapView = (MapView)FindViewById(Resource.Id.map);
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
                mUiSettings = aMap.UiSettings;
            }
            Button buttOnScale = (Button)FindViewById(Resource.Id.buttonScale);
            buttOnScale.SetOnClickListener(this);
            CheckBox scaleToggle = (CheckBox)FindViewById(Resource.Id.scale_toggle);
            scaleToggle.SetOnClickListener(this);
            CheckBox zoomToggle = (CheckBox)FindViewById(Resource.Id.zoom_toggle);
            zoomToggle.SetOnClickListener(this);
            CheckBox compassToggle = (CheckBox)FindViewById(Resource.Id.compass_toggle);
            compassToggle.SetOnClickListener(this);
            CheckBox mylocatiOnToggle = (CheckBox)FindViewById(Resource.Id.mylocation_toggle);
            mylocatiOnToggle.SetOnClickListener(this);
            CheckBox scrollToggle = (CheckBox)FindViewById(Resource.Id.scroll_toggle);
            scrollToggle.SetOnClickListener(this);
            CheckBox zoom_gesturesToggle = (CheckBox)FindViewById(Resource.Id.zoom_gestures_toggle);
            zoom_gesturesToggle.SetOnClickListener(this);
            RadioGroup radioGroup = (RadioGroup)FindViewById(Resource.Id.logo_position);
            radioGroup.SetOnCheckedChangeListener(this);

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
         * 设置logo位置，左下，底部居中，右下
         */

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            if (aMap != null)
            {
                if (checkedId == Resource.Id.bottom_left)
                {
                    mUiSettings
                        .LogoPosition = (AMapOptions.LogoPositionBottomLeft); // 设置地图logo显示在左下方
                }
                else if (checkedId == Resource.Id.bottom_center)
                {
                    mUiSettings
                        .LogoPosition = (AMapOptions.LogoPositionBottomCenter); // 设置地图logo显示在底部居中
                }
                else if (checkedId == Resource.Id.bottom_right)
                {
                    mUiSettings
                        .LogoPosition = (AMapOptions.LogoPositionBottomRight); // 设置地图logo显示在右下方
                }
            }

        }

        public void OnClick(View view)
        {
            switch (view.Id)
            {
                /**
		 * 一像素代表多少米
		 */
                case Resource.Id.buttonScale:
                    float scale = aMap.ScalePerPixel;
                    ToastUtil.show(
                    this,
                    "每像素代表" + scale + "米")
                    ;
                    break;
                /**
		 * 设置地图默认的比例尺是否显示
		 */
                case Resource.Id.scale_toggle:
                    mUiSettings.ScaleControlsEnabled = (((CheckBox)view).Checked);

                    break;
                /**
		 * 设置地图默认的缩放按钮是否显示
		 */
                case Resource.Id.zoom_toggle:
                    mUiSettings.ZoomControlsEnabled = (((CheckBox)view).Checked);
                    break;
                /**
		 * 设置地图默认的指南针是否显示
		 */
                case Resource.Id.compass_toggle:
                    mUiSettings.CompassEnabled = (((CheckBox)view).Checked);
                    break;
                /**
		 * 设置地图默认的定位按钮是否显示
		 */
                case Resource.Id.mylocation_toggle:
                    aMap.SetLocationSource(this); // 设置定位监听
                    mUiSettings.MyLocationButtonEnabled = (((CheckBox)view)
                        .Checked); // 是否显示默认的定位按钮
                    aMap.MyLocationEnabled = (((CheckBox)view).Checked); // 是否可触发定位并显示定位层
                    break;
                /**
		 * 设置地图是否可以手势滑动
		 */
                case Resource.Id.scroll_toggle:
                    mUiSettings.ScrollGesturesEnabled = (((CheckBox)view).Checked);
                    break;
                /**
		 * 设置地图是否可以手势缩放大小
		 */
                case Resource.Id.zoom_gestures_toggle:
                    mUiSettings.ZoomGesturesEnabled = (((CheckBox)view).Checked);
                    break;
                default:
                    break;
            }
        }





        /**
         * 激活定位
         */


        public void Activate(ILocationSourceOnLocationChangedListener listener)
        {
            mListener = listener;
            if (aMapManager == null)
            {
                aMapManager = LocationManagerProxy.GetInstance(this);
                /*
                 * mAMapLocManager.setGpsEnable(false);//
                 * 1.0.2版本新增方法，设置true表示混合定位中包含gps定位，false表示纯网络定位，默认是true
                 */
                // LocatiOn API定位采用GPS和网络混合定位方式，时间最短是2000毫秒
                aMapManager.RequestLocationUpdates(
                    LocationProviderProxy.AMapNetwork, 2000, 10, this);
            }
        }

        public void Deactivate()
        {
            mListener = null;
            if (aMapManager != null)
            {
                aMapManager.RemoveUpdates(this);
                aMapManager.Destory();
            }
            aMapManager = null;
        }

        /**
	 * 停止定位
	 */



        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }


        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        public void OnLocationChanged(AMapLocation p0)
        {
            if (mListener != null)
            {
                mListener.OnLocationChanged(p0); // 显示系统小蓝点
            }
        }

        /**
 * 定位成功后回调函数
 */
        public void OnLocationChanged(Location location)
        {

        }
    }
}