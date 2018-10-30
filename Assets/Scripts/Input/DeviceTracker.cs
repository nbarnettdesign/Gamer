using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public abstract class DeviceTracker : MonoBehaviour
{
    //ref to inputManager
    protected InputManager im;

    protected InputData data;
    protected bool newData;

    private void Awake()
    {
        im = GetComponent<InputManager>();

        //setup inputData Struct - pass the amounts of buttons and axes
        data = new InputData(im.axisCount, im.buttonCount);
    }

    public abstract void Refresh();
}
