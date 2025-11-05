using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] public bool playOnAwake = false;
    [SerializeField] public float destroyDelay = .5f;
    private void Start()
    {
        if (playOnAwake) 
        {
            StartCoroutine(DestroyCoroutine(destroyDelay));
        }
    }
    public void StartCount(float delay) => StartCoroutine(DestroyCoroutine(delay));
    IEnumerator DestroyCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
