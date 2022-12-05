using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
public class GameMgr : MonoBehaviour {
    private GameObject player1;             //玩家变量
    private GameUIScore gameUIScore;        //声明GameUI对象
    public Transform[] points;              //保存随机生成怪兽位置的数组
    public GameObject monsterPrefab;        //要分配的怪兽预设
    public int monsterCount = 0;            //场上的怪物数
    public float createTime = 2.0f;         //生成怪兽的周期
    public int maxMonster = 10;             //生成怪兽的最大数量
    public bool isGameOver = false;         //控制是否终止游戏的变量
    public static GameMgr instance = null;  //单例模式
    public float sfxVolunm = 1.0f;          //声明表示总音量的变量，用于声音共享函数
    public bool isSfxMute = false;          //静音功能
    public bool isLaserInSetting = true;    //是否使用镭射，在设置面板中初始化
    public float lightIntensityAddend = 0.0f;     //亮度数值(加数)
    //枚举游戏模式
    public enum GameMode { _3D_TPS , _2D_OVERLOOK };
    public GameMode gameMode = GameMode._2D_OVERLOOK;

    void Awake()
    {
        instance = this;
        LoadSetting();
    }
    void Start () {
        Debug.Log("GameMgr Starting ...");
        //获取层次视图SpawnPoint下的所有Transform组件
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            StartCoroutine(CreateMonster());
        }
        player1 = GameObject.FindGameObjectWithTag("Player");
        switch (gameMode)
        {
            case GameMode._2D_OVERLOOK:
                player1.GetComponent<PlayerCtrl>().enabled = false;
                player1.GetComponent<PlayerCtrlOverlook>().enabled = true;
                Camera.main.GetComponent<Kamera>().enabled = false;
                Camera.main.GetComponent<KameraOverlook>().enabled = true;
                break;
            case GameMode._3D_TPS:
                player1.GetComponent<PlayerCtrl>().enabled = true;
                player1.GetComponent<PlayerCtrlOverlook>().enabled = false;
                Camera.main.GetComponent<Kamera>().enabled = true;
                Camera.main.GetComponent<KameraOverlook>().enabled = false;
                break;
        }
        //获取分数板脚本对象
        gameUIScore = GameObject.Find("GameUI").GetComponent<GameUIScore>();
    }
    IEnumerator CreateMonster()
    {
        //无限循环直到游戏结束
        while (!isGameOver)
        {
            //当前场上的怪兽数量
            int monsterCountTag = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;
            if (monsterCountTag < maxMonster)
            {
                //程序挂起一段时间（怪兽生成周期）
                yield return new WaitForSeconds(createTime);
                int index = UnityEngine.Random.Range(1, points.Length);
                Instantiate(monsterPrefab, points[index].position, points[index].rotation);
                //monsterCount++;
            }
            else
            {
                yield return null;
            }
        }
    }
    //声音共享函数
    public void PlaySfx(Vector3 pos,AudioClip sfx)
    {
        if (isSfxMute)
            return;
        //动态生成游戏对象
        GameObject soundObj = new GameObject("Sfx");
        //移动到指定发声位置
        soundObj.transform.position = pos;
        //添加发生器
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        //设置AudioSource组件的属性
        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = sfxVolunm;
        //播放声音
        audioSource.Play();

        //播放声音结束之后删除对应游戏对象,sfx.length表示函数参数传过来的音效对象的时间长度
        Destroy(soundObj, sfx.length);
    }

    public int GetDamage()
    {
        return player1.GetComponent<FireCtrl>().GetPlayerDamage();
    }
    public GameObject GetPlayer()
    {
        return player1;
    }
    private void LoadSetting()
    {
        string filepath = Application.persistentDataPath;
        string filename = "MyTestSetting.ini";
        try
        {
            using (StreamReader sr = new StreamReader(filepath + "/" + filename))
            {
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    sb.Remove(0, sb.Length);
                    sb.Append(sr.ReadLine());
                    string[] temp = sb.ToString().Split(new char[] { ' ', '\r', '\n', '=' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Log("Rand=" + temp.Rank);
                    Debug.Log("GetLength=" + temp.GetLength(0));
                    if (temp.GetLength(0) < 2)
                    {
                        Debug.Log("The message in config.ini is wrong for less than 2 words per line");
                    }
                    else
                    {
                        ReadKey(temp[0], temp[1]);
                    }
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning(e.ToString());
            Debug.LogWarning("setting not found, we will use default setting");
        }
        finally
        {
            Debug.Log("LoadSetting over");
        }
    }
    private void ReadKey(string key, string value)
    {
        try
        {
            switch (key)
            {
                case "isSfxMute":
                    isSfxMute = Convert.ToBoolean(value);
                    break;
                case "volunm":
                    sfxVolunm = (float)Convert.ToDouble(value);
                    break;
                case "gameMode":
                    switch (value)
                    {
                        case "_3D_TPS":
                            gameMode = GameMode._3D_TPS;
                            break;
                        case "_2D_OVERLOOK":
                            gameMode = GameMode._2D_OVERLOOK;
                            break;
                    }
                    break;
                case "isLaser":
                    isLaserInSetting = Convert.ToBoolean(value);
                    break;
                case "lightIntensity":
                    lightIntensityAddend = (float)Convert.ToDouble(value);
                    break;
            }
        }       
        catch (FormatException ex)
        {
            Debug.Log("FormatException: "+ex.Message);
        }
        catch (OverflowException ex)
        {
            Debug.Log("OverflowException: "+ex.Message);
        }
    }
    
    public void CheckLevel()
    {
        LevelChange(gameUIScore.GetLevel());
    }
    private void LevelChange(int level)
    {
        player1.GetComponent<FireCtrl>().ChangeFireSpeed(level);
        switch (level)
        {
            case 4:
                isLaserInSetting = true;
                break;
            case 5:
                isLaserInSetting = true;
                break;
            case 6:
                isLaserInSetting = true;
                break;
            case 7:
                isLaserInSetting = true;
                break;
        }
    }
}
