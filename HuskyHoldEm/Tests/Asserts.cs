using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyHoldEm.Tests
{
	class Asserts
	{
		// Assert the given boolean expression is true
		public static void AssertTrue(bool result, string failMsg)
		{
			if (!result)
			{
				throw new Exception(failMsg);
			}
		}

		// Assert the given boolean expression is false
		public static void AssertFalse(bool result, string failMsg)
		{
			if (result)
			{
				throw new Exception(failMsg);
			}
		}

		// Assert the first object is equal to the second object
		public static void Assert(object given, object expected, string failMsg)
		{
			if (!given.Equals(expected))
			{
				throw new Exception(failMsg);
			}
		}
	}
}
