
using Android.App;
using Android.OS;
using Com.Amap.Api.Maps2d;
using JuzzPig.AMap.AndroidDemo;

namespace JuzzPig.AMap.AndroidDemo
{
    [Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class BasicMapActivity : Activity
    {
        private MapView mapView;
        private Com.Amap.Api.Maps2d.AMap aMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.basicmap_activity);
            mapView = FindViewById < MapView>(Resource.Id.map);
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
            }
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

    }
}