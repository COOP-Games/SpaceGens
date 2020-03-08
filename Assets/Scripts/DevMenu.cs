/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 8/10/2019
 * **************************************************/


using UnityEngine;
using UnityEngine.UI;


public class DevMenu : MonoBehaviour
{
    [SerializeField]
    private Text fpsCounter;

    private float then = 0;


    void Start()
    {
        
    }


    void Update()
    {
        if(gameObject.activeSelf)
        {
            float now = Time.time;

            fpsCounter.text = (now - then).ToString();

            then = now;
        }
    }


    public void SetTimeScale(Slider slider)
    {
        Time.timeScale = slider.value;
    }
}
