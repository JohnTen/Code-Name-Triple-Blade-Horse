using MultiplayersInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TripleBladeHorse
{
    public class InputWatcher : MonoBehaviour, IInputModelPlugable
    {

        bool controller;
        [SerializeField] StandaloneInputModule inputKeyboard;
        [SerializeField] StandaloneInputModule inputController;
        // Start is called before the first frame update
        void Start()
        {
            InputManager.Instance.RegisterPluggable(0, this);
        }

        // Update is called once per frame
        void Update()
        {
            if (controller == true)
            {
                inputController.enabled = true;
                inputKeyboard.enabled = false;
            }
            else
            {
                inputController.enabled = false;
                inputKeyboard.enabled = true;
            }

        }

        public void SetInputModel(IInputModel model)
        {
            if (model is ControllerInputModel)
            {
                controller = true;
            }
            else
            {
                controller = false;
            }

        }
    }
}
