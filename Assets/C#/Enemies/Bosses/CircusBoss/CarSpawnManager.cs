using System.Collections;
using UnityEngine;

public class CarSpawnManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] ClownCar carPrefab;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] private float initialDelay = 0f;
    [SerializeField] private Vector2 spawnTimeValues;
    [SerializeField] bool spawnToLeft = false;

    [Header("Car Values")]
    [SerializeField] private float carSpeed;
    [SerializeField] private int carDmg;

    private ClownCar currentCar;
    private bool canSpawn;
    public void StartSpawnLoop() 
    {
        canSpawn = true;
        StartCoroutine(SpawnLoop_Execute());
    }
    public void StopSpawnLoop() 
    {
        canSpawn = false;
        StopAllCoroutines();
    }
    IEnumerator SpawnLoop_Execute() 
    {
        if(initialDelay > 0)
            yield return new WaitForSeconds(initialDelay);

        while (canSpawn) 
        {
            if(currentCar == null) 
            {
                currentCar = Instantiate(carPrefab, startPos.position, Quaternion.identity);
                currentCar.OnCarDestroyed += HandleCarDestroyer;
                currentCar.left = spawnToLeft;

                currentCar.Initialize(endPos, carSpeed, carDmg);
            }

            float waitTime = Random.Range(spawnTimeValues.x, spawnTimeValues.y);
            yield return new WaitForSeconds(waitTime);
        }
    }
    private void HandleCarDestroyer() => currentCar = null;
}
