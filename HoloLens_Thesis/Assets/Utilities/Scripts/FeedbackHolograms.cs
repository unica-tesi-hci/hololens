using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackHolograms : MonoBehaviour {

    [Tooltip("Model to display when the selected object is in the right state.")]
    public GameObject CheckMarkObject;

    [Tooltip("Model to display when it is required to wait.")]
    public GameObject WaitingObject;

    [Tooltip("Color to shade the object currently used.")]
    public Color ObjectColor = Color.green;

    [Tooltip("Allowable percentage inside the holographic frame to continue to show a Feedback Hologram.")]
    [Range(-0.3f, 0.3f)]
    public float TitleSafeFactor = 0.0f;

    [Tooltip("The ContainerBox's script.")]
    public ContainerBox contBox;

    //The object currently instantiated.
    private GameObject CurrentObject;

    // The objects that the user has to interact.
    private GameObject[] objectIndicated = null;

    // The current object instances placed near the objectsIndicated during the process.
    private GameObject[] currentObject = null;

    // The MeshRenderer for the current objects placed near the objectsIndicated during the process.
    private MeshRenderer[] currentRenderer;

    // Check if at least one object is visible.
    private bool isObjectVisible;
    // Check if the single current object is visible.
    private bool visible;
    //Check during the process if all the objectIndicated are in the correct state.
    private bool areAllObjectsCorrect;
    //The minimum/maximum coordinate of the objects that have to be encapsulated by the bounding box;
    float minX, minY, minZ, maxX, maxY, maxZ;
    //The position and the old position of the advice text.
    Vector3 pos, oldPos;
    private Tagalong textTagAlong;
	int border = 0;
    bool blockUpdateText = false;

    // Use this for initialization
    void Awake()
    {
        if (CheckMarkObject == null && WaitingObject == null)
        {
            return;
        }

        InstantiateCheckMark();

        textTagAlong = GameObject.FindWithTag("AdviceBackground").GetComponent<Tagalong>();
    }

    public void OnDestroy()
    {
        GameObject.DestroyImmediate(CheckMarkObject);
        GameObject.DestroyImmediate(WaitingObject);
    }

    public void InstantiateCheckMark()
    {
        if (CurrentObject == CheckMarkObject)
        {
            return;
        }
        else
        {
            CurrentObject = null;
        }

        MeshRenderer objectRenderer = CheckMarkObject.GetComponentInChildren<MeshRenderer>();

        if (objectRenderer == null)
        {
            // The Direction Indicator must have a MeshRenderer so it can give visual feedback to the user which way to look.
            // Add one if there wasn't one.
            objectRenderer = CheckMarkObject.AddComponent<MeshRenderer>();
        }

        // Start with the indicator disabled.
        objectRenderer.enabled = false;

        // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
        foreach (Collider collider in CheckMarkObject.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in CheckMarkObject.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }

#if !UNITY_EDITOR
        Material objectMaterial = objectRenderer.material;
        objectMaterial.color = ObjectColor;
        objectMaterial.SetColor("_TintColor", ObjectColor);
#endif

        CurrentObject = CheckMarkObject;
    }

    public void InstantiateWaitingObject()
    {
        if (CurrentObject == WaitingObject)
        {
            return;
        }
        else
        {
            CurrentObject = null;
        }

        // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
        foreach (Collider collider in WaitingObject.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in WaitingObject.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }

        CurrentObject = WaitingObject;
    }

    public void DestroyObjectsIndicated()
    {
        int i;
        objectIndicated = null;

        if (currentObject != null)
        {
            for (i = 0; i < currentObject.Length; i++)
            {
                Destroy(currentObject[i]);
                currentObject[i] = null;
            }
        }
    }
	
	public GameObject[] GetFromObjects(GameObject[] objects){
		int i;
		
		GameObject[] tmpCurrentObject = null;
        MeshRenderer tmpRenderer = null;

        if (objects.Length <= 0 || objects == null)
        {
            return tmpCurrentObject;
        }
        else
        {
            tmpCurrentObject = new GameObject[objects.Length];
        }
		
		for (i = 0; i < objects.Length; i++)
		{
			tmpCurrentObject[i] = CreateFeedbackHologram(objects[i]);
            tmpRenderer = tmpCurrentObject[i].GetComponent<MeshRenderer>();

            if (tmpRenderer == null)
            {
                tmpRenderer = tmpCurrentObject[i].AddComponent<MeshRenderer>();
            }

            tmpRenderer.enabled = true;
        }
		
		return tmpCurrentObject;
	}

    public void SetObjectsIndicated(GameObject[] objects)
    {
        if (objects != null)
        {
            int i;
            minX = Mathf.Infinity;
            minY = Mathf.Infinity;
            minZ = Mathf.Infinity;
            maxX = Mathf.NegativeInfinity;
            maxY = Mathf.NegativeInfinity;
            maxZ = Mathf.NegativeInfinity;

            objectIndicated = new GameObject[objects.Length];
            currentObject = new GameObject[objects.Length];
            if (CurrentObject != WaitingObject)
            {
                currentRenderer = new MeshRenderer[objects.Length];
            }

            for (i = 0; i < objects.Length; i++)
            {
                objectIndicated[i] = objects[i];

                currentObject[i] = CreateFeedbackHologram(objectIndicated[i]);

                if (CurrentObject != WaitingObject)
                {
                    currentObject[i].transform.rotation = objectIndicated[i].transform.rotation;
                    currentObject[i].transform.Rotate(100, -90, 90);

                    currentRenderer[i] = currentObject[i].GetComponentInChildren<MeshRenderer>();
                    currentRenderer[i].enabled = false;
                }
            }
			
			if(contBox.isContainerBoxExisting()){
				GameObject[] box = new GameObject[4];
				box = contBox.getContainerBox();
				
				minX = box[0].transform.position.x - box[0].transform.right.x * (box[0].transform.localScale.x / 2 + 0.08f);
				maxX = box[2].transform.position.x + box[2].transform.right.x * (box[2].transform.localScale.x / 2 + 0.08f);
				minY = box[3].transform.position.y - box[3].transform.up.y * (box[3].transform.localScale.y / 2);
				maxY = box[1].transform.position.y + box[1].transform.up.y * (box[1].transform.localScale.y / 2);
				minZ = box[0].transform.position.z - box[0].transform.forward.z * (box[0].transform.localScale.z / 2 + 0.05f);
				maxZ = box[0].transform.position.z + box[0].transform.forward.z * (box[0].transform.localScale.z / 2 + 0.05f);
				
			}else
			{
				for(i = 0; i < objects.Length; i++){
					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.min.x < minX)
					{
						minX = objectIndicated[i].GetComponents<Collider>()[0].bounds.min.x - objectIndicated[i].transform.right.x * 0.1f;
					}

					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.max.x > maxX)
					{
						maxX = objectIndicated[i].GetComponents<Collider>()[0].bounds.max.x + objectIndicated[i].transform.right.x * 0.1f;
					}

					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.min.y < minY)
					{
						minY = objectIndicated[i].GetComponents<Collider>()[0].bounds.min.y - objectIndicated[i].transform.up.y * 0.1f;
					}

					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.max.y > maxY)
					{
						maxY = objectIndicated[i].GetComponents<Collider>()[0].bounds.max.y + objectIndicated[i].transform.up.y * 0.1f;
					}

					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.min.z < minZ)
					{
						minZ = objectIndicated[i].GetComponents<Collider>()[0].bounds.min.z - objectIndicated[i].transform.forward.z * 0.05f;
					}

					if (objectIndicated[i].GetComponents<Collider>()[0].bounds.max.z > maxZ)
					{
						maxZ = objectIndicated[i].GetComponents<Collider>()[0].bounds.max.z + objectIndicated[i].transform.forward.z * 0.05f;
					}
				}
			}
        }
        else
        {
            objectIndicated = null;
            TextManager.Instance.enableAdviceText(false);
        }
    }

    private GameObject CreateFeedbackHologram(GameObject obj)
    {
        Vector3 position;
        GetHologramPosition(obj, out position);

        GameObject hologram = Instantiate(CurrentObject);
        hologram.transform.position = position;

        return hologram;
    }

    public GameObject[] getCurrentObject()
    {
        return currentObject;
    }

    public void InitializeTextPosition()
    {
        if(objectIndicated != null)
        {
            TextManager.Instance.enableAdviceText(false);
            //Move the advice text near the right of the object (or container box having the objects indicated).
            if (InputSequence.Instance.isOnOhPanel())
            {
                pos.Set(maxX, (minY + maxY) / 2 - 0.05f, minZ);
            }
            else
            {
                pos.Set(maxX, (minY + maxY) / 2 + 0.05f, minZ);
            }
            textTagAlong.enabled = false;
			border = 1;
			TextManager.Instance.updateAdviceTextPosition(pos, border);
        }else if(InputSequence.Instance.getSeq() != 22 || InputSequence.Instance.getSeq() != 23 || InputSequence.Instance.getSeq() != 25 || InputSequence.Instance.getSeq() != 27 ||InputSequence.Instance.getSeq() != 29)
        {
            pos = Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0.5f, 2.0f));
            TextManager.Instance.enableAdviceText(true);
            TextManager.Instance.updateAdviceTextPosition(pos);
            textTagAlong.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentObject == null || objectIndicated == null)
        {
            return;
        }

        areAllObjectsCorrect = true;
        isObjectVisible = false;
        oldPos = pos;

        for (int i = 0; i < objectIndicated.Length; i++)
        {
            if (CurrentObject == CheckMarkObject)
            {
                currentRenderer[i].enabled = InputSequence.Instance.isObjectInCorrectState[i];
            }

            visible = IsTargetVisible(objectIndicated[i]);
            isObjectVisible = (isObjectVisible || visible || contBox.isContainerBoxVisible());
            visible = false;

            areAllObjectsCorrect = (areAllObjectsCorrect && InputSequence.Instance.isObjectInCorrectState[i]);
        }

        if (isObjectVisible)
        {
            RaycastHit hit;
            bool isTextOccludingObject = false;
            Vector3 camToObjectDirection;
            int occlusionPosition = 0;

            for (int i = 0; i < objectIndicated.Length; i++)
            {
                if(IsTargetVisible(objectIndicated[i]) && !InputSequence.Instance.isObjectInCorrectState[i])
                {
                    camToObjectDirection = Camera.main.transform.position - objectIndicated[i].transform.position;
                    if(Physics.Raycast(objectIndicated[i].transform.position,
                           camToObjectDirection,
                           out hit))
                    {
                        if (hit.collider.tag != "MainCamera" && hit.collider.tag == "AdviceBackground")
                        {
                            isTextOccludingObject = true;
                            occlusionPosition = border;
                            break;
                        }
                    }
                }
            }
            
            if (blockUpdateText)
            {
                return;
            }

            if (isTextOccludingObject || !TextManager.Instance.IsTextVisible(oldPos, 0, 0))
            {
                //Check if the text is fully visible from the right border of the camera.
                if (TextManager.Instance.IsTextVisible(new Vector3(maxX, (minY + maxY) / 2, minZ), 0, 1) && occlusionPosition != 1)
                {
                    //Move the advice text near the right of the object (or container box having the objects indicated).
                    if (InputSequence.Instance.isOnOhPanel())
                    {
                        pos.Set(maxX, (minY + maxY) / 2 - 0.05f, minZ);
                    }
                    else
                    {
                        pos.Set(maxX, (minY + maxY) / 2 + 0.05f, minZ);
                    }
                    border = 1;
                }//Check if the text is fully visible from the left border of the camera.
                else if (TextManager.Instance.IsTextVisible(new Vector3(minX, (minY + maxY) / 2, minZ), 0, 2) && occlusionPosition != 2)
                {
                    //Move the advice text near the left of the object (or container box having the objects indicated).
                    if (InputSequence.Instance.isOnOhPanel())
                    {
                        pos.Set(minX, (minY + maxY) / 2 - 0.05f, minZ - 0.01f);
                    }
                    else
                    {
                        pos.Set(minX, (minY + maxY) / 2 + 0.05f, minZ - 0.01f);
                    }
                    border = 2;
                }//Probably at this point the text should be visible when placed on the bottom of the object (or container box).
                else
                {
                    //Move the advice text near the bottom of the object (or container box having the objects indicated).
                    pos.Set((minX + maxX) / 2, minY - contBox.getContainerBox()[3].transform.up.y * 0.1f, minZ);
                    border = 3;
                }
                
                TextManager.Instance.updateAdviceTextPosition(pos, border);
                blockUpdateText = true;
                Invoke("unlockUpdateText", 1);
            }
            else
            {
                pos = oldPos;
                TextManager.Instance.updateAdviceTextPosition(pos, border);
            }
        }

        //Enable the advice text using as conditions the XOR between isObjectVisible and the correctness of all the objectIndicated OR the visibility of the text itself.
        TextManager.Instance.enableAdviceText((isObjectVisible ^ areAllObjectsCorrect) || TextManager.Instance.IsTextVisible(pos, 0, 0));
    }

    private void unlockUpdateText()
    {
        blockUpdateText = false;
        CancelInvoke();
    }

    private bool IsTargetVisible(GameObject obj)
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(obj.transform.position);
        return (targetViewportPosition.x > TitleSafeFactor && targetViewportPosition.x < 1 - TitleSafeFactor &&
            targetViewportPosition.y > TitleSafeFactor && targetViewportPosition.y < 1 - TitleSafeFactor &&
            targetViewportPosition.z > 0);
    }

    private void GetHologramPosition(GameObject obj, out Vector3 position)
    {
        // Save the cursor transform position in a variable.
        Vector3 origin = obj.transform.position;

        if(CurrentObject == WaitingObject)
        {
            position = origin + obj.transform.up * (obj.transform.localScale.y / 2 + 0.15f) - obj.transform.forward * 0.2f;
        }
        else
        {
            position = origin + obj.transform.up * (obj.transform.localScale.y / 2 + 0.02f) - obj.transform.forward * 0.01f;
        }
    }
}
