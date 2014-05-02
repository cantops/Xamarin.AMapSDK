
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Maps2d.Overlay;
using Com.Amap.Api.Services.Busline;
using Java.Lang;
using JuzzPig.AMap.AndroidDemo;
using JuzzPig.AMap.Demo.util;

namespace JuzzPig.AMap.AndroidDemo
{

/**
 * AMapV1地图中简单介绍公交线路搜索
 */
[Activity(Label = "JuzzPig.AMap.Android.Demo", Icon = "@drawable/icon")]
public class BuslineActivity : Activity , Com.Amap.Api.Maps2d.AMap.IOnMarkerClickListener,
		Com.Amap.Api.Maps2d.AMap.IInfoWindowAdapter, AdapterView.IOnItemSelectedListener, BusLineSearch.IOnBusLineSearchListener,
		BusStationSearch.IOnBusStationSearchListener, View.IOnClickListener {

	private Com.Amap.Api.Maps2d.AMap aMap;
	private MapView mapView;
	private ProgressDialog progDialog = null;// 进度框
	private EditText searchName;// 输入公交线路名称
	private Spinner selectCity;// 选择城市下拉列表
	private string[] itemCitys = { "北京-010", "郑州-0371", "上海-021" };
	private string cityCode = "";// 城市区号
	private int currentpage = 0;// 公交搜索当前页，第一页从0开始
	private BusLineResult busLineResult;// 公交线路搜索返回的结果
	private List<BusLineItem> lineItems = null;// 公交线路搜索返回的busline
	private BusLineQuery busLineQuery;// 公交线路查询的查询类

	private BusStationResult busStatiOnResult;// 公交站点搜索返回的结果
	private List<BusStationItem> statiOnItems;// 公交站点搜索返回的busStatiOn
	private BusStationQuery busStatiOnQuery;// 公交站点查询的查询类

	private BusLineSearch busLineSearch;// 公交线路列表查询

    protected override void OnCreate(Bundle bundle)
    {base.OnCreate(bundle);
		SetContentView(Resource.Layout.busline_activity);
        mapView = FindViewById < MapView>(Resource.Id.map);
		mapView.OnCreate(bundle);// 此方法必须重写
		init();
    }

    /**
	 * 初始化AMap对象
	 */
	private void init() {
		if (aMap == null) {
			aMap = mapView.Map;
			setUpMap();
		}
		Button searchByName = (Button) FindViewById(Resource.Id.searchbyname);
		searchByName.SetOnClickListener(this);
		selectCity = (Spinner) FindViewById(Resource.Id.cityName);
		var adapter = new ArrayAdapter<string>(this, global::Android.Resource.Layout.SimpleSpinnerItem, itemCitys);
		adapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
		selectCity.Adapter=(adapter);
		selectCity.Prompt=("请选择城市：");
		selectCity.OnItemSelectedListener=(this);
		searchName = (EditText) FindViewById(Resource.Id.busName);
	}

	/**
	 * 设置marker的监听和信息窗口的监听
	 */
	private void setUpMap() {
		aMap.SetOnMarkerClickListener(this);
		aMap.SetInfoWindowAdapter(this);
	}

	/**
	 * 方法必须重写
	 */
	protected override void OnResume() {
		base.OnResume();
		mapView.OnResume();
	}

	/**
	 * 方法必须重写
	 */
	protected override void OnPause() {
		base.OnPause();
		mapView.OnPause();
	}

	/**
	 * 方法必须重写
	 */
	protected  override void OnSaveInstanceState(Bundle outState) {
		base.OnSaveInstanceState(outState);
		mapView.OnSaveInstanceState(outState);
	}

	/**
	 * 方法必须重写
	 */
	protected override void OnDestroy() {
		base.OnDestroy();
		mapView.OnDestroy();
	}

	/**
	 * 公交线路搜索
	 */
	public void searchLine() {
		currentpage = 0;// 第一页默认从0开始
		showProgressDialog();
		string search = searchName.Text.ToString().Trim();
		if ("".Equals(search)) {
			search = "641";
			searchName.Text=(search);
		}
		busLineQuery = new BusLineQuery(search, BusLineQuery.SearchType.ByLineName,
				cityCode);// 第一个参数表示公交线路名，第二个参数表示公交线路查询，第三个参数表示所在城市名或者城市区号
		busLineQuery.PageSize=(10);// 设置每页返回多少条数据
		busLineQuery.PageNumber=(currentpage);// 设置查询第几页，第一页从0开始算起
		busLineSearch = new BusLineSearch(this, busLineQuery);// 设置条件
		busLineSearch.SetOnBusLineSearchListener(this);// 设置查询结果的监听
		busLineSearch.SearchBusLineAsyn();// 异步查询公交线路名称
		// 公交站点搜索事例
		/*
		 * BusStatiOnQuery query = new BusStatiOnQuery(search,cityCode);
		 * query.SetPageSize(10); query.SetPageNumber(currentpage);
		 * BusStatiOnSearch busStatiOnSearch = new BusStatiOnSearch(this,query);
		 * busStatiOnSearch.SetOnBusStatiOnSearchListener(this);
		 * busStatiOnSearch.searchBusStatiOnAsyn();
		 */
	}

	/**
	 * 显示进度框
	 */
	private void showProgressDialog() {
		if (progDialog == null)
			progDialog = new ProgressDialog(this);
		progDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
		progDialog.Indeterminate=(false);
		progDialog.SetCancelable(true);
		progDialog.SetMessage("正在搜索:\n");
		progDialog.Show();
	}

	/**
	 * 隐藏进度框
	 */
	private void dissmissProgressDialog() {
		if (progDialog != null) {
			progDialog.Dismiss();
		}
	}

	/**
	 * 提供一个给默认信息窗口定制内容的方法
	 */
	public View GetInfoContents(Marker marker) {
		return null;
	}

	/**
	 * 提供一个个性化定制信息窗口的方法
	 */
	public View GetInfoWindow(Marker marker) {
		return null;
	}

	/**
	 * 点击marker回调函数
	 */
	public bool OnMarkerClick(Marker marker) {
		return false;// 点击marker时把此marker显示在地图中心点
	}

	/**
	 * 选择城市
	 */
	public void OnItemSelected(AdapterView parent, View view, int positiOn,
			long id) {
		string cityString = itemCitys[positiOn];
		cityCode = cityString.Substring(cityString.IndexOf("-") + 1);
	}

	public void OnNothingSelected(AdapterView arg0) {
		cityCode = "010";
	}

	/**
	 * 公交线路搜索返回的结果显示在dialog中
	 */
	public void showResultList(List<BusLineItem> busLineItems) {
		BusLineDialog busLineDialog = new BusLineDialog(this, busLineItems);
        /*
		busLineDialog.ListItemClicklistener(new OnListItemlistener() {
			@Override
			public void OnListItemClick(BusLineDialog dialog,
					final BusLineItem item) {
				showProgressDialog();

				String lineId = item.getBusLineId();// 得到当前点击item公交线路id
				busLineQuery = new BusLineQuery(lineId, BusLineQuery.SearchType.BY_LINE_ID,
						cityCode);// 第一个参数表示公交线路id，第二个参数表示公交线路id查询，第三个参数表示所在城市名或者城市区号
				BusLineSearch busLineSearch = new BusLineSearch(
						BuslineActivity.this, busLineQuery);
				busLineSearch.SetOnBusLineSearchListener(BuslineActivity.this);
				busLineSearch.searchBusLineAsyn();// 异步查询公交线路id
			}
		});*/
		busLineDialog.Show();

	}

	interface OnListItemlistener {
		 void OnListItemClick(BusLineDialog dialog, BusLineItem item);
	}

	/**
	 * 所有公交线路显示页面
	 */
	class BusLineDialog : Dialog ,View.IOnClickListener {

		private List<BusLineItem> busLineItems;
		private BusLineAdapter busLineAdapter;
		private Button preButton, nextButton;
		private ListView listView;
		protected OnListItemlistener OnListItemlistener;
	    private BuslineActivity _mContext;
		public BusLineDialog(Context context, int theme):base(context, theme)
		{

		    _mContext = (BuslineActivity)context ;
		}

		public void OnListItemClicklistener(
				OnListItemlistener OnListItemlistener) {
			this.OnListItemlistener = OnListItemlistener;

		}

		public BusLineDialog(Context context, List<BusLineItem> busLineItems):this(context,global::Android.Resource.Style.ThemeBlackNoTitleBar) {
			//this(context, global::Android.Resource.Style.ThemeBlackNoTitleBar);
			this.busLineItems = busLineItems;
			busLineAdapter = new BusLineAdapter(context, busLineItems);
            
		    _mContext = (BuslineActivity)context ;
		}

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.busline_dialog);
			preButton = (Button) FindViewById(Resource.Id.preButton);
			nextButton = (Button) FindViewById(Resource.Id.nextButton);
			listView = (ListView) FindViewById(Resource.Id.listview);
			listView.SetAdapter(busLineAdapter);
            /*
			listView.OnItemClickListener(new OnItemClickListener() {

				@Override
				public void OnItemClick(AdapterView<?> arg0, View arg1,
						int arg2, lOng arg3) {
					OnListItemlistener.OnListItemClick(BusLineDialog.this,
							busLineItems.get(arg2));
					dismiss();

				}
			});*/
		    listView.ItemClick += (sender, args) =>
		    {
                OnListItemlistener.OnListItemClick(this,
							busLineItems[args.Position]);
					Dismiss();
		    };
			preButton.SetOnClickListener(this);
			nextButton.SetOnClickListener(this);
			if (_mContext.currentpage <= 0) {
				preButton.Enabled=(false);
			}
			if (_mContext.currentpage >= _mContext.busLineResult.PageCount - 1) {
				nextButton.Enabled=(false);
			}

		}

		public void OnClick(View v) {
			this.Dismiss();
			if (v.Equals(preButton)) {
				_mContext.currentpage--;
			} else if (v.Equals(nextButton)) {
				_mContext.currentpage++;
			}
		_mContext.	showProgressDialog();
		_mContext.	busLineQuery.PageNumber=(_mContext.currentpage);// 设置公交查询第几页
			_mContext.busLineSearch.SetOnBusLineSearchListener(_mContext);
		_mContext.	busLineSearch.SearchBusLineAsyn();// 异步查询公交线路名称
		}
	}
    
	/**
	 * 公交站点查询结果回调
	 */
    public void OnBusStationSearched(BusStationResult result, int rCode)
    {
		dissmissProgressDialog();
		if (rCode == 0) {
			if (result != null && result.PageCount > 0
					&& result.BusStations != null
					&& result.BusStations.Count> 0) {
				busStatiOnResult = result;
				statiOnItems = (List<BusStationItem>) result.BusStations;
			} else {
				ToastUtil.show(this, Resource.String.no_result);
			}
		} else if (rCode == 27) {
			ToastUtil.show(this, Resource.String.error_network);
		} else if (rCode == 32) {
			ToastUtil.show(this, Resource.String.error_key);
		} else {
			ToastUtil.show(this, Resource.String.error_other);
		}
	}

	/**
	 * 公交线路查询结果回调
	 */
	public void OnBusLineSearched(BusLineResult result, int rCode) {
		dissmissProgressDialog();
		if (rCode == 0) {
			if (result != null && result.Query != null
					&& result.Query.Equals(busLineQuery)) {
				if (result.Query.Category== BusLineQuery.SearchType.ByLineName) {
					if (result.PageCount > 0
							&& result.BusLines != null
							&& result.BusLines.Count > 0) {
						busLineResult = result;
						lineItems = (List<BusLineItem>) result.BusLines;
						showResultList(lineItems);
					}
				} else if (result.Query.Category == BusLineQuery.SearchType.ByLineId) {
					aMap.Clear();// 清理地图上的marker
					busLineResult = result;
					lineItems = (List<BusLineItem>) busLineResult.BusLines;
					BusLineOverlay busLineOverlay = new BusLineOverlay(this,
							aMap, lineItems[0]);
					busLineOverlay.RemoveFromMap();
					busLineOverlay.AddToMap();
					busLineOverlay.ZoomToSpan();
				}
			} else {
				
				ToastUtil.show(this, Resource.String.no_result);
			}
		} else if (rCode == 27) {
			ToastUtil.show(this,Resource.String.error_network);
		} else if (rCode == 32) {
			ToastUtil.show(this,Resource.String.error_key);
		} else {
			ToastUtil.show(this,Resource.String.error_other);
		}
	}

	/**
	 * 查询公交线路
	 */
	public void OnClick(View v) {
		searchLine();
	}

		}
}