using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cockpit_Component_Json {

    public string nameObject;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string[] parameter;
    public string[] dataType;
    public string[] param_path;
    public string[] interaction;
    public double[][] possible_values;
    public string[] IO_Data;

    public Cockpit_Component_Json()
    {

    }

    /**
     * This constructor contains the following parameters:
     * nameObject: the gameObject's Unity name.
     * position: the gameObject's position.
     * rotation: the gameObject's euler rotation.
     * scale: the gameObject's scale.
     * parameter: list containing the component's inner paramaters, each corresponding to a specific function.
     * dataType: list containing the type of data (bool, int or double) corresponding to each component's parameter.
     * param_path: list containing the paths of each parameter used in the flight simulator program, inside which there is stored the corresponding data value.
     * interaction: list containing the type of gesture that has to be used to interact whit the corresponding parameter.
     * possible_values: matrix containing, for each parameter, a list of values that can be obtained when that specific parameter has been interacted.
     * IO_Data: list containing if each parameter's values are used as input, output, or both.
     **/
    public Cockpit_Component_Json(string nameObj, Vector3 pos, Vector3 rot, Vector3 scl, string[] par, string[] type, string[] path, string[] action, double[][] values, string[] IO)
    {
        nameObject = nameObj;
        position = pos;
        rotation = rot;
        scale = scl;
        parameter = par;
        dataType = type;
        param_path = path;
        interaction = action;
        possible_values = values;
        IO_Data = IO;
    }

}
