using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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
 * AMapV1地图中对截屏简单介绍
 */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class ScreenShotActivity : Activity, Com.Amap.Api.Maps2d.AMap.IOnMapScreenShotListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.screenshot_activity);
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
        }

        /**
	 * 对地图添加一个marker
	 */

        private void setUpMap()
        {
            aMap.AddMarker(new MarkerOptions().SetPosition(Constants.FANGHENG)
                .SetTitle("方恒").SetSnippet("方恒国际中心大楼A座"));
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
	 * 对地图进行截屏
	 */

        public void getMapScreenShot(View v)
        {
            aMap.GetMapScreenShot(this);
            aMap.Invalidate(); // 刷新地图
        }

        public void OnMapScreenShot(Bitmap bitmap)
        {
            /*截图功能代码有问题，赞没实现
            SimpleDateFormat sdf = new SimpleDateFormat("yyyyMMddHHmmss");
            try
            {
                FileOutputStream fos = new FileOutputStream( Environment.ExternalStorageDirectory + "/test_"
                    + sdf.Format(new Date()) + ".png");
                bool b = bitmap.Compress(Bitmap.CompressFormat.Png, 100, fos);
                try
                {
                    fos.Flush();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
                try
                {
                    fos.Close();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
                if (b)
                    ToastUtil.show(this, "截屏成功");
                else
                {
                    ToastUtil.show(this, "截屏失败");
                }
            }
            catch (FileNotFoundException e)
            {
                e.PrintStackTrace();
            }*/
        }
    }
}