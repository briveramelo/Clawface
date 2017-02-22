using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputEcho : MonoBehaviour {

    public Text info;
	
	// Update is called once per frame
	void Update () {
        // Poll our inputs
        InputManager manager = InputManager.Instance;

        Vector2 move = manager.QueryAxes(Strings.Input.Axes.MOVEMENT);
        Vector2 look = manager.QueryAxes(Strings.Input.Axes.LOOK);

        ButtonMode swap = manager.QueryAction(Strings.Input.Actions.SWAP_MODE)[0];
        ButtonMode drop = manager.QueryAction(Strings.Input.Actions.DROP_MODE)[0];
        ButtonMode actionLegs = manager.QueryAction(Strings.Input.Actions.ACTION_LEGS)[0];
        ButtonMode actionArmLeft = manager.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT)[0];
        ButtonMode actionArmRight = manager.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT)[0];
        ButtonMode actionHead = manager.QueryAction(Strings.Input.Actions.ACTION_HEAD)[0];
        ButtonMode navUp = manager.QueryAction(Strings.Input.Actions.NAV_UP)[0];
        ButtonMode navDown = manager.QueryAction(Strings.Input.Actions.NAV_DOWN)[0];
        ButtonMode navLeft = manager.QueryAction(Strings.Input.Actions.NAV_LEFT)[0];
        ButtonMode navRight = manager.QueryAction(Strings.Input.Actions.NAV_RIGHT)[0];

        // Create a Nice String and Print out:
        string output = string.Format(
            "Input States:\n" +
            "     Movement:  {0}\n" +
            "         Look:  {1}\n\n" +
            "         Swap:  {2}\n" +
            "         Drop:  {3}\n" +
            "         Legs:  {4}\n" +
            "     Left Arm:  {5}\n" +
            "    Right Arm:  {6}\n" +
            "         Head:  {7}\n" +
            "       Nav Up:  {8}\n" +
            "     Nav Down:  {9}\n" +
            "     Nav Left:  {10}\n" +
            "    Nav Right:  {11}",
            Vector2String(move),
            Vector2String(look),
            ButtonModeString(swap),
            ButtonModeString(drop),
            ButtonModeString(actionLegs),
            ButtonModeString(actionArmLeft),
            ButtonModeString(actionArmRight),
            ButtonModeString(actionHead),
            ButtonModeString(navUp),
            ButtonModeString(navDown),
            ButtonModeString(navLeft),
            ButtonModeString(navRight)
        );

        info.text = output;
	}

    private string Vector2String(Vector2 vec)
    {
        string x = vec.x.ToString();
        string y = vec.y.ToString();
        x = x.PadRight(10, '0');
        y = y.PadRight(10, '0');

        string temp = string.Format("X: {0}, Y: {1}",
            x.Substring(0, 5), y.Substring(0, 5));
        return temp;
    }
    private string ButtonModeString(ButtonMode mode)
    {
        switch (mode)
        {
            case ButtonMode.DOWN:
                return "Button: DOWN";
            case ButtonMode.HELD:
                return "Button: HELD";
            case ButtonMode.UP:
                return "Button:   UP";
            case ButtonMode.IDLE:
                return "Button: IDLE";
            default:
                return "Button: WHAT";
        }
    }
}
