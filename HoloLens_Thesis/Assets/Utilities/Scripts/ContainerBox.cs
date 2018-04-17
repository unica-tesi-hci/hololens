using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContainerBox : MonoBehaviour
{

    [Tooltip("Model to display in order to denote a visible object the user has to interact.")]
    private GameObject BoxObject;

    [Tooltip("Color to shade the container box marker.")]
    public Color BoxColor = Color.yellow;

    [Tooltip("The material used for the container box.")]
    public Material boxMaterial;

    // The objects encapsulated by the Container Box.
    private GameObject[] objectIndicated = null;

    // The box instantiated when the respective objects are visible.
    private GameObject[] box = null;
    // Cache the MeshRenderer for the container box since it will be enabled and disabled frequently.
    private MeshRenderer[] boxRenderer;
	//The list of Container Box stored on file.
	private Vector3[][] boxFromFile = null;

    //Check during the process if all the objectIndicated are in the correct state.
    private bool areAllObjectsCorrect;
    //The minimum/maximum coordinate of the objects that have to be encapsulated by the container box;
    float minX, minY, minZ, maxX, maxY, maxZ;

    // Use this for initialization
    void Awake()
    {
        // Instantiate the container box.
        BoxObject = InstantiateBox();
    }

    public void OnDestroy()
    {
        GameObject.Destroy(BoxObject);
    }

    private GameObject InstantiateBox()
    {
        GameObject boxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer boxRenderer = boxObj.GetComponent<MeshRenderer>();

        if (boxRenderer == null)
        {
            // The Container Box must have a MeshRenderer so it can give visual feedback to the user which way to look.
            // Add one if there wasn't one.
            boxRenderer = boxObj.AddComponent<MeshRenderer>();
        }

        // Start with the indicator disabled.
        boxRenderer.enabled = false;

        // Remove any colliders and rigidbodies so the mark do not interfere with Unity's physics system.
        foreach (Collider collider in boxObj.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in boxObj.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }

        boxObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        return boxObj;
    }

    public void setContainerBoxColor(Color color)
    {
        color -= new Color(0, 0, 0, 0.5f);
#if !UNITY_EDITOR
        Material material;

        material = boxMaterial;
        material.color = color;
        material.SetColor("_TintColor", color);

        for(int i = 0; i < boxRenderer.Length; i++){
            boxRenderer[i].material = material;
        }
#endif
    }

    public void DestroyObjectsIndicated()
    {
        objectIndicated = null;
        int i;

        if (box != null)
        {
            for (i = 0; i < box.Length; i++)
            {
                Destroy(box[i]);
            }

            box = null;
            boxRenderer = null;
        }
    }
	
	public GameObject[] GetFromObjects(GameObject[] objects, int? _seq = null){
		if(objects == null)
		{
			return objects;
		}
		
		int i;
        float minX_scale = 0;
        float maxX_scale = 0;
        float minY_scale = 0;
        float maxY_scale = 0;
        float minZ_scale = 0;
        float maxZ_scale = 0;
        minX = Mathf.Infinity;
        minY = Mathf.Infinity;
        minZ = Mathf.Infinity;
        maxX = Mathf.NegativeInfinity;
        maxY = Mathf.NegativeInfinity;
        maxZ = Mathf.NegativeInfinity;
			
		for (i = 0; i < objects.Length; i++)
		{
			if (objects[i].transform.position.x < minX)
			{
				minX = objects[i].transform.position.x;
				minX_scale = objects[i].transform.localScale.x / 2;
			}

			if (objects[i].transform.position.x > maxX)
			{
				maxX = objects[i].transform.position.x;
				maxX_scale = objects[i].transform.localScale.x / 2;
			}

			if (objects[i].transform.position.y < minY)
			{
				minY = objects[i].transform.position.y;
				minY_scale = objects[i].transform.localScale.y / 2;
			}

			if (objects[i].transform.position.y > maxY)
			{
				maxY = objects[i].transform.position.y;
				maxY_scale = objects[i].transform.localScale.y / 2;
			}

			if (objects[i].transform.position.z < minZ)
			{
				minZ = objects[i].transform.position.z;
				minZ_scale = objects[i].transform.localScale.z / 2;
			}

			if (objects[i].transform.position.z > maxZ)
			{
				maxZ = objects[i].transform.position.z;
				maxZ_scale = objects[i].transform.localScale.z / 2;
			}
		}
		
		GameObject[] tmpBox;
		
		if(boxFromFile != null && _seq != null){
			tmpBox = CreateBox();
			
			int seq = (int)_seq;
			Vector3[] properties = boxFromFile[seq];
			
			tmpBox[0].transform.position = properties[0];
			tmpBox[0].transform.rotation = Quaternion.Euler(properties[1]);
			tmpBox[0].transform.localScale = properties[2];
			tmpBox[1].transform.position = properties[3];
			tmpBox[1].transform.rotation = Quaternion.Euler(properties[4]);
			tmpBox[1].transform.localScale = properties[5];
			tmpBox[2].transform.position = properties[6];
			tmpBox[2].transform.rotation = Quaternion.Euler(properties[7]);
			tmpBox[2].transform.localScale = properties[8];
			tmpBox[3].transform.position = properties[9];
			tmpBox[3].transform.rotation = Quaternion.Euler(properties[10]);
			tmpBox[3].transform.localScale = properties[11];
			
			minX = minX - minX_scale;
			maxX = maxX + maxX_scale;
			minY = minY - minY_scale;
			maxY = maxY + maxY_scale;
			minZ -= minZ_scale;
			maxZ += maxZ_scale;
			
			for(i = 0; i < tmpBox.Length; i++){
				tmpBox[i].GetComponent<MeshRenderer>().enabled = true;
			}
			
		}else{
			//Put some default offset, so that the box is not too attached to the objects.
			minX = minX - minX_scale - 0.03f;
			maxX = maxX + maxX_scale + 0.03f;
			minY = minY - minY_scale - 0.075f;
			maxY = maxY + maxY_scale + 0.075f;
			minZ -= minZ_scale;
			maxZ += maxZ_scale;
			
			tmpBox = CreateBox();
			SetBoxPosition(tmpBox);
			SetBoxRotation(tmpBox, objects);
			
			for(i = 0; i < tmpBox.Length; i++){
				tmpBox[i].transform.localScale = new Vector3(tmpBox[i].transform.localScale.x, tmpBox[i].transform.localScale.y, BoxObject.transform.localScale.z);
				tmpBox[i].GetComponent<MeshRenderer>().enabled = true;
			}
		}
		
		return tmpBox;
	}

    public void SetObjectsIndicated(GameObject[] objects, bool showBox)
    {
        if (objects != null)
        {
            int i;
            float minX_scale = 0;
            float maxX_scale = 0;
            float minY_scale = 0;
            float maxY_scale = 0;
            float minZ_scale = 0;
            float maxZ_scale = 0;
            minX = Mathf.Infinity;
            minY = Mathf.Infinity;
            minZ = Mathf.Infinity;
            maxX = Mathf.NegativeInfinity;
            maxY = Mathf.NegativeInfinity;
            maxZ = Mathf.NegativeInfinity;
            objectIndicated = new GameObject[objects.Length];

            for (i = 0; i < objects.Length; i++)
            {
                objectIndicated[i] = objects[i];
                

                if (objectIndicated[i].transform.position.x < minX)
                {
                    minX = objectIndicated[i].transform.position.x;
                    minX_scale = objectIndicated[i].transform.localScale.x / 2;
                }

                if (objectIndicated[i].transform.position.x > maxX)
                {
                    maxX = objectIndicated[i].transform.position.x;
                    maxX_scale = objectIndicated[i].transform.localScale.x / 2;
                }

                if (objectIndicated[i].transform.position.y < minY)
                {
                    minY = objectIndicated[i].transform.position.y;
                    minY_scale = objectIndicated[i].transform.localScale.y / 2;
                }

                if (objectIndicated[i].transform.position.y > maxY)
                {
                    maxY = objectIndicated[i].transform.position.y;
                    maxY_scale = objectIndicated[i].transform.localScale.y / 2;
                }

                if (objectIndicated[i].transform.position.z < minZ)
                {
                    minZ = objectIndicated[i].transform.position.z;
                    minZ_scale = objectIndicated[i].transform.localScale.z / 2;
                }

                if (objectIndicated[i].transform.position.z > maxZ)
                {
                    maxZ = objectIndicated[i].transform.position.z;
                    maxZ_scale = objectIndicated[i].transform.localScale.z / 2;
                }
			}
			
			if(boxFromFile != null){
				box = CreateBox();
				
				Vector3[] properties = boxFromFile[InputSequence.Instance.getSeq()];
				
				box[0].transform.position = properties[0];
				box[0].transform.rotation = Quaternion.Euler(properties[1]);
				box[0].transform.localScale = properties[2];
				box[1].transform.position = properties[3];
				box[1].transform.rotation = Quaternion.Euler(properties[4]);
				box[1].transform.localScale = properties[5];
				box[2].transform.position = properties[6];
				box[2].transform.rotation = Quaternion.Euler(properties[7]);
				box[2].transform.localScale = properties[8];
				box[3].transform.position = properties[9];
				box[3].transform.rotation = Quaternion.Euler(properties[10]);
				box[3].transform.localScale = properties[11];
				
				minX = minX - minX_scale;
				maxX = maxX + maxX_scale;
				minY = minY - minY_scale;
				maxY = maxY + maxY_scale;
				minZ -= minZ_scale;
				maxZ += maxZ_scale;
				boxRenderer = new MeshRenderer[4];
				
				for(i = 0; i < box.Length; i++){
					boxRenderer[i] = box[i].GetComponent<MeshRenderer>();
					boxRenderer[i].enabled = showBox;
				}
			}else{
				//Put some default offset, so that the box is not too attached to the objects.
				minX = minX - minX_scale - 0.03f;
				maxX = maxX + maxX_scale + 0.03f;
				minY = minY - minY_scale - 0.075f;
				maxY = maxY + maxY_scale + 0.075f;
				minZ -= minZ_scale;
				maxZ += maxZ_scale;

				box = CreateBox();
				SetBoxPosition();
				SetBoxRotation();
				boxRenderer = new MeshRenderer[4];

				for (i = 0; i < box.Length; i++)
				{
					box[i].transform.localScale = new Vector3(box[i].transform.localScale.x, box[i].transform.localScale.y, BoxObject.transform.localScale.z);

					boxRenderer[i] = box[i].GetComponent<MeshRenderer>();
					boxRenderer[i].enabled = showBox;
				}
			}

            setContainerBoxColor(BoxColor);
        }
        else
        {
            objectIndicated = null;
        }
    }

    public void getCurrentMinMax(out float minimumX, out float maximumX, out float minimumY, out float maximumY, out float minimumZ, out float maximumZ)
    {
        minimumX = minX;
        maximumX = maxX;
        minimumY = minY;
        maximumY = maxY;
        minimumZ = minZ;
        maximumZ = maxZ;
    }

    private GameObject[] CreateBox()
    {
        GameObject[] boxObj = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            boxObj[i] = Instantiate(BoxObject);
        }

        boxObj[0].transform.localScale = new Vector3(BoxObject.transform.localScale.x, maxY - minY + (1 * 0.02f), maxZ - minZ);
        boxObj[1].transform.localScale = new Vector3(maxX - minX + (1 * 0.02f), BoxObject.transform.localScale.y, maxZ - minZ);
        boxObj[2].transform.localScale = boxObj[0].transform.localScale;
        boxObj[3].transform.localScale = boxObj[1].transform.localScale;

        return boxObj;
    }

    public bool isContainerBoxVisible()
    {
        if (isContainerBoxExisting())
        {
            for (int i = 0; i < 4; i++)
            {
                if (boxRenderer[i].enabled && IsTargetVisible(box[i]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool isContainerBoxExisting()
    {
        return box != null;
    }
	
	public GameObject[] getContainerBox()
    {
        return box;
    }
	
	public void InitializeBoxFromFile(Vector3[][] boxFile = null)
	{
		//If there is a file that can initialize boxFromFile then use it, otherwise set the default properties.
		if(boxFile != null)
		{
			boxFromFile = boxFile;
		}else
		{
			if(BoxFromFileExists())
			{
				return;
			}
			
			int len = InputSequence.Instance.getSeqLength();
			Vector3[][] tmpBoxFromFile = new Vector3[len][];
			GameObject[] tmpBox;
			
			for(int i = 0; i < len; i++)
			{
				tmpBox = GetFromObjects(InputSequence.Instance.getObjectsFromSequence(i));
				
				if(tmpBox != null)
				{
					tmpBoxFromFile[i] = new Vector3[]{tmpBox[0].transform.position, tmpBox[0].transform.rotation.eulerAngles, tmpBox[0].transform.localScale, tmpBox[1].transform.position, tmpBox[1].transform.rotation.eulerAngles, tmpBox[1].transform.localScale, tmpBox[2].transform.position, tmpBox[2].transform.rotation.eulerAngles, tmpBox[2].transform.localScale, tmpBox[3].transform.position, tmpBox[3].transform.rotation.eulerAngles, tmpBox[3].transform.localScale};
					
					Destroy(tmpBox[0]);
					Destroy(tmpBox[1]);
					Destroy(tmpBox[2]);
					Destroy(tmpBox[3]);
				}else
				{
					tmpBoxFromFile[i] = new Vector3[]{};
				}
			}
			
			boxFromFile = tmpBoxFromFile;
		}
	}
	
	public void UpdateBoxFromFile(GameObject[] obj, int seq){
		GameObject[] fileBox;
		Vector3[] boxData;
		
		if(!BoxFromFileExists()){
			InitializeBoxFromFile();
		}
		
		fileBox = obj;
		boxData = new Vector3[]{fileBox[0].transform.position, fileBox[0].transform.rotation.eulerAngles, fileBox[0].transform.localScale, fileBox[1].transform.position, fileBox[1].transform.rotation.eulerAngles, fileBox[1].transform.localScale, fileBox[2].transform.position, fileBox[2].transform.rotation.eulerAngles, fileBox[2].transform.localScale, fileBox[3].transform.position, fileBox[3].transform.rotation.eulerAngles, fileBox[3].transform.localScale};
		boxFromFile[seq] = boxData;
	}
	
	public void Update_BFF_From_Modify(GameObject obj)
	{
		GameObject[] objects;
		GameObject[] fileBox = null;
		Vector3[] boxData;

        if (!BoxFromFileExists())
        {
            InitializeBoxFromFile();
        }

        for (int i = 0; i < InputSequence.Instance.getSeqLength(); i++)
		{
			objects = InputSequence.Instance.getObjectsFromSequence(i);
			
			if(objects != null && objects.Contains(obj))
			{
				fileBox = GetFromObjects(objects);
				boxData = new Vector3[]{fileBox[0].transform.position, fileBox[0].transform.rotation.eulerAngles, fileBox[0].transform.localScale, fileBox[1].transform.position, fileBox[1].transform.rotation.eulerAngles, fileBox[1].transform.localScale, fileBox[2].transform.position, fileBox[2].transform.rotation.eulerAngles, fileBox[2].transform.localScale, fileBox[3].transform.position, fileBox[3].transform.rotation.eulerAngles, fileBox[3].transform.localScale};
				boxFromFile[i] = boxData;

                for (int j = 0; j < fileBox.Length; j++)
                {
                    Destroy(fileBox[j]);
                }
            }
		}
    }
	
	public bool BoxFromFileExists()
	{
		return boxFromFile != null;
	}
	
	public Vector3[][] getBoxFromFile()
	{
		return boxFromFile;
	}

    public void Update()
    {
        if (BoxObject == null || objectIndicated == null)
        {
            return;
        }

        areAllObjectsCorrect = true;

        for (int i = 0; i < objectIndicated.Length; i++)
        {
            areAllObjectsCorrect = (areAllObjectsCorrect && InputSequence.Instance.isObjectInCorrectState[i]);
        }

        if (areAllObjectsCorrect)
        {
            setContainerBoxColor(Color.green);
        }
    }

    //Check if the target is visible to the user, that is, if the target is inside the screen of the camera.
    private bool IsTargetVisible(GameObject obj)
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(obj.transform.position);
        return (targetViewportPosition.x > 0 && targetViewportPosition.x < 1 - 0 &&
            targetViewportPosition.y > 0 && targetViewportPosition.y < 1 - 0 &&
            targetViewportPosition.z > 0);
    }

    private void SetBoxPosition()
    {
        Vector3 boxLeft = new Vector3(minX, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 boxRight = new Vector3(maxX, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 boxHigh = new Vector3((minX + maxX) / 2, maxY, (minZ + maxZ) / 2);
        Vector3 boxLow = new Vector3((minX + maxX) / 2, minY, (minZ + maxZ) / 2);

        box[0].transform.position = boxLeft;
        box[1].transform.position = boxHigh;
        box[2].transform.position = boxRight;
        box[3].transform.position = boxLow;
    }

    private void SetBoxRotation()
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Quaternion quaternion;

        for (int i = 0; i < objectIndicated.Length; i++)
        {
            quaternion = objectIndicated[i].transform.rotation;

            x += quaternion.eulerAngles.x;
            y += quaternion.eulerAngles.y;
            z += quaternion.eulerAngles.z;
        }

        x /= (float)(objectIndicated.Length);
        y /= (float)(objectIndicated.Length);
        z /= (float)(objectIndicated.Length);
        Vector3 averageEuler = new Vector3(x, y, z);
        quaternion = Quaternion.Euler(averageEuler);

        GameObject tmp = new GameObject();
        tmp.transform.position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        for (int i = 0; i < box.Length; i++)
        {
            box[i].transform.SetParent(tmp.transform);
        }

        tmp.transform.localRotation = quaternion;

        for (int i = 0; i < box.Length; i++)
        {
            box[i].transform.parent = null;
        }

        Destroy(tmp);
    }
	
	private GameObject[] SetBoxPosition(GameObject[] tmpObj)
    {
        Vector3 boxLeft = new Vector3(minX, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 boxRight = new Vector3(maxX, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 boxHigh = new Vector3((minX + maxX) / 2, maxY, (minZ + maxZ) / 2);
        Vector3 boxLow = new Vector3((minX + maxX) / 2, minY, (minZ + maxZ) / 2);

        tmpObj[0].transform.position = boxLeft;
        tmpObj[1].transform.position = boxHigh;
        tmpObj[2].transform.position = boxRight;
        tmpObj[3].transform.position = boxLow;
		
		return tmpObj;
    }

    private GameObject[] SetBoxRotation(GameObject[] tmpObj, GameObject[] objects)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Quaternion quaternion;

        for (int i = 0; i < objects.Length; i++)
        {
            quaternion = objects[i].transform.rotation;

            x += quaternion.eulerAngles.x;
            y += quaternion.eulerAngles.y;
            z += quaternion.eulerAngles.z;
        }

        x /= (float)(objects.Length);
        y /= (float)(objects.Length);
        z /= (float)(objects.Length);
        Vector3 averageEuler = new Vector3(x, y, z);
        quaternion = Quaternion.Euler(averageEuler);

        GameObject tmp = new GameObject();
        tmp.transform.position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        for (int i = 0; i < tmpObj.Length; i++)
        {
            tmpObj[i].transform.SetParent(tmp.transform);
        }

        tmp.transform.localRotation = quaternion;

        for (int i = 0; i < tmpObj.Length; i++)
        {
            tmpObj[i].transform.parent = null;
        }

        Destroy(tmp);
		
		return tmpObj;
    }
}
