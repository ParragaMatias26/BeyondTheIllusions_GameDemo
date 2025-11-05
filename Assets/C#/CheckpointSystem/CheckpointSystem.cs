using System;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;

    private Checkpoint actualCheckpoint;
    private Vector3 initialPlayerPos;

    public event Action OnSaveGame = delegate { };
    public event Action OnRespawn = delegate { };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        initialPlayerPos = GameManager.Instance.Player.transform.position;
    }

    public void GetNewCheckpoint(Checkpoint newCheck) 
    {
        OnSaveGame();

        if (actualCheckpoint != newCheck) 
        {
            if(actualCheckpoint != null) actualCheckpoint.SetCheckSprite(false);
            newCheck.SetCheckSprite(true);

            actualCheckpoint = newCheck;
        } 

        Debug.Log("New Checkpoint Adquired");
    }

    public void RespawnInCheckpoint(Transform player)
    {
        if(actualCheckpoint) player.position = actualCheckpoint.EndPos;
        else player.position = initialPlayerPos;

        OnRespawn?.Invoke();
    }

    public Checkpoint GetActualCheckpoint()
    {
        return actualCheckpoint;
    }
}
