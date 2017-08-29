using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputEcho : MonoBehaviour {

    public Text info;
	
	// Update is called once per frame
    /*
	void Update () {
        // Poll our inputs
        InputManager manager = InputManager.Instance;

        Vector2 move = manager.QueryAxes(Strings.Input.Axes.MOVEMENT);
        Vector2 look = manager.QueryAxes(Strings.Input.Axes.LOOK);

        ButtonMode swap = manager.QueryAction(Strings.Input.Actions.SWAP_MODE);
        ButtonMode drop = manager.QueryAction(Strings.Input.Actions.DROP_MODE);
        ButtonMode actionLegs = manager.QueryAction(Strings.Input.Actions.ACTION_LEGS);
        ButtonMode actionArmLeft = manager.QueryAction(Strings.Input.Actions.ACTION_ARM_LEFT);
        ButtonMode actionArmRight = manager.QueryAction(Strings.Input.Actions.ACTION_ARM_RIGHT);
        //ButtonMode actionHead = manager.QueryAction(Strings.Input.Actions.ACTION_HEAD)[0]; // outdated
        ButtonMode navUp = manager.QueryAction(Strings.Input.Actions.NAV_UP);
        ButtonMode navDown = manager.QueryAction(Strings.Input.Actions.NAV_DOWN);
        ButtonMode navLeft = manager.QueryAction(Strings.Input.Actions.NAV_LEFT);
        ButtonMode navRight = manager.QueryAction(Strings.Input.Actions.NAV_RIGHT);

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
            //"         Head:  {7}\n" +
            "       Nav Up:  {7}\n" +
            "     Nav Down:  {8}\n" +
            "     Nav Left:  {9}\n" +
            "    Nav Right:  {10}",
            Vector2String(move),
            Vector2String(look),
            ButtonModeString(swap),
            ButtonModeString(drop),
            ButtonModeString(actionLegs),
            ButtonModeString(actionArmLeft),
            ButtonModeString(actionArmRight),
            //ButtonModeString(actionHead),
            ButtonModeString(navUp),
            ButtonModeString(navDown),
            ButtonModeString(navLeft),
            ButtonModeString(navRight)
        );

        info.text = output;

        if (navLeft == ButtonMode.DOWN)
            manager.Vibrate(VibrationTargets.LEFT, 1.0F);
        else if (navLeft == ButtonMode.UP)
            manager.Vibrate(VibrationTargets.LEFT, 0.0F);

        if (navRight == ButtonMode.DOWN)
            manager.Vibrate(VibrationTargets.RIGHT, 1.0F);
        else if (navRight == ButtonMode.UP)
            manager.Vibrate(VibrationTargets.RIGHT, 0.0F);

        if (navDown == ButtonMode.DOWN)
            manager.Vibrate(VibrationTargets.BOTH, 1.0F);
        else if (navDown == ButtonMode.UP)
            manager.Vibrate(VibrationTargets.BOTH, 0.0F);
	}
    */

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
