using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public int hp;
    public int attack;
    public int defense;
    public int speed;
    public int checkRange;
    public int attackRange;
    public int missDist;
    public float attackCool;
}

public class SingleMonsterController : CreatureController
{
    //private UI_EnemyHpBar _hpBar;
    public new Status _stat = new Status();

    protected SingleMyPlayerController _target;
    public bool _canChase = false;
    public bool _canAttack = true;
    protected int _singleMonsterId;
    public AudioSource AudioSource;

    public int SingleMonsterId { get { return _singleMonsterId; } set { _singleMonsterId = value; } }
    public SingleMyPlayerController Target { get { return _target; } set { _target = value; } }

    void Start()
    {
        Init();
    }
    void Update()
    {
        UpdateAnimation();
        CheckTarget();
        if (_target != null)
            Rotate();
    }
    protected override void Init()
    {
        base.Init();
        //InitStat();
        State = CreatureState.Idle;
        //_hpBar = Managers.Resource.Instantiate("UI/Scene/UI_EnemyHpBar").GetComponent<UI_EnemyHpBar>();
        //_hpBar.Monster = this;
        //_hpBar.transform.position = transform.position;
        AudioSource = GetComponent<AudioSource>();
        SetTarget();
    }
    protected override void UpdateAnimation()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Idle:
                //_animator.Play($"PLAYER_IDLE_{(int)WeaponType}");
                _animator.SetBool("isMoving", false);
                break;
            case CreatureState.Moving:
                //_animator.Play($"PLAYER_MOVE_{(int)WeaponType}");
                _animator.SetBool("isMoving", true);
                break;
            case CreatureState.Skill:
                // 여기는 attack 한번만 하는 걸로 하는게 좋을듯
                //UseSkill에서 실행
                break;
            case CreatureState.Dead:
                _animator.SetBool("isMoving", false);
                break;
        }
    }
    protected void Rotate()
    {
        Vector2 value = transform.position - _target.transform.position;
        transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
    }
    protected void SetTarget()
    {
        GameObject go = FindObjectOfType<SingleWeaponSelect>().gameObject;
        foreach(SingleMyPlayerController tg in go.GetComponentsInChildren<SingleMyPlayerController>())
            if (tg.gameObject.activeSelf)
                _target = tg;
    }
    protected void CheckTarget()
    {
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        if (dist <= _stat.checkRange)
            FollowTarget();
    }
    protected virtual void FollowTarget()
    {
        _canChase = true;
        State = CreatureState.Moving;
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        if (dist <= _stat.attackRange && _canAttack)
            StartCoroutine(CoAttack());

    }
    public override void OnDamaged()
    {
        //base.OnDamaged();
        FollowTarget();
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    private IEnumerator CoAttack()
    {
        _canAttack = false;
        _animator.SetBool("isMoving", false);
        _animator.SetTrigger("doAttack");
        _target.Hp -= _stat.attack;
        _target.OnDamaged();
        yield return new WaitForSeconds(_stat.attackCool);
        _canAttack = true;
    }
    public override void OnDead()
    {
        base.OnDead();
        //if (_hpBar != null)
        //    Destroy(_hpBar.gameObject);
    }
}
