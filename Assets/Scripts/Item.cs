/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using UnityEngine;


public enum ItemType
{
    good,
    demand
}


public enum Variation
{
    red,
    green,
    blue
}


/// <summary>
/// Classe para dados os items.
/// </summary>
public class Item : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject demandGraphicsPrefab;
    [SerializeField]
    private GameObject goodGraphicsPrefab;
    [SerializeField]
    private GameObject redGemPrefab;
    [SerializeField]
    private GameObject greenGemPrefab;
    [SerializeField]
    private GameObject blueGemPrefab;

    [Header("References")]
    [SerializeField]
    private Transform demGoodTransform;
    [SerializeField]
    private Transform gemTransform;
    [SerializeField]
    private Animator anim;

    [Header("Properties")]
    public string itemName = "none";
    public ItemType type = ItemType.good;
    public Variation variation = Variation.red;
    public int points = 0;
    public int quantity = 0;

    private Vector3 demGoodTransformOrigPos;
    private Vector3 gemTransformOrigPos;
    private Quaternion demGoodTransformOrigRot;
    private Quaternion gemTransformOrigRot;


    /// <summary>
    /// 
    /// </summary>
    public void SetItem(ItemType _type, Variation _variation, int _points)
    {
        type = _type;
        variation = _variation;
        points = _points;
        quantity = 1;

        demGoodTransformOrigPos = demGoodTransform.position;
        gemTransformOrigPos = gemTransform.position;
        demGoodTransformOrigRot = demGoodTransform.rotation;
        gemTransformOrigRot = gemTransform.rotation;

        if (type == ItemType.demand)
        {
            GameObject clone = Instantiate(demandGraphicsPrefab, demGoodTransform.position, Quaternion.identity, demGoodTransform);
        }
        else
        {
            GameObject clone = Instantiate(goodGraphicsPrefab, demGoodTransform.position, Quaternion.identity, demGoodTransform);
        }

        if(variation == Variation.red)
        {
            GameObject clone = Instantiate(redGemPrefab, gemTransform.position, Quaternion.identity, gemTransform);
        }
        else if(variation == Variation.green)
        {
            GameObject clone = Instantiate(greenGemPrefab, gemTransform.position, Quaternion.identity, gemTransform);
        }
        else
        {
            GameObject clone = Instantiate(blueGemPrefab, gemTransform.position, Quaternion.identity, gemTransform);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void PlayAnimation()
    {
        if(type == ItemType.demand)
        {
            anim.Play("sell");
        }
        else
        {
            anim.Play("mine");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void Respawn()
    {
        gemTransform.position = gemTransformOrigPos;
        demGoodTransform.position = demGoodTransformOrigPos;
        gemTransform.rotation = gemTransformOrigRot;
        demGoodTransform.rotation = demGoodTransformOrigRot;
    }
}
