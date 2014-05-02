using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Widget;

namespace JuzzPig.AMap.Demo.util
{public class ToastUtil {

    public static void show(Context context, String info) {
		Toast.MakeText(context, info,ToastLength.Long ).Show();
	}

	public static void show(Context context, int info) {
		Toast.MakeText(context, info, ToastLength.Long).Show();
	}}
}
