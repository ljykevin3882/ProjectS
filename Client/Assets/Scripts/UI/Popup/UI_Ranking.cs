using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Firebase;
using Firebase.Database;
using System;
using TMPro;
//using UnityEngine.AddressableAssets;
public class UI_Ranking : UI_Scene
{
    public Button Button_Back;
    DatabaseReference reference;
    public TextMeshProUGUI[] Nickname, Score;
    int rankingCount = 10;

    public struct UserData
    {
        public string userID ;
        public string userNickname;
        public int Diamond;
        public int playCount; 
        public int winCount ;
        
        public UserData(string USERID, string USERNICKNAME, int DIAMOND, int PLAYCOUNT, int WINCOUNT)
        {
            userID = USERID;
            userNickname = USERNICKNAME;
            Diamond = DIAMOND;
            playCount = PLAYCOUNT;
            winCount = WINCOUNT;
        }
    }
    public List<UserData> rankList;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.PurgeOutstandingWrites();
        FirebaseDatabase.DefaultInstance.GoOnline();
        Button_Back = GameObject.Find("Button_Back").GetComponent<Button>();
        Button_Back.onClick.AddListener(ClosePopup);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        rankList = new List<UserData>();
        ReadDB();
        //SortScores();
        //PrintRanking();

    }
    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            Nickname[i + 1].text = rankList[i].userNickname;
            Score[i + 1].text = rankList[i].winCount.ToString();
            if (rankList[i].userNickname == PlayerPrefs.GetString("userName"))
            {
                Nickname[0].text = rankList[i].userNickname;
                Score[0].text = rankList[i].winCount.ToString();
            }
        }
    }
    private void RenderRankingName()
    {
    }
    private void ClosePopup()
    {
        Managers.Sound.Play("Effect/ClickButton");
        Managers.Game.ChooseCharacter.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ReadDB()
    {
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var data in snapshot.Children)
                {
                    if (data.Child("userID").Value != null)
                    {
                        
                        string Id = data.Child("userID").Value.ToString();
                        string Nickname = data.Child("userNickname").Value.ToString();
                        int Diamond = Int32.Parse(data.Child("Diamond").Value.ToString());
                        int Playcount = Int32.Parse(data.Child("playCount").Value.ToString());
                        int Wincount = Int32.Parse(data.Child("winCount").Value.ToString());
                        UserData user = new UserData();
                        user.userID = Id;
                        user.userNickname = Nickname;
                        user.Diamond = Diamond;
                        user.playCount = Playcount;
                        user.winCount = Wincount;
                        rankList.Add(user);
                        //print("���� ���̵�:" + user.userID+ " ���� �г���:" + rankList[i].userNickname + " ���̾Ƹ��:" + data.Child("Diamond").Value + " �÷��� Ƚ��:" + data.Child("playCount").Value + " �¸� Ƚ��:" + data.Child("winCount").Value);


                    }
                }               
            }
            rankList.Sort((x, y) => y.winCount.CompareTo(x.winCount)); //�����ϱ� 
            
            for (int i = 0; i < rankList.Count; i++) 
            {
                print(": �̸� - " + rankList[i].userNickname + ", winCount -" + rankList[i].winCount); //����Ʈ���� �� ���ĵǾ ��
            }
            

        });//���� ������ ������ rankList�� �ʱ�ȭ �Ǵ� ���������� �ȿ�������
        

    }
    //public void SortScores() //�������� ����
    //{
    //    rankList.Sort((x, y) => y.Diamond.CompareTo(x.Diamond));
    //}
    //public void PrintRanking() //��ŷ ǥ��
    //{
    //    Debug.Log("Current Ranking Count:"+rankList.Count);

    //    for (int i = 0; i < rankList.Count; i++)
    //    {
    //        print("Rank " + (i + 1) + ": �̸� - " + rankList[i].userNickname);
    //    }
    //}
}
