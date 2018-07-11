using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if !UNITY_EDITOR
using Windows.Storage;
using System.Threading.Tasks;
#endif

public class FileDataReader : Singleton<FileDataReader>
{

    private JsonSaveObject file;
    private GameObject component;
    public GameObject cockpit;
    private int max = -1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Loading the position, rotation and scale of all cockpit's components.
    public void LoadObjectsProperties()
    {
        Material material;
        NewParameterManager newParameterManager;
        Dictionary<GameObject, NewParameterManager> map = new Dictionary<GameObject, NewParameterManager>();

        component = GameObject.CreatePrimitive(PrimitiveType.Cube);
        component.AddComponent<Interactible>();
        component.AddComponent<TapToPlace>().enabled = false;
        component.AddComponent<HoldToRotate>().enabled = false;
        component.AddComponent<TapToRemove>().enabled = false;
        component.AddComponent<NewParameterManager>();
        component.GetComponent<MeshRenderer>().enabled = false;
        material = Resources.Load("Cockpit_Buttons", typeof(Material)) as Material;
        component.GetComponent<MeshRenderer>().material = material;
        material = Resources.Load("PlaceableShadow", typeof(Material)) as Material;
        component.GetComponent<TapToPlace>().PlacementMaterial = material;
        component.GetComponent<HoldToRotate>().PlacementMaterial = material;

        GameObject component_instantiated;
        int i = 0;
        int value;

        file = new JsonSaveObject();

#if !UNITY_EDITOR
        Load();
#else
        LoadUnity();
#endif

        try
        {
            foreach (Cockpit_Component_Json cockpit_component in file.components)
            {
                component_instantiated = Instantiate(component);
                component_instantiated.name = cockpit_component.nameObject;
                component_instantiated.transform.position = cockpit_component.position;
                component_instantiated.transform.rotation = Quaternion.Euler(cockpit_component.rotation);
                component_instantiated.transform.localScale = cockpit_component.scale;
                newParameterManager = component_instantiated.GetComponent<NewParameterManager>();
                newParameterManager.initID(cockpit_component.parameter);
                newParameterManager.setDataType(cockpit_component.dataType);
                newParameterManager.setPath(cockpit_component.param_path);
                newParameterManager.setInteraction(cockpit_component.interaction);
                newParameterManager.setPossibleValues(cockpit_component.possible_values);
                newParameterManager.setIOData(cockpit_component.IO_Data);

                component_instantiated.transform.SetParent(cockpit.transform);

                FileDataWriter.Instance.AddIntoList(component_instantiated.name);

                value = Convert.ToInt32(component_instantiated.name.Substring(4));
                if (value > max)
                {
                    max = value;
                }

                for(i = 0; i < cockpit_component.parameter.Length; i++)
                {
                    Parameters.Instance.AddValue(cockpit_component.param_path[i], 0, cockpit_component.dataType[i], cockpit_component.IO_Data[i]);
                }

                map.Add(component_instantiated, newParameterManager);
            }

            InputSequence.Instance.InitMap(map);

            Destroy(component);

            i = 0;

            int len = InputSequence.Instance.getSeqLength();

            List<FeedbackGroup> sequenceFromFile = file.feedback_group;

            InputSequence.Instance.InitSequenceObjects(sequenceFromFile);

            Container_Box_Json[] boxFromFile = new Container_Box_Json[len];

            foreach (Container_Box_Json container_box in file.boxes)
            {
                while (sequenceFromFile[i].sequence != container_box.sequence)
                {
                    if (sequenceFromFile[i].sequence != "End")
                    {
                        boxFromFile[i] = null;
                        ++i;
                    }
                    else
                    {
                        break;
                    }
                }

                boxFromFile[i] = container_box;
                ++i;
            }

            while (i < len)
            {
                boxFromFile[i] = null;
                ++i;
            }

            GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>().InitializeBoxFromFile(boxFromFile);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    public int GetNextObjectNumber()
    {
        ++max;

        return max;
    }

#if !UNITY_EDITOR
    public void Load()
    {
        string path = String.Format("/savedObjects.json");

        /*if (!Directory.Exists(Application.persistentDataPath + "/../ObjectsData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/../ObjectsData");
        }*/

        if (!File.Exists(Application.persistentDataPath + path))
        {
            path = String.Format("/defaultSavedObjects.json");

            if (!File.Exists(Application.persistentDataPath + path))
            {
                File.Copy(KnownFolders.CameraRoll.Path + path, Application.persistentDataPath + path);
            }
        }

        using (StreamReader reader = new StreamReader(new FileStream(Application.persistentDataPath + path, FileMode.Open)))
        {
            try
            {
                file = JsonUtility.FromJson<JsonSaveObject>(reader.ReadLine());
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        /*StorageFolder storageFolder = KnownFolders.CameraRoll;
        StorageFile sf = null;
        if (await storageFolder.TryGetItemAsync("savedObjects.json") != null)
        {
            sf = await storageFolder.GetFileAsync("savedObjects.json");
        }
        else
        {
            sf = await storageFolder.GetFileAsync("defaultSavedObjects.json");
        }

        while(sf == null)
        {
            //Wait...
        }

        var buffer = await FileIO.ReadBufferAsync(sf);

        while(buffer == null)
        {
            //Wait...
        }

        try
        {
            using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                file = JsonUtility.FromJson<JsonSaveObject>(reader.ReadString(buffer.Length));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }*/
    }
#endif

#if UNITY_EDITOR
    public void LoadUnity()
    {
        string path = String.Format("/ObjectsData/savedObjects.json");

        if (!Directory.Exists(Application.dataPath + "/../ObjectsData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../ObjectsData");
        }

        if (!File.Exists(Application.dataPath + "/.." + path))
        {
            path = String.Format("/ObjectsData/defaultSavedObjects.json");
        }

        using (StreamReader reader = new StreamReader(new FileStream(Application.dataPath + "/.." + path, FileMode.Open)))
        {
            try
            {
                file = JsonUtility.FromJson<JsonSaveObject>(reader.ReadLine());
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
#endif

}
