
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Busline;
using Com.Amap.Api.Services.Core;
using Com.Amap.Api.Services.Geocoder;
using Java.Lang;
using JuzzPig.AMap.AndroidDemo;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{
    /**
     * 地理编码与逆地理编码功能介绍
     */

[Activity(Label = "JuzzPig.AMap.Android.Demo",  Icon = "@drawable/icon")]
    public class GeocoderActivity: Activity, GeocodeSearch.IOnGeocodeSearchListener, View.IOnClickListener
    {
        private ProgressDialog progDialog = null;
        private GeocodeSearch geocoderSearch;
        private string addressName;
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;
        private Marker geoMarker;
        private Marker regeoMarker;
        private LatLonPoint latLOnPoint = new LatLonPoint(40.003662, 116.465271);


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.geocoder_activity);
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
                geoMarker = aMap.AddMarker(new MarkerOptions().Anchor(0.5f, 0.5f)
                    .SetIcon(BitmapDescriptorFactory
                        .DefaultMarker(BitmapDescriptorFactory.HueBlue)));
                regeoMarker = aMap.AddMarker(new MarkerOptions().Anchor(0.5f, 0.5f)
                    .SetIcon(BitmapDescriptorFactory
                        .DefaultMarker(BitmapDescriptorFactory.HueRed)));
            }
            Button geoButton = (Button)FindViewById(Resource.Id.geoButton);
            geoButton.SetOnClickListener(this);
            Button regeoButton = (Button)FindViewById(Resource.Id.regeoButton);
            regeoButton.SetOnClickListener(this);
            geocoderSearch = new GeocodeSearch(this);
            geocoderSearch.SetOnGeocodeSearchListener(this);
            progDialog = new ProgressDialog(this);

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
         * 显示进度条对话框
         */

        public void showDialog()
        {
            progDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progDialog.Indeterminate = (false);
            progDialog.SetCancelable(true);
            progDialog.SetMessage("正在获取地址");
            progDialog.Show();
        }

        /**
         * 隐藏进度条对话框
         */

        public void dismissDialog()
        {
            if (progDialog != null)
            {
                progDialog.Dismiss();
            }
        }

        /**
         * 响应地理编码
         */

        public void getLatlOn(string name)
        {
            showDialog();
            GeocodeQuery query = new GeocodeQuery(name, "010"); // 第一个参数表示地址，第二个参数表示查询城市，中文或者中文全拼，citycode、adcode，
            geocoderSearch.GetFromLocationNameAsyn(query); // 设置同步地理编码请求
        }

        /**
         * 响应逆地理编码
         */

        public void getAddress(LatLonPoint latLOnPoint)
        {
            showDialog();
            RegeocodeQuery query = new RegeocodeQuery(latLOnPoint, 200,
                GeocodeSearch.Amap); // 第一个参数表示一个Latlng，第二参数表示范围多少米，第三个参数表示是火系坐标系还是GPS原生坐标系
            geocoderSearch.GetFromLocationAsyn(query); // 设置同步逆地理编码请求
        }

        /**
         * 地理编码查询回调
         */

        public void OnGeocodeSearched(GeocodeResult result, int rCode)
        {
            dismissDialog();
            if (rCode == 0)
            {
                if (result != null && result.GeocodeAddressList != null
                    && result.GeocodeAddressList.Count > 0)
                {
                    GeocodeAddress address = result.GeocodeAddressList[0];
                    aMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(
                        AMapUtil.convertToLatLng(address.LatLonPoint), 15));
                    geoMarker.Position = (AMapUtil.convertToLatLng(address.LatLonPoint));
                    addressName = "经纬度值:" + address.LatLonPoint + "\n位置描述:" + address.FormatAddress;
                    ToastUtil.show(this, addressName);
                }
                else
                {
                    ToastUtil.show(
                    this, Resource.String.no_result)
                    ;
                }

            }
            else if (rCode == 27)
            {
                ToastUtil.show(
                this, Resource.String.error_network)
                ;
            }
            else if (rCode == 32)
            {
                ToastUtil.show(
                this, Resource.String.error_key)
                ;
            }
            else
            {
                ToastUtil.show(
                this,
                GetString(Resource.String.error_other)
                + rCode)
                ;
            }
        }

        /**
         * 逆地理编码回调
         */

        public void OnRegeocodeSearched(RegeocodeResult result, int rCode)
        {
            dismissDialog();
            if (rCode == 0)
            {
                if (result != null && result.RegeocodeAddress != null
                    && result.RegeocodeAddress.FormatAddress != null)
                {
                    addressName = result.RegeocodeAddress.FormatAddress
                                  + "附近";
                    aMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(
                        AMapUtil.convertToLatLng(latLOnPoint), 15));
                    regeoMarker.Position = (AMapUtil.convertToLatLng(latLOnPoint));
                    ToastUtil.show(
                    this,
                    addressName)
                    ;
                }
                else
                {
                    ToastUtil.show(
                    this, Resource.String.no_result)
                    ;
                }
            }
            else if (rCode == 27)
            {
                ToastUtil.show(
                this, Resource.String.error_network)
                ;
            }
            else if (rCode == 32)
            {
                ToastUtil.show(
                this, Resource.String.error_key)
                ;
            }
            else
            {
                ToastUtil.show(
                this,
                GetString(Resource.String.error_other)
                + rCode)
                ;
            }
        }


        public void OnClick(View v)
        {
            switch (v.Id)
            {
                /**
		 * 响应地理编码按钮
		 */
                case Resource.Id.geoButton:
                    getLatlOn("方恒国际中心");
                    break;
                /**
		 * 响应逆地理编码按钮
		 */
                case Resource.Id.regeoButton:
                    getAddress(latLOnPoint);
                    break;
                default:
                    break;
            }

        }
    }
}