using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SingleMonster_1300_Controller : SingleMonsterController
{
    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        InitStat();
        SingleMonsterId = 1300;
    }

    private void InitStat()
    {
        //TODO - 몬스터별 스탯 적용
        _stat.hp = 150;
        _stat.attack = 8;
        _stat.defense = 0;
        _stat.speed = 5;
        _stat.checkRange = 7;
        _stat.attackRange = 2;
        _stat.missDist = 20;
        _stat.attackCool = 1f;
    }
}
