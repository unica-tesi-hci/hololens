using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldToRotate : MonoBehaviour, IManipulationHandler {

    [Tooltip("The material displayed on the object during the placement.")]
    public Material PlacementMaterial;

    private float RotationSensitivity = 1.0f;
    private Material originalMaterial = null;
	private GameObject[] selectedObject;
	
	private MarkerRing markerRing;
    private ContainerBox contBox;
	private FeedbackHolograms feed;
	
	private GameObject[] rings;
	private GameObject[] box;
	private GameObject[] feedbackHolograms;

    // Use this for initialization
    void Start () {
		markerRing = GameObject.FindWithTag("MarkerRing").GetComponent<MarkerRing>();
        contBox = GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>();
        feed = GameObject.FindWithTag("FeedbackHolograms").GetComponent<FeedbackHolograms>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isRotating)
        {
#if !UNITY_EDITOR
            GameObject obj = gameObject;
            originalMaterial = obj.GetComponent<Renderer>().material;
            obj.GetComponent<Renderer>().material = PlacementMaterial;
#endif

            selectedObject = new GameObject[] { gameObject };
			setIndicationObjects(selectedObject);
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isRotating)
        {
            float cameraLocalYRotation = Camera.main.transform.localRotation.eulerAngles.y;
			
			if(cameraLocalYRotation > 270 || cameraLocalYRotation < 90){
				RotationSensitivity = -1.0f;
			}else{
				RotationSensitivity = 1.0f;
			}
			
			Vector3 rotVector = new Vector3(eventData.CumulativeDelta.y * -RotationSensitivity, eventData.CumulativeDelta.x * RotationSensitivity);

            gameObject.transform.Rotate(rotVector, Space.World);
			DestroyAllObjects();
			setIndicationObjects(selectedObject);
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isRotating)
        {
			DestroyAllObjects();
			
#if !UNITY_EDITOR
            GameObject obj = gameObject;
            obj.GetComponent<Renderer>().material = originalMaterial;
            originalMaterial = null;
#endif

            contBox.Update_BFF_From_Modify(selectedObject[0]);
        }
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isRotating)
        {
			DestroyAllObjects();
			
#if !UNITY_EDITOR
            GameObject obj = gameObject;
            obj.GetComponent<Renderer>().material = originalMaterial;
            originalMaterial = null;
#endif

            contBox.Update_BFF_From_Modify(selectedObject[0]);
        }
    }
	
	public void DestroyAllObjects(){
		int i;
		
        if(rings != null)
        {
            for (i = 0; i < rings.Length; i++)
            {
                Destroy(rings[i]);
            }
            rings = null;
        }

        if (box != null)
        {
            for (i = 0; i < box.Length; i++)
            {
                Destroy(box[i]);
            }
            box = null;
        }

        if (feedbackHolograms != null)
        {
            for (i = 0; i < feedbackHolograms.Length; i++)
            {
                Destroy(feedbackHolograms[i]);
            }
            feedbackHolograms = null;
        }
	}
	
	private void setIndicationObjects(GameObject[] obj)
    {
		int i;
		
		if(rings != null){
			for(i = 0; i < rings.Length; i++){
				Destroy(rings[i]);
			}
		}
		
		if(box != null){
			for(i = 0; i < box.Length; i++){
				Destroy(box[i]);
			}
		}
		
		if(feedbackHolograms != null){
			for(i = 0; i < feedbackHolograms.Length; i++){
				Destroy(feedbackHolograms[i]);
			}
		}
		
        rings = markerRing.GetFromObjects(obj);

        box = contBox.GetFromObjects(obj, new bool[] { true });

        feedbackHolograms = feed.GetFromObjects(obj);
    }
}
