package md561c7616142430b792c2b57a6f32fcc1a;


public class ChartAnimator
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Com.Syncfusion.Charts.ChartAnimator, Syncfusion.SfChart.XForms.Android", ChartAnimator.class, __md_methods);
	}


	public ChartAnimator ()
	{
		super ();
		if (getClass () == ChartAnimator.class)
			mono.android.TypeManager.Activate ("Com.Syncfusion.Charts.ChartAnimator, Syncfusion.SfChart.XForms.Android", "", this, new java.lang.Object[] {  });
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
