using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldToScale : MonoBehaviour, IManipulationHandler {

    private Material PlacementMaterial;
    private Material originalMaterial = null;
	private Vector3 previousPosition;
	private GameObject[] box;
	private ContainerBox contBox;
    private float minX, maxX, minY, maxY, minZ, maxZ;

	// Use this for initialization
	void Start () {
		contBox = GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Initialize(Material material, GameObject[] obj, float minimumX, float maximumX, float minimumY, float maximumY, float minimumZ, float maximumZ){
		PlacementMaterial = material;
		box = obj;
        minX = minimumX;
        maxX = maximumX;
        minY = minimumY;
        maxY = maximumY;
        minZ = minimumZ;
        maxZ = maximumZ;
	}

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isScaling)
        {
#if !UNITY_EDITOR
            var layerCacheTarget = gameObject;
            originalMaterial = layerCacheTarget.GetComponent<Renderer>().material;
            layerCacheTarget.GetComponent<Renderer>().material = PlacementMaterial;
#endif

            previousPosition = eventData.CumulativeDelta;
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isScaling)
        {
			float val;
			
            Vector3 moveVector = Vector3.zero;
            moveVector = eventData.CumulativeDelta - previousPosition;

            previousPosition = eventData.CumulativeDelta;

			if(gameObject.tag == "Left"){
                if ((gameObject.transform.position + gameObject.transform.right * moveVector.x).x <= minX || (gameObject.transform.position + gameObject.transform.right * moveVector.x).x < gameObject.transform.position.x)
                {
                    //Move the border to the left.
                    gameObject.transform.position += gameObject.transform.right * moveVector.x;

                    //Get the distance between the left and right border, then scale properly the top/bottom borders.
                    val = Vector3.Distance(box[2].transform.position, gameObject.transform.position);
                    box[1].transform.localScale = new Vector3(val + (1 * 0.02f), box[1].transform.localScale.y, box[1].transform.localScale.z);
                    box[3].transform.localScale = box[1].transform.localScale;

                    //Position the top/bottom borders between the right/left borders.
                    box[1].transform.position += box[1].transform.right * (moveVector.x / 2);
                    box[3].transform.position += box[3].transform.right * (moveVector.x / 2);
                }
			}else if(gameObject.tag == "Up"){
                if ((gameObject.transform.position + gameObject.transform.up * moveVector.y).y >= maxY || (gameObject.transform.position + gameObject.transform.up * moveVector.y).y > gameObject.transform.position.y)
                {
                    //Move the border Up.
                    gameObject.transform.position += gameObject.transform.up * moveVector.y;

                    //Get the distance between the top and bottom border, then scale properly the right/left borders.
                    val = Vector3.Distance(gameObject.transform.position, box[3].transform.position);
                    box[0].transform.localScale = new Vector3(box[0].transform.localScale.x, val + (1 * 0.02f), box[0].transform.localScale.z);
                    box[2].transform.localScale = box[0].transform.localScale;

                    //Position the right/left borders between the top/bottom borders.
                    box[0].transform.position += box[0].transform.up * (moveVector.y / 2);
                    box[2].transform.position += box[2].transform.up * (moveVector.y / 2);
                }
			}else if(gameObject.tag == "Right"){
                if ((gameObject.transform.position + gameObject.transform.right * moveVector.x).x >= maxX || (gameObject.transform.position + gameObject.transform.right * moveVector.x).x > gameObject.transform.position.x)
                {
                    //Move the border to the right.
                    gameObject.transform.position += gameObject.transform.right * moveVector.x;

                    //Get the distance between the left and right border, then scale properly the top/bottom borders.
                    val = Vector3.Distance(box[0].transform.position, gameObject.transform.position);
                    box[1].transform.localScale = new Vector3(val + (1 * 0.02f), box[1].transform.localScale.y, box[1].transform.localScale.z);
                    box[3].transform.localScale = box[1].transform.localScale;

                    //Position the top/bottom borders between the right/left borders.
                    box[1].transform.position += box[1].transform.right * (moveVector.x / 2);
                    box[3].transform.position += box[3].transform.right * (moveVector.x / 2);
                }
			}else if(gameObject.tag == "Down"){
                if ((gameObject.transform.position + gameObject.transform.up * moveVector.y).y <= minY || (gameObject.transform.position + gameObject.transform.up * moveVector.y).y < gameObject.transform.position.y)
                {
                    //Move the border down.
                    gameObject.transform.position += gameObject.transform.up * moveVector.y;

                    //Get the distance between the top and bottom border, then scale properly the right/left borders.
                    val = Vector3.Distance(gameObject.transform.position, box[1].transform.position);
                    box[0].transform.localScale = new Vector3(box[0].transform.localScale.x, val + (1 * 0.02f), box[0].transform.localScale.z);
                    box[2].transform.localScale = box[0].transform.localScale;

                    //Position the right/left borders between the top/bottom borders.
                    box[0].transform.position += box[0].transform.up * (moveVector.y / 2);
                    box[2].transform.position += box[2].transform.up * (moveVector.y / 2);
                }
			}
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isScaling)
        {
#if !UNITY_EDITOR
            var layerCacheTarget = gameObject;
            layerCacheTarget.GetComponent<Renderer>().material = originalMaterial;
            originalMaterial = null;
#endif
        }
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        if (MenuManager.Instance.isScaling)
        {
#if !UNITY_EDITOR
            var layerCacheTarget = gameObject;
            layerCacheTarget.GetComponent<Renderer>().material = originalMaterial;
            originalMaterial = null;
#endif
        }
    }
}
