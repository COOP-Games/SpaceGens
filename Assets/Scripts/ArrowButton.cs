/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/



using UnityEngine;


/// <summary>
/// Classe para logica dos botões de opção de rota.
/// </summary>
public class ArrowButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Aniamtor.")]
    private Animator anim;

    // Planeta para onde o jogador irá quando apertar este botão.
    private Planet destination;
    // GameManager.
    private GameManager gameManager;


    /// <summary>
    /// 
    /// </summary>
    public void Init(GameManager _gm)
    {
        gameManager = _gm;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetDestination(Planet _destination)
    {
        destination = _destination;
    }


    /// <summary>
    /// 
    /// </summary>
    public Planet GetDestination()
    {
        return destination;
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnClick()
    {
        gameManager.OpenRecordWindow(destination);
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideArrow()
    {
        anim.Play("hide_arrow");
    }


    /// <summary>
    /// 
    /// </summary>
    public void ShowArrow()
    {
        anim.Play("show_arrow");
    }
}
