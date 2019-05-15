
namespace JTUtility.Interactables
{
	public interface IInteractable
	{
		/// <summary>
		/// Wether this object is being interacting.
		/// </summary>
		bool IsInteracting { get; }

		/// <summary>
		/// Wether this object is Activated.
		/// </summary>
		bool IsActivated { get; }

		/// <summary>
		/// Calls when interaction start.
		/// </summary>
		event Action OnStartInteracting;

		/// <summary>
		/// Calls when interaction keep on.
		/// </summary>
		event Action OnKeepInteracting;

		/// <summary>
		/// Calls when interaction end.
		/// </summary>
		event Action OnStopInteracting;

		/// <summary>
		/// Calls when this object is activated.
		/// </summary>
		event Action OnActivated;

		/// <summary>
		/// Calls when while this object is deactivated.
		/// </summary>
		event Action OnDeactivated;

		/// <summary>
		/// Starts interacting with this object.
		/// </summary>
		void StartInteracting();

		/// <summary>
		/// Stop interacting with this object.
		/// </summary>
		void StopInteracting();
	}
}
