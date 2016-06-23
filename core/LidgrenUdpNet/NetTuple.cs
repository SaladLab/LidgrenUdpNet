using System;
using System.Collections.Generic;
using System.Text;

namespace Lidgren.Network
{
	// replace with BCL 4.0 Tuple<> when appropriate
	internal struct NetTuple<A, B>
	{
		public A Item1;
		public B Item2;

		public NetTuple(A item1, B item2)
		{
			Item1 = item1;
			Item2 = item2;
		}
	}

	// replace with BCL 4.0 Tuple<> when appropriate
	internal struct NetTuple<A, B, C>
	{
		public A Item1;
		public B Item2;
		public C Item3;

		public NetTuple(A item1, B item2, C item3)
		{
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
		}
	}
}
