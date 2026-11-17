[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);
        Assert.AreEqual(0, player.MaxHealthPoints);
        Assert.AreEqual(0, player.HealingPotions);

        Assert.IsNull(player.Weapon);
        Assert.IsNotNull(player.Inventory);
    }

    [TestMethod]
    public void Player_InventoryDelegation_ReflectsDirectMutations()
    {
        Player player = new Player();
        player.Inventory.AddGold(25);
        player.Inventory.TryAddPotion();
        Armor armor = new Armor("magical plate armor", 4, "[|###|]");
        player.Inventory.EquipArmor(armor);

        Assert.AreEqual(75, player.Gold);
        Assert.AreEqual(1, player.HealingPotions);
        Assert.AreSame(armor, player.Armor);
    }

    [TestMethod]
    public void HealWithPotion_WhenDamaged_ShouldRestoreToMaxHealthAndDecrementPotion()
    {
        Player player = new Player
        {
            MaxHealthPoints = 20,
            HealthPoints = 5,
            HealingPotions = 2
        };

        bool success = player.HealWithPotion();

        Assert.IsTrue(success);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(1, player.HealingPotions);
    }

    [TestMethod]
    public void HealWithPotion_WhenAlreadyAtMaxHealth_ShouldReturnFalseAndPreservePotion()
    {
        Player player = new Player
        {
            MaxHealthPoints = 20,
            HealthPoints = 20,
            HealingPotions = 1
        };

        bool success = player.HealWithPotion();

        Assert.IsFalse(success);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(1, player.HealingPotions);
    }

    [TestMethod]
    public void HealWithPotion_WhenNoPotionsRemaining_ShouldReturnFalseAndNotChangeHealth()
    {
        Player player = new Player
        {
            MaxHealthPoints = 20,
            HealthPoints = 8,
            HealingPotions = 0
        };

        bool success = player.HealWithPotion();

        Assert.IsFalse(success);
        Assert.AreEqual(8, player.HealthPoints);
        Assert.AreEqual(0, player.HealingPotions);
    }

    [TestMethod]
    public void GetInventoryDisplay_DefaultPlayer_ShowsStartingWeaponAndNoArmor()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "fighter",
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Gold = 50,
            HealingPotions = 0
        };

        string output = player.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventory =====");
        StringAssert.Contains(output, "--- Equipped Gear ---");
        StringAssert.Contains(output, "Weapon: long sword (Max Damage: 12)");
        StringAssert.Contains(output, "Armor: None");
        StringAssert.Contains(output, "--- Carried Items ---");
        StringAssert.Contains(output, "Gold: 50");
        StringAssert.Contains(output, "Healing Potions: 0/3");
        StringAssert.Contains(output, "=====================");
    }

    [TestMethod]
    public void GetInventoryDisplay_UpgradedAndArmoredPlayer_ShowsUpgradedEquipment()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "fighter",
            Weapon = new Weapon("flaming broadsword", 18, "~*~-)=====>"),
            Armor = new Armor("magical plate armor", 4, "[|###|]"),
            Gold = 20,
            HealingPotions = 2
        };

        string output = player.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "Weapon: flaming broadsword (Max Damage: 18)");
        StringAssert.Contains(output, "Armor: magical plate armor (Damage Reduction: 4)");
        StringAssert.Contains(output, "Gold: 20");
        StringAssert.Contains(output, "Healing Potions: 2/3");
    }

    [TestMethod]
    [DataRow(0, "0/3")]
    [DataRow(1, "1/3")]
    [DataRow(3, "3/3")]
    public void GetInventoryDisplay_ShowsCorrectPotionCapacity(int potions, string expectedDisplay)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "fighter",
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Gold = 50,
            HealingPotions = potions
        };

        string output = player.GetInventoryDisplay(messages);
        StringAssert.Contains(output, $"Healing Potions: {expectedDisplay}");
    }

    [TestMethod]
    public void GetInventoryDisplay_Spanish_ShowsLocalizedText()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "fighter",
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Gold = 50,
            HealingPotions = 1
        };

        string output = player.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventario =====");
        StringAssert.Contains(output, "--- Equipo Equipado ---");
        StringAssert.Contains(output, "Arma: espada larga (Danio Maximo: 12)");
        StringAssert.Contains(output, "Armadura: Ninguna");
        StringAssert.Contains(output, "--- Objetos Llevados ---");
        StringAssert.Contains(output, "Oro: 50");
        StringAssert.Contains(output, "Pociones de Curacion: 1/3");
    }

    [TestMethod]
    public void GetInventoryDisplay_French_ShowsLocalizedText()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "magician",
            Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~"),
            Gold = 40,
            HealingPotions = 3
        };

        string output = player.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventaire =====");
        StringAssert.Contains(output, "--- Equipement Equipe ---");
        StringAssert.Contains(output, "Arme : sort d'eclair (Degats Max : 12)");
        StringAssert.Contains(output, "Armure : Aucune");
        StringAssert.Contains(output, "--- Objets Transportes ---");
        StringAssert.Contains(output, "Or : 40");
        StringAssert.Contains(output, "Potions de Guerison : 3/3");
    }

    [TestMethod]
    public void GetInventoryDisplay_Italian_ShowsLocalizedText()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "magician",
            Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~"),
            Gold = 40,
            HealingPotions = 3
        };

        string output = player.GetInventoryDisplay(messages);

        StringAssert.Contains(output, "===== Inventario =====");
        StringAssert.Contains(output, "--- Equipaggiamento Equipaggiato ---");
        StringAssert.Contains(output, "Arma: incantesimo fulmine (Danno Massimo: 12)");
        StringAssert.Contains(output, "Armatura: Nessuna");
        StringAssert.Contains(output, "--- Oggetti Trasportati ---");
        StringAssert.Contains(output, "Oro: 40");
        StringAssert.Contains(output, "Pozioni Curative: 3/3");
    }

    [TestMethod]
    public void DisplayInventory_WritesToConsole()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Player player = new Player
        {
            Occupation = "fighter",
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Gold = 50,
            HealingPotions = 0
        };

        using var sw = new StringWriter();
        var originalOut = Console.Out;
        try
        {
            Console.SetOut(sw);
            player.DisplayInventory(messages);
            string consoleOutput = sw.ToString();
            StringAssert.Contains(consoleOutput, "===== Inventory =====");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
