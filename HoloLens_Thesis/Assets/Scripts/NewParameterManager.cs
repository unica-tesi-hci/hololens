using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NewParameterManager : MonoBehaviour {

    private ComponentsEnums.Components ID;
    private bool holding;
    private Vector3 offset;
    private Vector3 oldValue;

    public void setID(ComponentsEnums.Components id)
    {
        ID = id;
    }

    public ComponentsEnums.Components getID()
    {
        return ID;
    }

    void Select(bool isNavigating = false)
    {
        lock (Parameters.Instance)
        {
            switch (ID)
            {
                case ComponentsEnums.Components.External_Power:
                    if (Parameters.Instance.get_EXT_PWR() == 1)
                    {
                        Parameters.Instance.set_EXT_PWR(0);
                    }
                    else
                    {
                        Parameters.Instance.set_EXT_PWR(1);
                    }
                    break;
                case ComponentsEnums.Components.Battery:
                    if (Parameters.Instance.get_BAT() == 1)
                    {
                        Parameters.Instance.set_BAT(0);
                    }
                    else
                    {
                        Parameters.Instance.set_BAT(1);
                    }
                    break;
                case ComponentsEnums.Components.APU_Gen:
                    if (Parameters.Instance.get_APU_GEN() == 1)
                    {
                        Parameters.Instance.set_APU_GEN(0);
                    }
                    else
                    {
                        Parameters.Instance.set_APU_GEN(1);
                    }
                    break;
                case ComponentsEnums.Components.APU_Start:
                    if (Parameters.Instance.get_APU_START() == 1)
                    {
                        Parameters.Instance.set_APU_START(0);
                    }
                    else
                    {
                        Parameters.Instance.set_APU_START(1);
                    }
                    break;
                case ComponentsEnums.Components.APU_Master_SW:
                    if (Parameters.Instance.get_APU_M_SW() == 1)
                    {
                        Parameters.Instance.set_APU_M_SW(0);
                    }
                    else
                    {
                        Parameters.Instance.set_APU_M_SW(1);
                    }
                    break;
                case ComponentsEnums.Components.APU_Bleed:
                    if (Parameters.Instance.get_APU_BLEED() == 1)
                    {
                        Parameters.Instance.set_APU_BLEED(0);
                    }
                    else
                    {
                        Parameters.Instance.set_APU_BLEED(1);
                    }
                    break;
                case ComponentsEnums.Components.LTK_PUMPS_1:
                    if (Parameters.Instance.get_LTK_PUMPS_1() == 1)
                    {
                        Parameters.Instance.set_LTK_PUMPS_1(0);
                    }
                    else
                    {
                        Parameters.Instance.set_LTK_PUMPS_1(1);
                    }
                    break;
                case ComponentsEnums.Components.LTK_PUMPS_2:
                    if (Parameters.Instance.get_LTK_PUMPS_2() == 1)
                    {
                        Parameters.Instance.set_LTK_PUMPS_2(0);
                    }
                    else
                    {
                        Parameters.Instance.set_LTK_PUMPS_2(1);
                    }
                    break;
                case ComponentsEnums.Components.Pumps:
                    if (Parameters.Instance.get_PUMPS() == 1)
                    {
                        Parameters.Instance.set_PUMPS(0);
                    }
                    else
                    {
                        Parameters.Instance.set_PUMPS(1);
                    }
                    break;
                case ComponentsEnums.Components.RTK_PUMPS_1:
                    if (Parameters.Instance.get_RTK_PUMPS_1() == 1)
                    {
                        Parameters.Instance.set_RTK_PUMPS_1(0);
                    }
                    else
                    {
                        Parameters.Instance.set_RTK_PUMPS_1(1);
                    }
                    break;
                case ComponentsEnums.Components.RTK_PUMPS_2:
                    if (Parameters.Instance.get_RTK_PUMPS_2() == 1)
                    {
                        Parameters.Instance.set_RTK_PUMPS_2(0);
                    }
                    else
                    {
                        Parameters.Instance.set_RTK_PUMPS_2(1);
                    }
                    break;
                case ComponentsEnums.Components.ENG_Start_Switch:
                    if (isNavigating)
                    {
                        if (Math.Truncate(offset.x) > Math.Truncate(oldValue.x))
                        {
                            if (Parameters.Instance.get_ENG_START_SW() < 2)
                            {
                                Parameters.Instance.set_ENG_START_SW(Parameters.Instance.get_ENG_START_SW() + 1);
                            }
                        }
                        else if(Math.Truncate(offset.x) < Math.Truncate(oldValue.x))
                        {
                            if (Parameters.Instance.get_ENG_START_SW() > 0)
                            {
                                Parameters.Instance.set_ENG_START_SW(Parameters.Instance.get_ENG_START_SW() - 1);
                            }
                        }
                    }
                    break;
                case ComponentsEnums.Components.Engine1:
                    if (Parameters.Instance.get_ENG1() == 1)
                    {
                        Parameters.Instance.set_ENG1(0);
                    }
                    else
                    {
                        Parameters.Instance.set_ENG1(1);
                    }
                    break;
                case ComponentsEnums.Components.Engine2:
                    if (Parameters.Instance.get_ENG2() == 1)
                    {
                        Parameters.Instance.set_ENG2(0);
                    }
                    else
                    {
                        Parameters.Instance.set_ENG2(1);
                    }
                    break;
                case ComponentsEnums.Components.Brake_Parking:
                    if (Parameters.Instance.get_BRK_PRK() == 1)
                    {
                        Parameters.Instance.set_BRK_PRK(0);
                    }
                    else
                    {
                        Parameters.Instance.set_BRK_PRK(1);
                    }
                    break;
                case ComponentsEnums.Components.Engine1_Gen:
                    if (Parameters.Instance.get_ENG1_GEN() == 1)
                    {
                        Parameters.Instance.set_ENG1_GEN(0);
                    }
                    else
                    {
                        Parameters.Instance.set_ENG1_GEN(1);
                    }
                    break;
                case ComponentsEnums.Components.Engine2_Gen:
                    if (Parameters.Instance.get_ENG2_GEN() == 1)
                    {
                        Parameters.Instance.set_ENG2_GEN(0);
                    }
                    else
                    {
                        Parameters.Instance.set_ENG2_GEN(1);
                    }
                    break;
                case ComponentsEnums.Components.Pack1:
                    if (Parameters.Instance.get_PACK1() == 1)
                    {
                        Parameters.Instance.set_PACK1(0);
                    }
                    else
                    {
                        Parameters.Instance.set_PACK1(1);
                    }
                    break;
                case ComponentsEnums.Components.Pack2:
                    if (Parameters.Instance.get_PACK2() == 1)
                    {
                        Parameters.Instance.set_PACK2(0);
                    }
                    else
                    {
                        Parameters.Instance.set_PACK2(1);
                    }
                    break;
                case ComponentsEnums.Components.Throttle:
                    if (isNavigating)
                    {
                        if (Math.Sign(offset.y) >= 0)
                        {
                            Parameters.Instance.set_throttle1(offset.y);
                            Parameters.Instance.set_throttle2(offset.y);
                        }
                        else
                        {
                            Parameters.Instance.set_throttle1(0);
                            Parameters.Instance.set_throttle2(0);
                        }
                    }
                    break;
                case ComponentsEnums.Components.Auto_Brake:
                    if (Parameters.Instance.get_AUTO_BRK() == 1)
                    {
                        Parameters.Instance.set_AUTO_BRK(0);
                    }
                    else
                    {
                        Parameters.Instance.set_AUTO_BRK(1);
                    }
                    break;
                case ComponentsEnums.Components.Spoiler:
                    if (isNavigating)
                    {
                        if (Math.Truncate(offset.y) < Math.Truncate(oldValue.y))
                        {
                            if (Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) < 1)
                            {
                                if (Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) < 0.5)
                                {
                                    Parameters.Instance.set_spoiler_Speedbrake(0.5);
                                }
                                else
                                {
                                    Parameters.Instance.set_spoiler_Speedbrake(1);
                                }
                            }
                        }
                        else if (Math.Truncate(offset.y) > Math.Truncate(oldValue.y))
                        {
                            if (Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) > 0)
                            {
                                if (Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) > 0.5)
                                {
                                    Parameters.Instance.set_spoiler_Speedbrake(0.5);
                                }
                                else
                                {
                                    Parameters.Instance.set_spoiler_Speedbrake(0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if(Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) == 0)
                        {
                            if (Parameters.Instance.get_Spoiler_ARM() == 1)
                            {
                                Parameters.Instance.set_spoiler_ARM(0);
                            }
                            else
                            {
                                Parameters.Instance.set_spoiler_ARM(1);
                            }
                        }
                    }
                    break;
                case ComponentsEnums.Components.Gears:
                    if (Parameters.Instance.get_gear() == 1)
                    {
                        Parameters.Instance.set_gear(0);
                    }
                    else
                    {
                        Parameters.Instance.set_gear(1);
                    }
                    break;
                case ComponentsEnums.Components.Flaps:
                    if (isNavigating)
                    {
                        if (Math.Truncate(offset.y) < Math.Truncate(oldValue.y))
                        {
                            if (Math.Abs(Parameters.Instance.get_flaps()) < 0.875)
                            {
                                if (Math.Abs(Parameters.Instance.get_flaps()) < 0.25)
                                {
                                    Parameters.Instance.set_flaps(0.25);
                                }
                                else if (Math.Abs(Parameters.Instance.get_flaps()) < 0.375)
                                {
                                    Parameters.Instance.set_flaps(0.375);
                                }
                                else if (Math.Abs(Parameters.Instance.get_flaps()) < 0.5)
                                {
                                    Parameters.Instance.set_flaps(0.5);
                                }
                                else
                                {
                                    Parameters.Instance.set_flaps(0.875);
                                }
                            }
                        }
                        else if (Math.Truncate(offset.y) > Math.Truncate(oldValue.y))
                        {
                            if (Math.Abs(Parameters.Instance.get_flaps()) > 0.00)
                            {
                                if (Math.Abs(Parameters.Instance.get_flaps()) > 0.5)
                                {
                                    Parameters.Instance.set_flaps(0.5);
                                }
                                else if (Math.Abs(Parameters.Instance.get_flaps()) > 0.375)
                                {
                                    Parameters.Instance.set_flaps(0.375);
                                }
                                else if (Math.Abs(Parameters.Instance.get_flaps()) > 0.25)
                                {
                                    Parameters.Instance.set_flaps(0.25);
                                }
                                else
                                {
                                    Parameters.Instance.set_flaps(0.00);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
        }
    }

    void OnSelectCheck()
    {
        if (InputSequence.Instance.checkIfSeq(ID))
        {
            Select();
        }
    }

    void NavigationStart()
    {
        holding = true;
        oldValue = Vector3.zero;
    }

    void NavigationUpdated(Vector3 newOffset)
    {
        if (holding)
        {
            offset = newOffset;

            Select(true);

            oldValue = offset;
        }
    }

    void NavigationFinished()
    {
        holding = false;
        oldValue = Vector3.zero;
    }

}
