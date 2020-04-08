package mono.q.rorbin.badgeview;


public class Badge_OnDragStateChangedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		q.rorbin.badgeview.Badge.OnDragStateChangedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDragStateChanged:(ILq/rorbin/badgeview/Badge;Landroid/view/View;)V:GetOnDragStateChanged_ILq_rorbin_badgeview_Badge_Landroid_view_View_Handler:Q.Rorbin.Badgeview.IBadgeOnDragStateChangedListenerInvoker, Xamarin.BadgeView\n" +
			"";
		mono.android.Runtime.register ("Q.Rorbin.Badgeview.IBadgeOnDragStateChangedListenerImplementor, Xamarin.BadgeView", Badge_OnDragStateChangedListenerImplementor.class, __md_methods);
	}


	public Badge_OnDragStateChangedListenerImplementor ()
	{
		super ();
		if (getClass () == Badge_OnDragStateChangedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Q.Rorbin.Badgeview.IBadgeOnDragStateChangedListenerImplementor, Xamarin.BadgeView", "", this, new java.lang.Object[] {  });
	}


	public void onDragStateChanged (int p0, q.rorbin.badgeview.Badge p1, android.view.View p2)
	{
		n_onDragStateChanged (p0, p1, p2);
	}

	private native void n_onDragStateChanged (int p0, q.rorbin.badgeview.Badge p1, android.view.View p2);

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
