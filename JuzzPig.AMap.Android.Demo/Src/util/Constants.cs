using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Amap.Api.Maps2d.Model;

namespace JuzzPig.AMap.Demo.util
{
 
public class Constants {

	public  const int ERROR = 1001;// 网络异常
	public  const int ROUTE_START_SEARCH = 2000;
	public  const int ROUTE_END_SEARCH = 2001;
	public  const int ROUTE_BUS_RESULT = 2002;// 路径规划中公交模式
	public  const int ROUTE_DRIVING_RESULT = 2003;// 路径规划中驾车模式
	public  const int ROUTE_WALK_RESULT = 2004;// 路径规划中步行模式
	public  const int ROUTE_NO_RESULT = 2005;// 路径规划没有搜索到结果

	public  const int GEOCODER_RESULT = 3000;// 地理编码或者逆地理编码成功
	public  const int GEOCODER_NO_RESULT = 3001;// 地理编码或者逆地理编码没有数据

	public  const int POISEARCH = 4000;// poi搜索到结果
	public  const int POISEARCH_NO_RESULT = 4001;// poi没有搜索到结果
	public  const int POISEARCH_NEXT = 5000;// poi搜索下一页

	public  const int BUSLINE_LINE_RESULT = 6001;// 公交线路查询
	public  const int BUSLINE_id_RESULT = 6002;// 公交id查询
	public  const int BUSLINE_NO_RESULT = 6003;// 异常情况

    public static readonly LatLng BEIJING = new LatLng(39.90403, 116.407525);
    public static readonly LatLng ZHONGGUANCUN = new LatLng(39.983456, 116.3154950);
    public static readonly LatLng SHANGHAI = new LatLng(31.238068, 121.501654);
    public static readonly LatLng FANGHENG = new LatLng(39.989614, 116.481763);
    public static readonly LatLng CHENGDU = new LatLng(30.679879, 104.064855);
    public static readonly LatLng XIAN = new LatLng(34.341568, 108.940174);
    public static readonly LatLng ZHENGZHOU = new LatLng(34.7466, 113.625367);
}

}