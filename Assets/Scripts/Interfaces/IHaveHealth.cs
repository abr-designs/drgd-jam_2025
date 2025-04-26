namespace Interfaces
{
    public interface IHaveHealth
    {
        int Health { get; }
        int StartingHealth { get; }

        void ApplyDamage(int damage);
    }
}