using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    
    private GunCombat gunCombat;
    private SwordCombat swordCombat;
    public bool isPlanted;
    public bool IsPlanted => swordCombat.isPlanted;

    private void Awake()
    {
        swordCombat = GetComponent<SwordCombat>();
        gunCombat = GetComponent<GunCombat>();
    }

    private void Update()
    {
        swordCombat.HandleAttack();
        gunCombat.HandleGunAttacks();
    }
}