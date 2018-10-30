using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTracker : DeviceTracker
{

    public AxisKeys[] axisKeys;
    public KeyCode[] buttonKeys;


    private void Reset()
    {
        im = GetComponent<InputManager>();
        axisKeys = new AxisKeys[im.axisCount];
        buttonKeys = new KeyCode[im.buttonCount];

    }

    public override void Refresh()
    {
        im = GetComponent<InputManager>();

        //create two temp arrays for buttons and axes
        KeyCode[] newButtons = new KeyCode[im.buttonCount];
        AxisKeys[] newAxes = new AxisKeys[im.axisCount];

        if (buttonKeys != null)
        {
            for (int i = 0; i < Mathf.Min(newButtons.Length, buttonKeys.Length); i++)
            {
                newButtons[i] = buttonKeys[i];
            }
        }
        buttonKeys = newButtons;

        if (axisKeys != null)
        {
            for (int i = 0; i < Mathf.Min(newAxes.Length, axisKeys.Length); i++)
            {
                newAxes[i] = axisKeys[i];
            }
        }
        axisKeys = newAxes;
    }

    // Update is called once per frame
    void Update()
    {
        //check for inputs, if detected, set newData to true, so we know things need to be updated

        //populate InputData to pass to InputManager, which keys were pressed

        for (int i = 0; i < buttonKeys.Length; i++)
        {
            if (Input.GetKey(buttonKeys[i]))
            {
                data.buttons[i] = true;
                newData = true;
            }
        }

        //Check for positive or negative input
        for (int i = 0; i < axisKeys.Length; i++)
        {
            float val = 0;
            if (Input.GetKey(axisKeys[i].positive))
            {
                val += 1;
                newData = true;
            }
            if (Input.GetKey(axisKeys[i].negative))
            {
                val -= 1;
                newData = true;
            }
            data.axes[i] = val;
        }

        if (newData)
        {
            //pass data we have to input manager, toggle bool and reset data
            im.PassInput(data);
            newData = false;
            data.Reset();
        }
    }
}


[System.Serializable]
public struct AxisKeys
{
    public KeyCode positive;
    public KeyCode negative;
}
