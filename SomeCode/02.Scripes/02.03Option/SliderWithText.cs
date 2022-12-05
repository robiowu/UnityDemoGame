using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithText : MonoBehaviour {
    public Slider slider;
    public InputField inputField;
    private float value;
    public void S_to_I()
    {
        inputField.text = slider.value.ToString();
        value = slider.value;
    }
    public void I_to_S()
    {
        try
        {
            float temp = (float)System.Convert.ToDouble(inputField.text);
            if (temp > slider.maxValue)
            {
                temp = slider.maxValue;
                S_to_I();
                Debug.Log("Get upper");
            }
            else if(temp < slider.minValue)
            {
                temp = slider.minValue;
                S_to_I();
                Debug.Log("Get lower");
            }
            slider.value = temp;
            value = slider.value;
        }
        catch(System.FormatException ex)
        {
            Debug.Log("FormatException: " + ex.Message);
        }
    }
    public float ShowValue()
    {
        return value;
    }
}
