using System;
using System.Text;
using UnityEngine;

namespace MultiplayersInput
{
    public enum PS4ControllerInputButton
    {
        Square,
        X,
        Circle,
        Triangle,
        L1,
        R1,
        L2,
        R2,
        Share,
        Options,
        L3,
        R3,
        PS,
        PadPress,
    }

    public enum PS4ControllerInputAxis
    {
        LeftStickX,
        LeftStickY,
        RightStickX,
        RightStickY,
        DPadX,
        DPadY,
        L2Axis,
        R2Axis,
    }

    [Serializable]
    public class PS4InputModel
    {
        const int k_buttonCount = 14;
        const string k_basePrefix = "Joystick";

        [SerializeField] int joyStickNumber;

        [SerializeField] string[] axisNames;
        [SerializeField] KeyCode[] buttonCodes;

        public int JoyStickNumber => joyStickNumber;

        public PS4InputModel(int joyStickNumber)
        {
            this.joyStickNumber = joyStickNumber;
            BuildButtonCodes();
            BuildAxisNames();
        }

        public float GetAxis(PS4ControllerInputAxis axis)
        {
            return Input.GetAxis(axisNames[(int)axis]);
        }

        public bool GetButtonDown(PS4ControllerInputButton button)
        {
            return Input.GetKeyDown(buttonCodes[(int)button]);
        }

        public bool GetButton(PS4ControllerInputButton button)
        {
            return Input.GetKey(buttonCodes[(int)button]);
        }

        public bool GetButtonUp(PS4ControllerInputButton button)
        {
            return Input.GetKeyUp(buttonCodes[(int)button]);
        }

        private void BuildAxisNames()
        {
            string prefix = k_basePrefix + joyStickNumber + "_";
            var baseName = Enum.GetNames(typeof(PS4ControllerInputAxis));
            axisNames = new string[baseName.Length];

            StringBuilder nameBuilder = new StringBuilder();

            for (int i = 0; i < baseName.Length; i++)
            {
                nameBuilder.Append(prefix);
                nameBuilder.Append(baseName[i]);
                axisNames[i] = nameBuilder.ToString();
                nameBuilder.Clear();
            }
        }

        private void BuildButtonCodes()
        {
            string prefix = k_basePrefix + joyStickNumber + "Button";
            StringBuilder nameBuilder = new StringBuilder();
            buttonCodes = new KeyCode[k_buttonCount];

            for (int i = 0; i < k_buttonCount; i++)
            {
                nameBuilder.Append(prefix);
                nameBuilder.Append(i.ToString());
                buttonCodes[i] = (KeyCode)Enum.Parse(typeof(KeyCode), nameBuilder.ToString());
                nameBuilder.Clear();
            }
        }
    }
}
