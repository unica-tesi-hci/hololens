using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voice_Commands : MonoBehaviour {

    private BoxScaling boxScaling;

    private void Start()
    {
        boxScaling = GameObject.FindWithTag("Cockpit").GetComponent<BoxScaling>();
    }

    public void Show_HUD()
    {
        SocketManager.Instance.SendUdpDatagram("" + Parameters.Instance.get_EXT_PWR() + ";" + Parameters.Instance.get_BAT() + ";"/* + Parameters.Instance.get_BAT1() + ";" + Parameters.Instance.get_BAT2() + ";"*/ +
        Parameters.Instance.get_APU_GEN() + ";" + Parameters.Instance.get_APU_START() + ";" + Parameters.Instance.get_APU_M_SW() + ";" + Parameters.Instance.get_APU_BLEED() + ";" + Parameters.Instance.get_LTK_PUMPS_1() + ";" +
        Parameters.Instance.get_LTK_PUMPS_2() + ";" + Parameters.Instance.get_PUMPS() + ";" + Parameters.Instance.get_RTK_PUMPS_1() + ";" + Parameters.Instance.get_RTK_PUMPS_2() + ";" + Parameters.Instance.get_ENG_START_SW() + ";" +
        Parameters.Instance.get_ENG1() + ";" + Parameters.Instance.get_ENG2() + ";" + Parameters.Instance.get_BRK_PRK() + ";" + Parameters.Instance.get_ENG1_GEN() + ";" + Parameters.Instance.get_ENG2_GEN() + ";" +
        Parameters.Instance.get_PACK1() + ";" + Parameters.Instance.get_PACK2() + ";" + Parameters.Instance.get_throttle1() + ";" + Parameters.Instance.get_throttle2() + ";" + Parameters.Instance.get_AUTO_BRK() + ";" +
        Parameters.Instance.get_Spoiler_ARM() + ";" + Parameters.Instance.get_Spoiler_Speedbrake() + ";" + Parameters.Instance.get_gear() + ";" + Parameters.Instance.get_flaps() + ";1;");
    }

    public void Hide_HUD()
    {
        SocketManager.Instance.SendUdpDatagram("" + Parameters.Instance.get_EXT_PWR() + ";" + Parameters.Instance.get_BAT() + ";"/* + Parameters.Instance.get_BAT1() + ";" + Parameters.Instance.get_BAT2() + ";"*/ +
        Parameters.Instance.get_APU_GEN() + ";" + Parameters.Instance.get_APU_START() + ";" + Parameters.Instance.get_APU_M_SW() + ";" + Parameters.Instance.get_APU_BLEED() + ";" + Parameters.Instance.get_LTK_PUMPS_1() + ";" +
        Parameters.Instance.get_LTK_PUMPS_2() + ";" + Parameters.Instance.get_PUMPS() + ";" + Parameters.Instance.get_RTK_PUMPS_1() + ";" + Parameters.Instance.get_RTK_PUMPS_2() + ";" + Parameters.Instance.get_ENG_START_SW() + ";" +
        Parameters.Instance.get_ENG1() + ";" + Parameters.Instance.get_ENG2() + ";" + Parameters.Instance.get_BRK_PRK() + ";" + Parameters.Instance.get_ENG1_GEN() + ";" + Parameters.Instance.get_ENG2_GEN() + ";" +
        Parameters.Instance.get_PACK1() + ";" + Parameters.Instance.get_PACK2() + ";" + Parameters.Instance.get_throttle1() + ";" + Parameters.Instance.get_throttle2() + ";" + Parameters.Instance.get_AUTO_BRK() + ";" +
        Parameters.Instance.get_Spoiler_ARM() + ";" + Parameters.Instance.get_Spoiler_Speedbrake() + ";" + Parameters.Instance.get_gear() + ";" + Parameters.Instance.get_flaps() + ";0;");
    }

    public void Repeat_Text()
    {
        InputSequence.Instance.Speak_Text();
    }

    public void Open_Menu()
    {
        MenuManager.Instance.OpenMenu();
    }

    public void Close_Menu()
    {
        MenuManager.Instance.CloseMenu();
    }
	
	public void Choose_Sequence(int value)
	{
		if(MenuManager.Instance.isScaling && !MenuManager.Instance.isMenuOpen())
		{
			if(value == 22 || value == 23 || value == 25 || value == 27 || value == 29)
			{
				GameObject.FindWithTag("AudioManager").GetComponent<TextToSpeechManager>().SpeakText("This phase doesn't have any box to scale. Choose another one.");
				return;
			}
			
			boxScaling.ChooseSequence(value);
		}
	}
}
