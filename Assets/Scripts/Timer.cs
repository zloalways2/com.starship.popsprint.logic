using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float sec;
    public float min;
    public TextMeshProUGUI timeTxt;
    public bool startTimer;

    // Start is called before the first frame update
    void Start()
    {
        min = 2;
    }

    private void Update()
    {
        if (startTimer == true)
        {
            sec -= Time.deltaTime;
            if (sec < 0.1f)
            {
                if (min > 0)
                {
                    min--;
                    sec = 59.9f;
                }
                else
                    sec = 0;
            }
            timeTxt.text = min.ToString() + ":" + sec.ToString("00");
        }

        if (min <= 0 && sec <= 0)
            startTimer = false;
    }
}
