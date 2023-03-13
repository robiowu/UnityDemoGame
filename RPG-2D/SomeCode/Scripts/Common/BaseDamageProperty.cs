using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class BaseDamageProperty
{
    //角色的基本数据。用来确定是否保存快照的攻击
    private int baseAtk = 10;
    private float atkExtraMulti = 0f;
    private int extraAtk = 0;

    //这两个倍数放到Attack.cs中处理。因为Attack.cs是用来绑定到Collider上的，不同的skill攻击方式对应的Collider应该是不同的
    //private float skillRate = 1f;    //技能倍率
    //private float damageRate = 1f;   //增伤区

    private float criticalChance = 0f;
    private float criticalRate = 1.5f;

    public int Atk => (int)(baseAtk * (1 + atkExtraMulti) + extraAtk);

    public int Damage => (int)(Random.value < criticalChance ? Atk * criticalRate : Atk);

    public BaseDamageProperty(int baseAtk, float atkExtraMulti, int extraAtk, float criticalChance = 0f, float criticalRate = 1f)
    {
        this.baseAtk = baseAtk;
        this.atkExtraMulti = atkExtraMulti;
        this.extraAtk = extraAtk;
        //this.skillRate = skillRate;
        //this.damageRate = damageRate;
        this.criticalChance = criticalChance;
        this.criticalRate = criticalRate;
    }

    public BaseDamageProperty(BaseDamageProperty copy)
    {
        baseAtk = copy.baseAtk;
        atkExtraMulti = copy.atkExtraMulti;
        extraAtk = copy.extraAtk;
        criticalChance = copy.criticalChance;
        criticalRate = copy.criticalRate;
    }

    public void ChangeBaseAtk(int baseAtk) => this.baseAtk = baseAtk;
    public void ChangeAtkExtraMulti(float atkExtraMulti) => this.atkExtraMulti = atkExtraMulti;
    public void ChangeExtraAtk(int extraAtk) => this.extraAtk = extraAtk;
    public void ChangeCriticalChance(float criticalChance) => this.criticalChance = criticalChance;
    public void ChangeCriticalRate(float criticalRate) => this.criticalRate = criticalRate;

}
