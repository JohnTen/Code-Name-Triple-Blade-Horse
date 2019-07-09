using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Combat
{
	public class Sheath : MonoBehaviour
	{
		[SerializeField] int numberOfKnife;
		[SerializeField] Transform launchPosition;
		[SerializeField] Transform[] sheathPositions;
		[SerializeField] Queue<ThrowingKnife> knifesInSheath;
		[SerializeField] List<ThrowingKnife> allKnifes;

		public float ReloadSpeed { get; set; } = 3;
		public Transform LaunchPosition => launchPosition;
		public int knifeCount => knifesInSheath.Count;

		public event Action<ThrowingKnife> OnRecievedKnife;

		bool reloading;

		private void Awake()
		{
			knifesInSheath = new Queue<ThrowingKnife>();

			for (int i = 0; i < allKnifes.Count; i++)
			{
				allKnifes[i].SetSheath(this);
				knifesInSheath.Enqueue(allKnifes[i]);
			}
		}

		public void UpdateFacingDirection(bool right)
		{
			if (right && this.transform.eulerAngles.y != 0)
			{
				this.transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (!right && this.transform.eulerAngles.y == 0)
			{
				this.transform.rotation = Quaternion.Euler(0, 180, 0);
			}
		}

		public ThrowingKnife TakeKnife(bool force)
		{
			if ((!force && reloading) || knifesInSheath.Count <= 0)
				return null;

			StartCoroutine(ReloadKnife());
			knifesInSheath.Peek().transform.parent = null;
			return knifesInSheath.Dequeue();
		}

		public void PutBackKnife(ThrowingKnife knife)
		{
			knife.transform.parent = sheathPositions[knifesInSheath.Count];
			knife.transform.position = sheathPositions[knifesInSheath.Count].position;
			knife.transform.rotation = sheathPositions[knifesInSheath.Count].rotation;
			knifesInSheath.Enqueue(knife);
			if (OnRecievedKnife != null)
				OnRecievedKnife.Invoke(knife);
		}

		IEnumerator ReloadKnife()
		{
			reloading = true;

			float timer = 0;

			while (timer < 1)
			{
				yield return null;
				int index = 0;
				timer += TimeManager.PlayerDeltaTime * ReloadSpeed;
				foreach (var item in knifesInSheath)
				{
					item.transform.parent = sheathPositions[index];
					item.transform.position = Vector3.Lerp(sheathPositions[index + 1].position, sheathPositions[index].position, timer);
					item.transform.rotation = Quaternion.Lerp(sheathPositions[index + 1].rotation, sheathPositions[index].rotation, timer);
					index++;
					if (index >= 2) break;
				}
			}

			reloading = false;
		}
	}
}