using System.Collections;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró un SpriteRenderer en este objeto.");
        }
    }

    public void StartFade()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
    }
}
