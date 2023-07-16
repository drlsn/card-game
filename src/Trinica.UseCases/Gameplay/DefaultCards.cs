using Corelibs.Basic.DDD;
using System.Reflection;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;
using Trinica.Entities.ItemCards;
using Trinica.Entities.Shared;
using Trinica.Entities.SpellCards;
using Trinica.Entities.UnitCards;

using SpellCard = Trinica.Entities.Gameplay.Cards.SpellCard;

namespace Trinica.UseCases.Gameplay;

public static class DefaultCards
{
    public static readonly ICard[] All;

    static DefaultCards()
    {
        All = typeof(DefaultCards)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null))
            .OfType<ICard>()
            .ToArray();
    }

    #region Red House

    public static readonly HeroCard RH_Hero_RoodieTheCopier = new(
        EntityId.New<HeroCardId>(),
        "Roodie the copier",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 15,
            power: 0));


    public static readonly HeroCard RH_Hero_Ginny = new HeroCard(
        EntityId.New<HeroCardId>(),
        name: "Ginny",
        Race.Fairy,
        Class.Destroyer,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0)
    );

    public static readonly UnitCard RH_Unit_RedDwarf = new UnitCard(
    EntityId.New<UnitCardId>(),
    "Red Dwarf",
    Race.Shadow,
    Class.Destroyer,
    Fraction.RedHouse,
    new StatisticPointGroup(
        attack: 20,
        hp: 200,
        speed: 20,
        power: 0
    )
);

    public static readonly UnitCard RH_Unit_EnragedFairy = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Enraged Fairy",
        Race.Fairy,
        Class.Assasin,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly UnitCard RH_Unit_RedMage = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Red Mage",
        Race.Magus,
        Class.Wizard,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly UnitCard RH_Unit_TrainingDummy = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Training Dummy",
        Race.Construct,
        Class.Guardian,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly UnitCard RH_Unit_RedGiant = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Red Giant",
        Race.Shadow,
        Class.Destroyer,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly UnitCard RH_Unit_ExplosivesSpecialist = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Explosives Specialist",
        Race.Magus,
        Class.Destroyer,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly UnitCard RH_Unit_ChargingMage = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Charging Mage (Dressed like in rugby)",
        Race.Magus,
        Class.Destroyer,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 20,
            hp: 200,
            speed: 20,
            power: 0
        )
    );

    public static readonly SpellCard RH_Spell_CopyCard = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Copy Card",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_MidasTouch = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Midas Touch",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_FirstMove = new SpellCard(
        EntityId.New<SpellCardId>(),
        "First Move",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_EmergencyReserve = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Emergency Reserve",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new IEffect[] { },
        requiredElements: Enumerable.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_CopyDice = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Copy Dice",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new IEffect[] { },
        requiredElements: Enumerable.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_SpikedDice = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Spiked Dice",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new IEffect[] { },
        requiredElements: Enumerable.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard RH_Spell_Setup = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Setup",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new IEffect[] { },
        requiredElements: Enumerable.Empty<Element>(),
        damage: 0
    );

    public static readonly ItemCard RH_Item_TrainingDummy = new ItemCard(
    EntityId.New<ItemCardId>(),
    "Portable Training Dummy",
    Race.Shadow,
    Class.Destroyer,
    Fraction.RedHouse,
    new StatisticPointGroup(
        attack: 10,
        power: 0,
        speed: 0,
        hp: 0
    )
);

    public static readonly ItemCard RH_Item_FireWand = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Fire Wand",
        Race.Magus,
        Class.Support,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 5,
            speed: 0,
            hp: 0
        )
    );

    public static readonly ItemCard RH_Item_RedHouseSymbol = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Symbol of the Red House",
        Race.Plantoid,
        Class.Support,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 5,
            speed: 0,
            hp: 0
        )
    );

    public static readonly ItemCard RH_Item_CopyMirror = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Mirror Copy",
        Race.Shadow,
        Class.Support,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 0,
            speed: 0,
            hp: 0
        )
    );

    public static readonly ItemCard RH_Item_RedBirdKarma = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Red Bird Karma",
        Race.Shadow,
        Class.Destroyer,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 5,
            power: 0,
            speed: 0,
            hp: 0
        )
    );

    public static readonly ItemCard RH_Item_WhitePollen = new ItemCard(
        EntityId.New<ItemCardId>(),
        "White Pollen",
        Race.Fairy,
        Class.Support,
        Fraction.RedHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 0,
            speed: 1,
            hp: 0
        )
    );

    #endregion

    #region Orange House

    public static readonly HeroCard OH_Hero_GingerTheTimber = new HeroCard(
        EntityId.New<HeroCardId>(),
        "Ginger the Timber",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 25,
            hp: 200
        )
    );

    public static readonly HeroCard OH_Hero_BeatingatorV1_0 = new HeroCard(
        EntityId.New<HeroCardId>(),
        "Beatingator v1.0",
        Race.Construct,
        Class.Destroyer,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 24,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_DeceasedMage = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Deceased Mage",
        Race.Magus,
        Class.Guardian,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_OrangeMage = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Orange Mage",
        Race.Magus,
        Class.Destroyer,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_TheoryLecturer = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Theory Lecturer",
        Race.Magus,
        Class.Wizard,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_PotionSpecialist = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Potion Specialist",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_PotionAutomaton = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Potion Automaton",
        Race.Construct,
        Class.Guardian,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_AnimatedArmor = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Animated Armor",
        Race.Construct,
        Class.Destroyer,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard OH_Unit_MotivatedSpecter = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Motivated Specter",
        Race.Undead,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly SpellCard OH_Spell_CriticalRearmament = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Critical Rearmament",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_ExamFormation = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Exam Formation",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_StudentTrap = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Student Trap",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_BackupPotion = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Backup Potion",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_MandrakeScream = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Mandrake Scream",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_TeleportToTreasury = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Teleport to Treasury",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly SpellCard OH_Spell_StickyHands = new SpellCard(
        EntityId.New<SpellCardId>(),
        "Sticky Hands",
        Race.Magus,
        Class.Support,
        Fraction.OrangeHouse,
        Array.Empty<IEffect>(),
        requiredElements: Array.Empty<Element>(),
        damage: 0
    );

    public static readonly ItemCard OH_Item_HandyMandrake = new ItemCard(
    EntityId.New<ItemCardId>(),
    "Handy Mandrake",
    Race.Magus,
    Class.Assasin,
    Fraction.OrangeHouse,
    new StatisticPointGroup(
        attack: 0,
        power: 0,
        speed: 0,
        hp: 20
    )
);

    public static readonly ItemCard OH_Item_PotionRemnants = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Potion Remnants",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 10,
            power: 0,
            speed: 0,
            hp: 0
        )
    );

    public static readonly ItemCard OH_Item_StickyTape = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Sticky Tape",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 0,
            speed: 1,
            hp: 0
        )
    );

    public static readonly ItemCard OH_Item_SafeCode = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Safe Code",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 0,
            speed: 2,
            hp: 5
        )
    );

    public static readonly ItemCard OH_Item_OrangeHouseSymbol = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Orange House Symbol",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 0,
            power: 5,
            speed: 0,
            hp: 10
        )
    );

    public static readonly ItemCard OH_Item_RingOfPower = new ItemCard(
        EntityId.New<ItemCardId>(),
        "Ring of Power",
        Race.Magus,
        Class.Assasin,
        Fraction.OrangeHouse,
        new StatisticPointGroup(
            attack: 10,
            power: 5,
            speed: 0,
            hp: 15
        )
    );

    #endregion

    #region Yellow House

    public static readonly HeroCard YH_Hero_Ion = new HeroCard(
        EntityId.New<HeroCardId>(),
        "Ion not the fastest",
        Race.Magus,
        Class.Assasin,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 22,
            hp: 200
        )
    );

    public static readonly HeroCard YH_Hero_Sunarat = new HeroCard(
        EntityId.New<HeroCardId>(),
        "Sunarat",
        Race.Shadow,
        Class.Wizard,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 18,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_YellowMage = new UnitCard(
    EntityId.New<UnitCardId>(),
    "Yellow Mage",
    Race.Magus,
    Class.Assasin,
    Fraction.YellowHouse,
    new StatisticPointGroup(
        attack: 20,
        power: 0,
        speed: 20,
        hp: 200
    )
);

    public static readonly UnitCard YH_Unit_BatteryGolem = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Battery Golem",
        Race.Construct,
        Class.Destroyer,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_ManaSpecialist = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Mana Specialist",
        Race.Magus,
        Class.Support,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_LivingCurrent = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Living Current",
        Race.Elemental,
        Class.Assasin,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_ElectricTechnician = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Electric Technician",
        Race.Magus,
        Class.Guardian,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_Frankenstein = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Frankenstein",
        Race.Undead,
        Class.Destroyer,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    public static readonly UnitCard YH_Unit_ElectricFairy = new UnitCard(
        EntityId.New<UnitCardId>(),
        "Electric Fairy",
        Race.Fairy,
        Class.Assasin,
        Fraction.YellowHouse,
        new StatisticPointGroup(
            attack: 20,
            power: 0,
            speed: 20,
            hp: 200
        )
    );

    #endregion
}
