using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionCanvas : MonoBehaviour
{
    public static TransitionCanvas Instance { get; private set; }

    [Header("Transition Values")]
    [SerializeField] Image fadeImage;
    [SerializeField] float transitionTime = 5f;
    [SerializeField] float interTransitionTime = .2f;

    Color originalColor;
    float r, g, b;

    Coroutine transitionRoutine;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        originalColor = fadeImage.color;
    }

    private void Start()
    {
        r = originalColor.r;
        g = originalColor.g;
        b = originalColor.b;
    }

    public void FadeScreenEffect()
    {
        if (transitionRoutine != null) return;

        fadeImage.color = new Color(r, g, b, 0f);
        transitionRoutine = StartCoroutine(FadeRoutine(transitionTime, 1f));
    }

    IEnumerator FadeRoutine(float time, float alpha)
    {
        float elapsedTime = 0f;

        Color startColor = fadeImage.color;
        Color endColor = new Color(r, g, b, alpha);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / time);
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;

        if (alpha == 1f)
        {
            yield return new WaitForSeconds(interTransitionTime);
            yield return StartCoroutine(FadeRoutine(time / 2f, 0f));
        }

        transitionRoutine = null;
    }
}
