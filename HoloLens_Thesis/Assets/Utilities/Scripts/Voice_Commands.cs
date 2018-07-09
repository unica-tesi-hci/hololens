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
        Parameters.Instance.set_HUD(1);

        SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
    }

    public void Hide_HUD()
    {
        Parameters.Instance.set_HUD(0);

        SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
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
	
	/*public void Choose_Sequence(int value)
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
	}*/
}
