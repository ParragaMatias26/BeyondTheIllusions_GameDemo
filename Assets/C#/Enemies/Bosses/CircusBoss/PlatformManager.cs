using System.Collections.Generic;
using UnityEngine;
using System;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] Boss model;
    [SerializeField] BossPlatform platformPrefab;
    
    public BossPlatform currentPlatform;
    public List<GameObject> allPlatforms;
    private List<Vector3> platformsPositions = new List<Vector3>();

    public event Action OnPlatformDestroy = delegate { };
    public event Action OnLastPlatformDestroy = delegate { };
    public void InitializePlatforms() 
    {
        foreach (var platform in allPlatforms)
            platformsPositions.Add(platform.transform.position);

        foreach (var p in allPlatforms)
            p.GetComponent<BossPlatform>().Initialize(this);

        currentPlatform = allPlatforms[0].GetComponent<BossPlatform>();
        currentPlatform.PlatformHealt.CanTakeDamage = true;
    }
    public void AddToAllPlatforms(GameObject platform)
    {
        if (!allPlatforms.Contains(platform))
            allPlatforms.Add(platform);
    }
    public void RemovePlatform(GameObject platform)
    {
        if (!allPlatforms.Contains(platform)) return;

        OnPlatformDestroy?.Invoke();
        allPlatforms.Remove(platform);
        Destroy(platform);

        if (allPlatforms.Count == 0) 
        {
            OnLastPlatformDestroy?.Invoke();
            return;
        }

        currentPlatform = allPlatforms[0].GetComponent<BossPlatform>();
        currentPlatform.PlatformHealt.CanTakeDamage = true;
    }
    public void ResetPlatforms() 
    {
        foreach (var platform in allPlatforms)
            Destroy(platform);

        allPlatforms.Clear();

        foreach (var pos in platformsPositions) 
        {
            BossPlatform obj = Instantiate(platformPrefab, pos, Quaternion.identity);
            obj.Initialize(this);

            allPlatforms.Add(obj.gameObject);
        }

        currentPlatform = allPlatforms[0].GetComponent<BossPlatform>();
        currentPlatform.PlatformHealt.CanTakeDamage = true;
    }
}
