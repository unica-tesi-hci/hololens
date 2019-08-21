using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Parameters : MonoBehaviour {

    public static Parameters Instance { get; private set; }

    //Key: the parameter's path; Value: its current value.
    public Dictionary<string, double> mapValue = new Dictionary<string, double>();
    //Key: the parameter's path; Value: its corresponding data type (bool, int or double).
    public Dictionary<string, string> mapType = new Dictionary<string, string>();
    //Key: the parameter's path; Value: its corresponding IO value (Input, Output or Both).
    public Dictionary<string, string> mapIO = new Dictionary<string, string>();

 //   private int EXT_PWR;
 //   private int BAT;
 //   private int BAT1;
 //   private int BAT2;
 //   private int APU_GEN;
 //   private int APU_START;
 //   private int APU_M_SW;
 //   private int APU_BLEED;
 //   private int LTK_PUMPS_1;
 //   private int LTK_PUMPS_2;
 //   private int PUMPS;
 //   private int RTK_PUMPS_1;
 //   private int RTK_PUMPS_2;
 //   private int ENG_START_SW;
 //   private int ENG1;
 //   private int ENG2;
 //   private double ENG1_N1;
 //   private double ENG2_N1;
 //   private int BRK_PRK;
 //   private int ENG1_GEN;
 //   private int ENG2_GEN;
 //   private int PACK1;
 //   private int PACK2;
 //   private double throttle1;
 //   private double throttle2;
 //   private int auto_Brake;
 //   private int spoiler_ARM;
 //   private double spoiler_Speedbrake;
 //   private int gear;
 //   private double flaps;
 //   private double aileron;
 //   private double elevator;
 //   private double rudder;
 //   private double vertical_view;
 //   private double horizontal_view;
	//private double velocity;
	//private double altitude;
    private double initialAltitude;
    private int HUD;

    private string[] newParameters;
    private bool initAltitude;

    // Use this for initialization
    void Start () {
        Instance = this;
    
        /*
        EXT_PWR = 0;
        BAT = 0;
        BAT1 = 0;
        BAT2 = 0;
        APU_GEN = 0;
        APU_START = 0;
        APU_M_SW = 0;
        APU_BLEED = 0;
        LTK_PUMPS_1 = 0;
        LTK_PUMPS_2 = 0;
        PUMPS = 0;
        RTK_PUMPS_1 = 0;
        RTK_PUMPS_2 = 0;
        ENG_START_SW = 1;
        ENG1 = 1;
        ENG2 = 1;
        ENG1_N1 = 0;
        ENG2_N1 = 0;
        BRK_PRK = 1;
        ENG1_GEN = 0;
        ENG2_GEN = 0;
        PACK1 = 0;
        PACK2 = 0;
        throttle1 = 0.00;
        throttle2 = 0.00;
        auto_Brake = 0;
        spoiler_ARM = 0;
        spoiler_Speedbrake = 0;
        gear = 1;
        flaps = 0.00;
        aileron = 0;
        elevator = 0;
        rudder = 0;
        vertical_view = 0.00;
        horizontal_view = 0.00;
		velocity = 0.00;
		altitude = 0.00;*/
        initialAltitude = 0.00;
        HUD = 0;

        initAltitude = true;
    }

    public void AddValue(string id_path, double value, string type, string IO)
    {
        mapValue.Add(id_path, value);
        mapType.Add(id_path, type);
        mapIO.Add(id_path, IO);
    }

    public int GetValueInt(string id_path)
    {
        return Convert.ToInt32(mapValue[id_path]);
    }

    public double GetValueDouble(string id_path)
    {
        return mapValue[id_path];
    }

    public bool UpdateValue(string path, double value, string type)
    {
        bool result = false;

        try
        {
            if (type == "bool" && (value >= 0 && value < 2))
            {
                mapValue[path] = Convert.ToInt32(value);
                result = true;
            }
            else if (type == "int")
            {
                mapValue[path] = Convert.ToInt32(value);
                result = true;
            }
            else
            {
                mapValue[path] = value;
                result = true;
            }
        }catch(Exception e)
        {
            Debug.Log(e);
        }

        return result;
    }

    public void set_new_parameters(string value)
    {
        int i = 0;
        newParameters = value.Split(';');

        foreach (KeyValuePair<string, string> kvp in mapType)
        {
            UpdateValue(kvp.Key, Convert.ToDouble(newParameters[i].Substring(newParameters[i].IndexOf('=') + 1)), kvp.Value);

            if (kvp.Key == "/position/altitude-ft")
            {
                if (initAltitude)
                {
                    set_initialAltitude(mapValue[kvp.Key]);
                    initAltitude = false;
                }
            }

            ++i;
        }

        set_HUD(Convert.ToInt32(newParameters[i].Substring(newParameters[i].IndexOf('=') + 1)));
    }

    /*
    public void set_new_parameters(string value)
    {
        newParameters = value.Split(';');

        set_EXT_PWR(Convert.ToInt32(newParameters[0].Split('=')[1]));
        set_APU_M_SW(Convert.ToInt32(newParameters[1].Split('=')[1]));
        set_APU_START(Convert.ToInt32(newParameters[2].Split('=')[1]));
        set_APU_GEN(Convert.ToInt32(newParameters[3].Split('=')[1]));
        set_APU_BLEED(Convert.ToInt32(newParameters[4].Split('=')[1]));
        set_BRK_PRK(Convert.ToInt32(newParameters[5].Split('=')[1]));
        set_ENG_START_SW(Convert.ToInt32(newParameters[6].Split('=')[1]));
        set_ENG1(Convert.ToInt32(newParameters[7].Split('=')[1]));
        set_ENG2(Convert.ToInt32(newParameters[8].Split('=')[1]));
        set_ENG1_GEN(Convert.ToInt32(newParameters[9].Split('=')[1]));
        set_ENG2_GEN(Convert.ToInt32(newParameters[10].Split('=')[1]));
        set_PACK1(Convert.ToInt32(newParameters[11].Split('=')[1]));
        set_PACK2(Convert.ToInt32(newParameters[12].Split('=')[1]));
        set_BAT(Convert.ToInt32(newParameters[13].Split('=')[1]));
        set_flaps(Convert.ToDouble(newParameters[14].Split('=')[1]));
        set_gear(Convert.ToInt32(newParameters[15].Split('=')[1]));
        set_throttle1(Convert.ToDouble(newParameters[16].Split('=')[1]));
        set_throttle2(Convert.ToDouble(newParameters[17].Split('=')[1]));
        set_ENG1_N1(Convert.ToDouble(newParameters[18].Split('=')[1]));
        set_ENG2_N1(Convert.ToDouble(newParameters[19].Split('=')[1]));
        set_LTK_PUMPS_1(Convert.ToInt32(newParameters[20].Split('=')[1]));
        set_LTK_PUMPS_2(Convert.ToInt32(newParameters[21].Split('=')[1]));
        set_PUMPS(Convert.ToInt32(newParameters[22].Split('=')[1]));
        set_RTK_PUMPS_1(Convert.ToInt32(newParameters[23].Split('=')[1]));
        set_RTK_PUMPS_2(Convert.ToInt32(newParameters[24].Split('=')[1]));
        set_AUTO_BRK(Convert.ToInt32(newParameters[25].Split('=')[1]));
        set_spoiler_Speedbrake(Convert.ToDouble(newParameters[26].Split('=')[1]));
        set_spoiler_ARM(Convert.ToInt32(newParameters[27].Split('=')[1]));
        set_velocity(Convert.ToDouble(newParameters[28].Split('=')[1]));
        set_altitude(Convert.ToDouble(newParameters[29].Split('=')[1]));
        set_HUD(Convert.ToInt32(newParameters[30].Split('=')[1]));




        
        
        /*set_aileron(Convert.ToDouble(newParameters[28].Split('=')[1]));
        set_elevator(Convert.ToDouble(newParameters[0].Split('=')[1]));
        set_rudder(Convert.ToDouble(newParameters[30].Split('=')[1]));
        set_vertical_view(Convert.ToDouble(newParameters[31].Split('=')[1]));
        set_horizontal_view(Convert.ToDouble(newParameters[32].Split('=')[1]));*/


        /*
        set_EXT_PWR(Convert.ToInt32(newParameters[0].Substring(2, 1)));

        set_BAT(Convert.ToInt32(newParameters[1].Substring(3, 1)));

        set_APU_GEN(Convert.ToInt32(newParameters[2].Substring(2, 1)));

        set_APU_START(Convert.ToInt32(newParameters[3].Substring(2, 1)));

        set_APU_M_SW(Convert.ToInt32(newParameters[4].Substring(2, 1)));

        set_APU_BLEED(Convert.ToInt32(newParameters[5].Substring(3, 1)));

        set_LTK_PUMPS_1(Convert.ToInt32(newParameters[6].Substring(4, 1)));

        set_LTK_PUMPS_2(Convert.ToInt32(newParameters[7].Substring(4, 1)));

        set_PUMPS(Convert.ToInt32(newParameters[8].Substring(4, 1)));

        set_RTK_PUMPS_1(Convert.ToInt32(newParameters[9].Substring(4, 1)));

        set_RTK_PUMPS_2(Convert.ToInt32(newParameters[10].Substring(4, 1)));

        set_ENG_START_SW(Convert.ToInt32(newParameters[11].Substring(3, 1)));

        set_ENG1(Convert.ToInt32(newParameters[12].Substring(3, 1)));

        set_ENG2(Convert.ToInt32(newParameters[13].Substring(3, 1)));

        set_ENG1_N1(Convert.ToDouble(newParameters[14].Substring(4, newParameters[14].Length - 5)));

        set_ENG2_N1(Convert.ToDouble(newParameters[15].Substring(4, newParameters[15].Length - 5)));

        set_BRK_PRK(Convert.ToInt32(newParameters[16].Substring(2, 1)));

        set_ENG1_GEN(Convert.ToInt32(newParameters[17].Substring(4, 1)));

        set_ENG2_GEN(Convert.ToInt32(newParameters[18].Substring(4, 1)));

        set_PACK1(Convert.ToInt32(newParameters[19].Substring(3, 1)));

        set_PACK2(Convert.ToInt32(newParameters[20].Substring(3, 1)));

        set_throttle1(Convert.ToDouble(newParameters[21].Substring(3, newParameters[21].Length - 4)));

        set_throttle2(Convert.ToDouble(newParameters[22].Substring(3, newParameters[22].Length - 4)));

        set_AUTO_BRK(Convert.ToInt32(newParameters[23].Substring(4, 1)));

        set_spoiler_ARM(Convert.ToInt32(newParameters[24].Substring(4, 1)));

        set_spoiler_Speedbrake(Convert.ToDouble(newParameters[25].Substring(4, newParameters[25].Length - 5)));

        set_gear(Convert.ToInt32(newParameters[26].Substring(3, 1)));

        set_flaps(Convert.ToDouble(newParameters[27].Substring(3, newParameters[27].Length - 4)));

        set_aileron(Convert.ToDouble(newParameters[28].Substring(3, newParameters[28].Length - 4)));

        set_elevator(Convert.ToDouble(newParameters[29].Substring(3, newParameters[29].Length - 4)));

        set_rudder(Convert.ToDouble(newParameters[30].Substring(3, newParameters[30].Length - 4)));

        set_vertical_view(Convert.ToDouble(newParameters[31].Substring(3, newParameters[31].Length - 4)));

        set_horizontal_view(Convert.ToDouble(newParameters[32].Substring(3, newParameters[32].Length - 4)));
		
		set_velocity(Convert.ToDouble(newParameters[33].Substring(3, newParameters[33].Length - 4)));
		
		set_altitude(Convert.ToDouble(newParameters[34].Substring(3, newParameters[34].Length - 4)));

        set_HUD(Convert.ToInt32(newParameters[35].Substring(4, 1)));
        *//*
    }*/

    public string toFlightGear()
    {
        string s = "";
        foreach (KeyValuePair<string, double> kvp in mapValue)
        {
            if (mapIO[kvp.Key] != "Output")
            {

                if (mapType[kvp.Key] == "int" || mapType[kvp.Key] == "bool")
                {
                    s += Convert.ToInt32(kvp.Value) + ";";
                }
                else
                {
                    s += kvp.Value + ";";
                }
            }
        }

        String msgFG = s + get_HUD() + ";\n\r";
        return msgFG +"\n\r";

        //return s + get_HUD() + ";";


        //return String.Format(
        //    "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};" +
        //    "{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};" +
        //    "{20};{21};{22};{23};{24};{25};{26};\r\n",
        //    get_EXT_PWR(),
        //    get_BAT(),
        //    get_APU_GEN(),
        //    get_APU_START(),
        //    get_APU_M_SW(),
        //    get_APU_BLEED(),
        //    get_LTK_PUMPS_1(),
        //    get_LTK_PUMPS_2(),
        //    get_PUMPS(),
        //    get_RTK_PUMPS_1(),
        //    get_RTK_PUMPS_2(),
        //    get_ENG_START_SW(),
        //    get_ENG1(),
        //    get_ENG2(),
        //    get_BRK_PRK(),
        //    get_ENG1_GEN(),
        //    get_ENG2_GEN(),
        //    get_PACK1(),
        //    get_PACK2(),
        //    get_throttle1(),
        //    get_throttle2(),
        //    get_AUTO_BRK(),
        //    get_Spoiler_ARM(),
        //    get_Spoiler_Speedbrake(),
        //    get_gear(),
        //    get_flaps(),
        //    get_HUD());
        //return "" + get_EXT_PWR() + ";" + get_BAT() + ";"/* + get_BAT1() + ";" + get_BAT2() + ";"*/ +
        //get_APU_GEN() + ";" + get_APU_START() + ";" + get_APU_M_SW() + ";" + get_APU_BLEED() + ";" + get_LTK_PUMPS_1() + ";" +
        //get_LTK_PUMPS_2() + ";" + get_PUMPS() + ";" + get_RTK_PUMPS_1() + ";" + get_RTK_PUMPS_2() + ";" + get_ENG_START_SW() + ";" +
        //get_ENG1() + ";" + get_ENG2() + ";" + get_BRK_PRK() + ";" + get_ENG1_GEN() + ";" + get_ENG2_GEN() + ";" +
        //get_PACK1() + ";" + get_PACK2() + ";" + get_throttle1() + ";" + get_throttle2() + ";" + get_AUTO_BRK() + ";" +
        //get_Spoiler_ARM() + ";" + get_Spoiler_Speedbrake() + ";" + get_gear() + ";" + get_flaps() + ";" + get_HUD() + ";";
    }

    /*
    public int get_EXT_PWR()
    {
        return EXT_PWR;
    }

    public void set_EXT_PWR(int value)
    {
        EXT_PWR = value;
    }

    public int get_BAT()
    {
        return BAT;
    }

    public void set_BAT(int value)
    {
        BAT = value;
    }

    public int get_BAT1()
    {
        return BAT1;
    }

    public void set_BAT1(int value)
    {
        BAT1 = value;
    }

    public int get_BAT2()
    {
        return BAT2;
    }

    public void set_BAT2(int value)
    {
        BAT2 = value;
    }

    public int get_APU_GEN()
    {
        return APU_GEN;
    }

    public void set_APU_GEN(int value)
    {
        APU_GEN = value;
    }

    public int get_APU_START()
    {
        return APU_START;
    }

    public void set_APU_START(int value)
    {
        APU_START = value;
    }

    public int get_APU_M_SW()
    {
        return APU_M_SW;
    }

    public void set_APU_M_SW(int value)
    {
        APU_M_SW = value;
    }

    public int get_APU_BLEED()
    {
        return APU_BLEED;
    }

    public void set_APU_BLEED(int value)
    {
        APU_BLEED = value;
    }

    public int get_LTK_PUMPS_1()
    {
        return LTK_PUMPS_1;
    }

    public void set_LTK_PUMPS_1(int value)
    {
        LTK_PUMPS_1 = value;
    }

    public int get_LTK_PUMPS_2()
    {
        return LTK_PUMPS_2;
    }

    public void set_LTK_PUMPS_2(int value)
    {
        LTK_PUMPS_2 = value;
    }

    public int get_PUMPS()
    {
        return PUMPS;
    }

    public void set_PUMPS(int value)
    {
        PUMPS = value;
    }

    public int get_RTK_PUMPS_1()
    {
        return RTK_PUMPS_1;
    }

    public void set_RTK_PUMPS_1(int value)
    {
        RTK_PUMPS_1 = value;
    }

    public int get_RTK_PUMPS_2()
    {
        return RTK_PUMPS_2;
    }

    public void set_RTK_PUMPS_2(int value)
    {
        RTK_PUMPS_2 = value;
    }

    public int get_ENG_START_SW()
    {
        return ENG_START_SW;
    }

    public void set_ENG_START_SW(int value)
    {
        ENG_START_SW = value;
    }

    public int get_ENG1()
    {
        return ENG1;
    }

    public void set_ENG1(int value)
    {
        ENG1 = value;
    }

    public int get_ENG2()
    {
        return ENG2;
    }

    public void set_ENG2(int value)
    {
        ENG2 = value;
    }

    public double get_ENG1_N1()
    {
        return ENG1_N1;
    }

    public void set_ENG1_N1(double value)
    {
        ENG1_N1 = value;
    }

    public double get_ENG2_N1()
    {
        return ENG2_N1;
    }

    public void set_ENG2_N1(double value)
    {
        ENG2_N1 = value;
    }

    public int get_BRK_PRK()
    {
        return BRK_PRK;
    }

    public void set_BRK_PRK(int value)
    {
        BRK_PRK = value;
    }

    public int get_ENG1_GEN()
    {
        return ENG1_GEN;
    }

    public void set_ENG1_GEN(int value)
    {
        ENG1_GEN = value;
    }

    public int get_ENG2_GEN()
    {
        return ENG2_GEN;
    }

    public void set_ENG2_GEN(int value)
    {
        ENG2_GEN = value;
    }

    public int get_PACK1()
    {
        return PACK1;
    }

    public void set_PACK1(int value)
    {
        PACK1 = value;
    }

    public int get_PACK2()
    {
        return PACK2;
    }

    public void set_PACK2(int value)
    {
        PACK2 = value;
    }

    public double get_throttle1()
    {
        return throttle1;
    }

    public void set_throttle1(double value)
    {
        if(Math.Abs(value) < 0.01)
        {
            value = 0;
        }

        throttle1 = value;
    }

    public bool is_throttle1_max()
    {
        if((int)get_throttle1() == 1)
        {
            return true;
        }

        return false;
    }

    public double get_throttle2()
    {
        return throttle2;
    }

    public void set_throttle2(double value)
    {
        if (Math.Abs(value) < 0.01)
        {
            value = 0;
        }

        throttle2 = value;
    }

    public bool is_throttle2_max()
    {
        if ((int)get_throttle2() == 1)
        {
            return true;
        }

        return false;
    }

    public int get_AUTO_BRK()
    {
        return auto_Brake;
    }

    public void set_AUTO_BRK(int value)
    {
        auto_Brake = value;
    }

    public int get_Spoiler_ARM()
    {
        return spoiler_ARM;
    }

    public void set_spoiler_ARM(int value)
    {
        spoiler_ARM = value;
    }

    public double get_Spoiler_Speedbrake()
    {
        return spoiler_Speedbrake;
    }

    public void set_spoiler_Speedbrake(double value)
    {
        spoiler_Speedbrake = value;
    }

    public int get_gear()
    {
        return gear;
    }

    public void set_gear(int value)
    {
        gear = value;
    }

    public double get_flaps()
    {
        return flaps;
    }

    public void set_flaps(double value)
    {
        flaps = value;
    }

    public double get_aileron()
    {
        return aileron;
    }

    public void set_aileron(double value)
    {
        aileron = value;
    }

    public double get_elevator()
    {
        return elevator;
    }

    public void set_elevator(double value)
    {
        elevator = value;
    }

    public double get_rudder()
    {
        return rudder;
    }

    public void set_rudder(double value)
    {
        rudder = value;
    }

    public double get_vertical_view()
    {
        return vertical_view;
    }

    public void set_vertical_view(double value)
    {
        vertical_view = value;
    }

    public double get_horizontal_view()
    {
        return horizontal_view;
    }

    public void set_horizontal_view(double value)
    {
        horizontal_view = value;
    }
	
	public double get_velocity(){
		return velocity;
	}
	
	public void set_velocity(double value){
		velocity = value;
	}
	
	public double get_altitude(){
		return altitude;
	}
	
	public void set_altitude(double value){
		altitude = value;

        if (initAltitude)
        {
            set_initialAltitude(altitude);
            initAltitude = false;
        }
	}*/

    public double get_initialAltitude()
    {
        return initialAltitude;
    }

    public void set_initialAltitude(double value)
    {
        initialAltitude = value;
    }

    public int get_HUD()
    {
        return HUD;
    }

    public void set_HUD(int value)
    {
        HUD = value;
    }

    //aggiunto da giovanni
    public bool isAllZero()
    {
        var buffer = new List<KeyValuePair<string, double>>(this.mapValue);
        foreach (var val in buffer)
        {
            if (val.Value != 0.0)
                return false;
        }

        return true;
    }
}
