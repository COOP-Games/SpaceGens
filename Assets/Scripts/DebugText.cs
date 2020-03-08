/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 06/12/2019
 * **************************************************/


using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour
{
    private static Text text;
    private static float timer = 0;
    private static float textTime = 0;

    private void Awake()
    {
        text = gameObject.GetComponent<Text>();
        text.text = "";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > textTime)
        {
            timer = 0;
            text.text = "";
        }
    }

    public static void Log(string text, Color color, float time)
    {
        if(DebugText.text != null)
        {
            DebugText.text.text = text;
            DebugText.text.color = color;
            DebugText.textTime = time;
        }
    }
}
