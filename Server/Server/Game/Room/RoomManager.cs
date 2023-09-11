using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class RoomManager
	{
		// 싱글톤
		public static RoomManager Instance { get; } = new RoomManager();

		object _lock = new object();
		Dictionary<int, GameRoom> _rooms = new  Dictionary<int, GameRoom>();
		int _roomId = 1;
		public int MaxPlayer = 3;
		public int DelayBeforeStartGame = 3;
		public Dictionary<int, GameRoom> Rooms { get { return _rooms; } set { _rooms = value; } }

		// 룸 생성	
		public GameRoom Add()
		{
			GameRoom newRoom = new GameRoom();
			newRoom.Push(newRoom.Init);

            lock (_lock)
            {
				// 임의로 안정성 추가한 코드 TODO
				// 이미 방이 있을 때
				if (_rooms.ContainsKey(_roomId))
                {
                    Console.WriteLine($"That room already exist {_roomId}");
					return null;
				}

				newRoom.RoomId = _roomId;
				_rooms.Add(_roomId, newRoom);
				_roomId++;

				return newRoom;
			}
        }
		// 룸 삭제
		public bool Remove(int roomId)
        {
            lock (_lock)
			{
				// 원래 코드였음 TODO
				//return _rooms.Remove(roomId);

				if (_rooms.ContainsKey(roomId))
					return _rooms.Remove(roomId);
				else
				{
					Console.WriteLine($"That room already removed {_roomId}");
					return false;
				}
			}
		}
		public GameRoom Find(int roomId)
        {
            lock (_lock)
			{
				GameRoom room = null;
				if (_rooms.TryGetValue(roomId, out room))
					return room;

                Console.WriteLine($"Cant Find Room {roomId}");
				Console.WriteLine("<Remain Room List>");
				foreach (GameRoom r in RoomManager.Instance.Rooms.Values)
				{
					Console.WriteLine("Remain Room " + r.RoomId);
				}
				return null;
			}
        }
        public GameRoom FindGameRoomAndEnter(Player player)
        {
            if (player == null)
                return null;
            lock (_lock)
			{
				foreach (GameRoom room in RoomManager.Instance.Rooms.Values)
				{
					if (RoomManager.Instance.Find(room.RoomId).IsGameStart == false && RoomManager.Instance.Find(room.RoomId).IsFull == false)
					{
						Console.WriteLine("Find Room! "+ room.RoomId);
						room.Push(room.EnterGame, player);
						return room;
                    }
				}
                // 활성화된 방이 없으면 방 하나 새로 만들기
                Console.WriteLine($"Make new Room {RoomManager.Instance._roomId}");
				GameRoom newRoom = RoomManager.Instance.Add();
				newRoom.Push(newRoom.EnterGame, player);
				return newRoom;
			}
        }
    }
}
