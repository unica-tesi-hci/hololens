using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//API containing methods in order to perform actions towards the cockpit's objects.
public class CockpitObjectsAPI : MonoBehaviour {

    //Sets the position in x, y and z coordinates to the object having "objName" as name.
	public void SetPosition(string objName, double x, double y, double z)
    {
        GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.position = new Vector3((float)x, (float)y, (float)z);
    }

    //Gets the position in x, y and z coordinates from the object having "objName" as name.
    public void GetPosition(string objName, out double x, out double y, out double z)
    {
        Vector3 v = GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.position;

        x = v.x;
        y = v.y;
        z = v.z;
    }

    //Set the rotation in x, y and z values to the object having "objName" as name.
    public void SetRotation(string objName, double x, double y, double z)
    {
        GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.rotation = Quaternion.Euler(new Vector3((float)x, (float)y, (float)z));
    }

    //Gets the rotation in x, y and z value from the object having "objName" as name.
    public void GetRotation(string objName, out double x, out double y, out double z)
    {
        Vector3 v = GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.rotation.eulerAngles;

        x = v.x;
        y = v.y;
        z = v.z;
    }

    //Set the scale in x, y and z values to the object having "objName" as name.
    public void SetScale(string objName, double x, double y, double z)
    {
        GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.localScale = new Vector3((float)x, (float)y, (float)z);
    }

    //Gets the scale in x, y and z values from the object having "objName" as name.
    public void GetScale(string objName, out double x, out double y, out double z)
    {
        Vector3 v = GameObject.FindWithTag("Cockpit").transform.Find(objName).transform.localScale;

        x = v.x;
        y = v.y;
        z = v.z;
    }

    //Sets the parameter value to the object having "objName" as name.
    public void SetSimulationParameter(string objName, string[] param)
    {
        ComponentsEnums.Components[] enums = new ComponentsEnums.Components[param.Length];

        for(int i = 0; i < param.Length; i++)
        {
            enums[i] = ComponentsEnums.GetEnumFromString(param[i]);
        }

        GameObject.FindWithTag("Cockpit").transform.Find(objName).GetComponent<NewParameterManager>().setID(enums);
    }

    //Gets the parameter value from the object having "objName" as name.
    public string[] GetSimulationParameter(string objName)
    {
        return GameObject.FindWithTag("Cockpit").transform.Find(objName).GetComponent<NewParameterManager>().getIDAsString();
    }

    /*Sets the specified value to the object having "path" as path, then sends that value to the corresponding 
     *component into the flight simulator.
     *Returns true if the process has been done successfully, false otherwise.
     */
    public bool SetSimulationParameterValue(string path, int val)
    {
        bool b = Parameters.Instance.UpdateValue(path, val, "int");

        if (b)
        {
            SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
        }

        return b;
    }

    /*Gets the specified value from the object having "path" as path.
     */
    public void GetSimulationParameterValue(string path, out int val)
    {
        val = Parameters.Instance.GetValueInt(path);
    }

    /*Sets the specified value to the object having "path" as path, then sends that value to the corresponding 
     *component into the flight simulator.
     *Returns true if the process has been done successfully, false otherwise.
     */
    public bool SetSimulationParameterValue(string path, double val)
    {
        bool b = Parameters.Instance.UpdateValue(path, val, "double");

        if (b)
        {
            SocketManager.Instance.SendUdpDatagram(Parameters.Instance.toFlightGear());
        }

        return b;
    }

    /*Gets the specified value from the object having "path" as path.
     */
    public void GetSimulationParameterValue(string path, out double val)
    {
        val = Parameters.Instance.GetValueDouble(path);
    }

    //Sets the type of the data related to the object having "objName" as name.
    public void SetSimulationParameterType(string objName, string[] type)
    {
        GameObject.FindWithTag("Cockpit").transform.Find(objName).GetComponent<NewParameterManager>().setDataType(type);
    }

    //Gets the type of the data related to the object having "objName" as name.
    public string[] GetSimulationParameterType(string objName)
    {
        return GameObject.FindWithTag("Cockpit").transform.Find(objName).GetComponent<NewParameterManager>().getDataType();
    }

    /*Creates a new object with name "objName".
     *Returns true if the process has been done successfully, false otherwise.
     */
    public bool CreateObject(string objName)
    {
        try
        {
            Material material;
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject component = Instantiate(o);
            component.AddComponent<Interactible>();
            component.AddComponent<TapToPlace>().enabled = false;
            component.AddComponent<HoldToRotate>().enabled = false;
            component.AddComponent<TapToRemove>().enabled = false;
            component.AddComponent<NewParameterManager>();
            component.GetComponent<MeshRenderer>().enabled = true;
            material = Resources.Load("Cockpit_Buttons", typeof(Material)) as Material;
            component.GetComponent<MeshRenderer>().material = material;
            material = Resources.Load("PlaceableShadow", typeof(Material)) as Material;
            component.GetComponent<TapToPlace>().PlacementMaterial = material;
            component.GetComponent<HoldToRotate>().PlacementMaterial = material;

            component.name = objName;
            component.transform.position = new Vector3(0f, 0f, 0.5f);
            component.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            component.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            component.GetComponent<NewParameterManager>().setID(new ComponentsEnums.Components[] { ComponentsEnums.Components.None });
            component.GetComponent<NewParameterManager>().setDataType(new string[] { "double" });

            component.transform.SetParent(GameObject.FindWithTag("Cockpit").transform);
            FileDataWriter.Instance.AddIntoList(component.name);

            Destroy(o);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return false;
    }

    /*Deletes the object with name "objName".
     *Returns true if the process has been done successfully, false otherwise.
     */
    public bool DeleteObject(string objName)
    {
        try
        {
            GameObject obj = GameObject.FindWithTag("Cockpit").transform.Find(objName).gameObject;

            FileDataWriter.Instance.RemoveFromList(obj.name);
            Destroy(obj);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return false;
    }
}
