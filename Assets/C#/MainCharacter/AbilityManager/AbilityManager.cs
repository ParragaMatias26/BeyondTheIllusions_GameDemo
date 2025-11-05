using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private Dictionary<string, Ability> _abilities = new();

    private void Start()
    {
        foreach (var ability in GetComponents<Ability>())
        {
            //Debug.Log($"Registrando habilidad: {ability.abilityName}");
            _abilities[ability.abilityName] = ability;
        }
    }
    public void UnlockAbility(string abilityName)
    {
        if (_abilities.TryGetValue(abilityName, out Ability ability))
        {
            ability.isUnlocked = true;
        }
    }
    public Ability GetAbilityByName(string abilityName)
    {
        _abilities.TryGetValue(abilityName, out Ability ability);
        return ability;
    }
}
