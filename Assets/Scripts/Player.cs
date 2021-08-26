using System.Collections;
using System.Collections.Generic;
using Modifiers;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : LivingEntity
{
    public int inventorySize = 5;
    public ModifierInventory Inventory;
    
    void Start()
    {
        Heal(MaxHealth);
        Inventory = new ModifierInventory(inventorySize);
        // Inventory.Equip(new Ricochet());
        // Inventory.Equip(new GrapeShot());
        
        while (Inventory.Equip(new GrapeShot())) {}
    }

    void Update()
    {
        Inventory.Update();
    }
}
