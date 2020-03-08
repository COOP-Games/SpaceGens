/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * **************************************************/


using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Team : MonoBehaviour
{
    private int points = 0;
    public Color color;
    public string teamName = "none";
    public int[] pointsPerGem = { 0, 0, 0 };


    /// <summary>
    /// 
    /// </summary>
    public void AddPoints(int pts, int gemId)
    {
        Debug.Log("Recebendo pontos " + pts);
        points += pts;
        pointsPerGem[gemId] += pts;
    }


    /// <summary>
    /// 
    /// </summary>
    public int GetPoints()
    {
        return points;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetColor(Color col)
    {
        color = col;
    }
}
