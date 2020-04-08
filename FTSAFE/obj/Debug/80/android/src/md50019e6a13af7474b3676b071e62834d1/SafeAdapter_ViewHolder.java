package md50019e6a13af7474b3676b071e62834d1;


public class SafeAdapter_ViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FTSAFE.Adapter.SafeAdapter+ViewHolder, FTSAFE", SafeAdapter_ViewHolder.class, __md_methods);
	}


	public SafeAdapter_ViewHolder ()
	{
		super ();
		if (getClass () == SafeAdapter_ViewHolder.class)
			mono.android.TypeManager.Activate ("FTSAFE.Adapter.SafeAdapter+ViewHolder, FTSAFE", "", this, new java.lang.Object[] {  });
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
