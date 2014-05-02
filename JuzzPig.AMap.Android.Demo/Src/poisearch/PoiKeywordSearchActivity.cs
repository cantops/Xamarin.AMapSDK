
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Core;
using Com.Amap.Api.Services.Help;
using Com.Amap.Api.Services.Poisearch;
using Java.Lang;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{

/**
 * AMapV1地图中简单介绍poisearch搜索
 */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class PoiKeywordSearchActivity : FragmentActivity,
        Com.Amap.Api.Maps2d.AMap.IOnMarkerClickListener, Com.Amap.Api.Maps2d.AMap.IInfoWindowAdapter, ITextWatcher,
        PoiSearch.IOnPoiSearchListener, View.IOnClickListener
    {
        private Com.Amap.Api.Maps2d.AMap aMap;
        private AutoCompleteTextView searchText; // 输入搜索关键字
        private string keyWord = ""; // 要输入的poi搜索关键字
        private ProgressDialog progDialog = null; // 搜索时进度条
        private EditText editCity; // 要输入的城市名字或者城市区号
        private PoiResult poiResult; // poi返回的结果
        private int currentPage = 0; // 当前页面，从0开始计数
        private PoiSearch.Query query; // Poi查询条件类
        private PoiSearch poiSearch; // POI搜索


        protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.poikeywordsearch_activity);
            init();
        }

        /**
	 * 初始化AMap对象
	 */

        private void init()
        {
            if (aMap == null)
            {
                aMap = ((SupportMapFragment) SupportFragmentManager
                    .FindFragmentById(Resource.Id.map)).Map;
                setUpMap();
            }
        }

        /**
	 * 设置页面监听
	 */

        private void setUpMap()
        {
            Button searButton = (Button) FindViewById(Resource.Id.searchButton);
            searButton.SetOnClickListener(this);
            Button nextButton = (Button) FindViewById(Resource.Id.nextButton);
            nextButton.SetOnClickListener(this);
            searchText = (AutoCompleteTextView) FindViewById(Resource.Id.keyWord);
            searchText.AddTextChangedListener(this); // 添加文本输入框监听事件
            editCity = (EditText) FindViewById(Resource.Id.city);
            aMap.SetOnMarkerClickListener(this); // 添加点击marker监听事件
            aMap.SetInfoWindowAdapter(this); // 添加显示infowindow监听事件
        }

        /**
	 * 点击搜索按钮
	 */

        public void searchButton()
        {
            keyWord = AMapUtil.checkEditText(searchText);
            if ("".Equals(keyWord))
            {
                ToastUtil.show(
                this,
                "请输入搜索关键字")
                ;
                return;
            }
            else
            {
                doSearchQuery();
            }
        }

        /**
	 * 点击下一页按钮
	 */

        public void nextButton()
        {
            if (query != null && poiSearch != null && poiResult != null)
            {
                if (poiResult.PageCount - 1 > currentPage)
                {
                    currentPage++;
                    query.PageNum = (currentPage); // 设置查后一页
                    poiSearch.SearchPOIAsyn();
                }
                else
                {
                    ToastUtil.show(
                    this,Resource.String.no_result)
                    ;
                }
            }
        }

        /**
	 * 显示进度框
	 */

        private void showProgressDialog()
        {
            if (progDialog == null)
                progDialog = new ProgressDialog(this);
            progDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progDialog.Indeterminate=(false);
            progDialog.SetCancelable(false);
            progDialog.SetMessage("正在搜索:\n" + keyWord);
            progDialog.Show();
        }

        /**
	 * 隐藏进度框
	 */

        private void dissmissProgressDialog()
        {
            if (progDialog != null)
            {
                progDialog.Dismiss();
            }
        }

        /**
	 * 开始进行poi搜索
	 */

        protected void doSearchQuery()
        {
            showProgressDialog(); // 显示进度框
            currentPage = 0;
            query = new PoiSearch.Query(keyWord, "", editCity.Text.ToString());
                // 第一个参数表示搜索字符串，第二个参数表示poi搜索类型，第三个参数表示poi搜索区域（空字符串代表全国）
            query.PageSize=(10); // 设置每页最多返回多少条poiitem
            query.PageNum=(currentPage); // 设置查第一页

            poiSearch = new PoiSearch(this, query);
            poiSearch.SetOnPoiSearchListener(this);
            poiSearch.SearchPOIAsyn();
        }


        public bool OnMarkerClick(Marker marker)
        {
            marker.ShowInfoWindow();
            return false;
        }


        public View GetInfoContents(Marker marker)
        {
            return null;
        }


        public View GetInfoWindow( Marker marker)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.poikeywordsearch_uri,
                null);
            TextView title = (TextView) view.FindViewById(Resource.Id.title);
            title.Text=(marker.Title);

            TextView snippet = (TextView) view.FindViewById(Resource.Id.snippet);
            snippet.Text=(marker.Snippet);
            ImageButton buttOn = (ImageButton) view
                .FindViewById(Resource.Id.start_amap_app);
            // 调起高德地图app
            buttOn.Click+=(sender,args)=>
            {
    startURI(marker);
                
            }
            
            ;
            return view;
        }

        /**
	 * 通过URI方式调起高德地图app
	 */

        public void startURI(Marker marker)
        {
            if (getAppIn())
            {
                Intent intent = new Intent(
                    "android.intent.actiOn.VIEW",
                  global::  Android.Net.Uri
                        .Parse("androidamap://viewMap?sourceApplicatiOn="
                               + getApplicatiOnName() + "&poiname="
                               + marker.Title + "&lat="
                               + marker.Position.Latitude + "&lOn="
                               + marker.Position.Longitude + "&dev=0"));
                intent.SetPackage("com.autOnavi.minimap");
                StartActivity(intent);
            }
            else
            {
                string url = "http://mo.amap.com/?dev=0&q="
                             + marker.Position.Latitude + ","
                             + marker.Position.Longitude + "&name="
                             + marker.Title;
                Intent intent = new Intent(Intent.ActionView);
                intent.SetData(Uri.Parse(url));
                StartActivity(intent);
            }

        }

        /**
	 * 判断高德地图app是否已经安装
	 */

        public bool getAppIn()
        {
            PackageInfo packageInfo = null;
            try
            {
                packageInfo = this.PackageManager.GetPackageInfo(
                    "com.autOnavi.minimap", 0);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                packageInfo = null;
                e.PrintStackTrace();
            }
            // 本手机没有安装高德地图app
            if (packageInfo != null)
            {
                return true;
            }
                // 本手机成功安装有高德地图app
            else
            {
                return false;
            }
        }

        /**
	 * 获取当前app的应用名字
	 */

        public string getApplicatiOnName()
        {
            PackageManager packageManager = null;
            ApplicationInfo applicatiOnInfo = null;
            try
            {
                packageManager = ApplicationContext.PackageManager;
                applicatiOnInfo = packageManager.GetApplicationInfo(
                    PackageName, 0);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                applicatiOnInfo = null;
            }
           string applicatiOnName = (string) packageManager
                .GetApplicationLabel(applicatiOnInfo);
            return applicatiOnName;
        }

        /**
	 * poi没有搜索到数据，返回一些推荐城市的信息
	 */

        private void showSuggestCity(List<SuggestionCity> cities)
        {
            string infomatiOn = "推荐城市\n";
            for (int i = 0; i < cities.Count; i++)
            {
                infomatiOn += "城市名称:" + cities[(i)].CityName + "城市区号:"
                              + cities[(i)].CityCode + "城市编码:"
                              + cities[(i)].AdCode + "\n";
            }
            ToastUtil.show(
            this,
            infomatiOn)
            ;

        }

        public void AfterTextChanged(IEditable s)
        {

        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            throw new System.NotImplementedException();
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            throw new System.NotImplementedException();
        }


        public void BeforeTextChanged(string s, int start, int count,
            int after)
        {

        }

        public void OnTextChanged(string s, int start, int before, int count)
        {
            string newText = s.Trim();
            /*
            Inputtips inputTips = new Inputtips(
            this,
            new Inputtips.IInputtipsListener()
            {

                @Override
            public void OnGetInputtips(List<Tip> tipList,
                int rCode) {
    if (rCode == 0) { // 正确返回
    List<String> listString = new ArrayList<String>();
    for (int i = 0; i < tipList.size(); i++) {
    listString.add(tipList.get(i).getName());
            }
            ArrayAdapter<String> aAdapter = new ArrayAdapter<String>(
                getApplicatiOnCOntext(),
                Resource.Layout.route_inputs, listString);
            searchText.SetAdapter(aAdapter);
            aAdapter.notifyDataSetChanged();
        }
        }
        }
        )
            ;
            try
            {
                inputTips.RequestInputtips(newText, editCity.Text); // 第一个参数表示提示关键字，第二个参数默认代表全国，也可以为城市区号

            }
            catch (AMapException e)
            {
                e.PrintStackTrace();
            }*/
        }

        /**
	 * POI详情查询回调方法
	 */
        public void OnPoiItemDetailSearched(PoiItemDetail arg0, int rCode)
        {

        }

        /**
	 * POI信息查询回调方法
	 */
     
        public void OnPoiSearched(PoiResult result, int rCode)
        {
            dissmissProgressDialog(); // 隐藏对话框
            if (rCode == 0)
            {
                if (result != null && result.Query != null)
                {
// 搜索poi的结果
                    if (result.Query.Equals(query))
                    {
// 是否是同一条
                        poiResult = result;
                        // 取得搜索到的poiitems有多少页
                        List<PoiItem> poiItems = (List<PoiItem>) poiResult.Pois; // 取得第一页的poiitem数据，页数从数字0开始
                        List<SuggestionCity> suggestiOnCities = (List<SuggestionCity>) poiResult
                            .SearchSuggestionCitys; // 当搜索不到poiitem数据时，会返回含有搜索关键字的城市信息

                        if (poiItems != null && poiItems.Count > 0)
                        {
                            aMap.Clear(); // 清理之前的图标
                            PoiOverlay poiOverlay = new PoiOverlay(aMap, poiItems);
                            poiOverlay.RemoveFromMap();
                            poiOverlay.AddToMap();
                            poiOverlay.ZoomToSpan();
                        }
                        else if (suggestiOnCities != null
                                 && suggestiOnCities.Count> 0)
                        {
                            showSuggestCity(suggestiOnCities);
                        }
                        else
                        {
                            ToastUtil.show(
                            this,Resource.String.no_result)
                            ;
                        }
                    }
                }
                else
                {
                    ToastUtil.show(
                    this,
                  Resource.String.no_result)
                    ;
                }
            }
            else if (rCode == 27)
            {
                ToastUtil.show(
                this,
               Resource.String.error_network)
                ;
            }
            else if (rCode == 32)
            {
                ToastUtil.show(
                this,
               Resource.String.error_key)
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
	 * Button点击事件回调方法
	 */

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                    /**
		 * 点击搜索按钮
		 */
                case Resource.Id.searchButton:
                    searchButton();
                    break;
                    /**
		 * 点击下一页按钮
		 */
                case Resource.Id.nextButton:
                    nextButton();
                    break;
                default:
                    break;
            }
        }

    }
}