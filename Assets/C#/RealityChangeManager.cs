using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class RealityChangeManager : MonoBehaviour
{
    public static RealityChangeManager Instance;

    [SerializeField] Camera _camera;
    [SerializeField] HallucinationManager m_HallucinationManager;

    [Header("Transition Values")]
    [SerializeField] string transitionFloatName = "Progress";
    [SerializeField] Light2D globalLight;
    [SerializeField] Tilemap[] tilemaps;
    [SerializeField] Color gray_DarkWorld;
    [SerializeField] Color white_CuteWorld;

    private float lastToggleTime;

    [SerializeField][Range(0f, 1f)] float lerpValue;

    private bool cuteWorld = false;
    public bool CuteWorld { get { return cuteWorld; } }

    private bool wasOnCuteWorld = false;

    public static event Action OnCuteWorldEnabled = delegate { };
    public static event Action OnCuteWorldDisabled = delegate { };

    public float HallucinationAmmount { get { return m_HallucinationManager.HallucinationAmmount; } set { m_HallucinationManager.HallucinationAmmount = value; } }
    public float MaxHallucination { get { return m_HallucinationManager.MaxHallucination; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        PlayerModel.OnPlayerDie += () =>
        {
            StopAllCoroutines();
            ToggleCuteWorld(false);
            m_HallucinationManager.HallucinationAmmount = 0f;
        };
    }

    private void Start()
    {
        if(!Application.isEditor)
            Shader.SetGlobalFloat(transitionFloatName, 0f);
    }

    private void Update()
    {
        SetEnviromentValues();
    }

    public void ToggleCuteWorld(bool isCuteWorldEnabled)
    {
        if (isCuteWorldEnabled)
        {

            if (!wasOnCuteWorld) 
            {
                wasOnCuteWorld = true;
                OnCuteWorldEnabled();
            }

            cuteWorld = true;
        }
        else 
        {

            if (wasOnCuteWorld) 
            {
                wasOnCuteWorld = false;
                OnCuteWorldDisabled();
            }

            cuteWorld = false;
        }
    }
    void SetEnviromentValues()
    {
        if (tilemaps.Length <= 0 || m_HallucinationManager == null) return;

        lerpValue = m_HallucinationManager.HallucinationAmmount / m_HallucinationManager.MaxHallucination;

        Color currentColor = Color.Lerp(gray_DarkWorld, white_CuteWorld, lerpValue);
        foreach (Tilemap t in tilemaps) t.color = currentColor;

        Shader.SetGlobalFloat(transitionFloatName, lerpValue);
        globalLight.intensity = lerpValue;
    }
    public void DisableCuteWorld() 
    {
        ToggleCuteWorld(false);
        GameManager.Instance.Player._hallucinationManager.HallucinationAmmount = 0;
    }
    public void EnableCuteWorld() 
    {
        ToggleCuteWorld(true);
        GameManager.Instance.Player._hallucinationManager.HallucinationAmmount = GameManager.Instance.Player._hallucinationManager.MaxHallucination;
    }
}
