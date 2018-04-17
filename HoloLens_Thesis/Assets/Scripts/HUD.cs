using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    private int newHUD;

    private void Start()
    {

    }

    void Select()
    {

        if (Parameters.Instance.get_HUD() == 1)
        {
            Parameters.Instance.set_HUD(0);
        }
        else
        {
            Parameters.Instance.set_HUD(1);
        }

        SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());

    }

    void OnSelect()
    {
        Select();
    }
}
