using JTUtility;

namespace TripleBladeHorse.Combat
{
    public interface IAttacker
    {
        void Activate(AttackPackage attack, AttackMove move);
        void Deactivate();

        event Action<IAttackable, AttackResult, AttackPackage> OnHit;
    }
}