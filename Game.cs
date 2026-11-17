public class Game
{
    public bool ShrineClaimed { get; set; } = false;
    public bool ShrineSealed { get; set; } = false;

    public enum ShrineTrialResult
    {
        SuccessClaimed,
        SuccessBeltFull,
        FailedSealed,
        AlreadyClaimed,
        AlreadySealed
    }

    public ShrineTrialResult EvaluateShrineTrial(Player player, int d20Roll)
    {
        if (ShrineClaimed)
            return ShrineTrialResult.AlreadyClaimed;

        if (ShrineSealed)
            return ShrineTrialResult.AlreadySealed;

        if (d20Roll >= 12)
        {
            if (player.HealingPotions < 3)
            {
                player.HealingPotions++;
                ShrineClaimed = true;
                return ShrineTrialResult.SuccessClaimed;
            }
            else
            {
                return ShrineTrialResult.SuccessBeltFull;
            }
        }
        else
        {
            ShrineSealed = true;
            return ShrineTrialResult.FailedSealed;
        }
    }

    public void ResetShrineAfterDragonRetreat()
    {
        ShrineSealed = false;
    }

    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Seleziona lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string? langInput = Console.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Player player = new Player();
                player.CreateCharacter(messages);

                Merchant merchant = new Merchant();

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = PromptForPath(messages);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        merchant.EnterShop(player, messages);
                    }
                    else if (selectedPath == "east")
                    {
                        HandleShrineEncounter(player, die, messages);
                    }
                    else if (selectedPath == "inventory")
                    {
                        player.DisplayInventory(messages);
                    }
                    else if (selectedPath == "north")
                    {
                        bool adventureEnded = HandleDragonEncounter(player, dragon, messages);
                        if (adventureEnded)
                        {
                            running = false;
                            inAdventureMenu = false;
                        }
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "e" || trimmed == "east" ||
               trimmed == "i" || trimmed == "inv" || trimmed == "inventory" ||
               trimmed == "x" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "h" || trimmed == "heal" ||
               trimmed == "i" || trimmed == "inv" || trimmed == "inventory" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private string PromptForPath(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("path_prompt_full"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "east" || input == "e")
                return "east";

            if (input == "i" || input == "inv" || input == "inventory")
                return "inventory";

            if (input == "exit" || input == "x")
                return "exit";

            Console.WriteLine(messages.GetMessage("path_invalid"));
        }
    }

    public void HandleShrineEncounter(Player player, Die die, Messages messages)
    {
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("shrine_intro"));
        Console.WriteLine();

        if (ShrineClaimed)
        {
            Console.WriteLine(messages.GetMessage("shrine_already_claimed"));
            Console.WriteLine();
            return;
        }

        if (ShrineSealed)
        {
            Console.WriteLine(messages.GetMessage("shrine_sealed"));
            Console.WriteLine();
            return;
        }

        while (true)
        {
            Console.WriteLine(messages.GetMessage("shrine_roll_prompt"));
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input == "roll")
                break;

            Console.WriteLine(messages.GetMessage("roll_invalid"));
        }

        int roll = die.Roll(20);
        Console.WriteLine(string.Format(messages.GetMessage("shrine_roll_result"), roll));

        var result = EvaluateShrineTrial(player, roll);
        if (result == ShrineTrialResult.SuccessClaimed)
        {
            Console.WriteLine(Combat.PotionAsciiArt);
            Console.WriteLine(messages.GetMessage("shrine_success"));
        }
        else if (result == ShrineTrialResult.SuccessBeltFull)
        {
            Console.WriteLine(messages.GetMessage("shrine_success_belt_full"));
        }
        else if (result == ShrineTrialResult.FailedSealed)
        {
            Console.WriteLine(messages.GetMessage("shrine_failure"));
        }

        Console.WriteLine();
    }

    private bool HandleDragonEncounter(Player player, Dragon dragon, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);
        while (true)
        {
            Console.WriteLine(messages.GetMessage("dragon_encounter_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
            {
                Combat combat = new Combat(player, dragon, messages);
                bool playerWon = combat.StartCombat();

                if (combat.PlayerRetreated)
                {
                    ResetShrineAfterDragonRetreat();
                    return false; // combat already handled retreat message
                }
                else
                {
                    EndGame(playerWon, false, messages);
                    return true;
                }
            }

            if (input == "r" || input == "retreat")
            {
                ResetShrineAfterDragonRetreat();
                Console.WriteLine(messages.GetMessage("retreat"));
                return false;
            }

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }
}
