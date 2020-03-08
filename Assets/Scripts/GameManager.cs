/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified:
 * Arthur Gonze Machado - 8/2019
 * Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    starting,
    inGame,
    prepareToRecord,
    recording,
    goingToPlanet,
    newPlayer,
    gameOver
}


/// <summary>
/// Controla toda a logica da partida, inicia o jogo instanciando os planetas
/// e jogadores.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    [Tooltip("Prefabs dos planetas.")]
    private GameObject[] planetPrefabs = null;
    [SerializeField]
    [Tooltip("Preabs dos players.")]
    private GameObject[] playerPrefabs = null;
    [SerializeField]
    [Tooltip("Prefab do time.")]
    private Team teamPrefab;
    [Tooltip("Prefab das linhas que conectam os planetas.")]
    [SerializeField]
    private GameObject connectionLinePrefab;

    [Header("Audio")]
    [SerializeField]
    [Tooltip("Controlador de audio.")]
    private AudioController audioController;
    [SerializeField]
    private AudioClip clickClip;
    [SerializeField]
    private AudioClip clockClip;
    [SerializeField]
    private AudioClip notificationClip;


    [Header("Hierarchy References")]
    [SerializeField]
    [Tooltip("Camera")]
    private CameraController cam;
    [SerializeField]
    [Tooltip("Pasta para objetos spawnaveis.")]
    private Transform spawnables;
    [SerializeField]
    [Tooltip("Pasta para os planetas.")]
    private Transform planetsParent;
    [SerializeField]
    [Tooltip("Pasta para os jogadores.")]
    private Transform playersParent;
    [Tooltip("Pasta para as linhas de conexão.")]
    [SerializeField]
    private Transform connectionLinesParent;
    [SerializeField]
    [Tooltip("Controlador de UI.")]
    private UIController uIController;
    [SerializeField]
    [Tooltip("Nuvem de poeira do fundo.")]
    private BackgoundColors spaceFog;
    [SerializeField]
    [Tooltip("Marcador de planeta ativo.")]
    private PlanetMarker planetMarker;
    [SerializeField]
    [Tooltip("Comunicação com o servidor.")]
    private ServerCommunication server;
    [Tooltip("Janela de gravação.")]
    [SerializeField]
    private RecordWindow recordWindow;

    [Header("Animations")]
    [SerializeField]
    private Animator uiAnimator;


    [Header("Properties")]
    [SerializeField]
    [Tooltip("Maximo de jogadas por vez de cada jogador.")]
    private int maxPlayerTurns = 3;
    [SerializeField]
    [Tooltip("Delay para transição de jogador.")]
    private float newPlayerDelay = 0;
    [SerializeField]
    [Tooltip("Usar ou não o numero de turnos estabelecido pelo servidor.")]
    private bool useServerTurns = false;
    [SerializeField]
    [Tooltip("Numero maximo de turnos. Em cada turno cada jogador joga ''maxPlayerTurns'' vezes.")]
    private int turns = 2;
    [SerializeField]
    [Tooltip("Distancia entre os planetas.")]
    float planetsDistance = 100;
    //[SerializeField]
    //[Tooltip("Numero de linhas de planetas.")]
    //int planetsRows = 5;
    //[SerializeField]
    //[Tooltip("Numero de colunas de planetas.")]
    //int planetsCols = 5;
    [SerializeField]
    [Tooltip("Angulo de rotação da matriz de planetas.")]
    float planetsDistAngle = 30;
    [SerializeField]
    [Tooltip("Nome do time 1.")]
    private string team1Name = "none";
    [SerializeField]
    [Tooltip("Nome do time 2.")]
    private string team2Name = "none";
    [Tooltip("Tempo para iniciar a gravação.")]
    [SerializeField]
    private int timeToStartRecord = 3;
    [SerializeField]
    [Tooltip("Após esse tempo a gravação é cancelada. " +
             "É tambem a duração da faixa independente do tempo que o " +
             "botão de gravação for pressionado.")]
    private int recordTimeout = 3;


    // Identificador da partida.
    private string matchID = "matchIDGM";
    // Referencia ao time 1.
    private Team team1;
    // Referencia ao time 2.
    private Team team2;
    // Lista de planetas.
    private List<Planet> planets = new List<Planet>();
    // Lista de jogadores.
    private List<Player> players = new List<Player>();
    // Lista de palavras.
    private string[] words;
    // Linhas que conectam os planetas.
    private List<ConnectionLine> connectionLines = new List<ConnectionLine>();

    // Estado atual do jogo.
    private GameState state = GameState.starting;
    // ID do jogador ativo.
    private int activePlayer = 0;
    // Referencia ao planeta ativo.
    private Planet activePlanet = null;
    // Contagem de jogada sdo jogador atual.
    private int playerTurnCount = 0;
    // Palavra que acaba de ser lida.
    private string recordedWord = "word";

    // Ultimo inteiro do contador regresivo.
    private int lastIntTimeToStartRecord = 0;
    // Contador de tempo para cronometro de inicio de gravação.
    private float prepareToStartRecordTimer = 0;


    #region Get/Set

    public GameState GetState()
    {
        return state;
    }

    public AudioController GetAudioController()
    {
        return audioController;
    }

    #endregion


    void Start()
    {
        lastIntTimeToStartRecord = timeToStartRecord;
        matchID = server.GetMatchID();

        if (useServerTurns)
        {
            turns = server.GetTurns();
        }

        uIController.Init();
        
        uIController.SetTurnsCount(turns);

        // Configuração inicial do times.
        team1 = Instantiate(teamPrefab, this.transform).GetComponent<Team>();
        team2 = Instantiate(teamPrefab, this.transform).GetComponent<Team>();
        team1.teamName = team1Name;
        team2.teamName = team2Name;
        Color[] teamColors = uIController.GetTeamsColors();
        team1.SetColor(teamColors[0]);
        team2.SetColor(teamColors[1]);

        uIController.SetTeamNames(new string[] { team1.teamName, team2.teamName });

        SetWords();
        SetPlanets();
        SetPlayers();

        // Atualização da UI.
        uIController.SetPlayerGemsCount(0, 0, 0);
        uIController.SetNewPlayerText(players[activePlayer].GetName());
        planetMarker.SetPosition(activePlanet.GetItem().transform.position);
        planetMarker.HideMarker();
        uiAnimator.Play("start_game");

        cam.SetPosition(activePlanet.gameObject);
        spaceFog.SetSpriteColor(activePlanet.GetSpaceFogColor());

        uIController.RepositionatePlanetArrows(activePlanet);
    }


    /// <summary>
    /// 
    /// </summary>
    void SetWords()
    {
        words = server.GetWords();
    }


    /// <summary>
    /// Instancia os planetas a partir de um array de nomes.
    /// </summary>
    void SetPlanets()
    {
        Debug.Log("Criando o mapa...");

        Planet spawningPlanet = null;

        int planetsRows = 0;
        int planetsCols = 0;

        float result = Mathf.Sqrt(words.Length);

        if (result % 1 == 0)
        {
            planetsRows = (int)result;
            planetsCols = (int)result;
        }
        else
        {
            Debug.LogError("Numero de planetas impossivel de criar grade quadrada. Numero de planetas = " + words.Length);
            return;
        }

        for (int i = 0; i < planetsRows; i++)
        {
            for (int j = 0; j < planetsCols; j++)
            {
                float dist = planetsDistance / 4;

                spawningPlanet = Instantiate(planetPrefabs[(int)Random.Range(0, planetPrefabs.Length)],
                 new Vector3(j * planetsDistance + Random.Range(-dist, dist), i * planetsDistance + Random.Range(-dist, dist), 0),
                 Quaternion.identity, planetsParent).GetComponent<Planet>();


                spawningPlanet.gameObject.name = words[planets.Count];
                planets.Add(spawningPlanet);

                spawningPlanet.Init(spawningPlanet.gameObject.name, null, null, null, this);
            }
        }

        // Adiciona os vizinhos de cada planeta.
        List<Planet> neighbors = new List<Planet>();

        for (int i = 0; i < planets.Count; i++)
        {
            if (i % planetsCols != 0)
            {
                neighbors.Add(planets[i - 1]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (i + planetsCols < planets.Count)
            {
                neighbors.Add(planets[i + planetsCols]);
            }
            else
            {
                neighbors.Add(null);
            }

            if ((i + 1) % planetsCols != 0)
            {
                neighbors.Add(planets[i + 1]);
            }
            else
            {
                neighbors.Add(null);
            }

            if (i - planetsCols >= 0)
            {
                neighbors.Add(planets[i - planetsCols]);
            }
            else
            {
                neighbors.Add(null);
            }

            planets[i].SetNeighbors(neighbors);

            neighbors = new List<Planet>();
        }

        // Rotaciona o conjunto de planetas.
        planetsParent.transform.Rotate(new Vector3(0, 0, planetsDistAngle));

        // Adiciona os itens dos planetas
        for (int i = 0; i < planets.Count; i++)
        {
            int id = Random.Range(0, 3);
            Variation var = Variation.blue;

            if (id == 0)
            {
                var = Variation.red;
            }
            else if (id == 1)
            {
                var = Variation.green;
            }

            if (i % 2 == 0)
            {
                planets[i].SetItem(ItemType.demand, var, 100);
            }
            else
            {
                planets[i].SetItem(ItemType.good, var, 100);
            }
        }

        SetConnectionsGraphics(planetsCols);

        activePlanet = planets[0];
    }


    /// <summary>
    /// Instancia uma conexão para cada planeta com o seu vizinho.
    /// </summary>
    void SetConnectionsGraphics(int planetsCols)
    {
        for (int i = 0; i < planets.Count; i++)
        {

            GameObject clone;

            if (i + planetsCols < planets.Count)
            {
                clone = Instantiate(connectionLinePrefab, planets[i].transform.position, Quaternion.identity, connectionLinesParent);
                clone.GetComponent<ConnectionLine>().Init(planets[i], planets[i + planetsCols]);
                connectionLines.Add(clone.GetComponent<ConnectionLine>());
                clone.SetActive(false);
            }

            if ((i + 1) % planetsCols != 0)
            {
                clone = Instantiate(connectionLinePrefab, planets[i].transform.position, Quaternion.identity, connectionLinesParent);
                clone.GetComponent<ConnectionLine>().Init(planets[i], planets[i + 1]);
                connectionLines.Add(clone.GetComponent<ConnectionLine>());
                clone.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Instancia os jogadors a partir de um array de nomes.
    /// </summary>
    void SetPlayers()
    {
        Player spawningPlayer = null;
        
        string[] playersNames = server.GetPlayers();

        // Cria os jogadores
        for (int i = 0; i < playersNames.Length; i++)
        {
            if (i % 2 == 0)
            {
                spawningPlayer = Instantiate(playerPrefabs[0], planets[0].transform.position, Quaternion.identity, playersParent).GetComponent<Player>();

                spawningPlayer.Init(playersNames[i], i, planets[0], team1, this, null);
            }
            else
            {
                spawningPlayer = Instantiate(playerPrefabs[1], planets[0].transform.position, Quaternion.identity, playersParent).GetComponent<Player>();

                spawningPlayer.Init(playersNames[i], i, planets[0], team2, this, null);
            }

            spawningPlayer.gameObject.name = playersNames[i];
            players.Add(spawningPlayer);
        }

        activePlayer = 0;
        players[activePlayer].ActivatePlayer();
    }


    #region Update

    private void Update()
    {
        switch(state)
        {
            case GameState.starting:
                UpdateStarting();
                break;
            case GameState.inGame:
                UpdateInGame();
                break;
            case GameState.prepareToRecord:
                UpdatePrepareToRecord();
                break;
            case GameState.recording:
                UpdateRecording();
                break;
            case GameState.goingToPlanet:
                UpdateGoingToPlanet();
                break;
            case GameState.newPlayer:
                UpdateNewPlayer();
                break;
            case GameState.gameOver:
                UpdateGameOver();
                break;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateStarting()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateInGame()
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdatePrepareToRecord()
    {
        prepareToStartRecordTimer -= Time.deltaTime;

        if (prepareToStartRecordTimer <= 0)
        {
            prepareToStartRecordTimer = 0;
            lastIntTimeToStartRecord = timeToStartRecord;
            recordWindow.StartRecordAnimation();
            state = GameState.recording;
            audioController.PlaySFX(notificationClip);
        }
        else
        {
            if(Mathf.RoundToInt(prepareToStartRecordTimer) < lastIntTimeToStartRecord)
            {
                uIController.SetPrepareToRecordTimer(Mathf.RoundToInt(prepareToStartRecordTimer));
                lastIntTimeToStartRecord = Mathf.RoundToInt(prepareToStartRecordTimer);
                audioController.PlaySFX(clockClip);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateRecording()
    {
        uIController.SetProgressBarValue(audioController.GetRecordingTime() / recordTimeout);

        if (audioController.FinishedRecord())
        {
            recordWindow.CloseWindow();

            uIController.HideSoundWave();

            players[activePlayer].GoToPlanet(activePlanet);
            cam.ResetZoomOut();
            cam.SetTarget(players[activePlayer].gameObject);


            spaceFog.SetTargetColor(activePlanet.GetSpaceFogColor());

            audioController.ResetState();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateGoingToPlanet()
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateNewPlayer()
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdateGameOver()
    {

    }

    #endregion


    /// <summary>
    /// Operações para fim da animação de inicio do jogo.
    /// </summary>
    public void EventAnimEndOpening()
    {
        state = GameState.inGame;
    }


    #region Record

    /// <summary>
    /// 
    /// </summary>
    public void OpenRecordWindow(Planet destination)
    {
        activePlanet = destination;
        planetMarker.SetPosition(activePlanet.transform.position);
        recordWindow.OpenWindow();
        uIController.SetPrepareToRecordTimer(timeToStartRecord);
        uiAnimator.Play("hide_ui");
        uIController.EVENT_Hide_Active_Arrows();
        audioController.PauseMusic();
    }


    /// <summary>
    /// 
    /// </summary>
    public void StartRegressiveCounter()
    {
        int i = Random.Range(0, words.Length);
        uIController.UpdateWordToRecord(words[i]);
        recordedWord = words[i];

        state = GameState.prepareToRecord;
        prepareToStartRecordTimer = timeToStartRecord;
        uIController.DisplaySoundWave();
    }


    /// <summary>
    /// Inicia a gravação de audio.
    /// </summary>
    public void StartRecord()
    {
        audioController.StartAudioRecord(recordTimeout, matchID + "_" + players[activePlayer].GetName() + "_" + playerTurnCount.ToString() + "_" + turns.ToString() + "_" + recordedWord);
    }


    /// <summary>
    /// 
    /// </summary>
    public void AfterCloseRecordWindow()
    {
        state = GameState.goingToPlanet;
        audioController.PlayMusic();
    }

    #endregion


    /// <summary>
    /// Função para quando o jogador chega a um planeta novo.
    /// </summary>
    public void PlayerArrivedAtPlanet()
    {
        uIController.ShowUI();
        activePlanet.PlayerInteractionAnim();

        cam.SetTarget(players[activePlayer].GetPlanet().gameObject);

        players[activePlayer].SellOrMineItem();

        UpdateScoreBar();

        uIController.SetPlayerGemsCount(players[activePlayer].GetItems()[0],
                                        players[activePlayer].GetItems()[1],
                                        players[activePlayer].GetItems()[2]);
    }


    /// <summary>
    /// Atualiza a barra de pontuação.
    /// </summary>
    void UpdateScoreBar()
    {
        float totalPoints = team1.GetPoints() + team2.GetPoints();

        if (totalPoints == 0)
        {
            uIController.SetScoreBarValue(0.5f);
        }
        else
        {
            float sliderValue = team1.GetPoints() / totalPoints;
            uIController.SetScoreBarValue(sliderValue);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void EventEndPlayerInteractionAnim()
    {
        if (playerTurnCount >= maxPlayerTurns - 1)
        {
            playerTurnCount = 0;

            if(SetNewActivePlayer())
            {
                activePlanet = players[activePlayer].GetPlanet();
                planetMarker.SetPosition(activePlanet.GetItem().transform.position);
                cam.SetTarget(activePlanet.gameObject);
                uIController.NewPlayerAnimation();
            }
        }
        else
        {
            playerTurnCount++;
            uIController.RepositionatePlanetArrows(activePlanet);
            uIController.EVENT_Show_Active_Arrows();
            state = GameState.inGame;
        }
    }


    /// <summary>
    /// Determina o novo jogador da vez.
    /// </summary>
    public bool SetNewActivePlayer()
    {
        players[activePlayer].DeactivatePlayer();

        activePlayer++;

        bool haveNewTurn = true;

        if (activePlayer >= players.Count)
        {
            activePlayer = 0;

            haveNewTurn = NextTurn();
        }

        if (haveNewTurn)
        {
            uIController.SetNewPlayerText(players[activePlayer].GetName());

            uIController.SetPlayerGemsCount(players[activePlayer].GetItems()[0],
                                            players[activePlayer].GetItems()[1],
                                            players[activePlayer].GetItems()[2]);

            players[activePlayer].ActivatePlayer();

            state = GameState.newPlayer;

            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Função para iniciar um novo turno.
    /// </summary>
    bool NextTurn()
    {
        turns--;
        uIController.SetTurnsCount(turns);

        for(int i = 0; i < planets.Count; i++)
        {
            planets[i].UpdateRespawnCounter();
        }

        if (turns == 0)
        {
            FinishGame();

            return false;
        }

        return true;
    }


    /// <summary>
    /// Finaliza a partida calculandoa as pontuações e atualizando a UI.
    /// </summary>
    public void FinishGame()
    {
        Debug.Log("Fim de jogo.");

        state = GameState.gameOver;
        audioController.PauseMusic();

        // Calculo do bonus
        int min1 = team1.pointsPerGem[0];
        int min2 = team2.pointsPerGem[0];
        int team1Bonus = 0;
        int team2Bonus = 0;
        string winnerText;
        int[] team1Points = new int[5];
        int[] team2Points = new int[5];
        int totalPoints;

        for (int i = 0; i < team1.pointsPerGem.Length; i++)
        {
            if (team1.pointsPerGem[i] < min1)
            {
                min1 = team1.pointsPerGem[i];
            }

            if (team2.pointsPerGem[i] < min2)
            {
                min2 = team2.pointsPerGem[i];
            }
        }
        if (min1 > min2)
        {
            team1Bonus = 100;
        }
        else if (min2 > min1)
        {
            team2Bonus = 100;
        }


        // Verificação de vitoria
        if (team1.GetPoints() + team1Bonus > team2.GetPoints() + team2Bonus)
        {
            winnerText = team1.teamName;
        }
        else if (team1.GetPoints() + team1Bonus < team2.GetPoints() + team2Bonus)
        {
            winnerText = team2.teamName;
        }
        else
        {
            winnerText = "ninguém";
        }

        totalPoints = team1.GetPoints() + team2.GetPoints();

        team1Points[0] = team1.pointsPerGem[0];
        team1Points[1] = team1.pointsPerGem[1];
        team1Points[2] = team1.pointsPerGem[2];
        team1Points[3] = team1Bonus;
        team1Points[4] = team1.GetPoints() + team1Bonus;

        team2Points[0] = team2.pointsPerGem[0];
        team2Points[1] = team2.pointsPerGem[1];
        team2Points[2] = team2.pointsPerGem[2];
        team2Points[3] = team2Bonus;
        team2Points[4] = team2.GetPoints() + team1Bonus;

        uIController.SetGameOverUIValues(totalPoints, team1Points, team2Points, winnerText);
        uIController.EVENT_Hide_Active_Arrows();
        uIController.PlayGameOverAnimation();
    }


    /// <summary>
    /// 
    /// </summary>
    public void EventEndNewPlayerAnimation()
    {
        uIController.RepositionatePlanetArrows(activePlanet);
        uIController.EVENT_Show_Active_Arrows();
        state = GameState.inGame;
    }


    /// <summary>
    /// Atualiza o tamanho dos iems.
    /// </summary>
    public void ResizeItems(float size)
    {
        for (int i = 0; i < planets.Count; i++)
        {
            planets[i].ResizeItems(size);
        }
    }


    /// <summary>
    /// Atualiza o tamanho dos planetas.
    /// </summary>
    public void ResizePlanets(float size)
    {
        for (int i = 0; i < planets.Count; i++)
        {
            planets[i].ResizePlanetGraphics(size);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void ShowConnectionLines()
    {
        for (int i = 0; i < connectionLines.Count; i++)
        {
            connectionLines[i].gameObject.SetActive(true);
            connectionLines[i].ShowConnectionLine();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideConnectionLines()
    {
        for (int i = 0; i < connectionLines.Count; i++)
        {
            connectionLines[i].HideConnectionLine();
        }
    }


    /// <summary>
    /// Returna o centro do mapa.
    /// </summary>
    public Vector3 GetMapCenter()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        int count = 0;

        foreach (Planet planet in planets)
        {
            count++;
            pos.x += planet.transform.position.x;
            pos.y += planet.transform.position.y;
        }

        pos.x = pos.x / count;
        pos.y = pos.y / count;
        return pos;
    }


    /// <summary>
    /// Retorna ao menu inicial.
    /// </summary>
    public void BUTTON_Back_To_Main()
    {
        SceneManager.LoadScene("Main");
    }
}
