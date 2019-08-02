namespace JTUtility.Interactables
{
    public class Button : MonoInteractable
    {
        public override void StartInteracting()
        {
            SetActiveStatus(true);

            base.StartInteracting();
        }

        public override void StopInteracting()
        {
            SetActiveStatus(false);

            base.StopInteracting();
        }
    }
}
