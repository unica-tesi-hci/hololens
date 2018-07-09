using System;
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
        Monitor1,
        Monitor2,
        Brake_Parking,
        Engine1_Gen,
        Engine2_Gen,
        Pack1,
        Pack2,
        Throttle1,
        Throttle2,
        Auto_Brake,
        Spoiler,
        Spoiler_ARM,
        Gears,
        Flaps,
        Anemometer,
        Altimeter
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
            case Components.Monitor1:
                return "Monitor1";
            case Components.Monitor2:
                return "Monitor2";
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
            case Components.Throttle1:
                return "Throttle1";
            case Components.Throttle2:
                return "Throttle2";
            case Components.Auto_Brake:
                return "Auto_Brake";
            case Components.Spoiler:
                return "Spoiler";
            case Components.Spoiler_ARM:
                return "Spoiler_ARM";
            case Components.Gears:
                return "Gears";
            case Components.Flaps:
                return "Flaps";
            case Components.Anemometer:
                return "Anemometer";
            case Components.Altimeter:
                return "Altimeter";
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
        else if (s.Equals("Monitor1"))
        {
            return Components.Monitor1;
        }
        else if (s.Equals("Monitor2"))
        {
            return Components.Monitor2;
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
        else if (s.Equals("Throttle1"))
        {
            return Components.Throttle1;
        }
        else if (s.Equals("Throttle2"))
        {
            return Components.Throttle2;
        }
        else if (s.Equals("Auto_Brake"))
        {
            return Components.Auto_Brake;
        }
        else if (s.Equals("Spoiler"))
        {
            return Components.Spoiler;
        }
        else if (s.Equals("Spoiler_ARM"))
        {
            return Components.Spoiler_ARM;
        }
        else if (s.Equals("Gears"))
        {
            return Components.Gears;
        }
        else if (s.Equals("Flaps"))
        {
            return Components.Flaps;
        }else if (s.Equals("Anemometer"))
        {
            return Components.Anemometer;
        }else if (s.Equals("Altimeter"))
        {
            return Components.Altimeter;
        }

        return Components.None;
    }

    public static string GetPathFromEnum(Components c)
    {
        switch (c)
        {
            case Components.External_Power:
                return "/controls/electric/external-power";
            case Components.Battery:
                return "/controls/electric/battery-switch";
            case Components.APU_Gen:
                return "/controls/electric/APU-generator";
            case Components.APU_Start:
                return "/controls/APU/starter";
            case Components.APU_Master_SW:
                return "/controls/APU/master-switch";
            case Components.APU_Bleed:
                return "/controls/pneumatic/APU-bleed";
            case Components.LTK_PUMPS_1:
                return "/consumables/fuel/tank/selected";
            case Components.LTK_PUMPS_2:
                return "/consumables/fuel/tank[1]/selected";
            case Components.Pumps:
                return "/consumables/fuel/tank[3]/selected";
            case Components.RTK_PUMPS_1:
                return "/consumables/fuel/tank[4]/selected";
            case Components.RTK_PUMPS_2:
                return "/consumables/fuel/tank[5]/selected";
            case Components.ENG_Start_Switch:
                return "/controls/engines/engine-start-switch";
            case Components.Engine1:
                return "/controls/engines/engine/cutoff-switch";
            case Components.Engine2:
                return "/controls/engines/engine[1]/cutoff-switch";
            case Components.Monitor1:
                return "/engines/engine/n1";
            case Components.Monitor2:
                return "/engines/engine[1]/n1";
            case Components.Brake_Parking:
                return "/controls/gear/brake-parking";
            case Components.Engine1_Gen:
                return "/controls/electric/engine/generator";
            case Components.Engine2_Gen:
                return "/controls/electric/engine[1]/generator";
            case Components.Pack1:
                return "/controls/pressurization/pack/pack-on";
            case Components.Pack2:
                return "/controls/pressurization/pack[1]/pack-on";
            case Components.Throttle1:
                return "/controls/engines/engine/throttle";
            case Components.Throttle2:
                return "/controls/engines/engine[1]/throttle";
            case Components.Auto_Brake:
                return "/autopilot/autobrake/step";
            case Components.Spoiler:
                return "/controls/flight/speedbrake";
            case Components.Spoiler_ARM:
                return "/controls/flight/ground-spoilers-armed";
            case Components.Gears:
                return "/controls/gear/gear-down";
            case Components.Flaps:
                return "/controls/flight/flaps";
            case Components.Anemometer:
                return "/velocities/airspeed-kt";
            case Components.Altimeter:
                return "/position/altitude-ft";
            default:
                return "";
        }
    }

    public static Components GetEnumFromPath(string s)
    {
        if (s.Equals("/controls/electric/external-power"))
        {
            return Components.External_Power;
        }
        else if (s.Equals("/controls/electric/battery-switch"))
        {
            return Components.Battery;
        }
        else if (s.Equals("/controls/electric/APU-generator"))
        {
            return Components.APU_Gen;
        }
        else if (s.Equals("/controls/APU/starter"))
        {
            return Components.APU_Start;
        }
        else if (s.Equals("/controls/APU/master-switch"))
        {
            return Components.APU_Master_SW;
        }
        else if (s.Equals("/controls/pneumatic/APU-bleed"))
        {
            return Components.APU_Bleed;
        }
        else if (s.Equals("/consumables/fuel/tank/selected"))
        {
            return Components.LTK_PUMPS_1;
        }
        else if (s.Equals("/consumables/fuel/tank[1]/selected"))
        {
            return Components.LTK_PUMPS_2;
        }
        else if (s.Equals("/consumables/fuel/tank[3]/selected"))
        {
            return Components.Pumps;
        }
        else if (s.Equals("/consumables/fuel/tank[4]/selected"))
        {
            return Components.RTK_PUMPS_1;
        }
        else if (s.Equals("/consumables/fuel/tank[5]/selected"))
        {
            return Components.RTK_PUMPS_2;
        }
        else if (s.Equals("/controls/engines/engine-start-switch"))
        {
            return Components.ENG_Start_Switch;
        }
        else if (s.Equals("/controls/engines/engine/cutoff-switch"))
        {
            return Components.Engine1;
        }
        else if (s.Equals("/controls/engines/engine[1]/cutoff-switch"))
        {
            return Components.Engine2;
        }
        else if (s.Equals("/engines/engine/n1"))
        {
            return Components.Monitor1;
        }
        else if (s.Equals("/engines/engine[1]/n1"))
        {
            return Components.Monitor2;
        }
        else if (s.Equals("/controls/gear/brake-parking"))
        {
            return Components.Brake_Parking;
        }
        else if (s.Equals("/controls/electric/engine/generator"))
        {
            return Components.Engine1_Gen;
        }
        else if (s.Equals("/controls/electric/engine[1]/generator"))
        {
            return Components.Engine2_Gen;
        }
        else if (s.Equals("/controls/pressurization/pack/pack-on"))
        {
            return Components.Pack1;
        }
        else if (s.Equals("/controls/pressurization/pack[1]/pack-on"))
        {
            return Components.Pack2;
        }
        else if (s.Equals("/controls/engines/engine/throttle"))
        {
            return Components.Throttle1;
        }
        else if (s.Equals("/controls/engines/engine[1]/throttle"))
        {
            return Components.Throttle2;
        }
        else if (s.Equals("/autopilot/autobrake/step"))
        {
            return Components.Auto_Brake;
        }
        else if (s.Equals("/controls/flight/speedbrake"))
        {
            return Components.Spoiler;
        }
        else if (s.Equals("/controls/flight/ground-spoilers-armed"))
        {
            return Components.Spoiler_ARM;
        }
        else if (s.Equals("/controls/gear/gear-down"))
        {
            return Components.Gears;
        }
        else if (s.Equals("/controls/flight/flaps"))
        {
            return Components.Flaps;
        }
        else if (s.Equals("/velocities/airspeed-kt"))
        {
            return Components.Anemometer;
        }
        else if (s.Equals("/position/altitude-ft"))
        {
            return Components.Altimeter;
        }

        return Components.None;
    }

}
