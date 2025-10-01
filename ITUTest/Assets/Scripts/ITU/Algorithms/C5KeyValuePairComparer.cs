using C5;

namespace ITU.Algorithms
{
	internal class C5KeyValuePairComparer<T> : System.Collections.Generic.IComparer<KeyValuePair<T, int>>
	{
		public int Compare(KeyValuePair<T, int> x, KeyValuePair<T, int> y)
		{
			return x.Value.CompareTo(y.Value);
		}
	}
}
