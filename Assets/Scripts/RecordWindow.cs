/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * **************************************************/


using UnityEngine;


/// <summary>
/// Controle de animação da janela de gravação.
/// </summary>
public class RecordWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Animator da janela.")]
    private Animator anim;
    [SerializeField]
    [Tooltip("GameManager.")]
    private GameManager gm;


    /// <summary>
    /// 
    /// </summary>
    void Event_finish_open_window()
    {
        gm.StartRegressiveCounter();
    }


    /// <summary>
    /// 
    /// </summary>
    void Event_finish_start_record()
    {
        gm.StartRecord();
    }


    /// <summary>
    /// 
    /// </summary>
    void Event_finish_close_window()
    {
        gm.AfterCloseRecordWindow();
    }


    /// <summary>
    /// 
    /// </summary>
    public void OpenWindow()
    {
        anim.Play("open_window");
    }


    /// <summary>
    /// 
    /// </summary>
    public void StartRecordAnimation()
    {
        anim.Play("start_record");
    }


    /// <summary>
    /// 
    /// </summary>
    public void CloseWindow()
    {
        anim.Play("close_window");
    }
}
