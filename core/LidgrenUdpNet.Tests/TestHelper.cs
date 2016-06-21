using System;
using System.Reflection;
using Lidgren.Network;

namespace LidgrenUdpNet
{
	public static class TestHelper
	{
		// from UnitTests.Program.Main
		public static NetPeer CreatePeer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("unittests");
			config.EnableUPnP = true;
			NetPeer peer = new NetPeer(config);
			peer.Start(); // needed for initialization
			return peer;
		}

		// from UnitTests.Program.CreateIncomingMessage
		public static NetIncomingMessage CreateIncomingMessage(byte[] fromData, int bitLength)
		{
			NetIncomingMessage inc = (NetIncomingMessage)Activator.CreateInstance(typeof(NetIncomingMessage), true);
			typeof(NetIncomingMessage).GetField("m_data", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inc, fromData);
			typeof(NetIncomingMessage).GetField("m_bitLength", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inc, bitLength);
			return inc;
		}
	}
}
