using System.Collections;
using UnityEngine;

public class FungiExplosion : MonoBehaviour
{
    public float explosionDuration = .2f;
    public float explosionRadius = 3f;
    public GameObject explosionSprite;
    private SpriteFade spriteFade;

    private void Awake()
    {
        spriteFade = GetComponent<SpriteFade>();
    }

    private void Start()
    {
        spriteFade.StartFade();
        StartCoroutine(DestroyObjectAfterTime());

        explosionSprite.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
    }

    IEnumerator DestroyObjectAfterTime()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.GetComponent<Enemy>();
        //if (x != null) x.StartFleeBeheavior();
    }
}
