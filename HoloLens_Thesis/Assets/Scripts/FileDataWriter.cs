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
        string parameter;
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
                parameter = ComponentsEnums.GetStringFromEnum(component.GetComponent<NewParameterManager>().getID());

                file.components.Add(new Cockpit_Component_Json(cockpit_components[i], position, rotation, scale, parameter));
                ++i;
            }
        }

        ContainerBox contBox = GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>();
        int len = InputSequence.Instance.getSeqLength();

        if (!contBox.BoxFromFileExists())
        {
            contBox.InitializeBoxFromFile();
        }

        Vector3[][] boxFromFile = contBox.getBoxFromFile();
        Vector3[] boxFile;

        for (i = 0; i < len; i++)
        {
            boxFile = boxFromFile[i];

            if (boxFile.Length != 0)
            {
                file.boxes.Add(new Container_Box_Json(i, boxFile[0], boxFile[1], boxFile[2], boxFile[3], boxFile[4], boxFile[5], boxFile[6], boxFile[7], boxFile[8], boxFile[9], boxFile[10], boxFile[11]));
            }
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
