/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 11/07/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using UnityEngine;


/// <summary>
/// 
/// </summary>
public class SoundWave : MonoBehaviour
{
    [Header("Hierarchy References")]
    [SerializeField]
    [Tooltip("Line Renderer.")]
    private LineRenderer lineRenderer;
    [SerializeField]
    [Tooltip("Audio source de onde o algoritmo irá obter dados de frequencia.")]
    private AudioSource audioSource;
    [SerializeField]
    [Tooltip("Animator da onda.")]
    private Animator anim;

    [Header("Preferences")]
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Amplitude da onda exibida.")]
    private float amplitude = 1;
    [SerializeField]
    [Range(64, 256)]
    [Tooltip("Numero de amostras.")]
    private int samples = 256;
    [SerializeField]
    [Range(0f,1f)]
    [Tooltip("Intervalo para desenho da onda.")]
    private float clampThreshold = 0.05f;


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        float[] spectrum = new float[samples];

        //audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        audioSource.GetOutputData(spectrum, 0);

        for (int i = 0; i < spectrum.Length; i++)
        {
            //lineRenderer.SetPosition(i, new Vector3(i / 100f, Mathf.Clamp(spectrum[i] * (0.5f + i * i), 0, 0.5f) / 5, 1));
            lineRenderer.SetPosition(i, new Vector3(i / 100f, Mathf.Clamp(amplitude * spectrum[i], -clampThreshold, clampThreshold), 1));
        }
    }


    /// <summary>
    /// Oculta a onda.
    /// </summary>
    public void HideWave()
    {
        anim.Play("hide_soundwave");
    }


    /// <summary>
    /// Exibe a onda.
    /// </summary>
    public void DisplayWave()
    {
        anim.Play("show_soundwave");
    }
}
