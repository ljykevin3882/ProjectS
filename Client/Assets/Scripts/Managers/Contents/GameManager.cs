using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public float DelayBeforeStartGame;
    // 로비에 모인 플레이어의 수
    public int LobbyPlayer = 1;
    // 게임에 입장하면 부여 받는 플레이어 ID
    public int MyPlayerId = 0;
    // 한 방에 최대 인원수
    public int MaxPlayer;
    // 게임 시작 여부(timer 등 맞추기 위함)
    private bool _isStartGame = false;
    public bool IsStartGame
    {
        get { return _isStartGame; }
        set 
        { 
            _isStartGame = value;
            // 게임 입장하고 나서 IsGameStart가 True일 때 게임 Bgm 실행
            Managers.Sound.ChangeBgmWhenSceneLoaded();
        }
    }
    public WeaponType MyPlayerWeaponType = WeaponType.Pistol;
    public int ElapsedTime = 0;
    // 포탈 파괴하기 위해 포탈 폴더 가져오기
    GameObject portals;
    public GameObject ChooseCharacter;
    public void HandleDestroyPortal(int areaId)
    {
        if (portals == null)
            portals = GameObject.Find("Portals");

        GameObject areaPortals = portals.transform.Find($"Section {areaId}").gameObject;
        Managers.Resource.Destroy(areaPortals);
    }
}
