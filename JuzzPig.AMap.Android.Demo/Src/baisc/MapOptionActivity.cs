using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using JuzzPig.AMap.Demo.util;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace JuzzPig.AMap.AndroidDemo
{

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    /**
     * 通过Java代码添加一个SupportMapFragment对象
     */
    public class MapOptionActivity : FragmentActivity
    {

        private static readonly string MAP_FRAGMENT_TAG = "map";
        static readonly CameraPosition LUJIAZUI = new CameraPosition.Builder()
                .Target(Constants.SHANGHAI).Zoom(18).Bearing(0).Tilt(30).Build();
        private Com.Amap.Api.Maps2d.AMap aMap;
        private Com.Amap.Api.Maps2d.SupportMapFragment aMapFragment;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            init();
        }

        protected override void OnResume()
        {
            base.OnResume();
            initMap();
        }

        /**
         * 初始化AMap对象
         */
        private void init()
        {
            AMapOptions aOptiOns = new AMapOptions();
            aOptiOns.SetZoomGesturesEnabled((false));// 禁止通过手势缩放地图
            aOptiOns.SetScrollGesturesEnabled(false);// 禁止通过手势移动地图
            aOptiOns.SetCamera(LUJIAZUI);
            if (aMapFragment == null)
            {
                aMapFragment = SupportMapFragment.NewInstance(aOptiOns);
                FragmentTransaction fragmentTransactiOn = SupportFragmentManager
                        .BeginTransaction();
                fragmentTransactiOn.Add(global:: Android.Resource.Id.Content, aMapFragment,
                        MAP_FRAGMENT_TAG);
                fragmentTransactiOn.Commit();
            }

        }

        /**
         * 初始化AMap对象
         */
        private void initMap()
        {
            if (aMap == null)
            {
                aMap = aMapFragment.Map;// amap对象初始化成功
            }
        }
    }
}