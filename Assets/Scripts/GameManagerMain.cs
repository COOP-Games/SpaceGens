/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * **************************************************/


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Android;

/// <summary>
/// Gerencia a logica do menu inicial.
/// Se comunica com o server por meio da classe ServerCommunication
/// para iniciar uma nova partida.
/// </summary>
public class GameManagerMain : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Janela do menu inicial.")]
    private GameObject mainMenu;
    [SerializeField]
    [Tooltip("Janela para iniciar partida como visitante.")]
    private GameObject joinPassiveMenu;
    [SerializeField]
    [Tooltip("Janela para iniciar partida normal.")]
    private GameObject joinInteractiveMenu;
    [SerializeField]
    [Tooltip("Tela de loading.")]
    private GameObject loadingScreen;


    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }


    /// <summary>
    /// Inicia uma partida como observador.
    /// Abre a tela para inserir o endereço do servidor.
    /// </summary>
    public void StartPassiveGame()
    {
        mainMenu.SetActive(false);
        joinPassiveMenu.SetActive(true);
    }


    /// <summary>
    /// Inicia uma partida como jogador.
    /// Abre a tela para inserir o endereço do servidor.
    /// </summary>
    public void StartInteractiveGame()
    {
        //Microphone.End(null);
        mainMenu.SetActive(false);
        joinInteractiveMenu.SetActive(true);
    }


    /// <summary>
    /// Solicita ao servidor os dados da partida correspondente a string
    /// os dados seram armazenados na ServerCommunication.
    /// Apos receber os dados a partida passiva é iniciada.
    /// </summary>
    public void SendGameRequestPassive(InputField input)
    {
        string game = input.text;
        if (ServerCommunication.JoinGame(game))
        {
            SceneManager.LoadScene("Passive");
        }
    }


    /// <summary>
    /// Solicita ao servidor os dados da partida correspondente a string
    /// os dados seram armazenados na ServerCommunication.
    /// Apos receber os dados a partida interativa é iniciada.
    /// </summary>
    public void SendGameRequestInteractive(InputField input)
    {
        string game = input.text;
        if (ServerCommunication.JoinGame(game))
        {
            SceneManager.LoadScene("Interactive");
            loadingScreen.SetActive(true);
        }
        else
        {
            DebugText.Log("Falha na conexão.", Color.red, 3);
        }
    }
}
