package md5d10853154d5e61612980e1cdbeb04e52;


public class HidenAuitActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("FTSAFE.HidenAuitActivity, FTSAFE", HidenAuitActivity.class, __md_methods);
	}


	public HidenAuitActivity ()
	{
		super ();
		if (getClass () == HidenAuitActivity.class)
			mono.android.TypeManager.Activate ("FTSAFE.HidenAuitActivity, FTSAFE", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}