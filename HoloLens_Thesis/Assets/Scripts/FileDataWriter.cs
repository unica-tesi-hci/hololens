using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
#if !UNITY_EDITOR
using Windows.Storage;
#endif

public class FileDataWriter : Singleton<FileDataWriter>
{

    private List<String> cockpit_components = new List<String>();
    private JsonSaveObject file;
    public GameObject cockpit;

    public void SaveObjectsProperties()
    {
        Vector3 position;
        Vector3 rotation;
        Vector3 scale;
        string[] parameter;
        string[] dataType;
        string[] path;
        string[] interaction;
        double[][] possible_values;
        string[] IO_Data;
        NewParameterManager newParameterManager;
        int i = 0;

        file = new JsonSaveObject();

        foreach (Transform component in cockpit.GetComponentsInChildren<Transform>())
        {
            newParameterManager = component.gameObject.GetComponent<NewParameterManager>();

            if (newParameterManager != null)
            {
                position = component.position;
                rotation = component.rotation.eulerAngles;
                scale = component.localScale;
                parameter = component.GetComponent<NewParameterManager>().getIDAsString();
                dataType = component.GetComponent<NewParameterManager>().getDataType();
                path = component.GetComponent<NewParameterManager>().getPath();
                interaction = component.GetComponent<NewParameterManager>().getInteraction();
                possible_values = component.GetComponent<NewParameterManager>().getPossibleValues();
                IO_Data = component.GetComponent<NewParameterManager>().getIOData();

                file.components.Add(new Cockpit_Component_Json(cockpit_components[i], position, rotation, scale, parameter, dataType, path, interaction, possible_values, IO_Data));
                ++i;
            }
        }

        List<FeedbackGroup> sequences = InputSequence.Instance.GetSequenceObjects();
        foreach (FeedbackGroup feed_grp in sequences)
        {
            file.feedback_group.Add(feed_grp);
        }

        ContainerBox contBox = GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>();
        int len = InputSequence.Instance.getSeqLength();

        if (!contBox.BoxFromFileExists())
        {
            contBox.InitializeBoxFromFile();
        }

        Container_Box_Json[] boxFromFile = contBox.getBoxFromFile();

        for (i = 0; i < len; i++)
        {
            file.boxes.Add(boxFromFile[i]);
        }

#if !UNITY_EDITOR
        Save();
#else
        SaveUnity();
#endif

        TextManager.Instance.ShowMenuTipText("Save complete.");
    }

#if !UNITY_EDITOR
    public async void Save()
    {
        //Saving the position, rotation and scale of all cockpit's components and the container boxes of each sequence.
        try
        {
			StorageFolder storageFolder = KnownFolders.CameraRoll;
			StorageFile sf = await storageFolder.CreateFileAsync("savedObjects.json", CreationCollisionOption.ReplaceExisting);

			var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(JsonUtility.ToJson(file), Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
			await FileIO.WriteBufferAsync(sf, buffer);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        string path = String.Format("/savedObjects.json");

        FileStream fs = new FileStream(Application.persistentDataPath + path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
        {
            try
            {
                writer.WriteLine(JsonUtility.ToJson(file));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
#endif

#if UNITY_EDITOR
    public void SaveUnity()
    {
        //Saving the position, rotation and scale of all cockpit's components and the container boxes of each sequence.
        string path = String.Format("/ObjectsData/savedObjects.json");

        if (!Directory.Exists(Application.dataPath + "/../ObjectsData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../ObjectsData");
        }

        FileStream fs = new FileStream(Application.dataPath + "/.." + path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
        {
            try
            {
                writer.WriteLine(JsonUtility.ToJson(file));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
#endif

    public bool AddIntoList(String value)
    {
        if (!cockpit_components.Contains(value))
        {
            cockpit_components.Add(value);

            return true;
        }

        return false;
    }

    public void RemoveFromList(String value)
    {
        cockpit_components.Remove(value);
    }

}
