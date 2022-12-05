using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour {
    //声明要使用的粒子效果
    public GameObject sparkEffect;

    //碰撞开始时触发的事件
    private void OnCollisionEnter(Collision coll)
    {
        //比较发生碰撞的游戏对象的Tag值
        if (coll.collider.tag == "BULLET")
        {
            //动态生成火花粒子并保存到变量中
            GameObject spark = (GameObject)Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //经过ParticleSystem组件的duration时间后删除
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            Destroy(coll.gameObject);
        }
    }

}
