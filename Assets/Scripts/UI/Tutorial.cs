using MultiplayersInput;
using UnityEngine;
using UnityEngine.UI;

namespace TripleBladeHorse.UI
{
    public class Tutorial : MonoBehaviour, IInputModelPlugable
    {
        [SerializeField] Image keyboardTutorialImage;
        [SerializeField] Image controllerTutorialImage;
        [SerializeField] float showspeed;

        bool usingController;
        bool stay = false;

        public bool Stay
        {
            get => stay;
            set => stay = value;
        }

        void Start()
        {
            InputManager.Instance.RegisterPluggable(0, this);
        }

        void Update()
        {
            if (usingController == false)
            {
                FadingImage(keyboardTutorialImage, controllerTutorialImage);
            }
            else
            {
                FadingImage(controllerTutorialImage, keyboardTutorialImage);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                stay = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                stay = false;
            }
        }

        private void FadingImage(Image baseImage, Image cancelImage)
        {
            Color color = baseImage.color;
            var fadeValue = stay ? showspeed : -showspeed;
            color.a += TimeManager.DeltaTime * fadeValue;
            color.a = Mathf.Clamp01(color.a);

            cancelImage.color = new Color(color.r, color.g, color.b, 0);
            baseImage.color = color;
        }

        public void SetInputModel(IInputModel model)
        {
            usingController = model is ControllerInputModel;
        }
    }
}