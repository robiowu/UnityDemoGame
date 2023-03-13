using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2, GameObject> damageHitEvent;  //������˺�ʱ�������˺��������⣬��������Ķ����˺���������Ϣ�¼�
    private Animator animator;
    [SerializeField] private int _maxHealth = 100;
    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = Math.Max(value, 1);
    }

    [SerializeField]
    private int _currentHealth;

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Math.Min(value, MaxHealth);
            // ��hp<=0ʱ�����������¼�ί��
            if (_currentHealth <= 0 && IsAlive)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField] private bool _isAlive = true;

    public bool IsAlive
    {
        get => _isAlive;
        private set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            //Debug.Log(this.gameObject.name + " 's IsAlive is set to " + value);
        }
    }

    [SerializeField] private bool _isInvincible = false;     //�Ƿ��޵�״̬

    public bool IsInvincible
    {
        get => _isInvincible;
        set => _isInvincible = value;
    }

    /*ͨ���޵�֡���ж��޵�״̬
    [SerializeField] private int _invincibilityFrameNumber = 5;      //ʹ���޵�֡���ж����������޵�ʱ��
    private int _currentInvincibilityFrame;
    public int InvincibilityFrameNumber
    {
        get => _invincibilityFrameNumber;
        set => _invincibilityFrameNumber = Math.Max(value, 5);
    }
    */

    [SerializeField] private float _invincibilityTime = 0.25f;      //ʹ���޵�ʱ�����ж����������޵�֡
    private float timeSinceHit = 0;
    public float InvincibilityTime
    {
        get => _invincibilityTime;
        set => _invincibilityTime = Math.Max(value, 0.25f);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInvincible)
        {
            //ͨ���޵�ʱ�����ж��޵�״̬
            if (timeSinceHit > InvincibilityTime)
            {
                IsInvincible = false;
                timeSinceHit = 0;
            }
            else 
                timeSinceHit += Time.deltaTime;
            /* ͨ���޵�֡���ж��޵�״̬
            if (_currentInvincibilityFrame <= 0)
            {
                IsInvincible = false;
                _currentInvincibilityFrame = 0;
            }
            else 
                _currentInvincibilityFrame -= 1;
            */
        }
    }
    /// <summary>
    /// �������ɹ�������������˺���
    /// </summary>
    /// <param name="damage">��ɵ��˺���</param>
    /// <param name="knockback">�˴��˺������Ļ�������</param>
    /// <param name="attacker">�����ķ���</param>
    /// <returns></returns>
    public bool Hit(int damage, Vector2 knockback, GameObject attacker)
    {
        if (IsAlive && !IsInvincible)
        {
            CurrentHealth -= damage;
            IsInvincible = true;
            //_currentInvincibilityFrame = InvincibilityFrameNumber;
            animator.SetTrigger(AnimationStrings.hitTrigger);
            damageHitEvent?.Invoke(damage, knockback, attacker);
            return true;
        }
        return false;
    }
}
