using JTUtility;

namespace TripleBladeHorse
{
    public class AddToGlobalObject : MonoSingleton<AddToGlobalObject>
    {
        protected override void Awake()
        {
            base.Awake();
            this.transform.SetParent(GlobalObject.Instance.transform);
        }
    }
}
