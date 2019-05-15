
namespace JTUtility
{
	public delegate void Action();
	public delegate void Action<T1>(T1 t1);
	public delegate void Action<T1,T2>(T1 t1, T2 t2);
	public delegate void Action<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
	public delegate void Action<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
	public delegate TR Func<TR>();
	public delegate TR Func<TR, T1>(T1 t1);
	public delegate TR Func<TR, T1, T2>(T1 t1, T2 t2);
	public delegate TR Func<TR, T1, T2, T3>(T1 t1, T2 t2, T3 t3);
	public delegate TR Func<TR, T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
}
