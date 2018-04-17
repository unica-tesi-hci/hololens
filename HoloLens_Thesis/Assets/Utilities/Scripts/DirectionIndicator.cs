using UnityEngine;
using System;

/// <summary>
/// DirectionIndicator creates an indicator around the cursor showing
/// what direction to turn to find this GameObject.
/// </summary>
public class DirectionIndicator : MonoBehaviour
{
    [Tooltip("Model to display in order to point a non visible object the user has to interact.")]
    public GameObject DirectionIndicatorObject;

    [Tooltip("Color to shade the direction indicator.")]
    public Color DirectionIndicatorColor = Color.blue;

    [Tooltip("Allowable percentage inside the holographic frame to continue to show a directional indicator.")]
    [Range(-0.3f, 0.3f)]
    public float TitleSafeFactor = 0.0f;

    // The objects indicated by the Direction Indicator.
    private GameObject[] objectIndicated = null;
    //If the object indicated by the Direction Indicator is a single one, use this variable.
    private GameObject singleObject = null;

    // The indicator instantiated when one or more objects are not visible.
    private GameObject indicator = null;

    // The default rotation of the cursor direction indicator.
    private Quaternion directionIndicatorDefaultRotation = Quaternion.identity;

    // Cache the MeshRenderer for the on-cursor indicators since they will be enabled and disabled frequently.
    private MeshRenderer directionIndicatorRenderer;

    // Check if the cursor direction indicator is visible.
    private bool isDirectionIndicatorVisible;
    //The bounding box that will contain all the objects not visible by the user.
    private Bounds boundingBox;
    //The center of the bounding box.
    private Vector3 center;
    //Counts at each Update how much targets have to be pointed by the Direction Indicator.
    private int countTarget;

    public void Awake()
    {
        if (DirectionIndicatorObject == null)
        {
            return;
        }

        // Make the Direction Indicator's color semi-transparent.
        DirectionIndicatorColor.a = 0.5f;

        // Instantiate the direction indicator.
        DirectionIndicatorObject = InstantiateDirectionIndicator(DirectionIndicatorObject);
        directionIndicatorDefaultRotation = DirectionIndicatorObject.transform.rotation;
    }

    public void OnDestroy()
    {
        GameObject.Destroy(DirectionIndicatorObject);
        GameObject.Destroy(singleObject);
    }

    private GameObject InstantiateDirectionIndicator(GameObject indicator)
    {

        MeshRenderer indicatorRenderer = indicator.GetComponent<MeshRenderer>();

        if (indicatorRenderer == null)
        {
            // The Direction Indicator must have a MeshRenderer so it can give visual feedback to the user which way to look.
            // Add one if there wasn't one.
            indicatorRenderer = indicator.AddComponent<MeshRenderer>();
        }

        // Start with the indicator disabled.
        indicatorRenderer.enabled = false;

        // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
        foreach (Collider collider in indicator.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in indicator.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }

#if !UNITY_EDITOR
        Material indicatorMaterial = indicatorRenderer.material;
        indicatorMaterial.color = DirectionIndicatorColor;
        indicatorMaterial.SetColor("_TintColor", DirectionIndicatorColor);
#endif

        return indicator;
    }

    public void SetObjectsIndicated(GameObject[] objects)
    {
        if(objects != null)
        {
            boundingBox = new Bounds(Vector3.zero, Vector3.zero);
            objectIndicated = new GameObject[objects.Length];

            if (indicator != null)
            {
                Destroy(indicator);
                indicator = null;
                directionIndicatorRenderer = null;
            }

            indicator = CreateIndicator();
            directionIndicatorRenderer = indicator.GetComponent<MeshRenderer>();

            for (int i = 0; i < objects.Length; i++)
            {
                objectIndicated[i] = objects[i];
                boundingBox.Encapsulate(objectIndicated[i].GetComponent<MeshRenderer>().bounds);
            }

            center = boundingBox.center;
        }
        else
        {
            objectIndicated = null;
        }
    }

    private GameObject CreateIndicator()
    {
        GameObject indicator = Instantiate(DirectionIndicatorObject);

        return indicator;
    }

    public void Update()
    {
        if (DirectionIndicatorObject == null || objectIndicated == null)
        {
            return;
        }

        boundingBox.center = center;
        boundingBox.size = Vector3.zero;
        isDirectionIndicatorVisible = false;
        countTarget = 0;

        for (int i = 0; i < objectIndicated.Length; i++)
        {
            if (!InputSequence.Instance.isObjectInCorrectState[i] && !IsTargetVisible(objectIndicated[i]))
            {
                boundingBox.Encapsulate(objectIndicated[i].GetComponent<MeshRenderer>().bounds);
                isDirectionIndicatorVisible = true;
                countTarget++;
                singleObject = objectIndicated[i];
            }
        }

        // The directional indicator should only be visible if at least one target is not visible.
        directionIndicatorRenderer.enabled = isDirectionIndicatorVisible;
        TextManager.Instance.enableParametersText(isDirectionIndicatorVisible);

        if (isDirectionIndicatorVisible)
        {
            Vector3 camToObjectDirection;
            // Direction from the Main Camera to the indicated gameObject.
            if (countTarget >= 2)
            {
                camToObjectDirection = boundingBox.center - Camera.main.transform.position;
            }
            else
            {
                camToObjectDirection = singleObject.transform.position - Camera.main.transform.position;
            }
            camToObjectDirection.Normalize();

            Vector3 position;
            Quaternion rotation;
            GetDirectionIndicatorPositionAndRotation(
                camToObjectDirection,
                out position,
                out rotation);

            indicator.transform.position = position;
            indicator.transform.rotation = rotation;
        }
    }

    //Check if the target is visible to the user, that is, if the target is inside the screen of the camera.
    private bool IsTargetVisible(GameObject obj)
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(obj.transform.position);
        return (targetViewportPosition.x > TitleSafeFactor && targetViewportPosition.x < 1 - TitleSafeFactor &&
            targetViewportPosition.y > TitleSafeFactor && targetViewportPosition.y < 1 - TitleSafeFactor &&
            targetViewportPosition.z > 0);
    }

    //Retrieves the position and rotation of the Direction Indicator based on where the indicated object is.
    private void GetDirectionIndicatorPositionAndRotation(
        Vector3 camToObjectDirection,
        out Vector3 position,
        out Quaternion rotation)
    {
        // Project the camera to target direction onto the screen plane.
        Vector3 cameraIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * Camera.main.transform.forward);
        cameraIndicatorDirection.Normalize();

        // The final position is translated on the appropriate border of the camera, depending on where the object indicated is.
        position = getPositionFromDirection();

        // Find the rotation from the facing direction to the target object.
        if (Vector3.Angle(camToObjectDirection, Camera.main.transform.forward) > 90)
        {
            // If the camera is facing directly away from the target, set the direction to the right.
            cameraIndicatorDirection = Camera.main.transform.right;

            rotation = Quaternion.LookRotation(
            camToObjectDirection,
            cameraIndicatorDirection);
        }
        else
        {
            rotation = Quaternion.LookRotation(
            Camera.main.transform.forward,
            cameraIndicatorDirection) * directionIndicatorDefaultRotation;
        }
    }

    //Places the Direction Indicator on the screen based on where the pointed object is.
    private Vector3 getPositionFromDirection()
    {
        Vector3 position;
        Vector3 screenPos;
        if (countTarget >= 2)
        {
            screenPos = Camera.main.WorldToViewportPoint(boundingBox.center);
        }
        else
        {
            screenPos = Camera.main.WorldToViewportPoint(singleObject.transform.position);
        }
        
        Vector2 onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2;
        float max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
        onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.4f, 0.4f); //undo mapping

        if (Math.Abs(onScreenPos.x) > 0.9f)
        {
            onScreenPos.x = 0.9f;
        }else if(Math.Abs(onScreenPos.x) < 0.1f)
        {
            onScreenPos.x = 0.1f;
        }

        if (Math.Abs(onScreenPos.y) > 0.9f)
        {
            onScreenPos.y = 0.9f;
        }
        else if (Math.Abs(onScreenPos.y) < 0.1f)
        {
            onScreenPos.y = 0.1f;
        }

        position = Camera.main.ViewportToWorldPoint(new Vector3(onScreenPos.x, onScreenPos.y, 5.0f));
        TextManager.Instance.updateParameterTextPosition(position + new Vector3(-0.05f, 0.15f, 0));

        return position;
    }
}