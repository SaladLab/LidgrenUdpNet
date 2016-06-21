using System;
using Lidgren.Network;
using Xunit;

namespace LidgrenUdpNet
{
	// From UnitTests.MiscTests
	public class MiscTest
	{
		[Fact]
		public void TestScenario()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("Test");

			config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
			if (config.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData) == false)
				throw new NetException("setting enabled message types failed");

			config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, false);
			if (config.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData) == true)
				throw new NetException("setting enabled message types failed");

			Console.WriteLine("Misc tests OK");

			Console.WriteLine("Hex test: " + NetUtility.ToHexString(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }));

			if (NetUtility.BitsToHoldUInt64((ulong)UInt32.MaxValue + 1ul) != 33)
				throw new NetException("BitsToHoldUInt64 failed");
		}
	}
}
