using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
    public interface ICanHandlePullingKnife
    {
        void OnPullingKnife(ICanStickKnife canStick, ThrowingKnife knife);
    }
}
