using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIScore : MonoBehaviour {
    //声明分数文本变量
    public Text txtScore;
    //声明等级文本变量
    public Text txtLevel;
    //表示分数的变量
    private int totScore = 0;
    //表示升级所需分数的数组
    private int[] levelUp = { 100, 200, 400, 600, 1000, 1400, 2000, 2600, 3200, 4000 };
    private int levelNow = 0;
    private GameMgr gameMgr;

    void Start () {
        //初次运行时加载之前保存的分数值
        totScore = PlayerPrefs.GetInt("TOT_SCORE", defaultValue: 0);
        DispScore(0);
        levelNow = 0;
        gameMgr = GameMgr.instance;
        StartCoroutine(MyUpdate());
    }

    void OnDisable()
    {
        PlayerPrefs.DeleteAll();
    }
    IEnumerator MyUpdate()
    {
        while (true)
        {
            CheckLevel();
            if (totScore > 0)
                totScore = totScore - 10 * ((totScore / 400) % 4);
            txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
            txtLevel.text = "LEVEL <color=#f0000>" + levelNow.ToString() + "</color>";
            yield return new WaitForSeconds(1.0f);
        }
    }
    //检查当前等级
    private void CheckLevel()
    {
        if (totScore > levelUp[levelNow])
        {
            levelNow++;
            gameMgr.CheckLevel();
        }
    }
    //累加分数并更新显示，声明为public函数供其他脚本调用
    public void DispScore(int score)
    {
        totScore += score;
    }
    //返回等级
    public int GetLevel()
    {
        return levelNow;
    }
    //返回分数
    public int GetScore()
    {
        return totScore;
    }
}
