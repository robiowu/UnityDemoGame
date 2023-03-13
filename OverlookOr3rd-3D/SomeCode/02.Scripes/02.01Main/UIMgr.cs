using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour {
    public void OnClickStartBtn()
    {
        Debug.Log("Click StartBtn!");
        SceneManager.LoadScene("scPlay");
    }
    public void OnClickOptinBtn()
    {
        Debug.Log("Click Option!");
        SceneManager.LoadScene("Option");
    }
}
