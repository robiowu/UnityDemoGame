using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject attacker;
    public float skillRate = 1f;    //���ܱ���
    public float damageRate = 1f;   //������
    public bool isSnapShot = false; //��ǰ�˺������Ƿ��ǿ��չ���
    public Vector2 knockBack = Vector2.zero;
    private Component attackSript;

    private void Awake()
    {
        attackSript = attacker.GetComponent(typeof(IAttacker));
        if (attackSript is null)
        {
            throw new NotAttackerException(attacker);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable == null) 
            return;
        BaseDamageProperty attackDamageProperty = attackSript.GetType().GetMethod("DamageProperty")?.Invoke(attackSript, null) as BaseDamageProperty;
        //BaseDamageProperty attackDamageProperty = temp;
        if (isSnapShot)
            attackDamageProperty = new BaseDamageProperty(attackDamageProperty);
        if (attackDamageProperty == null)
            return;
        damageable.Hit((int)(attackDamageProperty.Damage * skillRate * damageRate), knockBack, attacker);
    }
}
