using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDJ_EffectLight : MonoBehaviour
{
    #region F/P
    Light thisLight;
    float lightIntensity;
    [SerializeField]
    float minIntensity = 2f;
    [SerializeField]
    float maxIntensity = 5f;
    [SerializeField]
    Color lightColor;
    [SerializeField]
    float ChangeIntensitySpeed = 1f;

    #endregion

    #region Meths
    void changeLightSettings()
    {
        lightIntensity = Mathf.PingPong(Time.time * ChangeIntensitySpeed,maxIntensity);
        thisLight.intensity = lightIntensity;
        thisLight.color = lightColor;
    }
    void GetLight()
    {
        thisLight = GetComponent<Light>();
    }
    #endregion

    #region UniMeths
    private void Start()
    {
        GetLight();
    }
    private void Update()
    {
        changeLightSettings();
    }
    #endregion
}
