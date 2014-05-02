
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Core;
using Com.Amap.Api.Services.Poisearch;
using Java.Lang;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{

/**
 * AMapV1地图中简单介绍poisearch搜索
 */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class PoiAroundSearchActivity: FragmentActivity ,
    Com.Amap.Api.Maps2d.AMap.IOnMarkerClickListener, Com.Amap.Api.Maps2d.AMap.IInfoWindowAdapter, AdapterView.IOnItemSelectedListener,
    PoiSearch.IOnPoiSearchListener, Com.Amap.Api.Maps2d.AMap.IOnMapClickListener, Com.Amap.Api.Maps2d.AMap.IOnInfoWindowClickListener,
    View.IOnClickListener
{
    private Com.Amap.Api.Maps2d.AMap aMap;
    private ProgressDialog progDialog = null; // 搜索时进度条
    private Spinner selectDeep; // 选择城市列表
    private string[] itemDeep = {"餐饮", "景区", "酒店", "影院"};
    private Spinner selectType; // 选择返回是否有团购，优惠
    private string[] itemTypes = {"所有poi", "有团购", "有优惠", "有团购或者优惠"};
    private string deepType = ""; // poi搜索类型
    private int searchType = 0; // 搜索类型
    private int tsearchType = 0; // 当前选择搜索类型
    private PoiResult poiResult; // poi返回的结果
    private int currentPage = 0; // 当前页面，从0开始计数
    private PoiSearch.Query query; // Poi查询条件类
    private LatLonPoint lp = new LatLonPoint(39.908127, 116.375257); // 默认西单广场
    private Marker locatiOnMarker; // 选择的点
    private PoiSearch poiSearch;
    private PoiOverlay poiOverlay; // poi图层
    private List<PoiItem> poiItems; // poi数据
    private Marker detailMarker; // 显示Marker的详情
    private Button nextButton; // 下一页


    protected override void OnCreate(Bundle savedInstanceState)
    {
       base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.poiaroundsearch_activity);
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
            setSelectType();
            Button locatiOnButton = (Button) FindViewById(Resource.Id.locationButton);
            locatiOnButton.SetOnClickListener(this);
            Button searchButton = (Button) FindViewById(Resource.Id.searchButton);
            searchButton.SetOnClickListener(this);
            nextButton = (Button) FindViewById(Resource.Id.nextButton);
            nextButton.SetOnClickListener(this);
            nextButton.Clickable=(false); // 默认下一页按钮不可点
            locatiOnMarker = aMap.AddMarker(new MarkerOptions()
                .Anchor(0.5f, 1)
                .SetIcon(BitmapDescriptorFactory
                    .FromResource(Resource.Drawable.point))
                .SetPosition(new LatLng(lp.Latitude, lp.Longitude))
                .SetTitle("西单为中心点，查其周边"));
            locatiOnMarker.ShowInfoWindow();

        }
    }

    /**
	 * 设置城市选择
	 */

    private void setUpMap()
    {
        selectDeep = (Spinner) FindViewById(Resource.Id.spinnerdeep);
        var adapter = new ArrayAdapter<string>(this,
            global::Android.Resource.Layout.SimpleSpinnerItem, itemDeep);
        adapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
        selectDeep.Adapter=(adapter);
        selectDeep.OnItemSelectedListener=(this); // 添加spinner选择框监听事件
        aMap.SetOnMarkerClickListener(this); // 添加点击marker监听事件
        aMap.SetInfoWindowAdapter(this); // 添加显示infowindow监听事件

    }

    /**
	 * 设置选择类型
	 */

    private void setSelectType()
    {
        selectType = (Spinner) FindViewById(Resource.Id.searchType); // 搜索类型
        var adapter = new ArrayAdapter<string>(this,
           global::Android.Resource.Layout.SimpleSpinnerItem, itemTypes);
        adapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
        selectType.Adapter=(adapter);
        selectType.OnItemSelectedListener=(this); // 添加spinner选择框监听事件
        aMap.SetOnMarkerClickListener(this); // 添加点击marker监听事件
        aMap.SetInfoWindowAdapter(this); // 添加显示infowindow监听事件
    }

    /**
	 * 注册监听
	 */

    private void registerListener()
    {
        aMap.SetOnMapClickListener(
        this)
        ;
        aMap.SetOnMarkerClickListener(
        this)
        ;
        aMap.SetOnInfoWindowClickListener(this);
        aMap.SetInfoWindowAdapter(
        this)
        ;
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
        progDialog.SetMessage("正在搜索中");
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
        aMap.SetOnMapClickListener(null); // 进行poi搜索时清除掉地图点击事件
        currentPage = 0;
        query = new PoiSearch.Query("", deepType, "北京市"); // 第一个参数表示搜索字符串，第二个参数表示poi搜索类型，第三个参数表示poi搜索区域（空字符串代表全国）
        query.PageSize=(10); // 设置每页最多返回多少条poiitem
        query.PageNum=(currentPage); // 设置查第一页

        searchType = tsearchType;

        switch (searchType)
        {
            case 0:
            {
// 所有poi
                query.SetLimitDiscount(false);
                query.SetLimitGroupbuy(false);
            }
                break;
            case 1:
            {
// 有团购
                query.SetLimitGroupbuy(true);
                query.SetLimitDiscount(false);
            }
                break;
            case 2:
            {
// 有优惠
                query.SetLimitGroupbuy(false);
                query.SetLimitDiscount(true);
            }
                break;
            case 3:
            {
// 有团购或者优惠
                query.SetLimitGroupbuy(true);
                query.SetLimitDiscount(true);
            }
                break;
        }

        if (lp != null)
        {
            poiSearch = new PoiSearch(this, query);
            poiSearch.SetOnPoiSearchListener(this);
            poiSearch.Bound=(new PoiSearch.SearchBound(lp, 2000, true)); //
            // 设置搜索区域为以lp点为圆心，其周围2000米范围
            /*
			 * List<LatLOnPoint> list = new ArrayList<LatLOnPoint>();
			 * list.add(lp);
			 * list.add(AMapUtil.cOnvertToLatLOnPoint(COnstants.BEIJING));
			 * poiSearch.SetBound(new SearchBound(list));// 设置多边形poi搜索范围
			 */
            poiSearch.SearchPOIAsyn(); // 异步搜索
        }
    }

    /**
	 * 点击下一页poi搜索
	 */

    public void nextSearch()
    {
        if (query != null && poiSearch != null && poiResult != null)
        {
            if (poiResult.PageCount - 1 > currentPage)
            {
                currentPage++;

                query.PageNum=(currentPage); // 设置查后一页
                poiSearch.SearchPOIAsyn();
            }
            else
            {
                ToastUtil
                    .show(
                this,Resource.String.no_result)
                ;
            }
        }
    }

    /**
	 * 查单个poi详情
	 * 
	 * @param poiId
	 */

    public void doSearchPoiDetail(string poiId)
    {
        if (poiSearch != null && poiId != null)
        {
            poiSearch.SearchPOIDetailAsyn(poiId);
        }
    }

    public bool OnMarkerClick(Marker marker)
    {
        if (poiOverlay != null && poiItems != null && poiItems.Count > 0)
        {
            detailMarker = marker;
            doSearchPoiDetail(poiItems[poiOverlay.GetPoiIndex(marker)].PoiId);
        }
        return false;
    }


    public View GetInfoContents(Marker marker)
    {
        return null;
    }


        public View GetInfoWindow(Marker marker)
    {
        return null;
    }

    /**
	 * poi没有搜索到数据，返回一些推荐城市的信息
	 */

    private void showSuggestCity(List<SuggestionCity> cities)
    {
        string infomatiOn = "推荐城市\n";
        for (int i = 0; i < cities.Count; i++)
        {
            infomatiOn += "城市名称:" + cities[i].CityName + "城市区号:"
                          + cities[i].CityCode + "城市编码:"
                          + cities[i].AdCode + "\n";
        }
        ToastUtil.show(
        this,
        infomatiOn)
        ;

    }

    public void OnItemSelected(AdapterView
     parent 
,
     View view,
     int positiOn,
     long id 
)
{
    if (parent == selectDeep)
    {
        deepType = itemDeep[positiOn];

    }
    else if (parent == selectType)
    {
        tsearchType = positiOn;
    }
    nextButton.Clickable=(false); // 改变搜索条件，需重新搜索
}


    public void OnNothingSelected(AdapterView parent)
    {
        if (parent == selectDeep)
        {
            deepType = "餐饮";
        }
        else if (parent == selectType)
        {
            tsearchType = 0;
        }
        nextButton.Clickable=(false); // 改变搜索条件，需重新搜索
    }

    /**
	 * POI详情回调
	 */

    public void OnPoiItemDetailSearched(PoiItemDetail result, int rCode)
    {
        dissmissProgressDialog(); // 隐藏对话框
        if (rCode == 0)
        {
            if (result != null)
            {
// 搜索poi的结果
                if (detailMarker != null)
                {
                    StringBuffer sb = new StringBuffer(result.Snippet);
                    if ((result.Groupbuys != null && result.Groupbuys
                        .Count > 0)
                        || (result.Discounts != null && result
                            .Discounts.Count > 0))
                    {

                        if (result.Groupbuys != null
                            && result.Groupbuys.Count > 0)
                        {
// 取第一条团购信息
                            sb.Append("\n团购："
                                      + result.Groupbuys[0].Detail);
                        }
                        if (result.Discounts != null
                            && result.Discounts.Count > 0)
                        {
// 取第一条优惠信息
                            sb.Append("\n优惠："
                                      + result.Discounts[(0)].Detail);
                        }
                    }
                    else
                    {
                        sb = new StringBuffer("地址：" + result.Snippet
                                              + "\n电话：" + result.Tel + "\n类型："
                                              + result.TypeDes);
                    }
                    // 判断poi搜索是否有深度信息
                    if (result.GetDeepType() != null)
                    {
                        sb = getDeepInfo(result, sb);
                        detailMarker.Snippet=(sb.ToString());
                    }
                    else
                    {
                        ToastUtil.show(
                        this,
                        "此Poi点没有深度信息")
                        ;
                    }
                }

            }
            else
            {
                ToastUtil
                    .show(
                this,Resource.String.no_result)
                ;
            }
        }
        else if (rCode == 27)
        {
            ToastUtil
                .show(
            this,Resource.String.error_network)
            ;
        }
        else if (rCode == 32)
        {
            ToastUtil.show(
            this,Resource.String.error_key)
            ;
        }
        else
        {
            ToastUtil.show(
            this,
            GetString(Resource.
            String.error_other)
            + rCode)
            ;
        }
    }

    /**
	 * POI深度信息获取
	 */

    private StringBuffer getDeepInfo(PoiItemDetail result, StringBuffer sbuBuffer)
    {
        /*
        switch (result.GetDeepType())
        {
                // 餐饮深度信息
            case PoiItemDetail.DeepType.Dining:
                if (result.Dining != null)
                {
                    Dining dining = result.Dining;
                    sbuBuffer
                        .Append("\n菜系：" + dining.Tag + "\n特色："
                                + dining.Recommend + "\n来源："
                                + dining.Deepsrc);
                }
                break;
                // 酒店深度信息
            case PoiItemDetail.DeepType.Hotel:
                if (result.Hotel != null)
                {
                    Hotel hotel = result.Hotel;
                    sbuBuffer.Append("\n价位：" + hotel.LowestPrice + "\n卫生："
                                     + hotel.HealthRating + "\n来源："
                                     + hotel.Deepsrc);
                }
                break;
                // 景区深度信息
            case PoiItemDetail.DeepType.Scenic:
                if (result.Scenic != null)
                {
                    Scenic scenic = result.Scenic;
                    sbuBuffer
                        .Append("\n价钱：" + scenic.Price + "\n推荐："
                                + scenic.Recommend + "\n来源："
                                + scenic.Deepsrc);
                }
                break;
                // 影院深度信息
            case PoiItemDetail.DeepType.Cinema:
                if (result.Cinema != null)
                {
                    Cinema cinema = result.Cinema;
                    sbuBuffer.Append("\n停车：" + cinema.Parking + "\n简介："
                                     + cinema.Intro + "\n来源：" + cinema.Deepsrc);
                }
                break;
            default:
                break;
        }*/
        return sbuBuffer;
    }

    /**
	 * POI搜索回调方法
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
                    poiItems = (List<PoiItem>) poiResult.Pois; // 取得第一页的poiitem数据，页数从数字0开始
                    List<SuggestionCity> suggestiOnCities = (List<SuggestionCity>) poiResult
                        .SearchSuggestionCitys; // 当搜索不到poiitem数据时，会返回含有搜索关键字的城市信息
                    if (poiItems != null && poiItems.Any())
                    {
                        aMap.Clear(); // 清理之前的图标
                        poiOverlay = new PoiOverlay(aMap, poiItems);
                        poiOverlay.RemoveFromMap();
                        poiOverlay.AddToMap();
                        poiOverlay.ZoomToSpan();

                        nextButton.Clickable=(true); // 设置下一页可点
                    }
                    else if (suggestiOnCities != null
                             && suggestiOnCities.Count > 0)
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
                ToastUtil
                    .show(
                this,Resource.String.no_result)
                ;
            }
        }
        else if (rCode == 27)
        {
            ToastUtil
                .show(
            this,
            Resource.String.error_network)
            ;
        }
        else if (rCode == 32)
        {
            ToastUtil.show(
                this,Resource.String.error_key)
            ;
        }
        else
        {
            ToastUtil.show(
            this,
            GetString(
               Resource.String.error_other)
            + rCode)
            ;
        }
    }


    public void OnMapClick(LatLng latng)
    {
        locatiOnMarker = aMap.AddMarker(new MarkerOptions().Anchor(0.5f, 1)
            .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.point))
            .SetPosition(latng).SetTitle("点击选择为中心点"));
        locatiOnMarker.ShowInfoWindow();
    }


    public void OnInfoWindowClick(Marker marker)
    {
        locatiOnMarker.HideInfoWindow();
        lp = new LatLonPoint(locatiOnMarker.Position.Latitude,
            locatiOnMarker.Position.Longitude);
        locatiOnMarker.Destroy();
    }

    
    public void OnClick(View v)
    {
        switch (v.Id)
        {
                /**
		 * 点击标记按钮
		 */
            case Resource.Id.locationButton:
                aMap.Clear(); // 清理所有marker
                registerListener();
                break;
                /**
		 * 点击搜索按钮
		 */
            case Resource.Id.searchButton:
                doSearchQuery();
                break;
                /**
		 * 点击下一页按钮
		 */
            case Resource.Id.nextButton:
                nextSearch();
                break;
            default:
                break;
        }
    }
}
}
