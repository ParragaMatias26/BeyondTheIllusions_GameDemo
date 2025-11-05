using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CinematicPlayer : MonoBehaviour
{
    public static CinematicPlayer Instance;

    [Header("Frames")]
    [SerializeField] Sprite[] frames;
    [SerializeField] Image cinematicImage;
    [SerializeField] Image blackImageFade;

    [Header("Cinematic Values")]
    [SerializeField] float cinematicDelayStart;
    [SerializeField] float timeInFrames;
    [SerializeField] float fadeTime;

    Color ogColor;
    Coroutine cinematicRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        ogColor = blackImageFade.color;
        cinematicRoutine = StartCoroutine(StartCinematic());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) EndCinematic();
    }

    IEnumerator StartCinematic()
    {
        GameManager.Instance.StopGlobalMovement();

        blackImageFade.gameObject.SetActive(!blackImageFade.gameObject.activeSelf);
        cinematicImage.gameObject.SetActive(!cinematicImage.gameObject.activeSelf);

        foreach (var frame in frames)
        {
            cinematicImage.sprite = frame;

            StartCoroutine(HideBlackFade());

            yield return new WaitForSeconds(timeInFrames);

            StartCoroutine(ShowBlackFade());

            yield return new WaitForSeconds(timeInFrames);
        }

        GameManager.Instance.ResumeGlobalMovement();

        blackImageFade.gameObject.SetActive(!blackImageFade.gameObject.activeSelf);
        cinematicImage.gameObject.SetActive(!cinematicImage.gameObject.activeSelf);
    }

    IEnumerator HideBlackFade()
    {
        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            blackImageFade.color = Color.Lerp(blackImageFade.color, new Color(1f, 1f, 1f, 0f), time / fadeTime);

            yield return null;
        }
    }

    IEnumerator ShowBlackFade()
    {
        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            blackImageFade.color = Color.Lerp(blackImageFade.color, ogColor, time / fadeTime);

            yield return null;
        }
    }

    void EndCinematic()
    {
        StopCoroutine(cinematicRoutine);

        GameManager.Instance.ResumeGlobalMovement();

        blackImageFade.gameObject.SetActive(false);
        cinematicImage.gameObject.SetActive(false);
    }
}
