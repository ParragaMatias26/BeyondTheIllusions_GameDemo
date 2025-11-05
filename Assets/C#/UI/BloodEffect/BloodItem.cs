using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodItem : MonoBehaviour
{
    [SerializeField] Image cwImage;
    [SerializeField] Image dwImage;
    public void PlayFade(float time)
    {
        StartCoroutine(FadeDissapearRoutine(time, cwImage));
        StartCoroutine(FadeDissapearRoutine(time, dwImage));
    }
    IEnumerator FadeDissapearRoutine(float time, Image myImage)
    {
        float elapsedTime = 0f;
        Color myColor = myImage.color;

        var r = myColor.r;
        var g = myColor.g;
        var b = myColor.b;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            myImage.color = Color.Lerp(myColor, new Color(r, g, b, 0f), elapsedTime / time);

            yield return null;
        }

        myColor.a = 0f;
        Destroy(gameObject);
    }
}
