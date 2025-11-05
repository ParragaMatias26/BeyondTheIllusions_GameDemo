using UnityEngine;

public class MemoriesManager : MonoBehaviour
{
    public static MemoriesManager Instance;
    [SerializeField] GameObject[] photoPieces;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowFragment(int index) 
    {
        if (photoPieces[index] != null) photoPieces[index].gameObject.SetActive(true);
        else Debug.LogWarning($"Photo Index Not Found");
    } 
}
