/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 11/07/2019
 * Modified: Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField]
    private Color team_1_Color;
    [SerializeField]
    private Color team_2_Color;

    [Header("Images")]
    [SerializeField]
    private Image scoreSliderBackground;
    [SerializeField]
    private Image scoreSliderFill;

    [Header("Texts")]
    [SerializeField]
    private Text newPlayerNameText;
    [SerializeField]
    private Text wordToRecord;

    // Game Over UI
    [SerializeField]
    private Text winnerTeamText;
    [SerializeField]
    private Text team_1_NameText;
    [SerializeField]
    private Text team_2_NameText;

    [Header("Counters")]

    // In Game UI
    [SerializeField]
    private Text playerGemCount_R;
    [SerializeField]
    private Text playerGemCount_G;
    [SerializeField]
    private Text playerGemCount_B;
    [SerializeField]
    private Text turnsCount;
    [SerializeField]
    private Text prepareToRecordTimer;

    // Game Over UI
    [SerializeField]
    private Text totalPointsCount;
    [SerializeField]
    private Text team_1_PointsCount_R;
    [SerializeField]
    private Text team_1_PointsCount_G;
    [SerializeField]
    private Text team_1_PointsCount_B;
    [SerializeField]
    private Text team_1_BonusPointsCount;
    [SerializeField]
    private Text team_1_TotalPoints;
    [SerializeField]
    private Text team_2_PointsCount_R;
    [SerializeField]
    private Text team_2_PointsCount_G;
    [SerializeField]
    private Text team_2_PointsCount_B;
    [SerializeField]
    private Text team_2_BonusPointsCount;
    [SerializeField]
    private Text team_2_TotalPoints;

    [Header("Groups")]
    [SerializeField]
    private GameObject gameOverUIGroup;
    [SerializeField]
    private GameObject interactiveUIGroup;
    [SerializeField]
    private GameObject newPlayerMesageGroup;

    [Header("SoundWave")]
    [SerializeField]
    private SoundWave soundWave;
    [SerializeField]
    private Slider recordProgress;

    [Header("Sliders")]
    [SerializeField]
    private Slider scoreSlider;

    [Header("Buttons")]
    [SerializeField]
    private ArrowButton[] arrowButtons;

    [Header("Hierarchy Refferences")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    public Canvas canvas;
    [SerializeField]
    private Animator anim;

    [Header("Properties")]
    [SerializeField]
    private float arrowDistance = 10;

    // Referencia ao RectTransform.
    private RectTransform canvasRect;


    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {
        scoreSliderFill.color = team_1_Color;
        scoreSliderBackground.color = team_2_Color;

        canvasRect = canvas.GetComponent<RectTransform>();

        for(int i = 0; i < arrowButtons.Length; i++)
        {
            arrowButtons[i].Init(gameManager);
        }
    }


    #region Display/Hide


    /// <summary>
    /// 
    /// </summary>
    public void ShowUI()
    {
        anim.Play("show_ui");
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideUI()
    {
        anim.Play("hide_ui");
    }


    /// <summary>
    /// 
    /// </summary>
    public void HideSoundWave()
    {
        soundWave.HideWave();
    }


    /// <summary>
    /// 
    /// </summary>
    public void DisplaySoundWave()
    {
        soundWave.DisplayWave();
    }

    #endregion

    #region Get/Set

    /// <summary>
    /// 
    /// </summary>
    public Color[] GetTeamsColors()
    {
        Color[] color = { team_1_Color, team_2_Color };
        return color;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetTurnsCount(int turns)
    {
        turnsCount.text = turns.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetTeamNames(string[] names)
    {
        team_1_NameText.text = names[0];
        team_2_NameText.text = names[1];
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetNewPlayerText(string name)
    {
        newPlayerNameText.text = name;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetPlayerGemsCount(int r, int g, int b)
    {
        playerGemCount_R.text = r.ToString();
        playerGemCount_G.text = g.ToString();
        playerGemCount_B.text = b.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetScoreBarValue(float val)
    {
        scoreSlider.value = val;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetProgressBarValue(float val)
    {
        recordProgress.value = val;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetGameOverUIValues(int total, int[] team1Points, int[] team2Points, string winner)
    {
        winnerTeamText.text = winner;

        totalPointsCount.text = total.ToString();

        team_1_PointsCount_R.text = team1Points[0].ToString();
        team_1_PointsCount_G.text = team1Points[1].ToString();
        team_1_PointsCount_B.text = team1Points[2].ToString();
        team_1_BonusPointsCount.text = team1Points[3].ToString();
        team_1_TotalPoints.text = team1Points[4].ToString();

        team_2_PointsCount_R.text = team2Points[0].ToString();
        team_2_PointsCount_G.text = team2Points[1].ToString();
        team_2_PointsCount_B.text = team2Points[2].ToString();
        team_2_BonusPointsCount.text = team2Points[3].ToString();
        team_2_TotalPoints.text = team2Points[4].ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetPrepareToRecordTimer(int time)
    {
        prepareToRecordTimer.text = time.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    public void UpdateWordToRecord(string word)
    {
        wordToRecord.text = word;
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    public void RepositionatePlanetArrows(Planet activePlanet)
    {
        List<Planet> planetNeighbors = activePlanet.GetNeighbors();

        for (int i = 0; i < arrowButtons.Length; i++)
        {
            if(planetNeighbors[i] != null)
            {
                arrowButtons[i].SetDestination(planetNeighbors[i]);

                Vector3 neighborDirection = (planetNeighbors[i].transform.position -
                                             activePlanet.transform.position).normalized;

                Vector3 pos = neighborDirection * arrowDistance + activePlanet.transform.position;

                arrowButtons[i].transform.rotation = Quaternion.identity;

                float angle = Vector3.Angle(arrowButtons[i].transform.right, neighborDirection);
                if (neighborDirection.y < 0) angle = -angle;

                WorldToCanvasPosition(pos, arrowButtons[i].GetComponent<RectTransform>());
                
                arrowButtons[i].transform.rotation = Quaternion.Euler(0,0, angle);
            }
            else
            {
                arrowButtons[i].SetDestination(null);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void EVENT_End_Start_Game_Anim()
    {
        gameManager.EventAnimEndOpening();
    }


    /// <summary>
    /// 
    /// </summary>
    public void EVENT_Show_Active_Arrows()
    {
        for(int i = 0; i < arrowButtons.Length; i++)
        {
            if(arrowButtons[i].GetDestination() != null)
            {
                arrowButtons[i].ShowArrow();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void EVENT_End_New_Player_Animation()
    {
        gameManager.EventEndNewPlayerAnimation();
    }


    /// <summary>
    /// 
    /// </summary>
    public void EVENT_Hide_Active_Arrows()
    {
        for (int i = 0; i < arrowButtons.Length; i++)
        {
            if (arrowButtons[i].GetDestination() != null)
            {
                arrowButtons[i].HideArrow();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void NewPlayerAnimation()
    {
        anim.Play("new_player");
    }

    
    /// <summary>
    /// 
    /// </summary>
    public void PlayGameOverAnimation()
    {
        anim.Play("game_over");
    }


    /// <summary>
    /// Converte um coordenada do mundo para uma do canvas.
    /// </summary>
    void WorldToCanvasPosition(Vector3 worldPos, RectTransform uIElement)
    {
        Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        uIElement.anchoredPosition = WorldObject_ScreenPosition;
    }
}
