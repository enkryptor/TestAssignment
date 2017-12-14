using System.Collections.Generic;
using System.Linq;

using Numbers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class NumberPairsTests
	{
		/// <summary>
		/// Checks basic requirements
		/// </summary>
		[TestMethod]
		public void PairsTest()
		{
			var testData = new List<int> { 1, 1, 2, 1, 1, 0, 1 };
			var result = testData.FindPairsFor(2).ToList();

			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(2, result.FindAll(x => x.Item1 == 1 && x.Item2 == 1).Count);
			Assert.AreEqual(1, result.FindAll(x => x.Item1 == 0 && x.Item2 == 2 || x.Item2 == 0 && x.Item1 == 2).Count);
		}
	}
}