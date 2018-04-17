using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentsEnums : Singleton<ComponentsEnums> {

    public enum Components
    {
        None,
        External_Power,
        Battery,
        APU_Gen,
        APU_Start,
        APU_Master_SW,
        APU_Bleed,
        LTK_PUMPS_1,
        LTK_PUMPS_2,
        Pumps,
        RTK_PUMPS_1,
        RTK_PUMPS_2,
        ENG_Start_Switch,
        Engine1,
        Engine2,
        Brake_Parking,
        Engine1_Gen,
        Engine2_Gen,
        Pack1,
        Pack2,
        Throttle,
        Auto_Brake,
        Spoiler,
        Gears,
        Flaps
    };

    public static string GetStringFromEnum(Components c)
    {
        switch (c)
        {
            case Components.External_Power:
                return "External_Power";
            case Components.Battery:
                return "Battery";
            case Components.APU_Gen:
                return "APU_Gen";
            case Components.APU_Start:
                return "APU_Start";
            case Components.APU_Master_SW:
                return "APU_Master_SW";
            case Components.APU_Bleed:
                return "APU_Bleed";
            case Components.LTK_PUMPS_1:
                return "LTK_PUMPS_1";
            case Components.LTK_PUMPS_2:
                return "LTK_PUMPS_2";
            case Components.Pumps:
                return "Pumps";
            case Components.RTK_PUMPS_1:
                return "RTK_PUMPS_1";
            case Components.RTK_PUMPS_2:
                return "RTK_PUMPS_2";
            case Components.ENG_Start_Switch:
                return "ENG_Start_Switch";
            case Components.Engine1:
                return "Engine1";
            case Components.Engine2:
                return "Engine2";
            case Components.Brake_Parking:
                return "Brake_Parking";
            case Components.Engine1_Gen:
                return "Engine1_Gen";
            case Components.Engine2_Gen:
                return "Engine2_Gen";
            case Components.Pack1:
                return "Pack1";
            case Components.Pack2:
                return "Pack2";
            case Components.Throttle:
                return "Throttle";
            case Components.Auto_Brake:
                return "Auto_Brake";
            case Components.Spoiler:
                return "Spoiler";
            case Components.Gears:
                return "Gears";
            case Components.Flaps:
                return "Flaps";
            default:
                return "None";
        }
    }

    public static Components GetEnumFromString(string s)
    {
        if (s.Equals("External_Power"))
        {
            return Components.External_Power;
        }
        else if (s.Equals("Battery"))
        {
            return Components.Battery;
        }
        else if (s.Equals("APU_Gen"))
        {
            return Components.APU_Gen;
        }
        else if (s.Equals("APU_Start"))
        {
            return Components.APU_Start;
        }
        else if (s.Equals("APU_Master_SW"))
        {
            return Components.APU_Master_SW;
        }
        else if (s.Equals("APU_Bleed"))
        {
            return Components.APU_Bleed;
        }
        else if (s.Equals("LTK_PUMPS_1"))
        {
            return Components.LTK_PUMPS_1;
        }
        else if (s.Equals("LTK_PUMPS_2"))
        {
            return Components.LTK_PUMPS_2;
        }
        else if (s.Equals("Pumps"))
        {
            return Components.Pumps;
        }
        else if (s.Equals("RTK_PUMPS_1"))
        {
            return Components.RTK_PUMPS_1;
        }
        else if (s.Equals("RTK_PUMPS_2"))
        {
            return Components.RTK_PUMPS_2;
        }
        else if (s.Equals("ENG_Start_Switch"))
        {
            return Components.ENG_Start_Switch;
        }
        else if (s.Equals("Engine1"))
        {
            return Components.Engine1;
        }
        else if (s.Equals("Engine2"))
        {
            return Components.Engine2;
        }
        else if (s.Equals("Brake_Parking"))
        {
            return Components.Brake_Parking;
        }
        else if (s.Equals("Engine1_Gen"))
        {
            return Components.Engine1_Gen;
        }
        else if (s.Equals("Engine2_Gen"))
        {
            return Components.Engine2_Gen;
        }
        else if (s.Equals("Pack1"))
        {
            return Components.Pack1;
        }
        else if (s.Equals("Pack2"))
        {
            return Components.Pack2;
        }
        else if (s.Equals("Throttle"))
        {
            return Components.Throttle;
        }
        else if (s.Equals("Auto_Brake"))
        {
            return Components.Auto_Brake;
        }
        else if (s.Equals("Spoiler"))
        {
            return Components.Spoiler;
        }
        else if (s.Equals("Gears"))
        {
            return Components.Gears;
        }
        else if (s.Equals("Flaps"))
        {
            return Components.Flaps;
        }

        return Components.None;
    }
}
