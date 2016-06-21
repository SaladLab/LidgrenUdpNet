using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lidgren.Network;
using Xunit;
using Xunit.Abstractions;

namespace LidgrenUdpNet
{
	public class CommunicateTest
	{
		private ITestOutputHelper _output;

		public CommunicateTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public async Task ClientCommunicateWithServer()
		{
			// Arrange

			var server = CreateEchoServer();
			var clientTuple = CreateClient();
			var client = clientTuple.Item1;
			var recvStrs = clientTuple.Item2;
			var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, server.Server.Configuration.Port));
			Assert.True(connected);

			// Act

			var sentStrs = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				var str = "TestMessage:" + i;
				sentStrs.Add(str);
				var msg = client.Client.CreateMessage(str);
				client.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
			}

			await Task.Delay(1000);

			// Assert

			Assert.Equal(sentStrs, recvStrs);

			await client.StopAsync();
			await server.StopAsync();
		}


		[Fact]
		public async Task MutipleClientCommunicateWithServer()
		{
			// Arrange

			var server = CreateEchoServer();
			var clients = new List<TestSimpleClient>();
			var clientRecvStrss = new List<List<string>>();
			for (int i = 0; i < 10; i++)
			{
				var clientTuple = CreateClient();
				var client = clientTuple.Item1;
				var recvStrs = clientTuple.Item2;
				var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, server.Server.Configuration.Port));
				Assert.True(connected);
				clients.Add(client);
				clientRecvStrss.Add(recvStrs);
			}

			// Act

			var sentStrs = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				var str = "TestMessage:" + i;
				sentStrs.Add(str);
			}

			foreach (var client in clients)
			{
				foreach (var str in sentStrs)
				{
					var msg = client.Client.CreateMessage(str);
					client.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
				}
			}

			await Task.Delay(1000);

			// Assert

			foreach (var recvstrs in clientRecvStrss)
				Assert.Equal(sentStrs, recvstrs);

			foreach (var client in clients)
				await client.StopAsync();

			await server.StopAsync();
		}

		private TestSimpleServer CreateEchoServer()
		{
			var config = new NetPeerConfiguration("test");
			config.Port = 10101;
			var server = new TestSimpleServer(config, _output);
			server.OnData = msg =>
			{
				var str = msg.ReadString();
				var newMsg = msg.SenderConnection.Peer.CreateMessage(str);
				msg.SenderConnection.SendMessage(newMsg, NetDeliveryMethod.ReliableOrdered, 0);
			};
			server.Start();
			return server;
		}

		private Tuple<TestSimpleClient, List<string>> CreateClient()
		{
			var recvStrs = new List<string>();
			var config2 = new NetPeerConfiguration("test");
			var client = new TestSimpleClient(config2, _output);
			client.Start();
			client.OnData = msg =>
			{
				var str = msg.ReadString();
				recvStrs.Add(str);
			};
			return Tuple.Create(client, recvStrs);
		}
	}
}
