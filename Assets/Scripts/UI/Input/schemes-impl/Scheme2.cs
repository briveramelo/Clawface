﻿/**
 *  @author Cornelia Schultz
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class Scheme2 : IControlScheme {

    #region Public Interface

        public string SchemeName
        {
            get
            {
                return "Scheme Two";
            }
        }

        public ButtonMode[] QueryAction(List<IController> controllers, string action)
        {
            ButtonMode[] modes = new ButtonMode[controllers.Count];
            for (int i = 0; i < controllers.Count; i++)
            {
                switch (action)
                {
                    case Strings.Input.Actions.SWAP_MODE:
                        modes[i] = controllers[i].GetLeftSecondary();
                        break;
                    //case Strings.Input.Actions.DROP_MODE:
                    //    modes[i] = controllers[i].GetRightSecondary();
                    //    break;
                    case Strings.Input.Actions.PICKUP:
                        modes[i] = controllers[i].GetAction3();
                        break;
                    case Strings.Input.Actions.LOCK:
                        modes[i] = controllers[i].GetRightSecondary();
                        break;
                    case Strings.Input.Actions.ACTION_LEGS:
                        modes[i] = controllers[i].GetAction1();
                        break;
                    case Strings.Input.Actions.ACTION_ARM_LEFT:
                        modes[i] = controllers[i].GetLeftPrimary();
                        break;
                    case Strings.Input.Actions.ACTION_ARM_RIGHT:
                        modes[i] = controllers[i].GetRightPrimary();
                        break;
                    case Strings.Input.Actions.ACTION_SKIN:
                        modes[i] = controllers[i].GetAction4();
                        break;
                    //case Strings.Input.Actions.NAV_UP:
                    //    modes[i] = controllers[i].GetDPadUp();
                    //    break;
                    //case Strings.Input.Actions.NAV_DOWN:
                    //    modes[i] = controllers[i].GetDPadDown();
                    //    break;
                    //case Strings.Input.Actions.NAV_LEFT:
                    //    modes[i] = controllers[i].GetDPadLeft();
                    //    break;
                    //case Strings.Input.Actions.NAV_RIGHT:
                    //    modes[i] = controllers[i].GetDPadRight();
                    //    break;
                    case Strings.Input.Actions.EQUIP_ARM_RIGHT:
                        modes[i] = controllers[i].GetDPadRight();
                        break;
                    case Strings.Input.Actions.EQUIP_LEGS:
                        modes[i] = controllers[i].GetDPadDown();
                        break;
                    case Strings.Input.Actions.EQUIP_ARM_LEFT:
                        modes[i] = controllers[i].GetDPadLeft();
                        break;
                    case Strings.Input.Actions.DODGE:
                        modes[i] = controllers[i].GetLeftSecondary();
                        break;
                    default:
                        throw new Exception("Bad Controller Action String: " + action);
                }
            }

            return modes;
        }

        public bool QueryAction(List<IController> controllers, string action, ButtonMode mode)
        {
            foreach (IController controller in controllers)
            {
                switch (action)
                {
                    case Strings.Input.Actions.SWAP_MODE:
                        if (controller.GetLeftSecondary(mode))
                            return true;
                        break;
                //case Strings.Input.Actions.DROP_MODE:
                //    if (controller.GetRightSecondary(mode))
                //        return true;
                //    break;
                    case Strings.Input.Actions.PICKUP:
                        if (controller.GetAction3(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.LOCK:
                        if (controller.GetRightSecondary(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.ACTION_LEGS:
                        if (controller.GetAction1(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.ACTION_ARM_LEFT:
                        if (controller.GetLeftPrimary(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.ACTION_ARM_RIGHT:
                        if (controller.GetRightPrimary(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.ACTION_SKIN:
                        if (controller.GetAction4(mode))
                            return true;
                        break;
                    //case Strings.Input.Actions.NAV_UP:
                    //    if (controller.GetDPadUp(mode))
                    //        return true;
                    //    break;
                    //case Strings.Input.Actions.NAV_DOWN:
                    //    if (controller.GetDPadDown(mode))
                    //        return true;
                    //    break;
                    //case Strings.Input.Actions.NAV_LEFT:
                    //    if (controller.GetDPadLeft(mode))
                    //        return true;
                    //    break;
                    case Strings.Input.Actions.NAV_RIGHT:
                        if (controller.GetDPadRight(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.DODGE:
                        if (controller.GetLeftSecondary(mode))
                            return true;
                        break;
                    //case Strings.Input.Actions.LOCK:
                    //    if (controller.GetRightPrimary(mode))
                    //        return true;
                    //    break;
                    case Strings.Input.Actions.PAUSE:
                        if (controller.GetStart(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.EQUIP_ARM_RIGHT:
                        if (controller.GetDPadRight(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.EQUIP_LEGS:
                        if (controller.GetDPadDown(mode))
                            return true;
                        break;
                    case Strings.Input.Actions.EQUIP_ARM_LEFT:
                        if (controller.GetDPadLeft(mode))
                            return true;
                        break;
                    default:
                        throw new Exception("Bad Controller Action String: " + action);
                }
            }

            return false;
        }

        public Vector2 QueryAxes(List<IController> controllers, string axes)
        {
            bool left;
            switch (axes)
            {
                case Strings.Input.Axes.MOVEMENT:
                    left = true;
                    break;
                case Strings.Input.Axes.LOOK:
                    left = false;
                    break;
                default:
                    throw new Exception("Bad Controller Axes String: " + axes);
            }

            Vector2 ret = new Vector2(0, 0);
            foreach (IController controller in controllers)
            {
                Vector2 conRet = ConstructAxisValue(controller, left);
                if (conRet.magnitude > ret.magnitude)
                    ret = conRet;
            }

            return ret;
        }

    #endregion

    #region Private Interface

        private Vector2 ConstructAxisValue(IController controller, bool left)
        {
            float x = left ? controller.GetLeftXAxis() : controller.GetRightXAxis();
            float y = left ? controller.GetLeftYAxis() : controller.GetRightYAxis();

            return new Vector2(x, y);
        }

    #endregion
}
