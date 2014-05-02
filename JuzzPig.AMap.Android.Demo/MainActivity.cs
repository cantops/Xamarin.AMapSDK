using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Lang;

namespace JuzzPig.AMap.AndroidDemo
{
  

/**
 * AMapV1地图demo总汇
 */
[Activity(Label = "JuzzPig.AMap.Android.Demo", MainLauncher = true, Icon = "@drawable/icon")]
public  class MainActivity : ListActivity {
	private  class DemoDetails:Java.Lang.Object {
	    public int titleId;
	    public int descriptionId;
	    public Type activityClass;

		public DemoDetails(int titleId, int descriptionId,Type  activityClass):base() {
			this.titleId = titleId;
			this.descriptionId = descriptionId;
			this.activityClass = activityClass;
		}
	}

	private  class CustomArrayAdapter : ArrayAdapter<DemoDetails> {
		public CustomArrayAdapter(Context context, DemoDetails[] demos):base(context,Resource.Layout.feature,Resource.Id.title,demos) {
			
		}

		public override View GetView(int position, View convertView, ViewGroup parent) {
			FeatureView featureView;
			if (convertView is FeatureView) {
				featureView = (FeatureView) convertView;
			} else {
				featureView = new FeatureView(Context);
			}
			DemoDetails demo = GetItem(position);
			featureView.setTitleId(demo.titleId);
			featureView.setDescriptionId(demo.descriptionId);
			return featureView;
		}
	}

	private static  DemoDetails[] demos = {
			new DemoDetails(Resource.String.basic_map, Resource.String.basic_description,
					typeof(BasicMapActivity)),
			new DemoDetails(Resource.String.camera_demo, Resource.String.camera_description,
					typeof(CameraActivity)),
			new DemoDetails(Resource.String.events_demo, Resource.String.events_description,
					typeof(EventsActivity)),
			new DemoDetails(Resource.String.layers_demo, Resource.String.layers_description,
				typeof(LayersActivity)),
			new DemoDetails(Resource.String.mapOption_demo,
					Resource.String.mapOption_description,typeof( MapOptionActivity)),
			new DemoDetails(Resource.String.screenshot_demo,
					Resource.String.screenshot_description, typeof(ScreenShotActivity)),
			new DemoDetails(Resource.String.uisettings_demo,
					Resource.String.uisettings_description,typeof( UiSettingsActivity)),
			new DemoDetails(Resource.String.polyline_demo,Resource.String.polyline_description,typeof( PolylineActivity)),
			new DemoDetails(Resource.String.polygon_demo,Resource.String.polygon_description, typeof(PolygonActivity)),
			new DemoDetails(Resource.String.circle_demo, Resource.String.circle_description,typeof(CircleActivity)),
			new DemoDetails(Resource.String.marker_demo, Resource.String.marker_description,typeof(MarkerActivity)),
			new DemoDetails(Resource.String.groundoverlay_demo,Resource.String.groundoverlay_description,typeof(GroundOverlayActivity)),
			new DemoDetails(Resource.String.tileoverlay_demo,Resource.String.tileoverlay_description, typeof(TileOverlayActivity)),
			new DemoDetails(Resource.String.geocoder_demo,
					Resource.String.geocoder_description,typeof( GeocoderActivity)),//.class),
			new DemoDetails(Resource.String.locationsource_demo,Resource.String.locationsource_description,typeof(LocationSourceActivity)),
			new DemoDetails(Resource.String.locationGPS_demo,Resource.String.locationGPS_description,typeof( LocationGPSActivity)),
			new DemoDetails(Resource.String.locationNetwork_demo,Resource.String.locationNetwork_description,typeof(LocationNetworkActivity)),
			new DemoDetails(Resource.String.poikeywordsearch_demo,Resource.String.poikeywordsearch_description,typeof(PoiKeywordSearchActivity)),
			new DemoDetails(Resource.String.poiaroundsearch_demo,Resource.String.poiaroundsearch_description,
				typeof(	PoiAroundSearchActivity)),
			new DemoDetails(Resource.String.busline_demo,
					Resource.String.busline_description,typeof( BuslineActivity)),
			new DemoDetails(Resource.String.route_demo, Resource.String.route_description,
					typeof(RouteActivity)) };

	protected override void OnCreate(Bundle savedInstanceState) {
		base.OnCreate(savedInstanceState);
		SetContentView(Resource.Layout.main_activity);
		var adapter = new CustomArrayAdapter(
				this.ApplicationContext, demos);
		ListAdapter=(adapter);
	}

	public override void OnBackPressed() {
		base.OnBackPressed();
		JavaSystem.Exit(0);
	}

	protected override void OnListItemClick(ListView l, View v, int position, long id) {
		DemoDetails demo = (DemoDetails) ListAdapter.GetItem(position);
		StartActivity(new Intent(this.ApplicationContext,
				demo.activityClass));
	}
}

}