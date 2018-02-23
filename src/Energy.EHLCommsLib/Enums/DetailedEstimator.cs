namespace Energy.EHLCommsLib.Enums
{
    public enum NumberOfBedrooms
    {
        OnetoTwo = 2,
        ThreetoFour = 4,
        MoreThanFive = 5
    }

    public enum HouseType
    {
        Flat = 1,
        DetachedHouse = 4,
        SemiDetachedHouse = 5
    }

    public enum NumberOfPeople
    {
        OnetoTwo = 2,
        ThreetoFour = 4,
        MoreThanFive = 5
    }

    public enum HeatingSource
    {
        Gas = 0,
        Electricity = 2,
        Other = 3
    }

    public enum HomeOccupied
    {
        HardlyEver = 0,
        EveningsandWeekends = 1,
        MostOfTheTime = 3
    }

    public enum HeatingLevel
    {
        Arctic = 0,
        Temperate = 1,
        Tropical = 2
    }

    public enum HomeInsulation
    {
        WaferThin = 0,
        WellWrapped = 1,
        AirtTight = 2
    }

    public enum CookingSource
    {
        Gas = 1,
        Electricity = 2,
        Other = 3
    }

    public enum CookingFrequency
    {
        Sometimes = 0,
        Daily = 1,
        AllTheTime = 2
    }
}