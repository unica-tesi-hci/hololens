using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonSaveObject2 {

    [HideInInspector]
    public List<Cockpit_Component_Json> components;


    public JsonSaveObject2()
    {
        components = new List<Cockpit_Component_Json>();
    }
}
