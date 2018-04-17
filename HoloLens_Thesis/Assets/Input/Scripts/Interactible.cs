using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour, IInputClickHandler, INavigationHandler {

	[Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;
    private AudioSource audioSource;

    private Material[] defaultMaterials;

    void Start()
    {
        defaultMaterials = GetComponent<Renderer>().materials;

        // Add a BoxCollider if the interactible does not contain one.
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        EnableAudioHapticFeedback();
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (TargetFeedbackSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = TargetFeedbackSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        for (int i = 0; i < defaultMaterials.Length; i++)
        {
            defaultMaterials[i].SetFloat("_Highlight", .5f);
        }

        // Play the audioSource feedback when we gaze and select a hologram.
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling && !MenuManager.Instance.isRemoving)
		{
			SendMessage("OnSelectCheck");
		}
    }
	
	public void OnNavigationStarted(NavigationEventData eventData)
	{
		if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling && !MenuManager.Instance.isRemoving)
		{
			SendMessage("NavigationStart");
		}
	}
	
	public void OnNavigationUpdated(NavigationEventData eventData)
	{
		if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling && !MenuManager.Instance.isRemoving)
		{
			SendMessage("NavigationUpdated", eventData.NormalizedOffset);
		}
	}
	
	public void OnNavigationCompleted(NavigationEventData eventData)
	{
		if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling && !MenuManager.Instance.isRemoving)
		{
			SendMessage("NavigationFinished");
		}
	}
	
	public void OnNavigationCanceled(NavigationEventData eventData)
	{
		if(!MenuManager.Instance.isPlacing && !MenuManager.Instance.isRotating && !MenuManager.Instance.isScaling && !MenuManager.Instance.isRemoving)
		{
			SendMessage("NavigationFinished");
		}
	}
}
