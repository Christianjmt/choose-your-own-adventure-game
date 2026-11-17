public class Armor
{
    public string Type { get; set; }
    public int DamageReduction { get; set; }
    public string AsciiArt { get; set; }

    public Armor(string type, int damageReduction, string asciiArt)
    {
        Type = type;
        DamageReduction = damageReduction;
        AsciiArt = asciiArt;
    }

    public int AbsorbDamage(int incomingDamage, out int absorbedDamage)
    {
        int finalDamage = Math.Max(1, incomingDamage - DamageReduction);
        absorbedDamage = incomingDamage - finalDamage;
        return finalDamage;
    }
}
