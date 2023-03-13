using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2, GameObject> damageHitEvent;  //当造成伤害时，除了伤害产生以外，触发其余的订阅伤害产生的消息事件
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
            // 当hp<=0时，触发死亡事件委托
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

    [SerializeField] private bool _isInvincible = false;     //是否无敌状态

    public bool IsInvincible
    {
        get => _isInvincible;
        set => _isInvincible = value;
    }

    /*通过无敌帧来判断无敌状态
    [SerializeField] private int _invincibilityFrameNumber = 5;      //使用无敌帧来判定，而不是无敌时间
    private int _currentInvincibilityFrame;
    public int InvincibilityFrameNumber
    {
        get => _invincibilityFrameNumber;
        set => _invincibilityFrameNumber = Math.Max(value, 5);
    }
    */

    [SerializeField] private float _invincibilityTime = 0.25f;      //使用无敌时间来判定，而不是无敌帧
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
            //通过无敌时间来判定无敌状态
            if (timeSinceHit > InvincibilityTime)
            {
                IsInvincible = false;
                timeSinceHit = 0;
            }
            else 
                timeSinceHit += Time.deltaTime;
            /* 通过无敌帧来判断无敌状态
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
    /// 用来给可攻击的物体造成伤害的
    /// </summary>
    /// <param name="damage">造成的伤害量</param>
    /// <param name="knockback">此次伤害附带的击退能力</param>
    /// <param name="attacker">攻击的发起方</param>
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
