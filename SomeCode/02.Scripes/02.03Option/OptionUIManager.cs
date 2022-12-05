using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionUIManager : MonoBehaviour {
    //静音设置
    public Toggle volunmToggle;
    private bool isSfxMute = false;
    public void OnSetMute()
    {
        isSfxMute = !volunmToggle.isOn;
        Debug.Log(volunmToggle.isOn.ToString() + "\t" + isSfxMute.ToString());
    }
    //音量控制
    public GameObject volunmSliderWithText;
    private float volunm = 0.0f;
    public void OnSetVolunm()
    {
        volunm = volunmSliderWithText.GetComponent<SliderWithText>().ShowValue();
        Debug.Log("volunm= " + volunm.ToString());
    }
    //游戏模式选择
    public ToggleGroup gameModeToggle;
    private GameMgr.GameMode gameMode = GameMgr.GameMode._2D_OVERLOOK;
    public void OnSetGameMode()
    {
        IEnumerable<Toggle> answersGroup = gameModeToggle.ActiveToggles();
        foreach(Toggle t in answersGroup)
        {
            switch (t.name)
            {
                case "2D":
                    gameMode = GameMgr.GameMode._2D_OVERLOOK;
                    break;
                case "3D":
                    gameMode = GameMgr.GameMode._3D_TPS;
                    break;
            }
            Debug.Log(t.name + " " + t.isOn.ToString() + " " + gameMode.ToString());
        }
    }
    //开火模式选择
    public ToggleGroup fireModeToggle;
    private bool isLaser = true;
    public void OnSetFireMode()
    {
        IEnumerable<Toggle> answersGroup = fireModeToggle.ActiveToggles();
        foreach(Toggle t in answersGroup)
        {
            switch (t.name)
            {
                case "Laser":
                    isLaser = true;
                    break;
                case "Bullet":
                    isLaser = false;
                    break;
            }
            Debug.Log(t.name + " " + t.isOn.ToString() + " " + isLaser.ToString());
        }
    }
    //亮度设置
    public GameObject lightSliderWithText;
    private float lightIntensity = 0f;
    public void OnSetLightIntensity()
    {
        lightIntensity = lightSliderWithText.GetComponent<SliderWithText>().ShowValue();
        Debug.Log("light= " + lightIntensity.ToString());
    }
    //确定设置(保存)
    public void OnSaveSetting()
    {
        string filepath = Application.persistentDataPath;
        string filename = "MyTestSetting.ini";
        Debug.Log(filepath + "/" + filename);
        StreamWriter sw = new StreamWriter(filepath + "/" + filename);
        sw.WriteLine("isSfxMute =" + isSfxMute.ToString());
        sw.WriteLine("volunm =" + volunm.ToString());
        sw.WriteLine("gameMode =" + gameMode.ToString());
        sw.WriteLine("isLaser =" + isLaser.ToString());
        sw.WriteLine("lightIntensity =" + lightIntensity.ToString());
        sw.Flush();
        sw.Close();
        SceneManager.LoadScene("scMain");
    }
    //取消设置
    public void OnCancelSetting()
    {
        SceneManager.LoadScene("scMain");
    }
}
