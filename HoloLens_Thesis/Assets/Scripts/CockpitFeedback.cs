using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CockpitFeedback
{

    public String name;
    public bool showRing;
    public bool insideBox;
    public bool showCheckMark;
    public bool showLoading;
    public bool[] sub_parameters;
    public double requiredValue;

    //aggiunto GIOVANNI
    public bool correctPosition;

    /**
     * This constructor contains the following parameters:
     * name: the Unity's name of the cockpit's component.
     * showRing: boolean specifying if the marker ring must be displayed or not.
     * insideBox: boolean specifying if the object is inside the (eventual) container box.
     * showCheckMark: boolean specifying if the check mark must be displayed or not.
     * showLoading: boolean specifying if the loading animation must be displayed or not.
     * sub_parametres: list specifying, for each inner parameter of the object, if its value must be checked during the current sequence.
     * requiredValue: value that the object must reach in order to be in the correct state.
     **/
    public CockpitFeedback(string n, bool ring, bool box, bool checkMark, bool loading, bool[] sub_param, double value)
    {
        name = n;
        showRing = ring;
        insideBox = box;
        showCheckMark = checkMark;
        showLoading = loading;
        sub_parameters = sub_param;
        requiredValue = value;

        correctPosition = false;
    }
}