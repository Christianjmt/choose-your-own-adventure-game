// CYOATests/MessagesTests.cs
[TestClass]
public class MessagesTests
{
    [TestMethod]
    public void Constructor_DefaultLanguageIsEnglish()
    {
        Messages messages = new Messages();
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        Assert.AreEqual("Spanish", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        Assert.AreEqual("French", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToItalian()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        Assert.AreEqual("Italian", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.SetCurrentLanguage("English");
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_LoadsEnglishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Welcome to Choose Your Adventure Game", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsSpanishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenido al Juego de Elige Tu Aventura", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsFrenchWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenue dans Choisissez Votre Aventure", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsItalianWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Benvenuto nel Gioco Scegli la Tua Avventura", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsFallbackForUnknownKey()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("unknown_key");
        Assert.AreEqual("[unknown_key]", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsDifferentMessageAfterLanguageSwitch()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string english = messages.GetMessage("welcome");

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string spanish = messages.GetMessage("welcome");

        Assert.AreNotEqual(english, spanish);
    }

    [TestMethod]
    public void ReadDictionary_LoadsGoodbyeMessageInEachLanguage()
    {
        Messages messages = new Messages();

        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Adventure ends. Bye.", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        Assert.AreEqual("Adios!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        Assert.AreEqual("Au revoir!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Assert.AreEqual("L'avventura finisce. Arrivederci.", messages.GetMessage("goodbye"));
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from English dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from Spanish dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from French dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInItalian()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from Italian dictionary.");
        }
    }

    // --- IsValidRace ---

    [TestMethod]
    public void IsValidRace_ReturnsTrue_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("human"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("HUMAN"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForInvalidRace()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace("orc"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace(null));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("human"));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("HUMAN"));
    }

    // --- IsValidOccupation ---

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("fighter"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("FIGHTER"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForInvalidOccupation()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation("wizard"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation(null));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("fighter"));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("FIGHTER"));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void ReadDictionary_LoadsAllMerchantKeys_AcrossLanguages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string[] requiredKeys = new[]
        {
            "stats_gold",
            "stats_armor",
            "stats_armor_defense",
            "armor_none",
            "merchant_intro",
            "merchant_gold_display",
            "merchant_menu",
            "merchant_option_armor",
            "merchant_option_weapon",
            "merchant_option_exit",
            "merchant_sold_out_tag",
            "merchant_bought_armor",
            "merchant_bought_weapon",
            "merchant_item_sold_out",
            "merchant_not_enough_gold",
            "merchant_leave",
            "armor_absorbed_damage"
        };

        foreach (var key in requiredKeys)
        {
            string message = messages.GetMessage(key);
            Assert.IsFalse(string.IsNullOrWhiteSpace(message), $"Key '{key}' was empty in {language}.");
            Assert.AreNotEqual($"[{key}]", message, $"Key '{key}' was missing in {language}.");
        }
    }

    [TestMethod]
    [DataRow("English", "magical plate armor")]
    [DataRow("Spanish", "armadura magica de placas")]
    [DataRow("French", "armure de plaques magique")]
    [DataRow("Italian", "armatura a piastre magica")]
    public void TranslateArmorForDisplay_TranslatesCorrectly_AcrossLanguages(string language, string expectedTranslation)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string result = messages.TranslateArmorForDisplay("magical plate armor");
        Assert.AreEqual(expectedTranslation, result);
    }

    [TestMethod]
    [DataRow("English", "flaming broadsword", "flaming broadsword")]
    [DataRow("Spanish", "flaming broadsword", "espada ancha llameante")]
    [DataRow("French", "flaming broadsword", "epee large flamboyante")]
    [DataRow("Italian", "flaming broadsword", "spadone fiammeggiante")]
    public void TranslateWeaponForDisplay_TranslatesUpgradedWeapon_AcrossLanguages(string language, string weaponKey, string expectedTranslation)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string result = messages.TranslateWeaponForDisplay(weaponKey);
        Assert.AreEqual(expectedTranslation, result);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void ReadDictionary_LoadsAllPotionAndShrineKeys_AcrossLanguages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string[] requiredKeys = new[]
        {
            "stats_potions",
            "potion_already_used_round",
            "potion_already_full_health",
            "potion_none_remaining",
            "potion_used_combat",
            "merchant_option_potion",
            "merchant_potion_belt_full",
            "merchant_bought_potion",
            "shrine_intro",
            "shrine_roll_prompt",
            "shrine_roll_result",
            "shrine_success",
            "shrine_success_belt_full",
            "shrine_failure",
            "shrine_sealed",
            "shrine_already_claimed"
        };

        foreach (var key in requiredKeys)
        {
            string message = messages.GetMessage(key);
            Assert.IsFalse(string.IsNullOrWhiteSpace(message), $"Key '{key}' was empty in {language}.");
            Assert.AreNotEqual($"[{key}]", message, $"Key '{key}' was missing in {language}.");
        }
    }
}
