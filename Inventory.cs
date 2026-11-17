using System;
using System.Collections.Generic;

public class Inventory
{
    public const int MaxPotionCapacity = 3;

    public Weapon? EquippedWeapon { get; private set; }
    public Armor? EquippedArmor { get; private set; }
    public int Gold { get; private set; } = 50;
    public int HealingPotions { get; private set; } = 0;

    public bool CanAddPotion => HealingPotions < MaxPotionCapacity;

    public Inventory()
    {
    }

    public void SetGold(int amount)
    {
        Gold = Math.Max(0, amount);
    }

    public void SetHealingPotions(int amount)
    {
        HealingPotions = Math.Clamp(amount, 0, MaxPotionCapacity);
    }

    public void EquipWeapon(Weapon? weapon)
    {
        EquippedWeapon = weapon;
    }

    public void EquipArmor(Armor? armor)
    {
        EquippedArmor = armor;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount)
            return false;

        Gold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            Gold += amount;
        }
    }

    public bool TryAddPotion()
    {
        if (!CanAddPotion)
            return false;

        HealingPotions++;
        return true;
    }

    public bool TryConsumePotion()
    {
        if (HealingPotions <= 0)
            return false;

        HealingPotions--;
        return true;
    }

    public int MitigateDamage(int incomingDamage, out int absorbedDamage)
    {
        if (EquippedArmor != null)
        {
            return EquippedArmor.AbsorbDamage(incomingDamage, out absorbedDamage);
        }

        absorbedDamage = 0;
        return incomingDamage;
    }

    public string GetInventoryDisplay(Messages messages)
    {
        var lines = new List<string>
        {
            messages.GetMessage("inventory_header"),
            messages.GetMessage("inventory_equipped_header")
        };

        if (EquippedWeapon != null)
        {
            string weaponName = messages.TranslateWeaponForDisplay(EquippedWeapon.Type);
            lines.Add(string.Format(messages.GetMessage("inventory_weapon_format"), weaponName, EquippedWeapon.MaxDamage));
        }

        if (EquippedArmor != null)
        {
            string armorName = messages.TranslateArmorForDisplay(EquippedArmor.Type);
            lines.Add(string.Format(messages.GetMessage("inventory_armor_format"), armorName, EquippedArmor.DamageReduction));
        }
        else
        {
            lines.Add(messages.GetMessage("inventory_armor_none"));
        }

        lines.Add(messages.GetMessage("inventory_carried_header"));
        lines.Add(string.Format(messages.GetMessage("inventory_gold_format"), Gold));
        lines.Add(string.Format(messages.GetMessage("inventory_potions_format"), HealingPotions));
        lines.Add(messages.GetMessage("inventory_footer"));

        return string.Join(Environment.NewLine, lines);
    }
}
