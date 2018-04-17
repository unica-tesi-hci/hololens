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
    public string parameter;

    public Cockpit_Component_Json()
    {

    }

    public Cockpit_Component_Json(string nameObj, Vector3 pos, Vector3 rot, Vector3 scl, string par)
    {
        nameObject = nameObj;
        position = pos;
        rotation = rot;
        scale = scl;
        parameter = par;
    }

}
