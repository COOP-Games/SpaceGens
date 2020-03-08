/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using UnityEngine;


/// <summary>
/// Linha que conecta visualmente os planetas.
/// </summary>
public class ConnectionLine : MonoBehaviour
{
    [Tooltip("Line renderer para a linha de conexão.")]
    [SerializeField]
    private LineRenderer line;
    [Tooltip("Animator da conexão.")]
    [SerializeField]
    private Animator anim;

    // Planeta de origen.
    private Planet origin;
    // Planeta destino.
    private Planet destination;


    /// <summary>
    /// Inicializa as variaveis necessarias, cria a linha de conexão(seus vetices).
    /// </summary>
    public void Init(Planet _origin, Planet _destination)
    {
        origin = _origin;
        destination = _destination;

        transform.position = origin.transform.position;

        line.positionCount = 2;
        line.SetPosition(0, origin.transform.position);
        line.SetPosition(1, destination.transform.position);
    }


    /// <summary>
    /// 
    /// </summary>
    public void ShowConnectionLine()
    {
        anim.Play("show_connection");
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideConnectionLine()
    {
        anim.Play("hide_connection");
    }


    /// <summary>
    /// 
    /// </summary>
    void EVENT_Hide_Connection()
    {
        gameObject.SetActive(false);
    }
}
