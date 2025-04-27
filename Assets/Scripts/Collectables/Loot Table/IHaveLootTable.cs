public interface IHaveLootTable
{
    LootTable LootTable { get; }
    void CalculateDropRates();
}