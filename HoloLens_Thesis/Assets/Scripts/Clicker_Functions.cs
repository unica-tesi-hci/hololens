using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker_Functions : MonoBehaviour, IInputClickHandler, IHoldHandler {
	
	private bool isHolding;
	private float timer;
	private float holdTime = 2f;

	// Use this for initialization
	void Start () {
		isHolding = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isHolding)
		{
			if(Time.time - timer > holdTime)
			{
				timer = float.PositiveInfinity;
				
				if(!MenuManager.Instance.isMenuOpen())
				{
					MenuManager.Instance.OpenMenu();
				}
			}
		}
	}
	
	public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling)
		{
			//InputSequence.Instance.setCorrect();
		}
    }
	
	public void OnHoldStarted(HoldEventData eventData)
    {
        isHolding = true;
		timer = Time.time;
    }
	
	public void OnHoldCompleted(HoldEventData eventData)
    {
        isHolding = false;
    }
	
	public void OnHoldCanceled(HoldEventData eventData)
    {
        isHolding = false;
    }
}
