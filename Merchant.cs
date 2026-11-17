using System;

public class Merchant
{
    public bool ArmorSold { get; set; } = false;
    public bool WeaponSold { get; set; } = false;
    public int ArmorCost { get; } = 30;
    public int WeaponCost { get; } = 30;
    public int PotionCost { get; } = 10;

    public Weapon GetUpgradedWeapon(string? occupation)
    {
        string occ = occupation?.ToLower() ?? "";
        return occ switch
        {
            "fighter" => new Weapon("flaming broadsword", 18, "~*~-)=====>"),
            "magician" => new Weapon("meteor strike spell", 18, "(((*)))~~~"),
            "thief" => new Weapon("shadowblade dagger", 12, "}=---=>"),
            "archer" => new Weapon("elven composite bow", 14, "}}==-->"),
            _ => new Weapon("flaming broadsword", 18, "~*~-)=====>")
        };
    }

    public bool BuyArmor(Player player)
    {
        if (ArmorSold || player.Gold < ArmorCost)
            return false;

        player.Gold -= ArmorCost;
        player.Armor = new Armor("magical plate armor", 4, "[|###|]");
        ArmorSold = true;
        return true;
    }

    public bool BuyWeapon(Player player)
    {
        if (WeaponSold || player.Gold < WeaponCost)
            return false;

        player.Gold -= WeaponCost;
        player.Weapon = GetUpgradedWeapon(player.Occupation);
        WeaponSold = true;
        return true;
    }

    public bool BuyPotion(Player player)
    {
        if (player.Gold < PotionCost || player.HealingPotions >= 3)
            return false;

        player.Gold -= PotionCost;
        player.HealingPotions++;
        return true;
    }

    public bool IsValidChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2" || trimmed == "3" || trimmed == "4";
    }

    public void EnterShop(Player player, Messages messages)
    {
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("merchant_intro"));
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_gold_display"), player.Gold));
            Console.WriteLine(messages.GetMessage("merchant_menu"));

            string armorOption = messages.GetMessage("merchant_option_armor");
            if (ArmorSold)
                armorOption += messages.GetMessage("merchant_sold_out_tag");
            Console.WriteLine(armorOption);

            Weapon upgradedWeapon = GetUpgradedWeapon(player.Occupation);
            string weaponName = messages.TranslateWeaponForDisplay(upgradedWeapon.Type);
            string weaponOption = string.Format(messages.GetMessage("merchant_option_weapon"), weaponName);
            if (WeaponSold)
                weaponOption += messages.GetMessage("merchant_sold_out_tag");
            Console.WriteLine(weaponOption);
            Console.WriteLine(messages.GetMessage("merchant_option_potion"));
            Console.WriteLine(messages.GetMessage("merchant_option_exit"));
            Console.Write(messages.GetMessage("enter_choice"));

            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                if (ArmorSold)
                {
                    Console.WriteLine(messages.GetMessage("merchant_item_sold_out"));
                }
                else if (player.Gold < ArmorCost)
                {
                    Console.WriteLine(messages.GetMessage("merchant_not_enough_gold"));
                }
                else
                {
                    BuyArmor(player);
                    Console.WriteLine(messages.GetMessage("merchant_bought_armor"));
                }
                Console.WriteLine();
            }
            else if (choice == "2")
            {
                if (WeaponSold)
                {
                    Console.WriteLine(messages.GetMessage("merchant_item_sold_out"));
                }
                else if (player.Gold < WeaponCost)
                {
                    Console.WriteLine(messages.GetMessage("merchant_not_enough_gold"));
                }
                else
                {
                    BuyWeapon(player);
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_weapon"), weaponName));
                }
                Console.WriteLine();
            }
            else if (choice == "3")
            {
                if (player.HealingPotions >= 3)
                {
                    Console.WriteLine(messages.GetMessage("merchant_potion_belt_full"));
                }
                else if (player.Gold < PotionCost)
                {
                    Console.WriteLine(messages.GetMessage("merchant_not_enough_gold"));
                }
                else
                {
                    BuyPotion(player);
                    Console.WriteLine(messages.GetMessage("merchant_bought_potion"));
                }
                Console.WriteLine();
            }
            else if (choice == "4")
            {
                Console.WriteLine(messages.GetMessage("merchant_leave"));
                Console.WriteLine();
                return;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
                Console.WriteLine();
            }
        }
    }
}
