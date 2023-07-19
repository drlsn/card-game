using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Cards;

public abstract class Card
{
    protected Card() {}
    protected Card(
        string name, 
        Race race, 
        Class @class, 
        Fraction fraction)
    {
        Name = name;
        Race = race;
        Class = @class;
        Fraction = fraction;
    }

    public string Name { get; private set; } = "Nameless";
    public Race Race { get; private set; } = Race.Fairy;
    public Class Class { get; private set; } = Class.Destroyer;
    public Fraction Fraction { get; private set; } = Fraction.Homeless;
}
