namespace TripleBladeHorse
{
    public interface ICanProcess<T>
    {
        T Process(T target);
    }
}