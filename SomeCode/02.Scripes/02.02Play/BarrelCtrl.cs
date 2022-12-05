using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {
    //表示爆炸效果的变量
    public GameObject expEffect;
    private Transform tr;

    //保存炸药桶剩余的血量
    private int expHP = 60;

    //要选择的随机纹理数组
    public Texture[] textures;

	void Start () {
        tr = GetComponent<Transform>();

        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
	}

    //爆炸触发的回调函数
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            //删除子弹
            Destroy(collision.gameObject);

            //计算炸弹桶剩余血量
            expHP -= collision.gameObject.GetComponent<BulletCtrl>().GetBulletDamage();

            if (expHP <= 0)
            {
                ExpBarrel();
            }
        }
    }
    //实现炸弹桶爆炸的函数
    void ExpBarrel()
    {
        //禁用碰撞箱
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        //生成爆炸效果的粒子
        GameObject expObject = (GameObject)Instantiate(expEffect, tr.position, Quaternion.identity);

        //以指定原点为中心，获取半径10.0f内的Collider对象
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);
        
        //对获取的Collider对象施加爆炸力
        foreach(Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if (rbody != null && coll.gameObject.tag != "BULLET")
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }

        //0s后删除炸药桶模型
        Destroy(gameObject);
        //Debug.Log(expObject.GetComponent<ParticleSystem>().main.duration);
        Destroy(expObject, 10f);
    }
    void OnDamage(object[] _params)
    {
        Vector3 firePoint = (Vector3)_params[0];
        Vector3 hitPoint = (Vector3)_params[1];
        Vector3 incomeVector = hitPoint - firePoint;
        incomeVector=incomeVector.normalized;
        //根据入射向量生成物理力
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1000f, hitPoint);

        expHP -= (int)_params[2];
        if (expHP <= 0)
        {
            ExpBarrel();
        }
    }
}
