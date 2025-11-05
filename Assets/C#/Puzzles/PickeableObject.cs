using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PickeableObject : MonoBehaviour
{
    public bool isPickedUp = false;
    public bool isThrowedUp = false;
    public bool canPickup = true;

    [Header("Properties")]
    [SerializeField] bool _throweable;
    [SerializeField] bool _explodeOnLand = false;
    [SerializeField] bool _onlyPickeableOnCuteWorld = true;
    [SerializeField] bool _dropOnDarkWorld = true;
    [SerializeField] GameObject _explosionPrefab;

    public bool OnlyCuteWorldPickeable { get { return _onlyPickeableOnCuteWorld; } }
    public bool Throweable { get { return _throweable; } }
    public bool DropOnDarkWorld { get { return _dropOnDarkWorld; } }

    [Header("Throw Values")]
    [SerializeField] float throwRange = 2f;
    [SerializeField] float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject throwProjectileShadow;
    [SerializeField] private Sprite throwSprite;
    [SerializeField] private GameObject throwEndMark;

    [Header("Renderers")]
    [SerializeField] SpriteRenderer _cuteWorldRenderer;
    [SerializeField] SpriteRenderer _darkWorldRenderer;

    [Header("Frame Sprite")]
    [SerializeField] GameObject FrameSprite;

    [Header("UI Values")]
    [SerializeField] Sprite _cuteSprite;
    [SerializeField] Sprite _darkSprite;
    [SerializeField] GameObject inputMark;
    public Sprite CuteItemSprite { get { return _cuteSprite; } }
    public Sprite DarkItemSprite { get { return _darkSprite; } }

    private GameObject mark;

    public event Action OnDrop = delegate { };

    private void Awake()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }
    public PickeableObject Pick(Transform pos)
    {
        if (!canPickup || !GameManager.Instance.InLineOfSight(transform.position, GameManager.Instance.Player.transform.position)) return null;

        if(inputMark != null) inputMark.SetActive(false);

        transform.position = pos.position;
        transform.parent = pos;

        StopAllCoroutines();

        isPickedUp = true;
        FrameSprite.SetActive(false);

        return this;
    }
    public void Drop()
    {
        if (!canPickup) return;

        if (inputMark != null) inputMark.SetActive(true);

        transform.parent = null;
        isPickedUp = false;
        OnDrop?.Invoke();
    }
    public void Throw(Vector3 dir)
    {
        if (isThrowedUp) return;
        isThrowedUp = true;

        Vector3 endPosition = transform.position + dir * throwRange;
        mark = Instantiate(throwEndMark, endPosition, Quaternion.identity);

        if (throwSprite != null)
        {
            _cuteWorldRenderer.sprite = throwSprite;
            _darkWorldRenderer.sprite = throwSprite;
        }

        if (_cuteWorldRenderer != null) _cuteWorldRenderer.enabled = true;
        GameObject itemShadow = Instantiate(throwProjectileShadow, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);
        Vector3 itemShadowStartPosition = itemShadow.transform.position;

        StartCoroutine(ProjectileCurveRoutine(transform.position, dir));
        StartCoroutine(MoveItemShadowRoutine(itemShadow, itemShadowStartPosition, dir));
    }
    IEnumerator ProjectileCurveRoutine(Vector3 startPosition, Vector3 dir)
    {
        float elapsedTime = 0f;

        Vector3 endPosition = startPosition + dir * throwRange;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float linearT = elapsedTime / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPosition, endPosition, linearT) + new Vector2(0f, height);

            yield return null;
        }

        if (_explodeOnLand) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(mark);
    }
    IEnumerator MoveItemShadowRoutine(GameObject itemShadow, Vector3 startPosition, Vector3 dir)
    {
        float elapsedTime = 0f;

        Vector3 endPosition = startPosition + dir * throwRange;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float linearT = elapsedTime / duration;
            itemShadow.transform.position = Vector2.Lerp(startPosition, endPosition, linearT);

            yield return null;
        }

        Destroy(itemShadow);
    }
    float GetRandomOustideRange(float min1, float max1, float min2, float max2)
    {
        if(UnityEngine.Random.value < .5f)
            return UnityEngine.Random.Range(min1, max1); //Lado izquierdo
        else
            return UnityEngine.Random.Range(min2, max2); //Lado derecho
        
    }
    private void OnDestroy()
    {
        if(InventoryUIManager.Instance != null) InventoryUIManager.Instance.HideInventorySlot();
    }
    public void ShowPickFrame()
    {
        if (isPickedUp || isThrowedUp) return;
        if (FrameSprite != null) FrameSprite.SetActive(true);
        if (inputMark != null) inputMark.SetActive(true);
    }
    public void HidePickFrame()
    {
        if (isPickedUp || isThrowedUp) return;
        if(FrameSprite != null) FrameSprite.SetActive(false);
        if(inputMark != null) inputMark.SetActive(false);
    }
}
