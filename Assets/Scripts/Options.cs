using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixerGroup mixer;
    public Slider slide;

    /*--- настройка громкости музыки ---*/
    public void Music(float volume)
    {
        Value(ref volume, slide.value);
        mixer.audioMixer.SetFloat("Music", volume);
        PlayerPrefs.SetFloat("music", slide.value);
        PlayerPrefs.Save();
    }

    /*--- настройка громкости эффектов ---*/
    public void Platform(float volume)
    {
        Value(ref volume, slide.value);
        mixer.audioMixer.SetFloat("Sound", volume);
        PlayerPrefs.SetFloat("sound", slide.value);
        PlayerPrefs.Save();
        //Debug.Log("save");
    }

    /*--- без вот этого громкость отказывалась работать ---*/
    static public void Value(ref float volume, float val)
    {
        switch (val)
        {
            case 10: volume = 0f; break;
            case 9: volume = -8f; break;
            case 8: volume = -16f; break;
            case 7: volume = -24f; break;
            case 6: volume = -32f; break;
            case 5: volume = -40f; break;
            case 4: volume = -48f; break;
            case 3: volume = -56f; break;
            case 2: volume = -64f; break;
            case 1: volume = -72f; break;
            case 0: volume = -80f; break;
            default: break;
        }
    }

}