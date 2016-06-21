using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Lidgren.Network;
using Xunit.Abstractions;

namespace LidgrenUdpNet
{
	public class TestSimpleClient
	{
		private NetClient _client;
		private Thread _thread;
		private ITestOutputHelper _output;
		private TaskCompletionSource<bool> _connectTcs;

		public string Name => "Client(" + _client.Configuration.AppIdentifier + ")";

		public NetClient Client => _client;
		public NetConnection Connection => _client.ServerConnection;

		public Action<NetIncomingMessage> OnData { get; set; }

		public TestSimpleClient(NetPeerConfiguration config, ITestOutputHelper output = null)
		{
			_client = new NetClient(config);
			_output = output;
		}

		public void Start()
		{
			_output?.WriteLine($"{Name}: Start");

			_client.Start();
			_thread = new Thread(WorkThead);
			_thread.Start();
		}

		public Task<bool> ConnectAsync(IPEndPoint endPoint, NetOutgoingMessage hailMessage = null)
		{
			_connectTcs = new TaskCompletionSource<bool>();

			_client.Connect(endPoint, hailMessage);

			return _connectTcs.Task;
		}

		public void Stop()
		{
			_output?.WriteLine($"{Name}: Stop");

			_client.Shutdown("Stop");
		}

		public async Task StopAsync()
		{
			Stop();

			while (_client.Status != NetPeerStatus.NotRunning)
			{
				await Task.Delay(10);
			}
		}

		private void WorkThead()
		{
			while (_client.Status == NetPeerStatus.Running)
			{
				var msg = _client.WaitMessage(100);
				if (msg == null)
					continue;

				switch (msg.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						var text = msg.ReadString();
						_output?.WriteLine($"{Name}: Message({msg.MessageType}) {text}");
						break;

					case NetIncomingMessageType.StatusChanged:
						var status = (NetConnectionStatus)msg.ReadByte();
						var reason = msg.ReadString();
						_output?.WriteLine($"{Name}: StatusChanged({status}) {reason}");

						if (status == NetConnectionStatus.Connected)
						{
							if (_connectTcs != null)
							{
								_connectTcs.TrySetResult(true);
								_connectTcs = null;
							}
						}
						else if (status == NetConnectionStatus.Disconnected)
						{
							if (_connectTcs != null)
							{
								_connectTcs.TrySetResult(false);
								_connectTcs = null;
							}
						}
						break;

					case NetIncomingMessageType.Data:
						_output?.WriteLine($"{Name}: Data Length={msg.LengthBytes}");
						OnData?.Invoke(msg);
						break;

					default:
						_output?.WriteLine($"{Name}: @{msg.MessageType}");
						break;
				}

				_client.Recycle(msg);
			}
		}
	}
}
