

using Android.Content;
using Android.Views;
using Android.Widget;

namespace JuzzPig.AMap.AndroidDemo
{

    public class FeatureView: FrameLayout
    {
        private Context _mContext;
    public FeatureView(Context context):base(context)
    {

        _mContext = context;
        LayoutInflater layoutInflater = (LayoutInflater) context
            .GetSystemService(Context.LayoutInflaterService);
        layoutInflater.Inflate(Resource.Layout.feature, this);
    }


    public void setTitleId(int titleId)
    {
        ((TextView) (FindViewById(Resource.Id.title))).Text = _mContext.GetString(titleId);
    }

    public void setDescriptionId(int descriptionId)
    {
        ((TextView) (FindViewById(Resource.Id.description))).Text=_mContext.GetString((descriptionId));
    }

}
}