using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class PartidaDTO
{
    // Start is called before the first frame update
    public int id
    { get; set; }
    public string titulo
    { get; set; }
    public int turnos { get; set; }
    public int numJogadores { get; set; }
    public string palavras { get; set; }
    public ICollection<JogadorDTO> jogadores { get; set; }

}
