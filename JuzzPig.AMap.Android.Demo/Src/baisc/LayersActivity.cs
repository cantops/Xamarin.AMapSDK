using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Java.Lang;

namespace JuzzPig.AMap.AndroidDemo
{

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    /**
     * AMapV1地图中简单介绍栅格地图和卫星地图模式切换
     */
    public class LayersActivity : Activity,
            View.IOnClickListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private MapView mapView;

        protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layers_activity);
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
            }
            CheckBox traffic = (CheckBox)FindViewById(Resource.Id.traffic);
            traffic.SetOnClickListener(this);
            Spinner spinner = (Spinner)FindViewById(Resource.Id.layers_spinner);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.layers_array,
                global::	Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(global::	Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = (adapter);
            spinner.ItemSelected += (sender, args) =>
            {

                if (aMap != null)
                {
                    setLayer((String)args.Parent.GetItemAtPosition(args.Position));
                }
            };
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
         * 选择矢量地图和卫星地图事件的响应
         */
        private void setLayer(String layerName)
        {
            if (layerName.Equals(GetString(Resource.String.normal)))
            {
                aMap.MapType = (Com.Amap.Api.Maps2d.AMap.MapTypeNormal);// 矢量地图模式
            }
            else if (layerName.Equals(GetString(Resource.String.satellite)))
            {
                aMap.MapType = (Com.Amap.Api.Maps2d.AMap.MapTypeSatellite);// 卫星地图模式
            }
        }

        public void OnNothingSelected(AdapterView parent)
        {
        }

        /**
         * 对选择是否显示交通状况事件的响应
         */
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.traffic)
            {
                aMap.TrafficEnabled = (((CheckBox)v).Checked);// 显示实时交通状况
            }
        }
    }

}