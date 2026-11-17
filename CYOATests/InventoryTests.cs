using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class InventoryTests
{
    private Messages CreateMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void Constructor_SetsDefaultValues()
    {
        Inventory inventory = new Inventory();

        Assert.AreEqual(50, inventory.Gold);
        Assert.AreEqual(0, inventory.HealingPotions);
        Assert.IsNull(inventory.EquippedWeapon);
        Assert.IsNull(inventory.EquippedArmor);
        Assert.IsTrue(inventory.CanAddPotion);
    }

    // --- Currency Tests ---

    [TestMethod]
    public void TrySpendGold_WhenExactFunds_DeductsAndReturnsTrue()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(30);

        bool success = inventory.TrySpendGold(30);

        Assert.IsTrue(success);
        Assert.AreEqual(0, inventory.Gold);
    }

    [TestMethod]
    public void TrySpendGold_WhenSufficientFunds_DeductsAndReturnsTrue()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(50);

        bool success = inventory.TrySpendGold(30);

        Assert.IsTrue(success);
        Assert.AreEqual(20, inventory.Gold);
    }

    [TestMethod]
    public void TrySpendGold_WhenInsufficientFunds_PreservesGoldAndReturnsFalse()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(25);

        bool success = inventory.TrySpendGold(30);

        Assert.IsFalse(success);
        Assert.AreEqual(25, inventory.Gold);
    }

    [TestMethod]
    public void TrySpendGold_WhenZeroOrNegative_ReturnsFalseAndPreservesGold()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(50);

        Assert.IsFalse(inventory.TrySpendGold(0));
        Assert.IsFalse(inventory.TrySpendGold(-10));
        Assert.AreEqual(50, inventory.Gold);
    }

    [TestMethod]
    public void AddGold_WhenPositive_IncreasesBalance()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(10);

        inventory.AddGold(40);

        Assert.AreEqual(50, inventory.Gold);
    }

    [TestMethod]
    public void AddGold_WhenZeroOrNegative_DoesNotChangeBalance()
    {
        Inventory inventory = new Inventory();
        inventory.SetGold(10);

        inventory.AddGold(0);
        inventory.AddGold(-5);

        Assert.AreEqual(10, inventory.Gold);
    }

    // --- Potion Belt Invariants ---

    [TestMethod]
    public void TryAddPotion_WhenUnderCapacity_IncrementsAndReturnsTrue()
    {
        Inventory inventory = new Inventory();
        inventory.SetHealingPotions(0);

        Assert.IsTrue(inventory.TryAddPotion());
        Assert.AreEqual(1, inventory.HealingPotions);

        Assert.IsTrue(inventory.TryAddPotion());
        Assert.AreEqual(2, inventory.HealingPotions);

        Assert.IsTrue(inventory.TryAddPotion());
        Assert.AreEqual(3, inventory.HealingPotions);
        Assert.IsFalse(inventory.CanAddPotion);
    }

    [TestMethod]
    public void TryAddPotion_WhenAtCapacity_ReturnsFalseAndPreservesCount()
    {
        Inventory inventory = new Inventory();
        inventory.SetHealingPotions(3);

        bool success = inventory.TryAddPotion();

        Assert.IsFalse(success);
        Assert.AreEqual(3, inventory.HealingPotions);
    }

    [TestMethod]
    public void TryConsumePotion_WhenPotionsAvailable_DecrementsAndReturnsTrue()
    {
        Inventory inventory = new Inventory();
        inventory.SetHealingPotions(2);

        bool success = inventory.TryConsumePotion();

        Assert.IsTrue(success);
        Assert.AreEqual(1, inventory.HealingPotions);
    }

    [TestMethod]
    public void TryConsumePotion_WhenZeroPotions_ReturnsFalse()
    {
        Inventory inventory = new Inventory();
        inventory.SetHealingPotions(0);

        bool success = inventory.TryConsumePotion();

        Assert.IsFalse(success);
        Assert.AreEqual(0, inventory.HealingPotions);
    }

    // --- Damage Mitigation ---

    [TestMethod]
    public void Armor_AbsorbDamage_ReducesDamageByReductionValue()
    {
        Armor armor = new Armor("magical plate armor", 4, "[|###|]");

        int finalDamage = armor.AbsorbDamage(10, out int absorbed);

        Assert.AreEqual(4, absorbed);
        Assert.AreEqual(6, finalDamage);
    }

    [TestMethod]
    public void Armor_AbsorbDamage_FloorsDamageAtOne_WhenAbsorptionExceedsOrEqualsIncoming()
    {
        Armor armor = new Armor("magical plate armor", 4, "[|###|]");

        // When incoming equals reduction: 4 incoming - 4 reduction -> floored at 1 -> absorbed is 4 - 1 = 3
        int finalDamageEqual = armor.AbsorbDamage(4, out int absorbedEqual);
        Assert.AreEqual(3, absorbedEqual);
        Assert.AreEqual(1, finalDamageEqual);

        // When incoming less than reduction: 2 incoming - 4 reduction -> floored at 1 -> absorbed is 2 - 1 = 1
        int finalDamageLess = armor.AbsorbDamage(2, out int absorbedLess);
        Assert.AreEqual(1, absorbedLess);
        Assert.AreEqual(1, finalDamageLess);

        // When incoming is 1: floored at 1 -> absorbed is 1 - 1 = 0
        int finalDamageOne = armor.AbsorbDamage(1, out int absorbedOne);
        Assert.AreEqual(0, absorbedOne);
        Assert.AreEqual(1, finalDamageOne);
    }

    [TestMethod]
    public void Inventory_MitigateDamage_WhenUnarmored_PassesRawDamageWithZeroAbsorbed()
    {
        Inventory inventory = new Inventory();

        int finalDamage = inventory.MitigateDamage(8, out int absorbed);

        Assert.AreEqual(0, absorbed);
        Assert.AreEqual(8, finalDamage);
    }

    [TestMethod]
    public void Inventory_MitigateDamage_WhenArmored_DelegatesToArmor()
    {
        Inventory inventory = new Inventory();
        inventory.EquipArmor(new Armor("magical plate armor", 4, "[|###|]"));

        int finalDamage = inventory.MitigateDamage(10, out int absorbed);

        Assert.AreEqual(4, absorbed);
        Assert.AreEqual(6, finalDamage);
    }

    // --- Equipment Assignment ---

    [TestMethod]
    public void EquipWeapon_ReplacesEquippedWeapon()
    {
        Inventory inventory = new Inventory();
        Weapon sword = new Weapon("long sword", 12, "-)=====>");

        inventory.EquipWeapon(sword);

        Assert.AreSame(sword, inventory.EquippedWeapon);
    }

    [TestMethod]
    public void EquipWeapon_AllowsNullToUnequip()
    {
        Inventory inventory = new Inventory();
        inventory.EquipWeapon(new Weapon("long sword", 12, "-)=====>"));
        Assert.IsNotNull(inventory.EquippedWeapon);

        inventory.EquipWeapon(null);
        Assert.IsNull(inventory.EquippedWeapon);
    }

    [TestMethod]
    public void EquipArmor_ReplacesEquippedArmor()
    {
        Inventory inventory = new Inventory();
        Armor armor = new Armor("magical plate armor", 4, "[|###|]");

        inventory.EquipArmor(armor);

        Assert.AreSame(armor, inventory.EquippedArmor);
    }

    // --- Presentation Formatting ---

    [TestMethod]
    public void GetInventoryDisplay_FormatsCorrectlyInEnglish()
    {
        Messages messages = CreateMessages("English");
        Inventory inventory = new Inventory();
        inventory.EquipWeapon(new Weapon("flaming broadsword", 18, "~*~-)=====>"));
        inventory.EquipArmor(new Armor("magical plate armor", 4, "[|###|]"));
        inventory.SetGold(20);
        inventory.SetHealingPotions(2);

        string output = inventory.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventory =====");
        StringAssert.Contains(output, "--- Equipped Gear ---");
        StringAssert.Contains(output, "  Weapon: flaming broadsword (Max Damage: 18)");
        StringAssert.Contains(output, "  Armor: magical plate armor (Damage Reduction: 4)");
        StringAssert.Contains(output, "--- Carried Items ---");
        StringAssert.Contains(output, "  Gold: 20");
        StringAssert.Contains(output, "  Healing Potions: 2/3");
        StringAssert.Contains(output, "=====================");
    }

    [TestMethod]
    public void GetInventoryDisplay_UnarmoredInSpanish()
    {
        Messages messages = CreateMessages("Spanish");
        Inventory inventory = new Inventory();
        inventory.EquipWeapon(new Weapon("long sword", 12, "-)=====>"));
        inventory.SetGold(50);
        inventory.SetHealingPotions(0);

        string output = inventory.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventario =====");
        StringAssert.Contains(output, "--- Equipo Equipado ---");
        StringAssert.Contains(output, "  Arma: espada larga (Danio Maximo: 12)");
        StringAssert.Contains(output, "  Armadura: Ninguna");
        StringAssert.Contains(output, "--- Objetos Llevados ---");
        StringAssert.Contains(output, "  Oro: 50");
        StringAssert.Contains(output, "  Pociones de Curacion: 0/3");
    }

    [TestMethod]
    public void GetInventoryDisplay_ArmoredInFrench()
    {
        Messages messages = CreateMessages("French");
        Inventory inventory = new Inventory();
        inventory.EquipWeapon(new Weapon("lightning bolt spell", 12, "zap~~~~~~"));
        inventory.EquipArmor(new Armor("magical plate armor", 4, "[|###|]"));
        inventory.SetGold(40);
        inventory.SetHealingPotions(3);

        string output = inventory.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventaire =====");
        StringAssert.Contains(output, "--- Equipement Equipe ---");
        StringAssert.Contains(output, "  Arme : sort d'eclair (Degats Max : 12)");
        StringAssert.Contains(output, "  Armure : armure de plaques magique (Reduction des Degats : 4)");
        StringAssert.Contains(output, "--- Objets Transportes ---");
        StringAssert.Contains(output, "  Or : 40");
        StringAssert.Contains(output, "  Potions de Guerison : 3/3");
    }

    [TestMethod]
    public void GetInventoryDisplay_ArmoredInItalian()
    {
        Messages messages = CreateMessages("Italian");
        Inventory inventory = new Inventory();
        inventory.EquipWeapon(new Weapon("lightning bolt spell", 12, "zap~~~~~~"));
        inventory.EquipArmor(new Armor("magical plate armor", 4, "[|###|]"));
        inventory.SetGold(40);
        inventory.SetHealingPotions(3);

        string output = inventory.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventario =====");
        StringAssert.Contains(output, "--- Equipaggiamento Equipaggiato ---");
        StringAssert.Contains(output, "  Arma: incantesimo fulmine (Danno Massimo: 12)");
        StringAssert.Contains(output, "  Armatura: armatura a piastre magica (Riduzione del Danno: 4)");
        StringAssert.Contains(output, "--- Oggetti Trasportati ---");
        StringAssert.Contains(output, "  Oro: 40");
        StringAssert.Contains(output, "  Pozioni Curative: 3/3");
    }
}
