
public class Item
{
    public void Equip(Character c)
    {
        c.Health.AddModifier(new StatModifier(10, StatModType.Flat, this));
        c.Health.AddModifier(new StatModifier(0.1f, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Health.RemoveAllModifiersFromSource(this);

    }


}
