namespace JTUtility.Interactables
{
    public class Switch : MonoInteractable
    {
        public override void StartInteracting()
        {
            SetActiveStatus(!isActivated);

            base.StartInteracting();
        }
    }
}
