using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Container_Box_Json {
    
    public int sequence;
    public Vector3 positionLeft;
    public Vector3 rotationLeft;
    public Vector3 scaleLeft;
    public Vector3 positionTop;
    public Vector3 rotationTop;
    public Vector3 scaleTop;
    public Vector3 positionRight;
    public Vector3 rotationRight;
    public Vector3 scaleRight;
    public Vector3 positionBottom;
    public Vector3 rotationBottom;
    public Vector3 scaleBottom;

    public Container_Box_Json()
    {
        
    }

    public Container_Box_Json(int seq, Vector3 pos1, Vector3 rot1, Vector3 scl1, Vector3 pos2, Vector3 rot2, Vector3 scl2, Vector3 pos3, Vector3 rot3, Vector3 scl3, Vector3 pos4, Vector3 rot4, Vector3 scl4)
    {
        sequence = seq;
        positionLeft = pos1;
        rotationLeft = rot1;
        scaleLeft = scl1;
        positionTop = pos2;
        rotationTop = rot2;
        scaleTop = scl2;
        positionRight = pos3;
        rotationRight = rot3;
        scaleRight = scl3;
        positionBottom = pos4;
        rotationBottom = rot4;
        scaleBottom = scl4;
    }
}
