using System;
using System.Threading;
using Lidgren.Network;
using Xunit.Abstractions;
using System.Threading.Tasks;

namespace LidgrenUdpNet
{
	public class TestSimpleServer
	{
		private NetServer _server;
		private Thread _thread;
		private ITestOutputHelper _output;

		public string Name => "Server(" + _server.Configuration.AppIdentifier + ")";

		public NetServer Server => _server;

		public Action<NetIncomingMessage> OnData { get; set; }

		public TestSimpleServer(NetPeerConfiguration config, ITestOutputHelper output = null)
		{
			_server = new NetServer(config);
			_output = output;
		}

		public void Start()
		{
			_output?.WriteLine($"{Name}: Start");

			_server.Start();
			_thread = new Thread(WorkThead);
			_thread.Start();
		}

		public void Stop()
		{
			_output?.WriteLine($"{Name}: Stop");

			_server.Shutdown("Stop");
		}

		public async Task StopAsync()
		{
			Stop();

			while (_server.Status != NetPeerStatus.NotRunning)
			{
				await Task.Delay(10);
			}
		}

		private void WorkThead()
		{
			while (_server.Status == NetPeerStatus.Running)
			{
				var msg = _server.WaitMessage(100);
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
						break;

					case NetIncomingMessageType.Data:
						_output?.WriteLine($"{Name}: Data Length={msg.LengthBytes}");
						OnData?.Invoke(msg);
						break;

					default:
						_output?.WriteLine($"{Name}: @{msg.MessageType}");
						break;
				}

				_server.Recycle(msg);
			}
		}
	}
}
