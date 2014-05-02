
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using JuzzPig.AMap.AndroidDemo;
using JuzzPig.AMap.Demo;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{
    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    /**
     * AMapV1地图中简单介绍一些Camera的用法.
     */
    public class CameraActivity : Activity, View.IOnClickListener,
            Com.Amap.Api.Maps2d.AMap.ICancelableCallback
    {
        private const int SCROLL_BY_PX = 100;
        private MapView mapView;
        private Com.Amap.Api.Maps2d.AMap aMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.camera_activity);
            mapView = FindViewById < MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);// 此方法必须重写
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
            Button stopAnimation = (Button)FindViewById(Resource.Id.stop_animation);
            stopAnimation.SetOnClickListener(this);
            ToggleButton animate = (ToggleButton)FindViewById(Resource.Id.animate);
            animate.SetOnClickListener(this);
            Button Lujiazui = (Button)FindViewById(Resource.Id.Lujiazui);
            Lujiazui.SetOnClickListener(this);
            Button Zhongguancun = (Button)FindViewById(Resource.Id.Zhongguancun);
            Zhongguancun.SetOnClickListener(this);
            Button scrollLeft = (Button)FindViewById(Resource.Id.scroll_left);
            scrollLeft.SetOnClickListener(this);
            Button scrollRight = (Button)FindViewById(Resource.Id.scroll_right);
            scrollRight.SetOnClickListener(this);
            Button scrollUp = (Button)FindViewById(Resource.Id.scroll_up);
            scrollUp.SetOnClickListener(this);
            Button scrollDown = (Button)FindViewById(Resource.Id.scroll_down);
            scrollDown.SetOnClickListener(this);
            Button zoomIn = (Button)FindViewById(Resource.Id.zoom_in);
            zoomIn.SetOnClickListener(this);
            Button zoomOut = (Button)FindViewById(Resource.Id.zoom_out);
            zoomOut.SetOnClickListener(this);
        }

        /**
         * 往地图上添加一个marker
         */
        private void setUpMap()
        {
            /*
            aMap.addMarker(new MarkerOptions().position(Constants.FANGHENG).icon(
                    BitmapDescriptorFactory
                            .defaultMarker(BitmapDescriptorFactory.HUE_RED)));
             * */
            /*
            var option = new MarkerOptions();
            option.Position = Constants.FANGHENG;
            option.Position(Constants.FANGHENG).icon(
                BitmapDescriptorFactory
                    .DefaultMarker(BitmapDescriptorFactory.HueRed));*/

            aMap.AddMarker(
                new MarkerOptions().SetPosition(Constants.FANGHENG).SetIcon(
                    BitmapDescriptorFactory
                            .DefaultMarker(BitmapDescriptorFactory.HueRed))
                );
        }

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
         * 根据动画按钮状态，调用函数animateCamera或moveCamera来改变可视区域
         */
        private void changeCamera(CameraUpdate update, Com.Amap.Api.Maps2d.AMap.ICancelableCallback callback)
        {
            bool animated = ((CompoundButton)FindViewById(Resource.Id.animate)).Checked;
            if (animated)
            {
                aMap.AnimateCamera(update, 1000, callback);
            }
            else
            {
                aMap.MoveCamera(update);
            }
        }


        public void OnClick(View v)
        {
            switch (v.Id)
            {
                /**
                 * 点击停止动画按钮响应事件
                 */
                case Resource.Id.stop_animation:
                    aMap.StopAnimation();
                    break;
                /**
                 * 点击“去中关村”按钮响应事件
                 */
                case Resource.Id.Zhongguancun:
                    changeCamera(
                            CameraUpdateFactory.NewCameraPosition(new CameraPosition(
                                    Constants.ZHONGGUANCUN, 18, 0, 30)), null);
                    break;

                /**
                 * 点击“去陆家嘴”按钮响应事件
                 */
                case Resource.Id.Lujiazui:
                    changeCamera(
                            CameraUpdateFactory.NewCameraPosition(new CameraPosition(
                                    Constants.SHANGHAI, 18, 30, 0)), this);
                    break;
                /**
                 * 点击向左移动按钮响应事件，camera将向左边移动
                 */
                case Resource.Id.scroll_left:
                    changeCamera(CameraUpdateFactory.ScrollBy(-SCROLL_BY_PX, 0), null);
                    break;
                /**
                 * 点击向右移动按钮响应事件，camera将向右边移动
                 */
                case Resource.Id.scroll_right:
                    changeCamera(CameraUpdateFactory.ScrollBy(SCROLL_BY_PX, 0), null);
                    break;
                /**
                 * 点击向上移动按钮响应事件，camera将向上边移动
                 */
                case Resource.Id.scroll_up:
                    changeCamera(CameraUpdateFactory.ScrollBy(0, -SCROLL_BY_PX), null);
                    break;
                /**
                 * 点击向下移动按钮响应事件，camera将向下边移动
                 */
                case Resource.Id.scroll_down:
                    changeCamera(CameraUpdateFactory.ScrollBy(0, SCROLL_BY_PX), null);
                    break;
                /**
                 * 点击地图放大按钮响应事件
                 */
                case Resource.Id.zoom_in:
                    changeCamera(CameraUpdateFactory.ZoomIn(), null);
                    break;
                /**
                 * 点击地图缩小按钮响应事件
                 */
                case Resource.Id.zoom_out:
                    changeCamera(CameraUpdateFactory.ZoomOut(), null);
                    break;
                default:
                    break;
            }
        }

        /**
         * 地图动画效果终止回调方法
         */
        public void OnCancel()
        {
            ToastUtil.show(this, "Animation to 陆家嘴 canceled");
        }

        /**
         * 地图动画效果完成回调方法
         */

        public void OnFinish()
        {
            ToastUtil.show(this, "Animation to 陆家嘴 complete");
        }
    }
}