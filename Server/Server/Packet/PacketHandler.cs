using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return; 
		GameRoom room = player.Room;
		if (room == null)
			return;

		// TODO - 보안 추가 가능한 부분 -> 서버에서 클라 인증 한번 해주는 부분 원하며 넣으면 될듯?
		// TODO
		room.Push(room.HandleMove, player, movePacket);
        //Console.WriteLine($"Move ({player.PosInfo.PosX}, {player.PosInfo.PosY})");
		//clientSession.MyPlayer.PosInfo = movePacket.PosInfo;
		//player.PosInfo.PosX = movePacket.PosInfo.PosX + movePacket.MovePosInfo.PosX;
		//player.PosInfo.PosY = movePacket.PosInfo.PosY + movePacket.MovePosInfo.PosY;
		//player.PosInfo.RotZ = movePacket.PosInfo.RotZ;
    }
	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSkill, player, skillPacket);
	}
	public static void C_HitPlayerHandler(PacketSession session, IMessage packet)
	{
		C_HitPlayer hitPacket = packet as C_HitPlayer;
		ClientSession clientSession = session as ClientSession;

		Bullet bullet = ObjectManager.Instance.Find<Bullet>(hitPacket.BulletId);
		Player hitter = bullet.Owner as Player;
		//Player hitter = clientSession.MyPlayer;
		//Player hitter = ObjectManager.Instance.Find<Player>(hitPacket.HitterObjectInfo.ObjectId);
		if (hitter == null)
		{
			Console.WriteLine("Hitter is Null");
			return;
		}
		GameRoom room = hitter.Room;
		if (room == null)
		{
			Console.WriteLine("Room is Null");
			return;
		}
		// 나 이외의 플레이어, 몬스터는 Enemy로 판단하기
		GameObjectType enemyType = ObjectManager.Instance.GetObjectTypeById(hitPacket.EnemyObjectInfo.ObjectId);

		if (enemyType == GameObjectType.Player)
		{
			Player player = ObjectManager.Instance.Find<Player>(hitPacket.EnemyObjectInfo.ObjectId);
			if (player == null)
			{
				Console.WriteLine($"Enemy is Null ID: {hitPacket.EnemyObjectInfo.ObjectId}");
				return;
			}
			room.Push(room.HandleHitPlayer, hitter, player, enemyType, hitPacket);
		}
		else if (enemyType == GameObjectType.Monster)
		{
			Monster monster = ObjectManager.Instance.Find<Monster>(hitPacket.EnemyObjectInfo.ObjectId);
			if (monster == null)
			{
				Console.WriteLine($"Enemy is Null ID: {hitPacket.EnemyObjectInfo.ObjectId}");
				return;
			}
			room.Push(room.HandleHitPlayer, hitter, monster, enemyType, hitPacket);
		}

	}
	public static void C_HitMonsterHandler(PacketSession session, IMessage packet)
	{
		C_HitMonster hitPacket = packet as C_HitMonster;
		ClientSession clientSession = session as ClientSession;

		//if (hitPacket.HitterObjectInfo.PosInfo.State != CreatureState.Moving)
		//	return;

		GameObjectType type = ObjectManager.Instance.GetObjectTypeById(hitPacket.HitterObjectInfo.ObjectId);

		if (type == GameObjectType.Player)
		{
			Bullet bullet = ObjectManager.Instance.Find<Bullet>(hitPacket.BulletId);
			Player hitter = bullet.Owner as Player;
			if (hitter == null)
			{
				Console.WriteLine("Hitter is Null");
				return;
			}
			GameRoom room = hitter.Room;
			if (room == null)
			{
				Console.WriteLine("Room is Null");
				return;
			}
			// 나 이외의 플레이어, 몬스터는 Enemy로 판단하기
			GameObjectType enemyType = ObjectManager.Instance.GetObjectTypeById(hitPacket.EnemyObjectInfo.ObjectId);

			if (enemyType == GameObjectType.Monster)
			{
				Monster monster = ObjectManager.Instance.Find<Monster>(hitPacket.EnemyObjectInfo.ObjectId);
				if (monster == null)
				{
					Console.WriteLine($"Enemy is Null ID: {hitPacket.EnemyObjectInfo.ObjectId}");
					return;
				}
				room.Push(room.HandleHitMonster, hitter, monster, enemyType, hitPacket);
			}
		}
		else if (type == GameObjectType.Monster)
		{
			Monster hitter = ObjectManager.Instance.Find<Monster>(hitPacket.HitterObjectInfo.ObjectId);
			if (hitter == null)
			{
				Console.WriteLine("Hitter is Null");
				return;
			}
			GameRoom room = hitter.Room;
			if (room == null)
			{
				Console.WriteLine("Room is Null");
				return;
			}
			// 나 이외의 플레이어, 몬스터는 Enemy로 판단하기
			GameObjectType enemyType = ObjectManager.Instance.GetObjectTypeById(hitPacket.EnemyObjectInfo.ObjectId);

			if (enemyType == GameObjectType.Player)
			{
				Player player = ObjectManager.Instance.Find<Player>(hitPacket.EnemyObjectInfo.ObjectId);
				if (player == null)
				{
					Console.WriteLine($"Enemy is Null ID: {hitPacket.EnemyObjectInfo.ObjectId}");
					return;
				}
				room.Push(room.HandleMonsterHitPlayer, hitter, player, enemyType, hitPacket);
			}
		}
	}
	// 플레이어가 매칭 방을 잡을 때
	public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
	{
		C_EnterRoom C_EnterRoomPacket = packet as C_EnterRoom;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
		{
			clientSession.MyPlayer.WeaponType = C_EnterRoomPacket.WeaponType;
            Console.WriteLine($"Player Id {clientSession.MyPlayer.Id}'s PlayerType {clientSession.MyPlayer.WeaponType}");
			player.Init();
			clientSession.EnterRoom();
			//RoomManager.Instance.FindGameRoomAndEnter(player);
			room = player.Room;
		}
		room.Push(room.HandleEnterPlayerInLobby, clientSession.MyPlayer, C_EnterRoomPacket);
	}
	// 플레이어가 잡은 방에서 나갈 때
	public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		C_LeaveGame C_EnterRoomPacket = packet as C_LeaveGame;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandleLeaveLobby, player);
	}
	public static void C_LevelUpHandler(PacketSession session, IMessage packet)
	{
		C_LevelUp levelUpPacket = packet as C_LevelUp;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandlePlayerLevelUp, player);
	}
	// 클라에서 경험치 얻었다는 정보 주는 용도
	// 상점 때 이용하면 될듯
	// 상점 때 이용하면 될듯
	public static void C_GetExpHandler(PacketSession session, IMessage packet)
	{
		C_GetExp expPacket = packet as C_GetExp;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandlePlayerGetExp, player, expPacket);
	}
	// 상점 때 이용하면 될듯
	public static void C_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		C_ChangeStat statPacket = packet as C_ChangeStat;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandleChangeStat, player, statPacket);
	}
	// 상태 바뀔 때(State)
	public static void C_ChangeStateHandler(PacketSession session, IMessage packet)
	{
		C_ChangeState statePacket = packet as C_ChangeState;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleChangeState, player, statePacket);
	}
	public static void C_ChangeRotzHandler(PacketSession session, IMessage packet)
	{
		C_ChangeRotz rotZPacket = packet as C_ChangeRotz;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandleChangeRotZ, player, rotZPacket);
	}
	public static void C_OutGameHandler(PacketSession session, IMessage packet)
	{
		C_OutGame outPacket = packet as C_OutGame;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.LeaveGame, outPacket.ObjectId, false);
	}
	public static void C_CheckInfoHandler(PacketSession session, IMessage packet)
	{
		C_CheckInfo checkPacket = packet as C_CheckInfo;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		////Console.WriteLine("클라에서 플레이어 각도: " + player.Info.PosInfo.RotZ);
	}
	public static void C_ChangeWeaponTypeHandler(PacketSession session, IMessage packet)
	{
		C_ChangeWeaponType weaponPacket = packet as C_ChangeWeaponType;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleChangeWeaponType, player, weaponPacket);
	}
	public static void C_PlaySoundHandler(PacketSession session, IMessage packet)
	{
		C_PlaySound soundPacket = packet as C_PlaySound;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		
		room.Push(room.HandlePlaySound, player, soundPacket);
	}
	public static void C_ChangeGoldHandler(PacketSession session, IMessage packet)
	{
		C_ChangeGold goldPacket = packet as C_ChangeGold;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleChangeGold, player, goldPacket);
	}
	public static void C_UseTeleportHandler(PacketSession session, IMessage packet)
	{
		C_UseTeleport teleportPacket = packet as C_UseTeleport;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		//Console.WriteLine(teleportPacket.PosInfo.State);
		room.Push(room.HandleUseTeleport, player, teleportPacket);
	}
	public static void C_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		C_ChangeHp hpPacket = packet as C_ChangeHp;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		//Console.WriteLine(teleportPacket.PosInfo.State);
		room.Push(room.HandleChangeHp, player, hpPacket);
	}
	public static void C_ChangeSpeedHandler(PacketSession session, IMessage packet)
	{
		C_ChangeSpeed speedPacket = packet as C_ChangeSpeed;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		//Console.WriteLine(teleportPacket.PosInfo.State);
		room.Push(room.HandleChangeSpeed, player, speedPacket);
	}
	public static void C_ChangeAttackHandler(PacketSession session, IMessage packet)
	{
		C_ChangeAttack attackPacket = packet as C_ChangeAttack;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		//Console.WriteLine(teleportPacket.PosInfo.State);
		room.Push(room.HandleChangeAttack, player, attackPacket);
	}
	public static void C_ChangeSightHandler(PacketSession session, IMessage packet)
	{
		C_ChangeSight sightPacket = packet as C_ChangeSight;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		//Console.WriteLine(teleportPacket.PosInfo.State);
		room.Push(room.HandleChangeSight, player, sightPacket);
	}
}
