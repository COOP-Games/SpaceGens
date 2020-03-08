/*****************************************************
 * Autor: Leandro Dornela Ribeiro
 * Contact: leandrodornela@ice.ufjf.br
 * Date: 12/05/2019
 * Modified:
 * Arthur Gonze Machado - 8/2019
 * Leandro Dornela Ribeiro - 10/2019
 * **************************************************/


using UnityEngine;
using UnityEngine.UI;


// Nivel de zoom out, global para zoom maximo e local
// para zoom dos vizinhos do planeta.
enum ZoomOutType
{
    GLOBAL,
    LOCAL,
    NONE
}


/// <summary>
/// Controla o movimento e propriedades da camera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Hierarchy Reference")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    [Tooltip("Copntrolador de interface.")]
    private UIController uIController;
    [SerializeField]
    [Tooltip("Trasform com a posição de destino do zoom out.")]
    private GameObject zoomTransform;
    [SerializeField]
    [Tooltip("Botão de zoomIn.")]
    private Button zoomInButton;
    [SerializeField]
    [Tooltip("Botão de zoomOut.")]
    private Button zoomOutButton;
    [SerializeField]
    [Tooltip("Marcador de planeta ativo.")]
    private PlanetMarker planetMarker;
    [SerializeField]
    [Tooltip("Material das estrelas do fundo.")]
    private Material starsMaterial;

    [Header("Preferences")]
    [SerializeField]
    [Tooltip("Distancia do zoom out maximo.")]
    private int zoomOutDistance = 0;
    [SerializeField]
    [Tooltip("Tamanho aparente fixo dos icones.")]
    private float itemResizeFactor = 1;

    // Planeta ativo.
    private GameObject currentPlanet;
    // Posição de destino para ondea camera irá se mover.
    private GameObject target;
    // Nivel de zoom atual.
    private ZoomOutType zoomOutType;
    // Posição original da camera.
    private Vector3 originalPos;
    // Velocidade de movimento usada pela SmoothDamp.
    private Vector3 refSpeed = Vector3.zero;
    // Tempo do SmoothDamp.
    private float smoothTime = 0.3f;
    // Fator de escala interpolado dos objetos que mudam de tamanho com o movimento da camera.
    private float factor;


    /// <summary>
    /// Seta valores iniciais para as variaveis que precisam.
    /// </summary>
    void Awake()
    {
        originalPos = transform.position;
        zoomInButton.interactable = false;
        zoomOutButton.interactable = true;
        zoomOutType = ZoomOutType.NONE;
        factor = itemResizeFactor;
    }


    // Update is called once per frame
    void Update()
    {
        Move();
    }


    /// <summary>
    /// Interpola aposição da camera e rescala os icones de acordo com a distanca da camera.
    /// </summary>
    void Move()
    {
        if (target != null)
        {
            Vector3 newPos = Vector3.SmoothDamp(transform.position, target.transform.position + originalPos, ref refSpeed, smoothTime);
            Vector2 textureDesloc = (newPos - transform.position) / 500;
            transform.position = newPos;

            starsMaterial.mainTextureOffset = starsMaterial.mainTextureOffset + textureDesloc;


            if (zoomOutType == ZoomOutType.GLOBAL)
            {
                factor = Mathf.Lerp(factor, itemResizeFactor * 0.7f, 2 * Time.deltaTime);
            }
            else
            {
                factor = Mathf.Lerp(factor, itemResizeFactor, 2 * Time.deltaTime);
            }

            gameManager.ResizeItems(factor * transform.position.z / originalPos.z);
            gameManager.ResizePlanets(originalPos.z / transform.position.z);
            planetMarker.ResizeMarker(factor * transform.position.z / originalPos.z);
        }
        else
        {
            Debug.LogWarning("No target.");
        }
    }


    /// <summary>
    /// Seta a nova posição para onde a camera irá.
    /// </summary>
    public void SetTarget(GameObject tar)
    {
        target = tar;
    }


    /// <summary>
    /// Seta a posição instantaneamente da camera.
    /// </summary>
    public void SetPosition(GameObject tar)
    {
        if (tar.GetComponentInChildren<Collider>())
        {
            if (tar.GetComponentInChildren<Collider>().tag == "Planet")
            {
                currentPlanet = tar;
            }
        }

        target = tar;
        transform.position = tar.transform.position + originalPos;
    }


    /// <summary>
    /// Afasta a camera levando em consideração o nivel de zoom atual.
    /// </summary>
    public void ZoomOut()
    {
        if (gameManager.GetState() == GameState.inGame)
        {
            // Se não estava com zoom out vai para o nivel local.
            if (zoomOutType == ZoomOutType.NONE)
            {
                zoomOutType = ZoomOutType.LOCAL;
                currentPlanet = target;

                zoomTransform.gameObject.transform.position = new Vector3(currentPlanet.transform.position.x,
                                                                          currentPlanet.transform.position.y,
                                                                          zoomOutDistance / 2);

                target = zoomTransform.gameObject;
                zoomInButton.interactable = true;
                zoomOutButton.interactable = true;



                planetMarker.DisplayMarker();
                gameManager.ShowConnectionLines();
                uIController.EVENT_Hide_Active_Arrows();
            }
            // Se estava com zoom local vai para o global.
            else if (zoomOutType == ZoomOutType.LOCAL)
            {
                zoomOutType = ZoomOutType.GLOBAL;
                // Encontra o centro do mapa e move a camera para la.
                Vector3 zoomOut = gameManager.GetMapCenter();
                zoomTransform.gameObject.transform.position = new Vector3(zoomOut.x, zoomOut.y, zoomOutDistance);

                target = zoomTransform.gameObject;
                zoomInButton.interactable = true;
                zoomOutButton.interactable = false;
            }
        }
    }


    /// <summary>
    /// Aproxima a camera levando em consideração o nivel de zoom atual.
    /// </summary>
    public void ZoomIn()
    {
        //no zoom out at all
        if (zoomOutType == ZoomOutType.LOCAL)
        {
            zoomOutType = ZoomOutType.NONE;
            target = currentPlanet;
            zoomInButton.interactable = false;
            zoomOutButton.interactable = true;

            planetMarker.HideMarker();
            gameManager.HideConnectionLines();
            uIController.EVENT_Show_Active_Arrows();
        }
        else if (zoomOutType == ZoomOutType.GLOBAL) // return to the 1st level of zoom out
        {
            zoomOutType = ZoomOutType.LOCAL;
            zoomTransform.gameObject.transform.position = new Vector3(currentPlanet.transform.position.x,
                                                                      currentPlanet.transform.position.y,
                                                                      zoomOutDistance / 2);
            target = zoomTransform.gameObject;
            zoomInButton.interactable = true;
            zoomOutButton.interactable = true;
        }
    }


    /// <summary>
    /// Reseta o estado de zoom.
    /// </summary>
    public void ResetZoomOut()
    {
        zoomOutType = ZoomOutType.NONE;
        zoomInButton.interactable = false;
        zoomOutButton.interactable = true;

        planetMarker.HideMarker();
    }
}
