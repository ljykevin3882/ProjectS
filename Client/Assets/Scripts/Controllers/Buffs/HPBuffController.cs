using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using TMPro;

public class HPBuffController : BuffController
{
    private void Start()
    {
        _costText = transform.Find("CostText").GetComponent<TextMeshPro>();
        _costText.text = $"{Stat.Cost} ∞ÒµÂ";
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // «√∑π¿ÃæÓ∞° æ∆¿Ã≈€ »πµÊ Ω√ 
        if (collision.gameObject.name.Contains("Player") && _isBuffed == false && Owner == null)
        {
            Owner = collision.GetComponent<PlayerController>();
            if (Owner.Gold >= Stat.Cost)
            {
                _isBuffed = true;
                C_ChangeHp hpPacket = new C_ChangeHp();
                hpPacket.Hp = collision.GetComponent<CreatureController>().Hp;
                hpPacket.IsBuff = true;
                hpPacket.BuffId = Id;
                Managers.Network.Send(hpPacket);
                Debug.Log("HP Buff!");
                base.OnTriggerEnter2D(collision);
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
