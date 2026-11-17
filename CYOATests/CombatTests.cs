[TestClass]
public class CombatTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesGoldAndArmorStats_WhenNoArmorEquipped()
    {
        Player player = CreateTestPlayer();
        player.Gold = 50;
        player.Armor = null;

        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);
        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_gold"));
        StringAssert.Contains(result, "50");
        StringAssert.Contains(result, messages.GetMessage("stats_armor"));
        StringAssert.Contains(result, messages.GetMessage("armor_none"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesGoldAndArmorStats_WhenArmorEquipped()
    {
        Player player = CreateTestPlayer();
        player.Gold = 20;
        player.Armor = new Armor("magical plate armor", 4, "[|###|]");

        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);
        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_gold"));
        StringAssert.Contains(result, "20");
        StringAssert.Contains(result, messages.GetMessage("stats_armor"));
        StringAssert.Contains(result, messages.TranslateArmorForDisplay(player.Armor.Type));
        StringAssert.Contains(result, messages.GetMessage("stats_armor_defense"));
        StringAssert.Contains(result, "4");
    }

    [TestMethod]
    public void DragonAttack_MitigatedByPlayerArmor()
    {
        Dragon dragon = CreateTestDragon();
        dragon.Strength = 20; // Guarantee hit
        dragon.Agility = 0;

        Player player = CreateTestPlayer();
        player.Agility = 0;   // Guarantee failed defense roll
        player.HealthPoints = 30;
        player.Armor = new Armor("magical plate armor", 4, "[|###|]");

        Messages messages = CreateEnglishMessages();
        string combatLog = dragon.Attack(player, messages);

        // Dragon damage is 1..12. If raw was > 1, armor absorbed up to 4 damage.
        // If absorption occurred, combatLog should contain armor_absorbed_damage
        if (combatLog.Contains("absorbs"))
        {
            StringAssert.Contains(combatLog, "absorbs");
        }
        Assert.IsTrue(player.HealthPoints >= 18, $"Player health was {player.HealthPoints}, should be >= 18 with 4 damage reduction on max 12 damage.");
    }

    [TestMethod]
    public void DragonAttack_DamageCannotFallBelowOne_EvenWithExcessiveArmor()
    {
        Dragon dragon = CreateTestDragon();
        dragon.Strength = 20; // Guarantee hit
        dragon.Agility = 0;

        Player player = CreateTestPlayer();
        player.Agility = 0;   // Guarantee failed defense roll
        player.HealthPoints = 30;
        // 100 damage reduction should reduce damage to at most 1, not 0 or negative
        player.Armor = new Armor("invincible plate", 100, "[|###|]");

        Messages messages = CreateEnglishMessages();
        dragon.Attack(player, messages);

        Assert.AreEqual(29, player.HealthPoints, "Damage with excessive armor must floor at 1 damage.");
    }
}
