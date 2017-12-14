using System;
using System.Collections.Generic;

namespace Numbers
{
	/// <summary>
	/// Finds pairs of numbers
	/// </summary>
	public static class PairFinder
	{
		/// <summary>
		/// Returns number pairs for the specified sum
		/// </summary>
		public static IEnumerable<Tuple<int, int>> FindPairsFor(this IList<int> list, int sum)
		{
			var result = new List<Tuple<int, int>>();
			var usedIndexes = new HashSet<int>();
			for (int i = 0; i < list.Count - 1; i++)
			{
				if (usedIndexes.Contains(i)) continue;

				for (int j = i + 1; j < list.Count; j++)
				{
					if (usedIndexes.Contains(j)) continue;
					var first = list[i];
					var second = list[j];
					if (first + second == sum)
					{
						usedIndexes.Add(j);
						result.Add(new Tuple<int, int>(first, second));
						break;
					}
				}
			}

			return result;
		}
	}
}