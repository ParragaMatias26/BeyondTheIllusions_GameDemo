using UnityEngine;

public class SwordDMGCollider : MonoBehaviour
{
    private int swordDMG;
    private Transform _playerTransform;
    private PlayerModel _model;

    public void Initialize(int DMG, Transform playerTransform, PlayerModel model)
    {
        swordDMG = DMG;
        _playerTransform = playerTransform;
        _model = model;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.InLineOfSight(_playerTransform.position, other.gameObject.transform.position)) return;

        var x = other.gameObject.GetComponent<IDamageable>();
        if (x != null) 
        {
            x.TakeDamage(swordDMG, _model.transform.position, null, 0f);

            Vector2 attackDir = (other.transform.position - _playerTransform.position).normalized;
            _model.ApplyAttackKnockback(attackDir, true);
        } 
    }
}
