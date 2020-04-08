using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace FTSAFE
{
    [Activity(Label = "隐患整改查询")]
    public class HidenSearchActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hiden_search);
        }
    }
}