/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Planet : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Graficos do planeta.")]
    [SerializeField]
    private GameObject graphics;
    [Tooltip("Animator do planeta.")]
    [SerializeField]
    private Animator anim;
    [Tooltip("GameObject dos itens.")]
    [SerializeField]
    private Item item;

    [Header("Properties")]
    [Tooltip("Raio de orbita dos jogadores.")]
    [SerializeField]
    [Range(1f, 50f)]
    private float playesOrbitRadius = 1;
    [Tooltip("Velocidade de rotação do planeta.")]
    [SerializeField]
    [Range(1f, 100f)]
    private float rotationSpeed = 1;
    [Tooltip("Cor da nevoa de fundo.")]
    [SerializeField]
    private Color spaceFogColor = Color.white;

    // Nome do planeta.
    private string planetName = "none";
    // Posição em graus onde um novo jogador irá entrar em orbita.
    private float addAngle = 0;
    // Numero de jogadores no planeta.
    private int playersCount = 0;
    // Vizinhos do planeta.
    private List<Planet> neighbors = new List<Planet>();
    // Tamanho original dos graficos.
    private Vector3 originalGraphicsScale;
    // Tamanho original dos graficos dos itens do planeta.
    private Vector3 orignalItemScale = new Vector3(1, 1, 1);
    // GameManager.
    private GameManager gm;

    private int turnsToRespawnItem = 1;
    private int respawnCounter = 0;


    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        originalGraphicsScale = graphics.transform.localScale;

        if(item != null)
        {
            orignalItemScale = item.transform.localScale;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        Vector3 newFoward = transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0)) * (transform.worldToLocalMatrix * graphics.transform.forward);
        graphics.transform.rotation = Quaternion.LookRotation(newFoward, transform.up);
    }


    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public void Init(string _name, GameObject[] _goodsGraphicsPrefabs, GameObject[] _demandsGraphicsPrefabs, Planet[] _neighbors, GameManager _gm)
    {
        planetName = _name;
        gm = _gm;

        if (_neighbors != null)
        {
            for (int i = 0; i < _neighbors.Length; i++)
            {
                neighbors.Add(_neighbors[i]);
            }
        }
    }


    #region Get/Set


    /// <summary>
    /// 
    /// </summary>
    public int GetPlayersCount()
    {
        return playersCount;
    }


    /// <summary>
    /// 
    /// </summary>
    public float GetOrbitAngle()
    {
        return addAngle;
    }


    /// <summary>
    /// 
    /// </summary>
    public GameObject GetGraphics()
    {
        return graphics;
    }


    /// <summary>
    /// 
    /// </summary>
    public float GetOrbitRadius()
    {
        return playesOrbitRadius;
    }


    /// <summary>
    /// 
    /// </summary>
    public string GetName()
    {
        return planetName;
    }


    /// <summary>
    /// 
    /// </summary>
    public List<Planet> GetNeighbors()
    {
        return neighbors;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetNeighbors(List<Planet> _neighbors)
    {
        if (_neighbors != null)
        {
            for (int i = 0; i < _neighbors.Count; i++)
            {
                neighbors.Add(_neighbors[i]);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public Color GetSpaceFogColor()
    {
        return spaceFogColor;
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    public void AddPlayer()
    {
        playersCount++;
        addAngle = 360f / playersCount;
    }


    /// <summary>
    /// 
    /// </summary>
    public void RemovePlayer()
    {
        playersCount--;
    }


    /// <summary>
    /// 
    /// </summary>
    public void ResizeItems(float factor)
    {
        item.gameObject.transform.localScale = orignalItemScale * factor;
    }


    /// <summary>
    /// 
    /// </summary>
    public void ResizePlanetGraphics(float factor)
    {
        graphics.transform.localScale = originalGraphicsScale * factor;

        if (graphics.transform.localScale.x <= 0.1)
        {
            DisableGraphics();
        }
        else
        {
            EnableGraphics();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void DisableGraphics()
    {
        graphics.SetActive(false);
    }


    /// <summary>
    /// 
    /// </summary>
    public void EnableGraphics()
    {
        graphics.SetActive(true);
    }
    

    /// <summary>
    /// 
    /// </summary>
    public void PlayerInteractionAnim()
    {
        anim.Play("player_interaction");
    }


    /// <summary>
    /// 
    /// </summary>
    public void RespawnItem()
    {
        item.transform.localScale = orignalItemScale;
        item.Respawn();
    }


    /// <summary>
    /// 
    /// </summary>
    public void RespawnItem(ItemType _type, Variation _variation, int _points)
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public void SetItem(ItemType type, Variation variation, int points)
    {
        item.SetItem(type, variation, points);
    }


    /// <summary>
    /// 
    /// </summary>
    public Item GetItem()
    {
        return item;
    }


    /// <summary>
    /// 
    /// </summary>
    public bool HaveItem()
    {
        if(respawnCounter > 0)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    public void UpdateRespawnCounter()
    {
        respawnCounter--;

        if(respawnCounter <= 0)
        {
            RespawnItem();
            respawnCounter = 0;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetRespawn()
    {
        respawnCounter = turnsToRespawnItem;
    }


    /// <summary>
    /// 
    /// </summary>
    void EVENT_End_Player_Interaction_Animation()
    {
        gm.EventEndPlayerInteractionAnim();
    }
}
