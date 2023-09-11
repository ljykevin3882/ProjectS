using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;

namespace Server.Game.Room
{
    public class DataManager
    {
        public static DataManager Instance { get; } = new DataManager();
        // 레벨당 레벨업에 필요한 경험치
        public int[] ReqExpData;
        public Dictionary<WeaponType, PlayerStat> PlayerStatData = new Dictionary<WeaponType, PlayerStat>();
        public Dictionary<BuffType, BuffInfo> BuffData = new Dictionary<BuffType, BuffInfo>();
        public void LoadAllData()
        {
            LoadReqExpData();
            LoadPlayerStatData();
            LoadBuffData();
            Console.WriteLine("All data has been downloaded...");
        }
        void LoadReqExpData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/ReqExpData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 배열로 변환
                    ReqExp[] dataArray = JsonConvert.DeserializeObject<ReqExp[]>(json);

                    // reqExp 값을 int 배열에 넣기
                    ReqExpData = new int[dataArray.Length];
                    for (int i = 0; i < dataArray.Length; i++)
                    {
                        ReqExpData[i] = dataArray[i].reqExp;
                    }

                    //// 결과 출력
                    //foreach (int reqExp in ReqExpData)
                    //{
                    //    Console.WriteLine(reqExp);
                    //}
                    Console.WriteLine("ReqExpData Downloaded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Detected: " + ex.Message);
            }
        }
        void LoadPlayerStatData()
        {
            try
            {
                string url = "https://evenidemonickitchen.s3.ap-northeast-2.amazonaws.com/PlayerStatData.json";

                using (WebClient client = new WebClient())
                {
                    // JSON 데이터 가져오기
                    string json = client.DownloadString(url);

                    // JSON 데이터를 List<PlayerStat>으로 변환
                    List<PlayerStat> playerStats = JsonConvert.DeserializeObject<List<PlayerStat>>(json);

                    // 결과 출력
                    foreach (PlayerStat playerStat in playerStats)
                    {
                        WeaponType type = WeaponType.Default;

                        if (playerStat.WeaponType == "Pistol")
                            type = WeaponType.Pistol;
                        else if (playerStat.WeaponType == "Rifle")
                            type = WeaponType.Rifle;
                        else if (playerStat.WeaponType == "Sniper")
                            type = WeaponType.Sniper;
                        else if (playerStat.WeaponType == "Shotgun")
                            type = WeaponType.Shotgun;
                        else
                            Console.WriteLine($"Cant find WeaponType: {playerStat.WeaponType}");
                        PlayerStatData.Add(type, playerStat);
                    }
                    Console.WriteLine("PlayerStatData Downloaded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Detected: " + ex.Message);
            }
        }
        // TODO - JSON
        void LoadBuffData()
        {
            BuffData.Add(BuffType.Hp, new BuffInfo(BuffType.Hp, 1.30f, 60));
            BuffData.Add(BuffType.Speed, new BuffInfo(BuffType.Speed, 1.20f, 45));
            BuffData.Add(BuffType.Attack, new BuffInfo(BuffType.Attack, 1.20f, 45));
            BuffData.Add(BuffType.Sight, new BuffInfo(BuffType.Sight, 1.15f, 50));
            BuffData.Add(BuffType.Light, new BuffInfo(BuffType.Light, 2.0f, 75));
            Console.WriteLine("BuffData Downloaded");
        }
        public class ReqExp
        {
            public int reqExp { get; set; }
        }
        public class PlayerStat
        {
            public string WeaponType;
            public int MaxHp;
            public int Hp;
            public int Attack;
            public int Defense;
            public float Speed;
            public float CameraSize;
            public float CoolTime;
        }
        public class BuffInfo
        {
            public BuffType Type;
            public float IncreaseRate;
            public int Cost;
            public BuffInfo(BuffType type, float increaseRate, int cost)
            {
                Type = type;
                IncreaseRate = increaseRate;
                Cost = cost;
            }
        }
    }
}