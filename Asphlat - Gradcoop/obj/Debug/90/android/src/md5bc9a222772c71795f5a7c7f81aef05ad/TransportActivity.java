package md5bc9a222772c71795f5a7c7f81aef05ad;


public class TransportActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Asphalt__Gradcoop.Activities.TransportActivity, Asphlat - Gradcoop", TransportActivity.class, __md_methods);
	}


	public TransportActivity ()
	{
		super ();
		if (getClass () == TransportActivity.class)
			mono.android.TypeManager.Activate ("Asphalt__Gradcoop.Activities.TransportActivity, Asphlat - Gradcoop", "", this, new java.lang.Object[] {  });
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