using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject endScreen;

    public bool startInInitialPos;
    public Transform startPos;

    public PlayerModel Player;
    public HallucinationManager HallucinationManager;
    public CameraFollow CameraController;
    public Camera MainCam;
    public GridGenerator GridGen;
    public CameraRoomManager RoomManager;

    public PerkManager PlayerPerkManager;
    public PerkInventoryUI PerkInventoryUI;

    public float genericDieAnimTime = 1.5f;
    public float circusBossPlayerKillTime = 8f;

    [Header("OnEnemyHit - Shake Values")]
    public float onHitDuration = .1f;
    public float onHitMagnitude = .1f;

    [Header("Enemies Flocking Values")]
    public List<Enemy> Enemies = new List<Enemy>();
    public float radiusSeparation;
    public float weightSeparation;
    public float weightAligment;
    public float weightCohesion;
    public float weightAvoidance;

    [Header("ObstaclesMask")]
    public string obstaclesLayerMask = "Obstacles";
    public string propsLayerMask = "Props";

    [Header("Canvas")]
    public GameObject[] AllCanvas;

    [Header("VFX")]
    public GameObject[] AllVFX;

    [Header("Develop Info")]
    public bool ShowDevelopInfo = false;
    public List<DevelopInfo> AllDevelopInfo = new List<DevelopInfo>();

    public List<PinkMushroom> Mushrooms = new List<PinkMushroom>();
    public Checkpoint[] AllCheckpoints;

    public event Action StopMovementEvent = delegate {  };
    public event Action ResumeMovementEvent = delegate { };

    public bool canActiveVFX = true;
    bool vfxActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        if (startInInitialPos) Player.transform.position = startPos.position;

        if (!Application.isEditor) 
            Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) CheckpointSystem.Instance.RespawnInCheckpoint(Player.transform);

        if (Input.GetKeyDown(KeyCode.F1)) Develop_TPToCheckpoint(0);
        if (Input.GetKeyDown(KeyCode.F2)) Develop_TPToCheckpoint(1);
        if (Input.GetKeyDown(KeyCode.F3)) Develop_TPToCheckpoint(2);
        if (Input.GetKeyDown(KeyCode.F4)) Develop_TPToCheckpoint(3);
        if (Input.GetKeyDown(KeyCode.F5)) Develop_TPToCheckpoint(4);
        if (Input.GetKeyDown(KeyCode.F6)) Develop_TPToCheckpoint(5);

        if (Input.GetKeyDown(KeyCode.F7)) 
        {
            Player._hallucinationManager.HallucinationAmmount = Player._hallucinationManager.MaxHallucination;
            RealityChangeManager.Instance.ToggleCuteWorld(true);
        }

        if (Input.GetKeyDown(KeyCode.F8)) 
        {
            Player._hallucinationManager.HallucinationAmmount = 0f;
            RealityChangeManager.Instance.ToggleCuteWorld(false);
        }

        if (Input.GetKeyDown(KeyCode.M)) ShowDevelopInfo = !ShowDevelopInfo;

        if (ShowDevelopInfo) 
        {
            foreach(var d in AllDevelopInfo) 
            {
                d.gameObject.SetActive(true);
            }
        }
        else 
        {
            foreach(var d in AllDevelopInfo) 
            {
                d.gameObject.SetActive(false);
            }
        }


        if (canActiveVFX) {

            if (!RealityChangeManager.Instance.CuteWorld && !vfxActive) {
                ToggleVFX(true);
            }
            else if (RealityChangeManager.Instance.CuteWorld && vfxActive) {
                ToggleVFX(false);
            }
        }
        else  {
            ToggleVFX(false);
        }
    }
    public bool InLineOfSight(Vector3 start, Vector3 end)
    {
        int obstacleMask = LayerMask.GetMask(obstaclesLayerMask, propsLayerMask);
        var dir = end - start;

        return !Physics2D.Raycast(start, dir, dir.magnitude, obstacleMask);
    }
    public void ToggleAllCanvas(bool state) 
    {
        foreach(var c in AllCanvas) 
        {
            c.SetActive(state);
        }
    }
    public void StopGlobalMovement() => StopMovementEvent();
    public void ResumeGlobalMovement() => ResumeMovementEvent();
    void Develop_TPToCheckpoint(int id) => Develop_TpPlayer(AllCheckpoints[id].EndPos);
    void Develop_TpPlayer(Vector3 newPos) => Player.transform.position = newPos;
    public void ToggleVFX(bool state) 
    {
        foreach (var item in AllVFX)
            item.SetActive(state);

        vfxActive = state;
    }
    public void BTN_CloseGame() => Application.Quit();
    public IEnumerator EndGame_Execute() 
    {
        StopGlobalMovement();

        yield return new WaitForSeconds(7f);
        endScreen.SetActive(true);
    }
}
