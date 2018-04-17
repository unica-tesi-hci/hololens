using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToRemove : MonoBehaviour, IInputClickHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (MenuManager.Instance.isRemoving)
        {
            FileDataWriter.Instance.RemoveFromList(gameObject.name);
            Destroy(gameObject);
        }
    }
}
