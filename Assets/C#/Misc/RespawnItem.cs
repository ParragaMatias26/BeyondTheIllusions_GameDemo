using System.Collections;
using UnityEngine;

public class RespawnItem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform spawnPos;

    [SerializeField] GameObject actualItem;

    [Header("Respawn Values")]
    [SerializeField] float respawnTime;

    Coroutine respawnRoutine;
    private void Update()
    {
        if(actualItem == null && respawnRoutine == null) respawnRoutine = StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        actualItem = Instantiate(itemPrefab, spawnPos.position, Quaternion.identity, transform);
        respawnRoutine = null;
    }
}
