using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//此类需要声明属性System.Serializable，表示可序列化
//显示到检视视图
[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour {

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

    //链接控制游戏的管理脚本
    private GameMgr gameMgr;

    //声明分配变量
    private Transform tr;
    //移速变量
    public float moveSpeed = 10.0f;
    //旋转速度变量（可运行时修改的鼠标灵敏度）
    public float rotspeed = 300.0f;

    //动画控制器变量
    //private Animator _animator;

    //要显示到检视视图的动画类型变量
    public Anim anim;

    //要访问下列3D模型的Animation组件对象的变量
    public Animation _animation;

	void Start () {
        Cursor.visible = false;

        //gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
        gameMgr = GameMgr.instance;
        //设置生命初始值
        initHp = hp;
        fHpPercent = 100.0f;
        HpPercent = imgHpbar.GetComponentInChildren<Text>();
        HpPercent.text = fHpPercent.ToString() + "%";

        //向脚本初始部分分配Transform组件，即获取此脚本绑定的对象的Transform组件
        tr = GetComponent<Transform>();

        //获取人物身上的动画组件控制器
        //_animator = GetComponentInChildren<Animator>();

        //查找位于自身下级的Animation组件并分配到变量
        _animation = GetComponentInChildren<Animation>();

        //保存并运行Animation组件的动画片段
        _animation.clip = anim.idle;
        _animation.Play();

    }
	
	
	void Update () {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        //Debug.Log("H=" + h.ToString());
        //Debug.Log("V=" + v.ToString());

        //计算前后左右移动方向向量(移动方向*移速)
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(移动方向*移速*位移值*持续时间——Time.deltaTime(上一帧到这一帧的持续时间),基准坐标系)
        //标准化方向向量使得主人公在任意方向上的移速相同
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        //以Vector3.up轴为基准以rotSpeed速度旋转
        tr.Rotate(Vector3.up * Time.deltaTime * rotspeed * Input.GetAxis("Mouse X"));

        if (v >= 0.1f)
        {
            //前进
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            //向右
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            //暂停时执行idle动画
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameMgr.instance.gameMode != GameMgr.GameMode._3D_TPS)
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
        Debug.Log("Player died!!");

        ////获取所有拥有MONSTER Tag的游戏对象
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        ////依次调用每个怪兽对玩家死亡的处理函数OnPlayerDie()
        ////SendMessageOptions.DontRequireReceiver选项可以使得当游戏对象没有第一个参数对应的函数时也不用做特殊处理
        //foreach (GameObject mo in monsters)
        //{
        //    mo.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        gameMgr.isGameOver = true;
        OnPlayDie();
    }
}
