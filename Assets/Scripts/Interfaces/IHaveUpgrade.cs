namespace Interfaces
{
    public enum UpgradeType
    {
        ShieldUpgrade = 0,
        RollUpgrade = 1,
        CapacityUpgrade = 2,
    }

    public interface IHaveUpgrade
    {
        float multiplier { get; }

        void ApplyUpgrade(float newMultiplier);
    }
}