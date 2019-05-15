using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace JTUtility.UI
{
	[System.Serializable]
	struct SwitchButtonData
	{
		public string Label;
		public Color ButtonColor;
		public Color LabelColor;
		public StringEvent OnSwitchOn;
	}

	class SwitchButton : MonoBehaviour
	{
		[System.Serializable]
		class IntEvent : UnityEvent<int> { }

		[SerializeField]
		SwitchButtonData[] data;

		[SerializeField]
		int CurrentIndex;

		[SerializeField]
		IntEvent OnSwitch;

		[SerializeField]
		Image ButtonImage;

		[SerializeField]
		Text ButtonLabel;

		bool disabledEvent;

		private void Awake()
		{
			ButtonImage = GetComponentInChildren<Image>();
			ButtonLabel = GetComponentInChildren<Text>();
			disabledEvent = true;
			SetTo(CurrentIndex);
			disabledEvent = false;
		}

		public void Switch()
		{
			CurrentIndex++;
			if (CurrentIndex >= data.Length) CurrentIndex = 0;

			setButton(CurrentIndex);
		}

		public void SetTo(int index)
		{
			setButton(index);
		}

		void setButton(int index)
		{
			CurrentIndex = index;
			if (ButtonImage != null) ButtonImage.color = data[index].ButtonColor;
			if (ButtonLabel != null) ButtonLabel.color = data[index].LabelColor;
			if (ButtonLabel != null) ButtonLabel.text = data[index].Label;

			if (disabledEvent)
				return;

			if (data[index].OnSwitchOn != null)
				data[index].OnSwitchOn.Invoke(data[index].Label);

			OnSwitch.Invoke(index);
		}
	}
}
