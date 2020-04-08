package md50019e6a13af7474b3676b071e62834d1;


public class CustomAdapter_ViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FTSAFE.Adapter.CustomAdapter+ViewHolder, FTSAFE", CustomAdapter_ViewHolder.class, __md_methods);
	}


	public CustomAdapter_ViewHolder ()
	{
		super ();
		if (getClass () == CustomAdapter_ViewHolder.class)
			mono.android.TypeManager.Activate ("FTSAFE.Adapter.CustomAdapter+ViewHolder, FTSAFE", "", this, new java.lang.Object[] {  });
	}

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
