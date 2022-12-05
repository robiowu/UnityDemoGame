using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour {
    //声明表示怪兽状态信息的Enumerable变量
    public enum MonsterState { idle, trace, attack, die };
    //保存怪兽当前状态的Enum变量
    public MonsterState monsterState = MonsterState.idle;

    //为了提高速度而分配的各个变量
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;
    //声明GameUI对象
    private GameUIScore gameUIScore;

    public int punchDamage = 10;    //攻击伤害
    public int hp = 200;            //生命膜力
    public int score = 50;          //每只的分数
    public GameObject bloodEffect;  //身上的血渍
    public GameObject bloodDecal;   //地上血渍贴图预设
    public float traceDist = 100.0f; //追击范围
    public float attachDist = 2.0f; //攻击范围
    private bool isDead = false;    //怪兽是否死亡

    private void OnEnable()
    {
        if(GameMgr.instance.gameMode==GameMgr.GameMode._2D_OVERLOOK)
            PlayerCtrlOverlook.OnPlayDie += this.OnPlayerDie;
        else
            PlayerCtrl.OnPlayDie += this.OnPlayerDie;
    }
    private void OnDisable()
    {
        if (GameMgr.instance.gameMode == GameMgr.GameMode._2D_OVERLOOK)
            PlayerCtrlOverlook.OnPlayDie -= this.OnPlayerDie;
        else
            PlayerCtrl.OnPlayDie -= this.OnPlayerDie;
    }

    void Start () {
        //获取怪兽的Transform组件
        monsterTr = this.gameObject.GetComponent<Transform>();
        //获取怪兽要追击的对象的Transform组件
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //获取NavMeshAgent组件
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
        //为了修正怪物数量而获取GameMgr脚本对象
        //gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();


        //运行定期检查怪兽当前状态的协程函数
        StartCoroutine(this.CheckMonsterState());
        //运行定期根据怪兽当前状态更改怪兽的行动模式
        StartCoroutine(this.Monsteraction());

        //获取分数板脚本对象
        gameUIScore = GameObject.Find("GameUI").GetComponent<GameUIScore>();
	}
	
	void Update () {
		
	}

    //定期检查怪兽当前状态并更新monsterState值
    IEnumerator CheckMonsterState()
    {
        while (!isDead)
        {
            //等待0.2s后再执行后续代码
            yield return new WaitForSeconds(0.2f);

            //测量怪兽与玩家的距离
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attachDist)
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }

    //根据怪兽当前状态执行适当的状态
    IEnumerator Monsteraction()
    {
        while (!isDead)
        {
            switch (monsterState)
            {
                //idle状态
                case MonsterState.idle:
                    //追击停止
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    //追击再启动
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;
                case MonsterState.attack:
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }

    //检查与子弹的碰撞
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(collision.transform.position);
            hp -= collision.gameObject.GetComponent<BulletCtrl>().GetBulletDamage();
            if (hp <= 0)
            {
                MonsterDie();
            }
            Destroy(collision.gameObject);
            animator.SetTrigger("IsHit");
        }
    }

    void CreateBloodEffect(Vector3 pos)
    {
        //生成血渍效果
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 0.2f);

        //贴图生成位置，在脚下稍微抬上一点避免贴图闪烁
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //随机设置贴图旋转值
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //生成贴图预设
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //调整贴图尺寸大小随机
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood2, 5f);
    }

    void OnPlayerDie()
    {
        if (!isDead)
        {
            //停止所有检测怪兽状态的协程
            StopAllCoroutines();
            nvAgent.isStopped = true;
            animator.SetTrigger("IsPlayerDie");
        }
    }

    void MonsterDie()
    {
        //gameMgr.monsterCount--;
        //将死亡的怪兽标签设为Untagged
        gameObject.tag = "Untagged";
        //停止所有协程
        StopAllCoroutines();
        isDead = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;

        //禁用碰撞箱
        //GetComponentInChildren是获取当前对象以及其子对象中名字为<>内的组件，若有多个（如当前对象有，子对象也有，则会以TransformSort顺序返回最上面的那个组件）

        //gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;     //由于当前对象有CapsuleCollider组件，因此和上面注释掉的话是一个意思

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        gameUIScore.DispScore(score);
        animator.SetTrigger("IsDead");
        //Debug.Log("monster die!");
        Destroy(gameObject, 5.0f);
    }

    void OnDamage(object[] _params)
    {
        CreateBloodEffect((Vector3)_params[0]);
        hp -= (int)_params[1];
        if (hp <= 0)
        {
            MonsterDie();
        }
        animator.SetTrigger("IsHit");
    }
}
