namespace Interfaces
{
    public enum upgradeType
    {
        ShieldUpgrade,

        RollUpgrade,

        CapacityUpgrade
    }
    public interface IHaveUpgrade
    {
        float multiplier { get; }


        void ApplyUpgrade(float newMultiplier);
    }
}