using UnityEngine;
using System.Collections;

public class AudienceManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints = new Transform[5];
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private GameObject damageVFX;

    [Header("Proyectile Settings")]
    [SerializeField] private int projectileDmg;
    [SerializeField] private float projectileSpeed;

    [Header("Wave")]
    [SerializeField] private Vector2 timeBetweenWaves;

    private Transform target;
    private Coroutine CurrentLoop;
    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }
    public void StartLoop() 
    {
        if (CurrentLoop != null) return;
            CurrentLoop = StartCoroutine(ThrowLoop());
    }
    public void StopLoop() 
    {
        if(CurrentLoop != null) StopCoroutine(CurrentLoop);
        CurrentLoop = null;
    }
    private IEnumerator ThrowLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeBetweenWaves.x, timeBetweenWaves.y));
            ThrowWave();
        }
    }
    private void ThrowWave()
    {
        Transform[] chosenSpawns = ChooseRandomSpawns(3);

        int mushroomIndex = Random.Range(0, chosenSpawns.Length);

        for (int i = 0; i < chosenSpawns.Length; i++)
        {
            Transform sp = chosenSpawns[i];
            GameObject prefab;

            bool isMush = (i == mushroomIndex);

            prefab = isMush ? mushroomPrefab : damagePrefab;

            GameObject go = Instantiate(prefab, sp.position, Quaternion.identity);

            var dir = (target.position - go.transform.position).normalized;

            if (isMush)
                go.AddComponent<Projectile>();

            go.GetComponent<Projectile>().Initialize(
                dir,
                target.position,
                projectileSpeed,
                isMush,
                mushroomPrefab,
                projectileDmg,
                damageVFX);
        }
    }
    private Transform[] ChooseRandomSpawns(int count)
    {
        Transform[] result = new Transform[count];
        Transform[] pool = (Transform[])spawnPoints.Clone();


        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Length - i);
            result[i] = pool[index];
            pool[index] = pool[pool.Length - 1 - i];
        }
        return result;
    }
}
