using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonSaveObject {

    [HideInInspector]
    public string name;
    [HideInInspector]
    public List<Cockpit_Component_Json> components;
    [HideInInspector]
    public List<FeedbackGroup> feedback_group;
    [HideInInspector]
    public List<Container_Box_Json> boxes;
    [HideInInspector]
    public List<Task> tasks;

    public JsonSaveObject()
    {
        name = "";
        tasks = new List<Task>();
        components = new List<Cockpit_Component_Json>();
        feedback_group = new List<FeedbackGroup>();
        boxes = new List<Container_Box_Json>();
    }
}
