using System.Collections.Generic;
using Android.Text;
using Android.Widget;
using Com.Amap.Api.Maps2d.Model;
using Com.Amap.Api.Services.Core;
using Java.Lang;
using Java.Text;
using Java.Util;

namespace JuzzPig.AMap.Demo.util
{

    public class AMapUtil
    {
        /**
	 * 判断edittext是否null
	 */

        public static string checkEditText(EditText editText)
        {
            if (editText != null && editText.Text != null
                && !(editText.Text.ToString().Trim().Equals("")))
            {
                return editText.Text.ToString().Trim();
            }
            else
            {
                return "";
            }
        }

        public static ISpanned stringToSpan(String src)
        {
            return src == null ? null : Html.FromHtml(src.Replace("\n", "<br />"));
        }

        public static string colorFont(String src, String color)
        {
            StringBuffer strBuf = new StringBuffer();

            strBuf.Append("<font color=").Append(color).Append(">").Append(src)
                .Append("</font>");
            return strBuf.ToString();
        }

        public static string makeHtmlNewLine()
        {
            return "<br />";
        }

        public static string makeHtmlSpace(int number)
        {
            string space = "&nbsp;";
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < number; i++)
            {
                result.Append(space);
            }
            return result.ToString();
        }

        public static string getFriendlyLength(int lenMeter)
        {
            if (lenMeter > 10000) // 10 km
            {
                int dis1 = lenMeter/1000;
                return dis1 + Chstring.Kilometer;
            }

            if (lenMeter > 1000)
            {
                float dis2 = (float) lenMeter/1000;
                DecimalFormat fnum = new DecimalFormat("##0.0");
                string dstr = fnum.Format(dis2);
                return dstr + Chstring.Kilometer;
            }

            if (lenMeter > 100)
            {
                int dis3 = lenMeter/50*50;
                return dis3 + Chstring.Meter;
            }

            int dis = lenMeter/10*10;
            if (dis == 0)
            {
                dis = 10;
            }

            return dis + Chstring.Meter;
        }

        public static bool IsEmptyOrNullString(String s)
        {
            return (s == null) || (s.Trim().Length == 0);
        }

        /**
	 * 把LatLng对象转化为LatLonPoint对象
	 */

        public static LatLonPoint convertToLatLonPoint(LatLng latlon)
        {
            return new LatLonPoint(latlon.Latitude, latlon.Longitude);
        }

        /**
	 * 把LatLonPoint对象转化为LatLon对象
	 */

        public static LatLng convertToLatLng(LatLonPoint latLonPoint)
        {
            return new LatLng(latLonPoint.Latitude, latLonPoint.Longitude);
        }

        /**
	 * 把集合体的LatLonPoint转化为集合体的LatLng
	 */

        public static List<LatLng> convertArrList(List<LatLonPoint> shapes)
        {
            List<LatLng> lineShapes = new List<LatLng>();
            foreach (var point in shapes)
            {
                 LatLng latLngTemp = AMapUtil.convertToLatLng(point);
                lineShapes.Add(latLngTemp);
            }
            return lineShapes;
        }

        /**
	 * long类型时间格式化
	 */

        public static string convertToTime(long time)
        {
            SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            Date date = new Date(time);
            return df.Format(date);
        }

        public static readonly string  HtmlBlack = "#000000";
        public static readonly string HtmlGray  = "#808080";
    }
}