/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 11/07/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using UnityEngine;
using UnityEngine.Audio;


// Formato de gravação do audio.
public enum AudioFormat
{
    WAV,
    MP3
}


/// <summary>
/// Classe para controle da reprodução e gravação de audio.
/// </summary>
public class AudioController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField]
    private AudioMixer mixer;

    [Header("Sources")]
    [SerializeField]
    private AudioSource recordingSource;
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource musicSource;

    [Header("References")]
    [SerializeField]
    private Animator anim;

    [Header("Properties")]
    [SerializeField]
    private AudioFormat audioFormat = AudioFormat.WAV;
    [SerializeField]
    private string mixerPropToMute = "LevelAudio";
    [SerializeField]
    private string audioSavePath = "";

    // Tempo da gravação atual.
    private float recordingTime = 0;
    // Verdadeiro quando está gravando.
    private bool recording = false;
    // Verdadeiro quando acabou de gravar e a janela de gravação está aberta.
    private bool finishedRecord = false;
    // Tempo para finalizar automaticamente a gravação.
    private int recordingTimeout = 0;
    // Nome do arquivo de audio a ser salvo.
    private string audioFileName = "audio_record";


    private void Start()
    {
        anim.Play("music_fade_in");

        if(audioSavePath == "")
        {
            audioSavePath = Application.persistentDataPath;
        }
    }


    /// <summary>
    /// Verifica se o tempo de gavação foi ecedido e, se foi, para a
    /// gravação e limpao cache de gravação sem salvar a faixa.
    /// </summary>
    void Update()
    {
        if (recording)
        {
            if (recordingTime >= recordingTimeout)
            {
                Debug.Log("Recording timeout.");

                FinishAudioRecord();

                finishedRecord = true;
            }
            else
            {
                recordingTime += Time.deltaTime;
            }
        }
    }


    /// <summary>
    /// Salva a gravação e limpa o cache.
    /// </summary>
    void FinishAudioRecord()
    {
        SaveRecord(audioFileName);

        EndRecordAndClearSource();

        Debug.Log("Recorded!");
    }


    /// <summary>
    /// Para a gravação e salva o conteudo do cache(recordingSource).
    /// </summary>
    void SaveRecord(string fileName)
    {
        Microphone.End(null);
        recordingSource.Stop();

        SavWav.Save(audioSavePath, fileName, recordingSource.clip);
    }


    /// <summary>
    /// Finaliza a gravação sem salvar a faixa e limpa o cache.
    /// </summary>
    void EndRecordAndClearSource()
    {
        recording = false;
        recordingTime = 0;
        mixer.SetFloat(mixerPropToMute, 0);
        Microphone.End(null);
        recordingSource.Stop();
        recordingSource.clip = null;
    }


    /// <summary>
    /// 
    /// </summary>
    public float GetRecordingTime()
    {
        return recordingTime;
    }


    /// <summary>
    /// 
    /// </summary>
    public bool FinishedRecord()
    {
        return finishedRecord;
    }


    /// <summary>
    /// Reproduz um efeito sonoro usando o sfxSource.
    /// A função pode ser chamada diretamente de botões ou por scripts.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }


    /// <summary>
    /// 
    /// </summary>
    public void PauseMusic()
    {
        anim.Play("music_fade_out");
    }


    /// <summary>
    /// 
    /// </summary>
    public void PlayMusic()
    {
        anim.Play("music_fade_in");
    }


    /// <summary>
    /// Inicia a gravação de audio.
    /// </summary>
    public void StartAudioRecord(int duration, string fileName)
    {
        Debug.Log("Recording...");

        recordingTimeout = duration;
        audioFileName = fileName;

        recordingSource.clip = Microphone.Start(null, false, duration, 44100);

        // Muta o audio do jogo.
        mixer.SetFloat(mixerPropToMute, -80);
        recording = true;

        while (!(Microphone.GetPosition(null) > 0))
        {
            recordingSource.Play();
        }
    }
    

    /// <summary>
    /// 
    /// </summary>
    public void ResetState()
    {
        finishedRecord = false;
        recording = false;
    }


    /// <summary>
    /// Retorna o caminho da pasta onde os audios são salvos.
    /// </summary>
    public string GetAudioSavePath()
    {
        return audioSavePath;
    }
}
