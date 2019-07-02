using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
	[SerializeField] float breakTime = 2;
	[SerializeField] float reappearTime = 2;

	public void StartBreaking()
	{
		if (!IsInvoking())
			Invoke("Break", breakTime);
	}

	private void Break()
	{
		this.gameObject.SetActive(false);
		Invoke("Reappear", reappearTime);
	}

	private void Reappear()
	{
		this.gameObject.SetActive(true);
	}
}
