using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {
    //子弹破坏力
    private int damage;
    //子弹存活时间
    private float livetime = 10.0f;
    //子弹发射速度
    public float speed = 1000.0f;

    void Start () {
        damage = GameMgr.instance.GetDamage();
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        Invoke("Clear", livetime);
	}
    public int GetBulletDamage()
    {
        return damage;
    }
    private void Clear()
    {
        Debug.Log("kill myself ... " + gameObject.GetInstanceID());
        Destroy(gameObject);
    }
}
