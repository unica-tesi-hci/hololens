using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NewParameterManager : MonoBehaviour {

    private ComponentsEnums.Components[] ID;
    private string[] dataType;
    private string[] path;
    private string[] interaction;
    private double[][] possible_values;
    private string[] IO_Data;
    private bool holding;
    private Vector3 offset;
    private Vector3 oldValue;

    public void initID(string[] string_id)
    {
        ComponentsEnums.Components[] ids = new ComponentsEnums.Components[string_id.Length];

        for (int i = 0; i < string_id.Length; i++)
        {
            ids[i] = ComponentsEnums.GetEnumFromString(string_id[i]);
        }

        setID(ids);
    }

    public void setID(ComponentsEnums.Components[] id)
    {
        ID = id;
    }

    public ComponentsEnums.Components[] getID()
    {
        return ID;
    }

    public string[] getIDAsString()
    {
        string[] string_id = new string[ID.Length];

        for(int i = 0; i < ID.Length; i++)
        {
            string_id[i] = ComponentsEnums.GetStringFromEnum(ID[i]);
        }

        return string_id;
    }

    public void setDataType(string[] value)
    {
        dataType = value;
    }

    public string[] getDataType()
    {
        return dataType;
    }

    public void setPath(string[] p)
    {
        path = p;
    }

    public string[] getPath()
    {
        return path;
    }

    public void setInteraction(string[] action)
    {
        interaction = action;
    }

    public string[] getInteraction()
    {
        return interaction;
    }

    public void setPossibleValues(double[][] values)
    {
        possible_values = values;
    }

    public double[][] getPossibleValues()
    {
        return possible_values;
    }

    public void setIOData(string[] IO)
    {
        IO_Data = IO;
    }

    public string[] getIOData()
    {
        return IO_Data;
    }

    void Select(bool isNavigating = false)
    {
        lock (Parameters.Instance)
        {
            for (int i = 0; i < ID.Length; i++)
            {
                //If the current parameter can be activated through tap gesture.
                if (interaction[i] == "Tap" && !isNavigating)
                {
                    //If the current parameter has boolean data type (0 or 1): turn on or off the button.
                    if(dataType[i] == "bool")
                    {
                        Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueInt(path[i]) == 0 ? 1 : 0, dataType[i]);
                    }
                    else if (dataType[i] == "int")//If the current parameter has integer data type: set the value in "possible_values" when activated.
                    {
                        Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueInt(path[i]) == 0 ? possible_values[i][0] : 0, dataType[i]);
                    }
                    else//If the current parameter has double data type: set the value in "possible_values" when activated.
                    {
                        Parameters.Instance.UpdateValue(path[i], Math.Abs(Parameters.Instance.GetValueDouble(path[i])) == 0 ? possible_values[i][0] : 0, dataType[i]);
                    }
                }//If the current parameter can be manipulated through navigation gesture, and the data is updated each time by adding/subtracting with a specific value.
                else if(interaction[i] == "Navigation" && isNavigating)
                {//If the current parameter has integer data type: check the range between min (possible_values[i][0]) and max (possible_values[i][1]), then add or subtract the value possible_values[i][2].
                    if (dataType[i] == "int")
                    {
                        if (Math.Truncate(offset.x) > Math.Truncate(oldValue.x))
                        {
                            if (Parameters.Instance.GetValueInt(path[i]) < possible_values[i][1])
                            {
                                Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueInt(path[i]) + possible_values[i][2], dataType[i]);
                            }
                        }
                        else if (Math.Truncate(offset.x) < Math.Truncate(oldValue.x))
                        {
                            if (Parameters.Instance.GetValueInt(path[i]) > possible_values[i][0])
                            {
                                Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueInt(path[i]) - possible_values[i][2], dataType[i]);
                            }
                        }
                    }//If the current parameter has double data type:
                    else if(dataType[i] == "double")
                    {//Set the navigation gesture's offset as new value.
                        if (Math.Abs(possible_values[i][2]) == 0.01)
                        {
                            if (Math.Sign(offset.y) >= 0)
                            {
                                Parameters.Instance.UpdateValue(path[i], offset.y, dataType[i]);
                            }
                            else
                            {
                                Parameters.Instance.UpdateValue(path[i], 0, dataType[i]);
                            }
                        }//Check the range between min (possible_values[i][0]) and max (possible_values[i][1]), then add or subtract the value possible_values[i][2].
                        else
                        {
                            if (Math.Truncate(offset.x) > Math.Truncate(oldValue.x))
                            {
                                if (Math.Abs(Parameters.Instance.GetValueDouble(path[i])) < Math.Abs(possible_values[i][1]))
                                {
                                    Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueDouble(path[i]) + possible_values[i][2], dataType[i]);
                                }
                            }
                            else if (Math.Truncate(offset.x) < Math.Truncate(oldValue.x))
                            {
                                if (Parameters.Instance.GetValueInt(path[i]) > possible_values[i][0])
                                {
                                    Parameters.Instance.UpdateValue(path[i], Parameters.Instance.GetValueDouble(path[i]) - possible_values[i][2], dataType[i]);
                                }
                            }
                        }
                    }
                }//If the current parameter can be manipulated through navigation gesture, and the possible data range is subdivided by uneven values.
                else if (interaction[i] == "Uneven_Nav" && isNavigating)
                {
                    if (dataType[i] == "double")
                    {
                        if (Math.Truncate(offset.x) > Math.Truncate(oldValue.x))
                        {
                            int idx = Array.IndexOf(possible_values[i], Math.Abs(Parameters.Instance.GetValueDouble(path[i])));

                            if (idx != -1 && idx < possible_values.Length - 1)
                            {
                                Parameters.Instance.UpdateValue(path[i], possible_values[i][idx + 1], dataType[i]);
                            }
                        }
                        else if (Math.Truncate(offset.x) < Math.Truncate(oldValue.x))
                        {
                            int idx = Array.IndexOf(possible_values[i], Math.Abs(Parameters.Instance.GetValueDouble(path[i])));

                            if (idx > 0)
                            {
                                Parameters.Instance.UpdateValue(path[i], possible_values[i][idx - 1], dataType[i]);
                            }
                        }
                    }
                }

                /*switch (id)
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
                            else if (Math.Truncate(offset.x) < Math.Truncate(oldValue.x))
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
                    case ComponentsEnums.Components.Throttle1:
                        if (isNavigating)
                        {
                            if (Math.Sign(offset.y) >= 0)
                            {
                                Parameters.Instance.set_throttle1(offset.y);
                            }
                            else
                            {
                                Parameters.Instance.set_throttle1(0);
                            }
                        }
                        break;
                    case ComponentsEnums.Components.Throttle2:
                        if (isNavigating)
                        {
                            if (Math.Sign(offset.y) >= 0)
                            {
                                Parameters.Instance.set_throttle2(offset.y);
                            }
                            else
                            {
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
                        break;
                    case ComponentsEnums.Components.Spoiler_ARM:
                        if (!isNavigating)
                        {
                            if (Math.Abs(Parameters.Instance.get_Spoiler_Speedbrake()) == 0)
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
                }*/
            }

            SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
        }
    }

    void OnSelectCheck()
    {
        /*foreach (ComponentsEnums.Components id in ID)
        {
            if (InputSequence.Instance.checkIfSeq(id))
            {*/
                Select();
            /*}
        }*/    
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
