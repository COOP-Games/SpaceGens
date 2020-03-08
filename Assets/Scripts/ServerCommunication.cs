/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modificado:
 * 06/12/2019 - Igor
 * **************************************************/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;



/// <summary>
/// Classe para se comunicar com o servidor e armazenar os dados da partida.
/// </summary>
public class ServerCommunication : MonoBehaviour
{
    public static ServerCommunication instance = null;
    private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

    private string matchID = "matchIDSC";
    private static PartidaDTO partida;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    public void SetMatchID(string id)
    {
        instance.matchID = id;
    }

    public string GetMatchID()
    {
        if(partida == null)
        {
            return instance.matchID;
        }

        return partida.id.ToString();
    }

    /// <summary>
    /// tenta entrar em uma partida no servidor identificado
    /// pela string.
    /// </summary>
    public static bool JoinGame(string game)
    {
        partida = GetGameFromServer(game).Result;

        //Debug.Log(partida.titulo);

        if (partida == null)
        {
            return false;
        }

        instance.SetMatchID(partida.id.ToString());
        autoResetEvent.WaitOne();
        //print(partida.titulo);

        return true;
    }

    public static async Task<PartidaDTO> GetGameFromServer(string game)
    {
        HttpClient client;

        using (client = new HttpClient())
        {
            var response = await client.GetAsync("https://cryptic-cliffs-86576.herokuapp.com/info-partida?game=" + game).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
                Debug.Log("Conectado.");
            }
            else
            {
                Debug.Log(response.StatusCode);
                return null;
            }

            var obj = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            //string name = "{\"id\":1,\"titulo\":\"Partida 1\",\"turnos\":1,\"numJogadores\":3,\"palavras\":\"teste,teste,teste,teste\",\"jogadores\":[{\"id\":4,\"nome\":\"Jogador 1\"},{\"id\":3,\"nome\":\"Jogador 3\"},{\"id\":2,\"nome\":\"Jogador 2\"}]}";
            /*
            string name = "{" +
                "\"id\":123," +
                "\"titulo\":\"Partida_1\"," +
                "\"turnos\":1," +
                "\"numJogadores\":3," +
                "\"palavras\":\"" +
                "SOFÁ, BONÉ, AQUARELA, NOVELA, RUA, GELO, AMIGO, PETECA, MIL, DINOSSAURO, CABIDE," +
                "EXEMPLO, SECA, BARRIGA, CUBO, BATUCADA, LEI, NINHO, FAIXA, COLMEIA, TACO, HIPOPÓTAMO," +
                "TAXA, SOL, FOLIA, JOGADA, VÉU, FADA, PIQUE, SAPATO, TOMATE, FIVELA, TELEFONE, PLUMA," +
                "OURO, POSE, MARINHEIRO, REDE, PIRULITO, FIXO, CARAMELO, TREM, VICE, ÚLTIMO, ABELHA," +
                "MARIDO, PILHA, CAMINHÃO, XÍCARA\"" +
                ",\"jogadores\":" +
                "[{\"id\":4,\"nome\":\"Jogador 1\"}," +
                "{\"id\":3,\"nome\":\"Jogador 3\"}," +
                "{\"id\":2,\"nome\":\"Jogador 2\"}]}";*/

            
            partida = JsonConvert.DeserializeObject<PartidaDTO>(obj);
        }

        autoResetEvent.Set();

        return partida;
    }



    /// <summary>
    /// Envia um arquivo de audio para o servidor.
    /// </summary>
    public static void UploadAudioFile(AudioClip audio)
    {

    }


    /// <summary>
    /// Solicita o json do servidor com os dados da partida identificada
    /// por game.
    /// </summary>
    bool RequestGame(string game)
    {
        return true;
    }


    //=======================================================================================

    public string[] GetWords()
    {
        List<string> planetsNames = new List<string>();

        string palavras = partida.palavras.ToString();

        string[] auxPal = palavras.Split(',');

        for (int i = 0; i < auxPal.Length; i++)
        {
            planetsNames.Add(auxPal[i]);
        }

        return planetsNames.ToArray();
    }

    public string[] GetPlayers()
    {
        List<string> playersNames = new List<string>();

        if(partida == null)
        {
            Debug.LogWarning("Não há partida. Usando lista de nomes padrão.");
        }
        else
        {
            JogadorDTO[] auxJog = new JogadorDTO[partida.jogadores.Count];
            partida.jogadores.CopyTo(auxJog, 0);


            for (int i = 0; i < partida.jogadores.Count; i++)
            {
                playersNames.Add(auxJog[i].nome);
            }
        }

        string[] names = playersNames.ToArray();

        return names;
    }

    public int GetTurns()
    {
        return partida.turnos;
    }
}
