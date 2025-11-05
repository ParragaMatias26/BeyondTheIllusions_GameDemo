using System.Collections;
using UnityEngine;

public class CoinProyectile : MonoBehaviour
{
    float duration = 1f;
    private AnimationCurve animCurve;
    private float heightY = 3f;
    private new Collider2D collider2D;

    public void Initialize(float dur, AnimationCurve curve, float height, GameObject shadow, Vector3 dir)
    {
        duration = dur;
        animCurve = curve;
        heightY = height;

        collider2D = GetComponent<CircleCollider2D>();

        Vector3 itemShadowStartPosition = shadow.transform.position;

        StartCoroutine(ProjectileCurveRoutine(transform, transform.position, dir));
        StartCoroutine(MoveItemShadowRoutine(shadow, itemShadowStartPosition, dir));
    }

    IEnumerator ProjectileCurveRoutine(Transform coin, Vector3 startPosition, Vector3 dir)
    {
        float elapsedTime = 0f;

        Vector3 endPosition = startPosition + dir;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float linearT = elapsedTime / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            coin.position = Vector2.Lerp(startPosition, endPosition, linearT) + new Vector2(0f, height);

            yield return null;
        }

        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        collider2D.enabled = true;
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }

    IEnumerator MoveItemShadowRoutine(GameObject itemShadow, Vector3 startPosition, Vector3 dir)
    {
        float elapsedTime = 0f;

        Vector3 endPosition = startPosition + dir;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float linearT = elapsedTime / duration;
            itemShadow.transform.position = Vector2.Lerp(startPosition, endPosition, linearT);

            yield return null;
        }

        Destroy(itemShadow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.GetComponent<PlayerModel>();
        if (x != null) x.PlayerHealth.TakeDamage(5, transform.position, null, 0);
    }
}
