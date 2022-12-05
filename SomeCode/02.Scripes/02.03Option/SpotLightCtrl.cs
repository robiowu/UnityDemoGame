using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightCtrl : MonoBehaviour {
    private Light spotLight;
	void Start () {
        spotLight = gameObject.GetComponent<Light>();
        //Debug.Log("spotLight.intensity= " + spotLight.intensity.ToString());
        spotLight.intensity = spotLight.intensity + GameMgr.instance.lightIntensityAddend;
        //Debug.Log("spotLight.intensity= " + spotLight.intensity.ToString());
    }
	
}
