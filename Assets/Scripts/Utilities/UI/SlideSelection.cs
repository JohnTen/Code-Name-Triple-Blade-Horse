using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JTUtility.UI
{
	public class SlideSelection : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] Vector2 viewportExpandment;
		[SerializeField] float spacing;
		[SerializeField] int numberOfShowingItems;
		[SerializeField] Vector2 itemSize;
		[SerializeField] Sprite[] items;
		[SerializeField] bool level;
		[SerializeField] Gradient gradientByRange = null;
		[SerializeField] AnimationCurve scaleByRange = new AnimationCurve();

		[SerializeField] bool horizontal;
		[SerializeField] Canvas viewport = null;
		[SerializeField] Sprite idleMask = null;
		[SerializeField] Sprite selectingMask = null;

		public IntEvent onValueChange;
		public IntEvent onValueSettle;

		List<Image> images = new List<Image>();

		Image viewportMaskImage = null;
		Vector3 relativePosToPointer;

		Vector2 viewportIdleSize;
		Vector2 viewportExpandedSize;

		Vector2 viewportRange;

		int tailIndex;
		int headIndex;

		void Initialize()
		{
			// Setting up parameters for viewport
			var size = itemSize;
			var length = horizontal ? itemSize.x : itemSize.y;

			viewportRange.x = -length;
			viewportRange.y = length;

			length = length * numberOfShowingItems + spacing * (numberOfShowingItems - 1);

			viewportRange.x -= length * 0.5f;
			viewportRange.y += length * 0.5f;

			if (horizontal)
				size.x = length;
			else
				size.y = length;

			viewportIdleSize = itemSize;
			viewportExpandedSize = size;

			viewportIdleSize.x += viewportExpandment.x;
			viewportIdleSize.y += viewportExpandment.y;
			viewportExpandedSize.x += viewportExpandment.x;
			viewportExpandedSize.y += viewportExpandment.y;

			// Creating needde images, +2 for bleeding
			for (int i = 0; i < numberOfShowingItems + 2; i++)
			{
				var img = new GameObject("Item " + i, typeof(Image)).GetComponent<Image>();

				img.rectTransform.SetParent(viewport.transform);
				img.rectTransform.localScale = Vector3.one;
				img.rectTransform.sizeDelta = itemSize;

				img.sprite = items[i % items.Length];

				images.Add(img);
			}

			// Setting up the head and tail of images
			tailIndex = 0;
			headIndex = images.Count - 1;

			// Setting the position of each image by size and spacing
			for (int i = 0; i < numberOfShowingItems + 2; i++)
			{
				var localPos = Vector3.zero;

				if (horizontal)
					localPos.x = i * (itemSize.x + spacing);
				else
					localPos.y = i * (itemSize.y + spacing);

				images[i].transform.localPosition = localPos;
			}

			OnPointerUp(new PointerEventData(EventSystem.current));
		}

		int FindNearestImageIndex()
		{
			int minIndex = -1;
			float min = float.PositiveInfinity;;
			for (int i = 0; i < images.Count; i ++)
			{
				var pos = images[i].transform.localPosition;
				var dist = Mathf.Abs(pos.x) + Mathf.Abs(pos.y);
				if (min > dist)
				{
					min = dist;
					minIndex = i;
				}
			}

			return minIndex;
		}

		int GetItemIndexByImage(Image image)
		{
			for (int i = 0; i < items.Length; i ++)
			{
				if (image.sprite == items[i])
					return i;
			}

			return -1;
		}

		void MoveImages(Vector3 delta)
		{
			for (int i = 0; i < images.Count; i++)
			{
				var index = tailIndex + i;
				index %= images.Count;

				var localPos = images[index].transform.localPosition;

				if (horizontal)
					localPos.x = index * (itemSize.x + spacing);
				else
					localPos.y = index * (itemSize.y + spacing);

				images[index].transform.localPosition += delta;
			}
		}

		void RearrangeImages()
		{
			// Begin from tail
			for (int i = 0; i < images.Count; i++)
			{
				var tailPos = horizontal ? images[tailIndex].transform.localPosition.x : images[tailIndex].transform.localPosition.y;

				// if tail is within reasonable range, no need to check more.
				if (tailPos > viewportRange.x) break;

				// otherwise, get the position of head image...
				var pos = images[headIndex].transform.localPosition;

				if (horizontal)
					pos.x += spacing + itemSize.x;
				else
					pos.y += spacing + itemSize.y;

				// ...and make it the new head
				images[tailIndex].transform.localPosition = pos;

				// Setting the picture/item it carrys
				var itmIndex = GetItemIndexByImage(images[headIndex]);
				itmIndex++;
				itmIndex %= items.Length;
				images[tailIndex].sprite = items[itmIndex];

				// Setting the new head and tail index;
				tailIndex++;
				tailIndex %= images.Count;
				headIndex++;
				headIndex %= images.Count;
			}

			// Continue from head
			for (int i = 0; i < images.Count; i++)
			{
				var headPos = horizontal ? images[headIndex].transform.localPosition.x : images[headIndex].transform.localPosition.y;

				// if head is within reasonable range, no need to check more.
				if (headPos < viewportRange.y) break;

				// otherwise, get the position of tail image...
				var pos = images[tailIndex].transform.localPosition;

				if (horizontal)
					pos.x -= spacing + itemSize.x;
				else
					pos.y -= spacing + itemSize.y;

				// ...and make it the new head
				images[headIndex].transform.localPosition = pos;

				// Setting the picture/item it carrys
				var itmIndex = GetItemIndexByImage(images[tailIndex]);
				itmIndex += items.Length - 1;
				itmIndex %= items.Length;
				images[headIndex].sprite = items[itmIndex];

				// Setting the new head and tail index;
				tailIndex += images.Count - 1;
				tailIndex %= images.Count;
				headIndex += images.Count - 1;
				headIndex %= images.Count;
			}
		}

		void SetFadeAmount()
		{
			var totalLength = horizontal ?
				viewportExpandedSize.x - itemSize.x - viewportExpandment.x:
				viewportExpandedSize.y - itemSize.y - viewportExpandment.y;

			for (int i = 0; i < images.Count; i++)
			{
				var pos = images[i].transform.localPosition;

				float dist = horizontal ? pos.x : pos.y;

				var percentage = dist / totalLength + 0.5f;

				images[i].color = gradientByRange.Evaluate(percentage);
				images[i].transform.localScale = Vector3.one * scaleByRange.Evaluate(percentage);
			}
		}

		void Awake()
		{
			var mask = viewport.GetComponent<Mask>();
			if (mask == null) return;
			viewportMaskImage = mask.GetComponent<Image>();
		}

		void Start()
		{
			Initialize();
		}

		public void OnDrag(PointerEventData eventData)
		{
			// Getting the delta from last frame position
			var position = viewport.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
			var delta = position - relativePosToPointer;
			relativePosToPointer = position;
			
			if (horizontal)
				delta.y = 0;
			else
				delta.x = 0;
			delta.z = 0;

			MoveImages(delta);
			RearrangeImages();
			SetFadeAmount();

			var nearestImageIndex = FindNearestImageIndex();
			if (onValueChange != null)
				onValueChange.Invoke(GetItemIndexByImage(images[nearestImageIndex]));
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			(viewport.transform as RectTransform).sizeDelta = viewportExpandedSize;
			viewport.overrideSorting = true;
			viewport.sortingLayerName = "Top";

			if (viewportMaskImage != null)
				viewportMaskImage.sprite = selectingMask;
			
			SetFadeAmount();

			relativePosToPointer = viewport.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			var nearestImageIndex = FindNearestImageIndex();
			var nearestImage = images[nearestImageIndex];
			var delta = -nearestImage.transform.localPosition;
			if (horizontal)
				delta.y = 0;
			else
				delta.x = 0;
			delta.z = 0;

			MoveImages(delta);
			RearrangeImages();
			SetFadeAmount();

			for (int i = 0; i < images.Count; i ++)
			{
				if (i == nearestImageIndex)
					continue;

				var color = images[i].color;
				color.a = 0;
				images[i].color = color;
			}

			if (onValueSettle != null)
				onValueSettle.Invoke(GetItemIndexByImage(nearestImage));
			
			if (viewportMaskImage != null)
				viewportMaskImage.sprite = idleMask;

			(viewport.transform as RectTransform).sizeDelta = viewportIdleSize;
			viewport.overrideSorting = false;
		}
	}
}
