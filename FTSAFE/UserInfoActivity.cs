using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace FTSAFE
{
    [Activity(Label = "我的信息")]
    public class UserInfoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_user_info);

        
        }
    }
}