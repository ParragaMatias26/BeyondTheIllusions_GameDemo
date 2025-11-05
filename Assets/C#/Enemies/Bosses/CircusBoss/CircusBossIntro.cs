using System.Collections;
using UnityEngine;

public class CircusBossIntro : Cutscene
{
    public float zoomTarget = 10f;
    public float cutseneDuration = 2f;
    public float waitBeforeFight = 1.5f;
    public float timeAfterDarkTransition = 3f;
    public Transform camPosition;
    public GameObject Lights;
    public GameObject darkVignette;

    public RoomArea bossArea;
    public CameraRoomManager roomManager;
    
    public override IEnumerator Play(PlayerModel player, Camera mainCamera)
    {
        roomManager.SetBounds(bossArea);

        RealityChangeManager.Instance.ToggleCuteWorld(true);
        GameManager.Instance.Player._hallucinationManager.HallucinationAmmount = GameManager.Instance.Player._hallucinationManager.MaxHallucination;
        GameManager.Instance.Player._hallucinationManager.canDecrease = false;

        var controller = player.GetComponent<PlayerModel>();
        controller.Movement.CanMove = false;

        CutsceneManager.Instance.CameraZoom(mainCamera, zoomTarget, cutseneDuration);
        CutsceneManager.Instance.MoveCamera(mainCamera, camPosition.position, cutseneDuration);

        yield return new WaitForSeconds(timeAfterDarkTransition);

        Lights.SetActive(false);

        var halManager = GameManager.Instance.Player._hallucinationManager;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            halManager.HallucinationAmmount = Mathf.Lerp(halManager._hallucinationAmmount, 0f, t);
            yield return null;
        }

        RealityChangeManager.Instance.ToggleCuteWorld(false);
        darkVignette.SetActive(true);

        halManager.HallucinationAmmount = 0f;

        yield return new WaitForSeconds(waitBeforeFight);

        GameManager.Instance.CameraController.SetFollowActive(true);
        controller.Movement.CanMove = true;
        GameManager.Instance.Player._hallucinationManager.canDecrease = true;

        yield return new WaitForSeconds(2f);
        EndCutscene();
    }
}
