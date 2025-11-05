using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;
    public PixelPerfectCamera mPixelPerfectCamera;
    private Coroutine CurrentZoom;
    private Coroutine CurrentMove;

    float ogCameraSize;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start() => ogCameraSize = GameManager.Instance.MainCam.orthographicSize;
    public void PlayCutscene(Cutscene cutscene, PlayerModel player, Camera mainCamera) 
    {
        StartCoroutine(cutscene.Play(player, mainCamera));
    }
    public void CameraZoom(Camera cam, float targetSize, float duration) 
    {
        if(CurrentZoom != null)
        {
            StopCoroutine(CurrentZoom);
            CurrentZoom = null;
        }
        StartCoroutine(ZoomRoutine(cam, targetSize, duration));
    }
    IEnumerator ZoomRoutine(Camera cam, float targetSize, float duration) 
    {
        mPixelPerfectCamera.enabled = false;

        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }
    }
    public void MoveCamera(Camera cam, Vector3 newPos, float duration) 
    {
        if(CurrentMove != null) 
        {
            StopCoroutine(CurrentMove);
            CurrentMove = null;
        }
        StartCoroutine(MoveRoutine(cam, newPos, duration));
    }
    IEnumerator MoveRoutine(Camera cam, Vector3 newPos, float duration) 
    {
        var startPos = cam.transform.position;
        float elapsed = 0f;

        var fixedPos = new Vector3(newPos.x, newPos.y, -50f);
        GameManager.Instance.CameraController.SetFollowActive(false);

        while(elapsed < duration) 
        {
            elapsed += Time.deltaTime;
            cam.transform.position = Vector3.Lerp(startPos, fixedPos, elapsed / duration);
            yield return null;
        }
    }
    public void CancelZoom() 
    {
        mPixelPerfectCamera.enabled = true;
        CameraZoom(GameManager.Instance.MainCam, ogCameraSize, 1f);
    }
}
