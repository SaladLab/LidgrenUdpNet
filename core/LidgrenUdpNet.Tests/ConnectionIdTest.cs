using System;
using System.Collections.Generic;
using Lidgren.Network;
using Xunit;

namespace LidgrenUdpNet
{
	public class ConnectionIdTest
	{
		[Fact]
		public void TestMakeConnectionIdUnique()
		{
			var ids = new HashSet<long>();
			for (int i = 0; i < 100; i++)
			{
				var id = NetUtility.MakeConnectionId();
				Assert.NotEqual(0, id);
				var done = ids.Add(id);
				Assert.True(done);
			}
		}
	}
}
