using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//此类需要声明属性System.Serializable，表示可序列化
//显示到检视视图
//[System.Serializable]
//public class Anim
//{
//    public AnimationClip idle;
//    public AnimationClip runForward;
//    public AnimationClip runBackward;
//    public AnimationClip runRight;
//    public AnimationClip runLeft;
//}

public class PlayerCtrlOverlook : MonoBehaviour
{

    //声明委派和事件
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayDie;

    private float h = 0.0f;
    private float v = 0.0f;
    public int hp = 100;
    private int initHp;
    public Image imgHpbar;
    private Text HpPercent;
    private float fHpPercent;
    private GameMgr gameMgr;        //链接控制游戏的管理脚本
    private Transform tr;           //声明分配变量        
    public float moveSpeed = 10.0f; //移速变量
    public Anim anim;               //要显示到检视视图的动画类型变量
    public Animation _animation;    //要访问下列3D模型的Animation组件对象的变量

    public float sceneHeight = 0f; //调试时功能，正式上线时去除

    void Start()
    {
        gameMgr = GameMgr.instance;
        //设置生命初始值
        initHp = hp;
        fHpPercent = 100.0f;
        HpPercent = imgHpbar.GetComponentInChildren<Text>();
        HpPercent.text = fHpPercent.ToString() + "%";

        //向脚本初始部分分配Transform组件，即获取此脚本绑定的对象的Transform组件
        tr = GetComponent<Transform>();

        //查找位于自身下级的Animation组件并分配到变量
        _animation = GetComponentInChildren<Animation>();
        //保存并运行Animation组件的动画片段
        _animation.clip = anim.idle;
        _animation.Play();

    }


    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        //计算前后左右移动方向向量(移动方向*移速)
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.World);
        Vector3 temp = Input.mousePosition;
        temp = Camera.main.ScreenToWorldPoint(temp);
        //取消高度的方向向量，使得look at的方向是角色同一水平面的方向
        temp = temp - Vector3.up * (Camera.main.transform.position.y - sceneHeight);
        tr.LookAt(temp);
        float rot = angle360_xOz(tr.forward, moveDir);
        //Debug.Log(rot.ToString());
        if (h != 0 || v != 0)
        {
            if (rot >= 0 && rot < 90)
            {
                //播放动画前进
                _animation.CrossFade(anim.runForward.name, 0.3f);
            }
            else if (rot >= 90 && rot < 180)
            {
                //播放动画向左
                _animation.CrossFade(anim.runLeft.name, 0.3f);
            }
            else if (rot >= 180 && rot < 270)
            {
                //播放动画向后
                _animation.CrossFade(anim.runBackward.name, 0.3f);
            }
            else if (rot >= 270 && rot < 360)
            {
                //播放动画向右
                _animation.CrossFade(anim.runRight.name, 0.3f);
            }
        }
        else
        {
            //暂停时执行idle动画
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameMgr.instance.gameMode != GameMgr.GameMode._2D_OVERLOOK)
            return;
        if ((!gameMgr.isGameOver) && other.gameObject.tag == "PUNCH") 
        {
            hp -= other.gameObject.GetComponentInParent<MonsterCtrl>().punchDamage;
            //调整血条的显示长度
            fHpPercent = (float)hp / (float)initHp;
            imgHpbar.fillAmount = fHpPercent;
            HpPercent.text = (fHpPercent * 100).ToString() + "%";

            //Debug.Log("hp=" + hp.ToString());
            if (hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        gameMgr.isGameOver = true;
        OnPlayDie();
    }

    float angle360_xOz(Vector3 from_, Vector3 to_)
    {
        Vector3 v3 = Vector3.Cross(from_, to_);
        if (v3.y > 0)
            return Vector3.Angle(from_, to_);
        else
            return 360 - Vector3.Angle(from_, to_);
    }
}
