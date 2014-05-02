
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
using Com.Amap.Api.Services.Route;
using Java.Lang;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{
/**
 * AMapV1地图中简单介绍route搜索
 */

[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
    public class RouteActivity
    

: Activity , Com.Amap.Api.Maps2d.AMap.IOnMarkerClickListener,
    Com.Amap.Api.Maps2d.AMap.IOnMapClickListener, Com.Amap.Api.Maps2d.AMap.IOnInfoWindowClickListener, Com.Amap.Api.Maps2d.AMap.IInfoWindowAdapter,
    PoiSearch.IOnPoiSearchListener, RouteSearch.IOnRouteSearchListener, View.IOnClickListener
{
    private Com.Amap.Api.Maps2d.AMap aMap;
    private MapView mapView;
    private Button drivingButton;
    private Button busButton;
    private Button walkButton;

    private ImageButton startImageButton;
    private ImageButton endImageButton;
    private ImageButton routeSearchImagebtn;

    private EditText startTextView;
    private EditText endTextView;
    private ProgressDialog progDialog = null; // 搜索时进度条
    private int busMode = RouteSearch.BusDefault; // 公交默认模式
    private int drivingMode = RouteSearch.DrivingDefault; // 驾车默认模式
    private int walkMode = RouteSearch.WalkDefault; // 步行默认模式
    private BusRouteResult busRouteResult; // 公交模式查询结果
    private DriveRouteResult driveRouteResult; // 驾车模式查询结果
    private WalkRouteResult walkRouteResult; // 步行模式查询结果
    private int routeType = 1; // 1代表公交模式，2代表驾车模式，3代表步行模式
    private string strStart;
    private string strEnd;
    private LatLonPoint startPoint = null;
    private LatLonPoint endPoint = null;
    private PoiSearch.Query startSearchQuery;
    private PoiSearch.Query endSearchQuery;

    private bool isClickStart = false;
    private bool isClickTarget = false;
    private Marker startMk, targetMk;
    private RouteSearch routeSearch;
    public ArrayAdapter<String> aAdapter;


    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.route_activity);
        mapView = FindViewById < MapView>(Resource.Id.map);
        mapView.OnCreate(bundle); // 此方法必须重写
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
            registerListener();
        }
        routeSearch = new RouteSearch(this);
        routeSearch.SetRouteSearchListener(this);
        startTextView = (EditText) FindViewById(Resource.Id.autotextview_roadsearch_start);
        endTextView = (EditText) FindViewById(Resource.Id.autotextview_roadsearch_goals);
        busButton = (Button) FindViewById(Resource.Id.imagebtn_roadsearch_tab_transit);
        busButton.SetOnClickListener(this);
        drivingButton = (Button) FindViewById(Resource.Id.imagebtn_roadsearch_tab_driving);
        drivingButton.SetOnClickListener(this);
        walkButton = (Button) FindViewById(Resource.Id.imagebtn_roadsearch_tab_walk);
        walkButton.SetOnClickListener(this);
        startImageButton = (ImageButton) FindViewById(Resource.Id.imagebtn_roadsearch_startoption);
        startImageButton.SetOnClickListener(this);
        endImageButton = (ImageButton) FindViewById(Resource.Id.imagebtn_roadsearch_endoption);
        endImageButton.SetOnClickListener(this);
        routeSearchImagebtn = (ImageButton) FindViewById(Resource.Id.imagebtn_roadsearch_search);
        routeSearchImagebtn.SetOnClickListener(this);
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

    protected  override void OnPause()
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
	 * 选择公交模式
	 */

    private void busRoute()
    {
        routeType = 1; // 标识为公交模式
        busMode = RouteSearch.BusDefault;
        drivingButton.SetBackgroundResource(Resource.Drawable.mode_driving_off);
        busButton.SetBackgroundResource(Resource.Drawable.mode_transit_on);
        walkButton.SetBackgroundResource(Resource.Drawable.mode_walk_off);

    }

    /**
	 * 选择驾车模式
	 */

    private void drivingRoute()
    {
        routeType = 2; // 标识为驾车模式
        drivingMode = RouteSearch.DrivingSaveMoney;
        drivingButton.SetBackgroundResource(Resource.Drawable.mode_driving_on);
        busButton.SetBackgroundResource(Resource.Drawable.mode_transit_off);
        walkButton.SetBackgroundResource(Resource.Drawable.mode_walk_off);
    }

    /**
	 * 选择步行模式
	 */

    private void walkRoute()
    {
        routeType = 3; // 标识为步行模式
        walkMode = RouteSearch.WalkMultipath;
        drivingButton.SetBackgroundResource(Resource.Drawable.mode_driving_off);
        busButton.SetBackgroundResource(Resource.Drawable.mode_transit_off);
        walkButton.SetBackgroundResource(Resource.Drawable.mode_walk_on);
    }

    /**
	 * 在地图上选取起点
	 */

    private void startImagePoint()
    {
        ToastUtil.show(
        this,
        "在地图上点击您的起点")
        ;
        isClickStart = true;
        isClickTarget = false;
        registerListener();
    }

    /**
	 * 在地图上选取终点
	 */

    private void endImagePoint()
    {
        ToastUtil.show(
        this,
        "在地图上点击您的终点")
        ;
        isClickTarget = true;
        isClickStart = false;
        registerListener();
    }

    /**
	 * 点击搜索按钮开始Route搜索
	 */

    public void searchRoute()
    {
        strStart = startTextView.Text.ToString().Trim();
        strEnd = endTextView.Text.ToString().Trim();
        if (strStart == null || strStart.Length == 0)
        {
            ToastUtil.show(
            this,
            "请选择起点")
            ;
            return;
        }
        if (strEnd == null || strEnd.Length == 0)
        {
            ToastUtil.show(
            this,
            "请选择终点")
            ;
            return;
        }
        if (strStart.Equals(strEnd))
        {
            ToastUtil.show(
            this,
            "起点与终点距离很近，您可以步行前往")
            ;
            return;
        }

        startSearchResult(); // 开始搜终点
    }


    public void OnInfoWindowClick(Marker marker)
    {
        isClickStart = false;
        isClickTarget = false;
        if (startMk.Equals(marker))
        {
            startTextView.Text=("地图上的起点");
            startPoint = AMapUtil.convertToLatLonPoint(startMk.Position);
            startMk.HideInfoWindow();
            startMk.Remove();
        }
        else if (targetMk.Equals(marker))
        {
            endTextView.Text=("地图上的终点");
            endPoint = AMapUtil.convertToLatLonPoint(targetMk.Position);
            targetMk.HideInfoWindow();
            targetMk.Remove();
        }
    }


    public bool OnMarkerClick(Marker marker)
    {
        if (marker.IsInfoWindowShown)
        {
            marker.HideInfoWindow();
        }
        else
        {
            marker.ShowInfoWindow();
        }
        return false;
    }


    public void OnMapClick(LatLng latng)
    {
        if (isClickStart)
        {
            startMk = aMap.AddMarker(new MarkerOptions()
                .Anchor(0.5f, 1)
                .SetIcon(BitmapDescriptorFactory
                    .FromResource(Resource.Drawable.point)).SetPosition(latng)
                .SetTitle("点击选择为起点"));
            startMk.ShowInfoWindow();
        }
        else if (isClickTarget)
        {
            targetMk = aMap.AddMarker(new MarkerOptions()
                .Anchor(0.5f, 1)
                .SetIcon(BitmapDescriptorFactory
                    .FromResource(Resource.Drawable.point)).SetPosition(latng)
                .SetTitle("点击选择为目的地"));
            targetMk.ShowInfoWindow();
        }
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
        aMap.SetOnInfoWindowClickListener(
        this)
        ;
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
        progDialog.SetCancelable(true);
        progDialog.SetMessage("正在搜索");
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
	 * 查询路径规划起点
	 */

    public void startSearchResult()
    {
        strStart = startTextView.Text.ToString().Trim();
        if (startPoint != null && strStart.Equals("地图上的起点"))
        {
            endSearchResult();
        }
        else
        {
            showProgressDialog();
            startSearchQuery = new PoiSearch.Query(strStart, "", "010"); // 第一个参数表示查询关键字，第二参数表示poi搜索类型，第三个参数表示城市区号或者城市名
            startSearchQuery.PageNum=(0); // 设置查询第几页，第一页从0开始
            startSearchQuery.PageSize=(20); // 设置每页返回多少条数据
            PoiSearch poiSearch = new PoiSearch(
            this,
            startSearchQuery)
            ;
            poiSearch.SetOnPoiSearchListener(this);
            poiSearch.SearchPOIAsyn(); // 异步poi查询
        }
    }

    /**
	 * 查询路径规划终点
	 */

    public void endSearchResult()
    {
        strEnd = endTextView.Text.ToString().Trim();
        if (endPoint != null && strEnd.Equals("地图上的终点"))
        {
            searchRouteResult(startPoint, endPoint);
        }
        else
        {
            showProgressDialog();
            endSearchQuery = new PoiSearch.Query(strEnd, "", "010"); // 第一个参数表示查询关键字，第二参数表示poi搜索类型，第三个参数表示城市区号或者城市名
            endSearchQuery.PageNum=(0); // 设置查询第几页，第一页从0开始
            endSearchQuery.PageSize=(20); // 设置每页返回多少条数据

            PoiSearch poiSearch = new PoiSearch(
            this,
            endSearchQuery)
            ;
            poiSearch.SetOnPoiSearchListener(this);
            poiSearch.SearchPOIAsyn(); // 异步poi查询
        }
    }

    /**
	 * 开始搜索路径规划方案
	 */

    public void searchRouteResult(LatLonPoint startPoint, LatLonPoint endPoint)
    {
        showProgressDialog();
        RouteSearch.FromAndTo fromAndTo = new RouteSearch.FromAndTo(
            startPoint, endPoint);
        if (routeType == 1)
        {
// 公交路径规划
            RouteSearch.BusRouteQuery query = new RouteSearch.BusRouteQuery(fromAndTo, busMode, "北京", 0);
                // 第一个参数表示路径规划的起点和终点，第二个参数表示公交查询模式，第三个参数表示公交查询城市区号，第四个参数表示是否计算夜班车，0表示不计算
            routeSearch.CalculateBusRouteAsyn(query); // 异步路径规划公交模式查询
        }
        else if (routeType == 2)
        {
// 驾车路径规划
            RouteSearch.DriveRouteQuery query = new RouteSearch.DriveRouteQuery(fromAndTo, drivingMode,
                null, null, ""); // 第一个参数表示路径规划的起点和终点，第二个参数表示驾车模式，第三个参数表示途经点，第四个参数表示避让区域，第五个参数表示避让道路
            routeSearch.CalculateDriveRouteAsyn(query); // 异步路径规划驾车模式查询
        }
        else if (routeType == 3)
        {
// 步行路径规划
            RouteSearch.WalkRouteQuery query = new RouteSearch.WalkRouteQuery(fromAndTo, walkMode);
            routeSearch.CalculateWalkRouteAsyn(query); // 异步路径规划步行模式查询
        }
    }

    
    public void OnPoiItemDetailSearched(PoiItemDetail arg0, int arg1)
    {

    }

    /**
	 * POI搜索结果回调
	 */

    public void OnPoiSearched(PoiResult result, int rCode)
    {
        dissmissProgressDialog();
        if (rCode == 0)
        {
// 返回成功
            if (result != null && result.Query != null
                && result.Pois != null && result.Pois.Count > 0)
            {
// 搜索poi的结果
                if (result.Query.Equals(startSearchQuery))
                {
                    List<PoiItem> poiItems = (List<PoiItem>) result.Pois; // 取得poiitem数据
                    RouteSearchPoiDialog dialog = new RouteSearchPoiDialog(
                        
                    this,
                    poiItems)
                    ;
                    dialog.SetTitle("您要找的起点是:");
                    dialog.Show();
                    /*
                    dialog.SetOnListClickListener(new OnListItemClick()
                    {
                        @Override
                    public void onListItemClick(
    RouteSearchPoiDialog dialog,
                        PoiItem startpoiItem) {
    startPoint = startpoiItem.getLatLonPoint();
    strStart = startpoiItem.getTitle();
    startTextView.SetText(strStart);
    endSearchResult(); // 开始搜终点
                    }

                }
                )
  */
                    ;
                }
                else if (result.Query.Equals(endSearchQuery))
                {
                    List<PoiItem> poiItems = (List<PoiItem>) result.Pois; // 取得poiitem数据
                    RouteSearchPoiDialog dialog = new RouteSearchPoiDialog(
                        
                    this,
                    poiItems)
                    ;
                    dialog.SetTitle("您要找的终点是:");
                    dialog.Show();
                    /*
                    dialog.SetOnListClickListener(new OnListItemClick()
                    {
                        @Override
                    public void onListItemClick(
    RouteSearchPoiDialog dialog,
                        PoiItem endpoiItem) {
    endPoint = endpoiItem.getLatLonPoint();
    strEnd = endpoiItem.getTitle();
    endTextView.SetText(strEnd);
    searchRouteResult(startPoint,
                        endPoint); // 进行路径规划搜索
                    }

                }
                )*/
                    ;
                }
            }
            else
            {
                ToastUtil.show(
                this,Resource.String.no_result)
                ;
            }
        }
        else if (rCode == 27)
        {
            ToastUtil.show(
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
            GetString(Resource.String.error_other)
            + rCode)
            ;
        }
    }

    /**
	 * 公交路线查询回调
	 */

    public void OnBusRouteSearched(BusRouteResult result, int rCode)
    {
        dissmissProgressDialog();
        if (rCode == 0)
        {
            if (result != null && result.Paths != null
                && result.Paths.Count > 0)
            {
                busRouteResult = result;
                BusPath busPath = busRouteResult.Paths[0];
                aMap.Clear(); // 清理地图上的所有覆盖物
                BusRouteOverlay routeOverlay = new BusRouteOverlay(this, aMap,
                    busPath, busRouteResult.StartPos,
                    busRouteResult.TargetPos);
                routeOverlay.RemoveFromMap();
                routeOverlay.AddToMap();
                routeOverlay.ZoomToSpan();
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
	 * 驾车结果回调
	 */

    public void OnDriveRouteSearched(DriveRouteResult result, int rCode)
    {
        dissmissProgressDialog();
        if (rCode == 0)
        {
            if (result != null && result.Paths != null
                && result.Paths.Count > 0)
            {
                driveRouteResult = result;
                DrivePath drivePath = driveRouteResult.Paths[0];
                aMap.Clear(); // 清理地图上的所有覆盖物
                DrivingRouteOverlay drivingRouteOverlay = new DrivingRouteOverlay(
                    this, aMap, drivePath, driveRouteResult.StartPos,
                    driveRouteResult.TargetPos);
                drivingRouteOverlay.RemoveFromMap();
                drivingRouteOverlay.AddToMap();
                drivingRouteOverlay.ZoomToSpan();
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
	 * 步行路线结果回调
	 */

    public void OnWalkRouteSearched(WalkRouteResult result, int rCode)
    {
        dissmissProgressDialog();
        if (rCode == 0)
        {
            if (result != null && result.Paths != null
                && result.Paths.Count > 0)
            {
                walkRouteResult = result;
                WalkPath walkPath = walkRouteResult.Paths[0];
                aMap.Clear(); // 清理地图上的所有覆盖物
                WalkRouteOverlay walkRouteOverlay = new WalkRouteOverlay(this,
                    aMap, walkPath, walkRouteResult.StartPos,
                    walkRouteResult.TargetPos);
                walkRouteOverlay.RemoveFromMap();
                walkRouteOverlay.AddToMap();
                walkRouteOverlay.ZoomToSpan();
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


    public void OnClick(View v)
    {
        switch (v.Id)
        {
            case Resource.Id.imagebtn_roadsearch_startoption:
                startImagePoint();
                break;
            case Resource.Id.imagebtn_roadsearch_endoption:
                endImagePoint();
                break;
            case Resource.Id.imagebtn_roadsearch_tab_transit:
                busRoute();
                break;
            case Resource.Id.imagebtn_roadsearch_tab_driving:
                drivingRoute();
                break;
            case Resource.Id.imagebtn_roadsearch_tab_walk:
                walkRoute();
                break;
            case Resource.Id.imagebtn_roadsearch_search:
                searchRoute();
                break;
            default:
                break;
        }
    }
}
}