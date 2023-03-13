using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//声明需要声音发生器
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {
    public GameObject bullet;           //子弹预设
    public Transform firePos;           //子弹发射坐标
    private float fireSpeedInit;        //游戏开始时的初始射速
    public float fireSpeed = 5.0f;      //子弹射速，每秒射出子弹数
    private float bulletPerSecond;      //子弹射速，每颗子弹消耗的秒数
    private float workTime = 0f;        //实现子弹射速的待退火时间
    private bool canShoot = true;

    public AudioClip fireSfx;           //子弹发射声音
    /*//保存AudioSource组件的变量
    private AudioSource source = null;*/
    public MeshRenderer muzzleFlash;    //连接MuzzleFlash的MeshRenderer组件
    private GameObject laserBeam;       //镭射激光发射点游戏对象
    private bool isLaser = true;        //使用镭射还是子弹
    public int damageInit = 20;         //人物造成的基础伤害
    private int damageFinal;            //计算加成后人物的最终伤害

	void Start () {
        bulletPerSecond = 1 / fireSpeed;    //计算每发子弹射出间隔
        muzzleFlash.enabled = false;        //禁用MuzzleFlash MeshRenderer
        isLaser = GameMgr.instance.isLaserInSetting;
        laserBeam = transform.Find("FirePos/LaserBeam").gameObject;
        laserBeam.SendMessage("SetLaser", isLaser, SendMessageOptions.DontRequireReceiver);
        damageFinal = damageInit;
        fireSpeedInit = fireSpeed;
    }

	void Update () {
        //CheckDamage();
        if (workTime <= 0)
        {
            canShoot = true;
        }
        else
        {
            workTime -= Time.deltaTime;
        }
        //鼠标左键时调用Fire
        if (canShoot)
        {
            if (Input.GetMouseButton(0))
            {
                Fire();
                //OnLaser();
                workTime = bulletPerSecond;
                canShoot = false;
            }
        }
	}
    //Fire函数有生成子弹预设以及音效控制和动画生成
    void Fire()
    {
        if (!isLaser)
            OnBullet();
        else
            OnLaser();
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);
    }

    void CreateBullet()
    {
        //动态生成Bullet预设
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    void OnBullet()
    {
        CreateBullet(); //动态生成子弹的函数
        StartCoroutine(this.ShowMuzzleFlash()); //使用例程调用处理MuzzleFlash效果的函数
    }

    void OnLaser()
    {
        //Debug.Log(damageFinal.ToString());
        RaycastHit hit;     //获取被射线击中的游戏对象
        //通过Raycast函数发射射线，有游戏对象被击中时返回true，射线最大距离为100
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 100.0f))
        {
            //判断被击中的物体是否是怪物
            if (hit.collider.tag == "MONSTER")
            {
                //SendMassage函数要传递的参数
                object[] _params = new object[2];
                _params[0] = hit.point;
                _params[1] = damageFinal;
                hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
            }
            //判断被击中的物体是否是炸药桶
            if (hit.collider.tag == "BARREL")
            {
                //SendMassage函数要传递的参数
                object[] _params = new object[3];
                _params[0] = firePos.position;
                _params[1] = hit.point;
                _params[2] = damageFinal;
                hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //对时间内反复激活/禁用MuzzleFlash的效果例程
    IEnumerator ShowMuzzleFlash()
    {
        //随机更改MuzzleFlash的大小
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash绕z轴随机旋转
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        //激活使其显示
        muzzleFlash.enabled = true;
        //等待随机时间后禁用MeshRenderer组件
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        muzzleFlash.enabled = false;
    }

    public void CheckDamage(int damageGet)
    {
        damageFinal = damageGet;
    }

    public int GetPlayerDamage()
    {
        return damageFinal;
    }

    public void ChangeFireSpeed(int level)
    {
        level++;
        fireSpeed = fireSpeedInit * level;
        bulletPerSecond = 1 / fireSpeed;    //重新计算每发子弹射出间隔
        isLaser = GameMgr.instance.isLaserInSetting;
        laserBeam = transform.Find("FirePos/LaserBeam").gameObject;
        laserBeam.SendMessage("SetLaser", isLaser, SendMessageOptions.DontRequireReceiver);
    }
}
