
public class Item
{
    public void Equip(Character c)
    {
        c.Muscle.AddModifier(new StatModifier(10, StatModType.Flat, this));
        c.Muscle.AddModifier(new StatModifier(0.1f, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Muscle.RemoveAllModifiersFromSource(this);

    }


}
