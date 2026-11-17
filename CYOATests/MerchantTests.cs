using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MerchantTests
{
    private Player CreateTestPlayer(string occupation = "Fighter", int gold = 50)
    {
        return new Player
        {
            Name = "Sir Test",
            Race = "Human",
            Occupation = occupation,
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Gold = gold,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void Merchant_InitialState_HasStockAndCorrectPricing()
    {
        Merchant merchant = new Merchant();

        Assert.IsFalse(merchant.ArmorSold);
        Assert.IsFalse(merchant.WeaponSold);
        Assert.AreEqual(30, merchant.ArmorCost);
        Assert.AreEqual(30, merchant.WeaponCost);
    }

    [TestMethod]
    public void Merchant_IsValidChoice_ValidatesInputCorrectly()
    {
        Merchant merchant = new Merchant();

        Assert.IsTrue(merchant.IsValidChoice("1"));
        Assert.IsTrue(merchant.IsValidChoice("2"));
        Assert.IsTrue(merchant.IsValidChoice("3"));
        Assert.IsTrue(merchant.IsValidChoice(" 1 "));

        Assert.IsFalse(merchant.IsValidChoice("0"));
        Assert.IsFalse(merchant.IsValidChoice("4"));
        Assert.IsFalse(merchant.IsValidChoice("attack"));
        Assert.IsFalse(merchant.IsValidChoice(""));
        Assert.IsFalse(merchant.IsValidChoice(null));
    }

    [TestMethod]
    [DataRow("Fighter", "flaming broadsword", 18, "~*~-)=====>")]
    [DataRow("Magician", "meteor strike spell", 18, "(((*)))~~~")]
    [DataRow("Thief", "shadowblade dagger", 12, "}=---=>")]
    [DataRow("Archer", "elven composite bow", 14, "}}==-->")]
    public void Merchant_GetUpgradedWeapon_ReturnsCorrectOccupationGear(string occupation, string expectedType, int expectedDamage, string expectedArt)
    {
        Merchant merchant = new Merchant();

        Weapon weapon = merchant.GetUpgradedWeapon(occupation);

        Assert.AreEqual(expectedType, weapon.Type);
        Assert.AreEqual(expectedDamage, weapon.MaxDamage);
        Assert.AreEqual(expectedArt, weapon.AsciiArt);
    }

    [TestMethod]
    public void Merchant_BuyArmor_SucceedsWhenPlayerHasEnoughGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Fighter", 50);

        bool success = merchant.BuyArmor(player);

        Assert.IsTrue(success);
        Assert.AreEqual(20, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual("magical plate armor", player.Armor.Type);
        Assert.AreEqual(4, player.Armor.DamageReduction);
        Assert.AreEqual("[|###|]", player.Armor.AsciiArt);
        Assert.IsTrue(merchant.ArmorSold);
    }

    [TestMethod]
    public void Merchant_BuyArmor_FailsWhenPlayerHasInsufficientGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Fighter", 20);

        bool success = merchant.BuyArmor(player);

        Assert.IsFalse(success);
        Assert.AreEqual(20, player.Gold);
        Assert.IsNull(player.Armor);
        Assert.IsFalse(merchant.ArmorSold);
    }

    [TestMethod]
    public void Merchant_BuyArmor_FailsWhenAlreadySoldOut()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Fighter", 100);

        bool firstPurchase = merchant.BuyArmor(player);
        bool secondPurchase = merchant.BuyArmor(player);

        Assert.IsTrue(firstPurchase);
        Assert.IsFalse(secondPurchase);
        Assert.AreEqual(70, player.Gold);
        Assert.IsTrue(merchant.ArmorSold);
    }

    [TestMethod]
    public void Merchant_BuyWeapon_SucceedsWhenPlayerHasEnoughGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Thief", 50);

        bool success = merchant.BuyWeapon(player);

        Assert.IsTrue(success);
        Assert.AreEqual(20, player.Gold);
        Assert.IsNotNull(player.Weapon);
        Assert.AreEqual("shadowblade dagger", player.Weapon.Type);
        Assert.AreEqual(12, player.Weapon.MaxDamage);
        Assert.IsTrue(merchant.WeaponSold);
    }

    [TestMethod]
    public void Merchant_BuyWeapon_FailsWhenPlayerHasInsufficientGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Archer", 15);

        bool success = merchant.BuyWeapon(player);

        Assert.IsFalse(success);
        Assert.AreEqual(15, player.Gold);
        Assert.AreEqual("long sword", player.Weapon!.Type);
        Assert.IsFalse(merchant.WeaponSold);
    }

    [TestMethod]
    public void Merchant_BuyWeapon_FailsWhenAlreadySoldOut()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Magician", 100);

        bool firstPurchase = merchant.BuyWeapon(player);
        bool secondPurchase = merchant.BuyWeapon(player);

        Assert.IsTrue(firstPurchase);
        Assert.IsFalse(secondPurchase);
        Assert.AreEqual(70, player.Gold);
        Assert.IsTrue(merchant.WeaponSold);
    }

    [TestMethod]
    public void Merchant_StartingGold_AllowsOnlyOneUpgrade()
    {
        Merchant merchant = new Merchant();
        Player player = CreateTestPlayer("Fighter", 50);

        bool armorBought = merchant.BuyArmor(player);
        bool weaponBought = merchant.BuyWeapon(player);

        Assert.IsTrue(armorBought);
        Assert.IsFalse(weaponBought);
        Assert.AreEqual(20, player.Gold);
    }
}
