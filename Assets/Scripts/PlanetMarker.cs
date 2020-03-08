/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 14/05/2019
 * **************************************************/


using UnityEngine;


/// <summary>
/// 
/// </summary>
public class PlanetMarker : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;

    // Tamanho original do marcador.
    private Vector3 originalSize;


    void Start()
    {
        originalSize = transform.localScale;
    }


    void Update()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }


    /// <summary>
    /// 
    /// </summary>
    public void DisplayMarker()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideMarker()
    {
        sprite.color = new Color(1, 1, 1, 0);
    }


    /// <summary>
    /// 
    /// </summary>
    public void ResizeMarker(float factor)
    {
        transform.localScale = originalSize * factor;
    }
}
