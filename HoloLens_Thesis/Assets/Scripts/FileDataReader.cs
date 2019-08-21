using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Xml;

#if !UNITY_EDITOR
using Windows.Storage;
using System.Threading.Tasks;
#endif

public class FileDataReader : Singleton<FileDataReader>
{
    public List<JsonSaveObject> listaSequenze = new List<JsonSaveObject>();
    private JsonSaveObject file;
    private JsonSaveObject2 mapValues;
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
        LoadUnity2();
#endif

        //List<FeedbackGroup> sequenceFromFile = new List<FeedbackGroup>();

        //modificato giovanni
        List<List<FeedbackGroup>> sequencesFromFile = new List<List<FeedbackGroup>>();
        List<List<Task>> tasksList = new List<List<Task>>();
        List<Cockpit_Component_Json> components = new List<Cockpit_Component_Json>();
        List<Container_Box_Json> boxes = new List<Container_Box_Json>();

        foreach (JsonSaveObject seq in listaSequenze)
        {
            sequencesFromFile.Add(seq.feedback_group);
            tasksList.Add(seq.tasks);

            foreach (Cockpit_Component_Json comp in seq.components)
            {
                if (!components.Contains(comp))
                    components.Add(comp);
            }
            foreach (Container_Box_Json box in seq.boxes)
            {
                if (!boxes.Contains(box))
                    boxes.Add(box);
            }
        }
        try
        {
            //
            foreach (Cockpit_Component_Json cockpit_component in mapValues.components)
            {
                component_instantiated = Instantiate(component);
                component_instantiated.name = cockpit_component.nameObject;
                component_instantiated.transform.position = cockpit_component.position;
                component_instantiated.transform.rotation = Quaternion.Euler(cockpit_component.rotation);
                component_instantiated.transform.localScale = cockpit_component.scale;
                newParameterManager = component_instantiated.GetComponent<NewParameterManager>();

                newParameterManager.setDataType(cockpit_component.dataType);
                newParameterManager.setPath(cockpit_component.param_path);

                for (i = 0; i < cockpit_component.parameter.Length; i++)
                {
                    Parameters.Instance.AddValue(cockpit_component.param_path[i], 0, cockpit_component.dataType[i], cockpit_component.IO_Data[i]);
                }
            }

            foreach (Cockpit_Component_Json cockpit_component in components)// file.components)
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

                //for(i = 0; i < cockpit_component.parameter.Length; i++)
                //{
                //    Parameters.Instance.AddValue(cockpit_component.param_path[i], 0, cockpit_component.dataType[i], cockpit_component.IO_Data[i]);
                //}

                map.Add(component_instantiated, newParameterManager);
            }

            InputSequence.Instance.InitMap(map);

            Destroy(component);

            i = 0;

            int len = InputSequence.Instance.getSeqLength();

            //List<FeedbackGroup> sequenceFromFile = file.feedback_group;
            List<FeedbackGroup> sequenceFromFile = sequencesFromFile[0];

            //modificato
            //string subRoutineName = file.name;
            //List<Task> tasks = file.tasks;
            //InputSequence.Instance.InitSequenceObjects(sequenceFromFile, listaSequenze[0].name, listaSequenze[0].tasks);
            InputSequence.Instance.InitSequenceObjects(sequencesFromFile, listaSequenze);

            Container_Box_Json[] boxFromFile = new Container_Box_Json[len];

            //foreach (Container_Box_Json container_box in file.boxes)
            foreach (JsonSaveObject seq in listaSequenze)
            {
                int j = 0;

                foreach (Container_Box_Json container_box in boxes)
                {
                    while (seq.feedback_group[j].sequence != container_box.sequence)
                    {
                        if (seq.feedback_group[j].sequence != "End")
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
        path = String.Format("/ObjectsData/BRAKE_RELEASE.json");
        ////path = String.Format("/ObjectsData/CHECK_100_KNOTS.json");
        //path = String.Format("/ObjectsData/GEAR_UP.json");
        //path = String.Format("/ObjectsData/BRAKE_RELEASE.json");

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
                String line = null;
                StringBuilder fullFile = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    fullFile.Append(line).Append("\n\r");
                }
                file = JsonUtility.FromJson<JsonSaveObject>(fullFile.ToString());
                int first = path.LastIndexOf("/") + 1;
                file.name = path.Substring(first);
                int last = file.name.LastIndexOf(".json");
                file.name = file.name.Remove(last);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        string pathValues = String.Format("/ObjectsData/mapValues.json");
        using (StreamReader reader = new StreamReader(new FileStream(Application.dataPath + "/.." + pathValues, FileMode.Open)))
        {
            try
            {
                String line = null;
                StringBuilder fullFile = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    fullFile.Append(line).Append("\n\r");
                }
                mapValues = JsonUtility.FromJson<JsonSaveObject2>(fullFile.ToString());
                int a = 0;
                a++;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public void LoadUnity2()
    {
        string path = String.Format("/ObjectsData/sequences.json");
        String line = "";
        //modifica GIOVANNI
        //questo serve per acquisire tutte le informazioni di tutti i json, affinchè sia possibile decidere se effettuare i task in modo sequenziale o meno
        using (StreamReader reader = new StreamReader(new FileStream(Application.dataPath + "/.." + path, FileMode.Open)))
        {
            try
            {
                line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Equals("{") && !line.Equals("}"))
                    {
                        //analizzo la sequence i-esima
                        using (StreamReader reader2 = new StreamReader(new FileStream(Application.dataPath + "/../ObjectsData/" + line.Replace("\t\"", "").Replace("\",", "").Replace("\"", ""), FileMode.Open)))
                        {
                            try
                            {
                                String line2 = null;
                                StringBuilder fullFile = new StringBuilder();
                                while ((line2 = reader2.ReadLine()) != null)
                                {
                                    fullFile.Append(line2).Append("\n\r");
                                }
                                listaSequenze.Add(JsonUtility.FromJson<JsonSaveObject>(fullFile.ToString()));
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        string pathValues = String.Format("/ObjectsData/mapValues.json");
        using (StreamReader reader = new StreamReader(new FileStream(Application.dataPath + "/.." + pathValues, FileMode.Open)))
        {
            try
            {
                line = null;
                StringBuilder fullFile = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    fullFile.Append(line).Append("\n\r");
                }
                mapValues = JsonUtility.FromJson<JsonSaveObject2>(fullFile.ToString());
                int a = 0;
                a++;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
#endif
}