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
}
