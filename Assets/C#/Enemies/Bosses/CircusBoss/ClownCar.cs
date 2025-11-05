using System;
using UnityEngine;
public class ClownCar : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    public bool left = false;

    private Transform _endPos;
    private float _carSpeed;
    private int _carDmg;

    public Action OnCarDestroyed = delegate { };
    public void Initialize(Transform endPos, float carSpeed, int carDmg) 
    {
        _endPos = endPos;
        _carSpeed = carSpeed;
        _carDmg = carDmg;

        if(left)
            foreach(var s in sprites)
                s.flipX = true;
    }
    private void Update()
    {
        var dir = (_endPos.position - transform.position).normalized;
        transform.position += dir * _carSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _endPos.position) <= .5f) Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var x = collision.gameObject.GetComponent<PlayerModel>();
        if(x != null) 
            x.PlayerHealth.TakeDamage(_carDmg, transform.position, null, GameManager.Instance.circusBossPlayerKillTime);
    }
    private void OnDestroy()
    {
        OnCarDestroyed?.Invoke();
    }
}
