using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChange : MonoBehaviour
{
    public Slider slider;
    public string audio;
    public bool isBG;
    void Start()
    {
        
        if (isBG)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(audio, 0.65f);
            slider.value = 0.65f;
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(audio, 0.65f);
            slider.value = 0.65f;
        }
    }

    public void SetVolume(float vol)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(audio, vol);
        slider.value = vol;
    }
}
