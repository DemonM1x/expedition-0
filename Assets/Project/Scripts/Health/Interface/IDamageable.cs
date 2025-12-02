namespace Expedition0.Health
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void Heal(float health);
        float GetCurrentHealth();
        float GetMaxHealth();
        bool IsDead();
    }
}