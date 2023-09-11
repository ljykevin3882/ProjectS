using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }

		public int RoomId;
		public int SessionId { get; set; }

		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// TODO - DB에서 긁어올 부분, 유저 정보
			MyPlayer = ObjectManager.Instance.Add<Player>();
			MyPlayer.Session = this;
			// 이후에 게임에 연결 부분 만들기
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			// TODO - 1번 방으로 고정 시키면 안 됨
			// 테스트 해보기
			GameRoom room = RoomManager.Instance.Find(RoomId);
			if (room == null)
			{
                Console.WriteLine("Cant Leave the game because room is null");
				return;
			}
			room.Push(room.LeaveGame, MyPlayer.Info.ObjectId, true);
			SessionManager.Instance.Remove(this);
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
		// 로비에서 멀티게임 누르면 실행되는 함수
		public void EnterRoom()
		{
			GameRoom room = RoomManager.Instance.FindGameRoomAndEnter(MyPlayer);
			RoomId = room.RoomId;
			Console.WriteLine($"Player Connected in GameRoom {room.RoomId}");
		}
	}
}
