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

        file = new JsonSaveObject();

#if !UNITY_EDITOR
        Load().Wait();
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
                component_instantiated.GetComponent<NewParameterManager>().setID(ComponentsEnums.GetEnumFromString(cockpit_component.parameter));

                component_instantiated.transform.SetParent(cockpit.transform);

                FileDataWriter.Instance.AddIntoList(component_instantiated.name);
            }

            Destroy(component);

            int len = InputSequence.Instance.getSeqLength();
            i = 0;
            Vector3[][] boxFromFile = new Vector3[len][];
            Vector3[] boxData;

            foreach (Container_Box_Json container_box in file.boxes)
            {
                while (i != container_box.sequence)
                {
                    boxData = new Vector3[] { };
                    boxFromFile[i] = boxData;
                    ++i;
                }

                boxData = new Vector3[12] { container_box.positionLeft, container_box.rotationLeft, container_box.scaleLeft, container_box.positionTop, container_box.rotationTop, container_box.scaleTop, container_box.positionRight, container_box.rotationRight, container_box.scaleRight, container_box.positionBottom, container_box.rotationBottom, container_box.scaleBottom };
                boxFromFile[container_box.sequence] = boxData;
                ++i;
            }

            while(i < len)
            {
                boxData = new Vector3[] { };
                boxFromFile[i] = boxData;
                ++i;
            }

            GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>().InitializeBoxFromFile(boxFromFile);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

#if !UNITY_EDITOR
    public async Task Load()
    {
        StorageFolder storageFolder = KnownFolders.CameraRoll;
        StorageFile sf;
        if (await storageFolder.TryGetItemAsync("savedObjects.json") != null)
        {
            sf = await storageFolder.GetFileAsync("savedObjects.json");
        }
        else
        {
            sf = await storageFolder.GetFileAsync("defaultSavedObjects.json");
        }

        var buffer = await FileIO.ReadBufferAsync(sf);

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
        }
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
