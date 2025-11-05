using UnityEngine;
using UnityEngine.UI;

public class BloodCanvas : MonoBehaviour
{
    public static BloodCanvas Instance;

    [SerializeField] BloodItem _bloodImagePrefab;

    [Header("BloodValues")]
    [SerializeField] float fadeTime = 1.0f;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] Vector2 randomScaleValues;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CreateBlood()
    {
        var x = Random.Range(spawnArea.x, spawnArea.y);
        var y = Random.Range(spawnArea.x, spawnArea.y);

        Vector2 position = new Vector2(x, y);

        var blood = Instantiate(_bloodImagePrefab, (Vector2)transform.position + position, Quaternion.identity, transform);

        var randomScale = Random.Range(randomScaleValues.x , randomScaleValues.y);
        blood.transform.localScale *= randomScale;

        blood.PlayFade(fadeTime);
    }
}
