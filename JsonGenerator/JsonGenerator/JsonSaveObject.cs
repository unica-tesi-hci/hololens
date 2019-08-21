using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class JsonSaveObject {

    public string name;
    public List<Task> tasks = new List<Task>();
    public List<Cockpit_Component_Json> components;
    public List<FeedbackGroup> feedback_group;
    public List<Container_Box_Json> boxes;

    public JsonSaveObject()
    {
        name = "";
        components = new List<Cockpit_Component_Json>();
        feedback_group = new List<FeedbackGroup>();
        boxes = new List<Container_Box_Json>();
    }
}
