/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 14/07/2019
 * Modifications:
 * **************************************************/


using UnityEngine;


/// <summary>
/// Muda a cor do background graduamente para o valor determinado.
/// </summary>
public class BackgoundColors : MonoBehaviour
{
    [Tooltip("Imagem que será modificada.")]
    [SerializeField]
    private SpriteRenderer sprite;
    [Tooltip("Duração da transição de cor.")]
    [SerializeField]
    private float trasitionDuration = 1;

    // Cor objetivo da transição.
    private Color targetColor;


    /// <summary>
    /// Interpola a cor atual com a de destino.
    /// </summary>
    void Update()
    {
        if (sprite.color != targetColor)
        {
            sprite.color = Color.Lerp(sprite.color, targetColor, trasitionDuration * Time.deltaTime);
        }
    }


    /// <summary>
    /// Função publica para setar a cor do sprite instantaneamente.
    /// </summary>
    public void SetSpriteColor(Color color)
    {
        sprite.color = color;
        targetColor = color;
    }


    /// <summary>
    /// Função publica para setar a cor do sprite para mudança gradual.
    /// </summary>
    public void SetTargetColor(Color color)
    {
        targetColor = color;
    }
}
