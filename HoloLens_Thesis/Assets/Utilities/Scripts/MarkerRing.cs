using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerRing : MonoBehaviour
{

    [Tooltip("Model to display in order to denote a visible object the user has to interact.")]
    public GameObject MarkerObject;

    [Tooltip("Color to shade the marker sign.")]
    public Color MarkerColor = Color.red - new Color(0, 0, 0, 0.8f);

    [Tooltip("Allowable percentage inside the holographic frame to continue to show a marker ring.")]
    [Range(-0.3f, 0.3f)]
    public float TitleSafeFactor = 0.0f;

    // The objects indicated by the Marker Ring.
    private GameObject[] objectIndicated = null;

    // The markers instantiated when the respective object is visible.
    private GameObject[] marker = null;

    // Cache the MeshRenderer for the marker rings since they will be enabled and disabled frequently.
    private MeshRenderer[] markerRenderer;

    // Use this for initialization
    void Awake()
    {
        if (MarkerObject == null)
        {
            return;
        }

        // Instantiate the marker ring.
        MarkerObject = InstantiateMarker(MarkerObject);
    }

    public void OnDestroy()
    {
        GameObject.DestroyImmediate(MarkerObject);
    }

    private GameObject InstantiateMarker(GameObject mark)
    {

        MeshRenderer markerRenderer = mark.GetComponent<MeshRenderer>();

        if (markerRenderer == null)
        {
            // The Marker Ring must have a MeshRenderer so it can give visual feedback to the user which way to look.
            // Add one if there wasn't one.
            markerRenderer = mark.AddComponent<MeshRenderer>();
        }

        // Start with the indicator disabled.
        markerRenderer.enabled = false;

        // Remove any colliders and rigidbodies so the mark do not interfere with Unity's physics system.
        foreach (Collider collider in mark.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in mark.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }

#if !UNITY_EDITOR
        Material markerMaterial = markerRenderer.material;
        markerMaterial.color = MarkerColor;
        markerMaterial.SetColor("_TintColor", MarkerColor);
#endif

        return mark;
    }

    public void DestroyObjectsIndicated()
    {
        objectIndicated = null;
        int i;

        if (marker != null)
        {
            for (i = 0; i < marker.Length; i++)
            {
                Destroy(marker[i]);
                marker[i] = null;
                markerRenderer[i] = null;
            }
        }
    }
	
	public GameObject[] GetFromObjects(GameObject[] objects){
        GameObject[] tmpMarker = null;
        MeshRenderer tmpRenderer = null;
        
        if(objects.Length <= 0 || objects == null)
        {
            return tmpMarker;
        }
        else
        {
            tmpMarker = new GameObject[objects.Length];
        }
        

		for (int i = 0; i < objects.Length; i++)
		{
			tmpMarker[i] = CreateMarker(objects[i]);
            tmpRenderer = tmpMarker[i].GetComponent<MeshRenderer>();

            if(tmpRenderer == null)
            {
                tmpRenderer = tmpMarker[i].AddComponent<MeshRenderer>();
            }

            tmpRenderer.enabled = true;
		}
		
		return tmpMarker;
	}

    public void SetObjectsIndicated(GameObject[] objects)
    {
        int i;

        if (objects != null)
        {
            objectIndicated = new GameObject[objects.Length];
            marker = new GameObject[objects.Length];
            markerRenderer = new MeshRenderer[objects.Length];

            for (i = 0; i < objects.Length; i++)
            {
                objectIndicated[i] = objects[i];

                marker[i] = CreateMarker(objectIndicated[i]);
                markerRenderer[i] = marker[i].GetComponent<MeshRenderer>();
            }
        }
        else
        {
            objectIndicated = null;
        }

    }

    private GameObject CreateMarker(GameObject obj)
    {
        Vector3 position;
        GetMarkerPosition(obj, out position);

        GameObject marker = Instantiate(MarkerObject);
        marker.transform.position = position;
        marker.transform.rotation = obj.transform.rotation;
        marker.transform.Rotate(-90.001f, 0, 0);
        float mark_scale = ((obj.transform.localScale.x + obj.transform.localScale.y + obj.transform.localScale.z) / 3) + 0.01f;
        marker.transform.localScale = new Vector3(mark_scale, mark_scale, mark_scale);

        return marker;
    }

    public GameObject[] getRings()
    {
        return marker;
    }

    public void Update()
    {
        if (MarkerObject == null || objectIndicated == null)
        {
            return;
        }

        for (int i = 0; i < objectIndicated.Length; i++)
        {
            if (!InputSequence.Instance.isObjectInCorrectState[i])
            {
                // The marking ring should only be visible if the target is visible.
                markerRenderer[i].enabled = IsTargetVisible(objectIndicated[i]);
            }
            else
            {
                markerRenderer[i].enabled = false;
            }
        }
    }

    private bool IsTargetVisible(GameObject obj)
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(obj.transform.position);
        return (targetViewportPosition.x > TitleSafeFactor && targetViewportPosition.x < 1 - TitleSafeFactor &&
            targetViewportPosition.y > TitleSafeFactor && targetViewportPosition.y < 1 - TitleSafeFactor &&
            targetViewportPosition.z > 0);
    }

    private void GetMarkerPosition(GameObject obj, out Vector3 position)
    {
        // Save the cursor transform position in a variable.
        Vector3 origin = obj.transform.position;

        position = origin;
    }

}
