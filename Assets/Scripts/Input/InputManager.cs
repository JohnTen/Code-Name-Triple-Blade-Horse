using JTUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayersInput
{
    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField] string[] commandList;
        [SerializeField] string[] keyBoardInputList;
        [SerializeField] string[] controllerInputList;

        const int k_playerCount = 1;

        [SerializeField] bool[] modelType = new bool[k_playerCount];

        XboxInputModel[] joystickInput = new XboxInputModel[k_playerCount];
        ControllerInputModel[] controllers = new ControllerInputModel[k_playerCount];
        KeyboardInputModel keyboard;

        Action<IInputModel>[] InputModelChanged = new Action<IInputModel>[k_playerCount];

        protected override void Awake()
        {
            keyboard = new KeyboardInputModel(commandList, keyBoardInputList);

            for (int i = 0; i < k_playerCount; i++)
            {
                joystickInput[i] = new XboxInputModel(i + 1);
                controllers[i] = new ControllerInputModel(joystickInput[i], commandList, controllerInputList);
            }

            StartCoroutine(DetectGameController());
            base.Awake();
        }

        public void RegisterPluggable(int id, IInputModelPlugable plugable)
        {
            InputModelChanged[id] += plugable.SetInputModel;
            if (modelType[id])
            {
                plugable.SetInputModel(controllers[id]);
            }
            else
            {
                plugable.SetInputModel(keyboard);
            }
        }

        public void UnregisterPluggable(int id, IInputModelPlugable plugable)
        {
            InputModelChanged[id] -= plugable.SetInputModel;
        }

        IEnumerator DetectGameController()
        {
            WaitForSeconds wait = new WaitForSeconds(0.3f);
            List<string> controllerNames = new List<string>();
            string[] names;

            while (true)
            {
                controllerNames.Clear();
                names = Input.GetJoystickNames();

                foreach (var name in names)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    controllerNames.Add(name);
                }

                while (controllerNames.Count < k_playerCount)
                {
                    controllerNames.Add("");
                }

                for (int i = 0; i < k_playerCount && i < controllerNames.Count; i++)
                {
                    if (InputModelChanged[i] == null)
                        continue;

                    if (string.IsNullOrEmpty(controllerNames[i]))
                    {
                        // If controller isn't connected and using controller before
                        if (modelType[i])
                        {
                            InputModelChanged[i].Invoke(keyboard);
                            modelType[i] = false;
                        }
                    }
                    else
                    {
                        // If controller is connected and using keyboard before
                        if (!modelType[i])
                        {
                            InputModelChanged[i].Invoke(controllers[i]);
                            modelType[i] = true;
                        }
                    }
                }

                yield return wait;
            }
        }
    }
}
