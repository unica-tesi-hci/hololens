using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Task
{
    public string duration;
    public string id;
    public string name;
    public string nb_iteration;
    public string optional;
    public string type;
    public string x;
    public string y;
    public string sequence;

    public Task() { }

    public Task(string duration, string id, string name, string nb_iteration, string optional, string type, string x, string y, string sequence)
    {
        this.duration = duration;
        this.id = id;
        this.name = name;
        this.nb_iteration = nb_iteration;
        this.optional = optional;
        this.type = type;
        this.x = x;
        this.y = y;
        this.sequence = sequence;
    }
}
