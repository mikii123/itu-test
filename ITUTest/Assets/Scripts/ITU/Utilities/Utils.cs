using System;
using System.Collections.Generic;
using System.Linq;

namespace ITU.Utilities
{
	public static class Utils
	{
		public static T GetBestComparison<T>(this IEnumerable<T> enumerable, Func<T, T, bool> compare)
		{
			var items = enumerable.ToList();
			T lowest = items.First();
			
			foreach (T item in items)
			{
				if (compare(item, lowest))
				{
					lowest = item;
				}
			}

			return lowest;
		}
	}
}
