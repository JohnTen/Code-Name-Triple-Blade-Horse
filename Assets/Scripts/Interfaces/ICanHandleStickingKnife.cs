using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
    public interface ICanHandleStickingKnife
    {
        void OnStickingKnife(ICanStickKnife canStick, ThrowingKnife knife);
    }
}
