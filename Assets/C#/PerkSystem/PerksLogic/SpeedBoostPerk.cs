using UnityEngine;
[CreateAssetMenu(fileName = "SpeedBoostPerk", menuName = "Perks/Speed Boost Perk")]
public class SpeedBoostPerk : PerkSO
{
    public float speedBonus = 3f;
    public override void Apply(PlayerStats stats, GameObject player) => stats.dw_MovementSpeed += speedBonus;
    public override void Remove(PlayerStats stats, GameObject player) => stats.dw_MovementSpeed -= speedBonus;
}
