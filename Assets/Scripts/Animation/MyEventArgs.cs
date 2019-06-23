using UnityEngine;
using System.Collections;
using System;

public class MyEventArgs : EventArgs
{
    private float frameFloatData;
    private bool frameBoolData;
    private int frameIntData;

    public float FrameFloatData { get { return frameFloatData; } set { frameFloatData = value; } }
    public bool FrameBoolData { get { return frameBoolData; } set { frameBoolData = value; } }
    public int FrameIntData { get { return frameIntData; } set { frameIntData = value; } }

    public MyEventArgs(bool boolData, float floatData, int intData)
    {
        FrameFloatData = floatData;
        frameBoolData = boolData;
        frameIntData = intData;
    }
}
