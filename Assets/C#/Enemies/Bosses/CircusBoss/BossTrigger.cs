using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class BossTrigger : MonoBehaviour
{
    public Boss bossModel;
    public Cutscene bossIntro;

    bool hasCutscenePlayed = false;
    private void Start()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.gameObject.GetComponent<PlayerModel>();
        if (x != null) 
        {
            bossModel.PlayBossIntro();

            var player = GameManager.Instance.Player;
            var camera = GameManager.Instance.MainCam;

            GameManager.Instance.canActiveVFX = false;

            if (!hasCutscenePlayed)
                CutsceneManager.Instance.PlayCutscene(bossIntro, player, camera);
            else
                CutsceneManager.Instance.CameraZoom(GameManager.Instance.MainCam, 13.5f, 1.5f);

            hasCutscenePlayed = true;
            gameObject.SetActive(false);
        }
    }
}
