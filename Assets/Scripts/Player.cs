/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject graphics;
    [SerializeField]
    private TextMeshPro playerTextMesh;
    [SerializeField]
    private Outline outlineScript;

    [Header("Audio")]
    [SerializeField]
    private AudioSource rocketSource;
    [SerializeField]
    private AudioController audioController;
    [SerializeField]
    private AudioClip mineSound;
    [SerializeField]
    private AudioClip sellSound;

    [Header("Properties")]
    [SerializeField]
    private string playerName = "none";
    [Header("Campo para associar manualmnete o planeta, para ser usado no player que fica no menu.")]
    [SerializeField]
    private Planet planet;

    // Identificador do jogador.
    private int id = 0;
    // Pontos do jogador.
    private int points = 0;
    // Time do jogador.
    private Team team;
    // Itens do jogador.
    private int[] itens = new int[3];
    // GameMnager.
    private GameManager gameManager;
    // Variavel auxiliar de velocidade para o SmoothDamp.
    private Vector3 speed = Vector3.zero;
    // Plavras lidas pelo jogador.
    private List<string> wordsRead = new List<string>();


    /// <summary>
    /// 
    /// </summary>
    public void Init(string _name, int _id, Planet _planet, Team _team, GameManager _gm, Item[] itensPrefabs)
    {
        itens[0] = 0;
        itens[1] = 0;
        itens[2] = 0;

        playerName = _name;
        id = _id;
        planet = _planet;
        team = _team;
        gameManager = _gm;
        audioController = _gm.GetAudioController();

        planet.AddPlayer();

        playerTextMesh.text = playerName;
        playerTextMesh.color = team.color;

        outlineScript.OutlineColor = team.color;

        DeactivatePlayer();
    }


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        Move();
    }


    /// <summary>
    /// 
    /// </summary>
    void Move()
    {
        // Define posicao inicial sendo o planeta inicial
        Vector3 orig = new Vector3(planet.transform.position.x, planet.transform.position.y);
        // Define o vetor de uma possivel nova posicao como a posicao inicial
        Vector3 newPos = orig;
        // Faz a nave girar em torno do planeta
        int idPlus = id + 1;
        orig = planet.GetGraphics().transform.right * planet.GetOrbitRadius();
        orig = Matrix4x4.Rotate(Quaternion.Euler(Mathf.Sin(idPlus) * idPlus * planet.GetOrbitAngle(), idPlus * planet.GetOrbitAngle(), Mathf.Cos(idPlus) * idPlus * planet.GetOrbitAngle())) * orig;
        orig = new Vector3(orig.x + planet.transform.position.x, orig.y + planet.transform.position.y, orig.z + planet.transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, orig, ref speed, 2);

        graphics.transform.LookAt(orig);
    }


    #region Get/Set

    /// <summary>
    /// 
    /// </summary>
    public int[] GetItems()
    {
        return itens;
    }


    /// <summary>
    /// 
    /// </summary>
    public Planet GetPlanet()
    {
        return planet;
    }


    /// <summary>
    /// 
    /// </summary>
    public string GetName()
    {
        return playerName;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetTeam(Team _team)
    {
        team = _team;
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    public void ActivatePlayer()
    {
        playerTextMesh.gameObject.SetActive(true);
        outlineScript.OutlineColor = team.color;
    }


    /// <summary>
    /// 
    /// </summary>
    public void DeactivatePlayer()
    {
        playerTextMesh.gameObject.SetActive(false);
        outlineScript.OutlineColor = new Color(10 / 255, 35 / 255, 70 / 255);
    }


    /// <summary>
    /// 
    /// </summary>
    public void GoToPlanet(Planet _planet)
    {
        planet.RemovePlayer();
        planet = _planet;
        planet.AddPlayer();
        rocketSource.Play();
    }


    /// <summary>
    /// 
    /// </summary>
    public void SellOrMineItem()
    {
        if(planet.HaveItem())
        {
            planet.SetRespawn();

            if (planet.GetItem().type == ItemType.good)
            {
                if (planet.GetItem().variation == Variation.red)
                {
                    itens[0]++;
                }
                else if (planet.GetItem().variation == Variation.green)
                {
                    itens[1]++;
                }
                else if (planet.GetItem().variation == Variation.blue)
                {
                    itens[2]++;
                }

                planet.GetItem().PlayAnimation();
                audioController.PlaySFX(mineSound);
            }
            else
            {
                if (planet.GetItem().variation == Variation.red && itens[0] > 0)
                {
                    itens[0]--;
                    team.AddPoints(planet.GetItem().points, 0);
                    planet.GetItem().PlayAnimation();
                    audioController.PlaySFX(sellSound);
                }
                else if (planet.GetItem().variation == Variation.green && itens[1] > 0)
                {
                    itens[1]--;
                    team.AddPoints(planet.GetItem().points, 1);
                    planet.GetItem().PlayAnimation();
                    audioController.PlaySFX(sellSound);
                }
                else if (planet.GetItem().variation == Variation.blue && itens[2] > 0)
                {
                    itens[2]--;
                    team.AddPoints(planet.GetItem().points, 2);
                    planet.GetItem().PlayAnimation();
                    audioController.PlaySFX(sellSound);
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Planet" && gameManager.GetState() == GameState.goingToPlanet)
        {
            // Para evitar que ele aceite uma colisão caso haja um outro planeta no caminho.
            if (col.gameObject.GetComponentInParent<Planet>().GetName() == planet.GetName())
            {
                gameManager.PlayerArrivedAtPlanet();
                rocketSource.Stop();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="word"></param>
    public void AddWordRead(string word)
    {
        wordsRead.Add(word);
    }
}
