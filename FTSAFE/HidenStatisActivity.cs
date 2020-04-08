using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace FTSAFE
{
    [Activity(Label = "隐患统计")]
    public class HidenStatisActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_hiden_statisaxml);
        }
    }
}