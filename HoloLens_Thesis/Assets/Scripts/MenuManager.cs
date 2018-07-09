using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager> {
    private GameObject menu;
    private GameObject menuScaleOptions;
    private TextMesh menuTitle;
    private MeshRenderer menuTitleRenderer;
    [HideInInspector]
    public bool isPlacing;
    [HideInInspector]
    public bool isScaling;
    [HideInInspector]
    public bool isRotating;
    [HideInInspector]
    public bool isRemoving;

    private BoxScaling boxScaling;

    private GameObject cockpit;
    private GameObject deck;
    private GameObject panel;
    private GameObject directionIndicator;
    private MarkerRing markerRing;
    private ContainerBox contBox;
    private FeedbackHolograms feed;
    private GameObject displayText;

    // Use this for initialization
    void Start () {
        menu = GameObject.FindWithTag("Menu").gameObject;
        menuScaleOptions = GameObject.FindWithTag("MenuScaleOptions").gameObject;
        menu.SetActive(false);
        menuScaleOptions.SetActive(false);

        menuTitle = GameObject.FindWithTag("MenuTitle").GetComponent<TextMesh>();
        menuTitleRenderer = menuTitle.gameObject.GetComponent<MeshRenderer>();
        menuTitle.text = "Menu";
        menuTitle.color = Color.white;
        menuTitleRenderer.enabled = false;

        menuTitle.transform.SetParent(menu.transform, false);

        cockpit = GameObject.FindWithTag("Cockpit").gameObject;
        deck = GameObject.FindWithTag("Deck");
        panel = GameObject.FindWithTag("Panel");
        directionIndicator = GameObject.FindWithTag("DirectionalIndicator");
        markerRing = GameObject.FindWithTag("MarkerRing").GetComponent<MarkerRing>();
        contBox = GameObject.FindWithTag("ContainerBox").GetComponent<ContainerBox>();
        feed = GameObject.FindWithTag("FeedbackHolograms").GetComponent<FeedbackHolograms>();
        displayText = GameObject.FindWithTag("Text");
		boxScaling = cockpit.GetComponent<BoxScaling>();

        isPlacing = false;
        isScaling = false;
        isRotating = false;
        isRemoving = false;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void OpenMenu()
    {
        cockpit.SetActive(false);
        deck.SetActive(false);
        panel.SetActive(false);

        if (!isScaling)
        {
            menu.SetActive(true);
            menu.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, 1f));
            menuTitle.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.45f, 0.80f, 1f));
            menuTitleRenderer.enabled = true;
        }
        else
        {
            menuScaleOptions.SetActive(true);
            menuScaleOptions.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        }

        SetHolograms(false);
    }

    public void CloseMenu()
    {
        if (!isScaling)
        {
            menu.SetActive(false);
            menuTitleRenderer.enabled = false;
        }
        else
        {
            menuScaleOptions.SetActive(false);
        }
        
        

        cockpit.SetActive(true);
        deck.SetActive(true);
        panel.SetActive(true);
		
		if(!isPlacing && !isRotating && !isScaling && !isRemoving)
		{
			InputSequence.Instance.DestroyIndicationObjects();
			InputSequence.Instance.setIndicationObjects();
			GameObject.FindWithTag("FeedbackHolograms").GetComponent<FeedbackHolograms>().InitializeTextPosition();
			SetHolograms(true);
		}else
		{
			SetHolograms(false);
		}
    }

    public bool isMenuOpen()
	{
		return menu.activeSelf || menuScaleOptions.activeSelf;
	}
	
	private void SetHolograms(bool b){
		GameObject[] obj;
		int i;
		
		directionIndicator.SetActive(b);
        obj = markerRing.getRings();
        if(obj != null)
        {
            for (i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(b);
            }
        }
        obj = contBox.getContainerBox();
        if (obj != null)
        {
            for (i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(b);
            }
        }
        obj = feed.getCurrentObject();
        if (obj != null)
        {
            for (i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(b);
            }
        }

        if (!b && menu.activeSelf)
        {
            displayText.SetActive(b);
        }
        else if(!b && !menu.activeSelf)
        {
            displayText.SetActive(!b);
            displayText.transform.GetChild(1).gameObject.SetActive(b);
            displayText.transform.GetChild(2).gameObject.SetActive(b);
            displayText.transform.GetChild(3).gameObject.SetActive(b);
        }
        else
        {
            displayText.SetActive(true);
            displayText.transform.GetChild(1).gameObject.SetActive(true);
            displayText.transform.GetChild(2).gameObject.SetActive(true);
            displayText.transform.GetChild(3).gameObject.SetActive(true);
        }

        boxScaling.SetBox();
    }

    public void ObjectPlacement()
    {
        if (isRotating)
        {
            ObjectRotation();
        }else if (isScaling)
        {
            BoxScale();
        }else if (isRemoving)
        {
            RemoveComponent();
        }

        isPlacing = !isPlacing;
		
		if (isPlacing)
        {
            TextManager.Instance.substituteStageText(isPlacing, "Placement mode:\n Tap on a component to start placing it.");
			menu.transform.GetChild(2).GetComponent<CompoundButtonText>().Text = "Stop Placement";
        }
        else
        {
            TextManager.Instance.substituteStageText(isPlacing);
			menu.transform.GetChild(2).GetComponent<CompoundButtonText>().Text = "Place Objects";
        }

        TapToPlace tapToPlace;

        foreach(Transform component in cockpit.GetComponentsInChildren<Transform>())
        {
            tapToPlace = component.gameObject.GetComponent<TapToPlace>();

            if(tapToPlace != null)
            {
                if (isPlacing)
                {
                    tapToPlace.enabled = true;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    tapToPlace.enabled = false;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    public void ObjectRotation()
    {
        if (isPlacing)
        {
            ObjectPlacement();
        }else if (isScaling)
        {
            BoxScale();
        }else if (isRemoving)
        {
            RemoveComponent();
        }

        isRotating = !isRotating;
		
		if (isRotating)
        {
            TextManager.Instance.substituteStageText(isRotating, "Rotation mode:\n Hold on a component to start rotate it.");
			menu.transform.GetChild(3).GetComponent<CompoundButtonText>().Text = "Stop Rotation";
        }
        else
        {
            TextManager.Instance.substituteStageText(isRotating);
			menu.transform.GetChild(3).GetComponent<CompoundButtonText>().Text = "Rotate Objects";
        }

        HoldToRotate holdToRotate;

        foreach (Transform component in cockpit.GetComponentsInChildren<Transform>())
        {
            holdToRotate = component.gameObject.GetComponent<HoldToRotate>();

            if (holdToRotate != null)
            {
                if (isRotating)
                {
                    holdToRotate.enabled = true;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    holdToRotate.enabled = false;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

    }

    public void BoxScale()
    {
        if (isPlacing)
        {
            ObjectPlacement();
        }
        else if (isRotating)
        {
            ObjectRotation();
        }
        else if (isRemoving)
        {
            RemoveComponent();
        }

        isScaling = !isScaling;
		
		if (isScaling)
        {
            TextManager.Instance.substituteStageText(isScaling, "Scaling mode:\n 1)Choose a number from the menu to show the box of the corresponding sequence.\n 2)Hold a box's border to scale it.");
			boxScaling.enabled = true;
            menu.SetActive(false);
            menuTitleRenderer.enabled = false;
            menuScaleOptions.SetActive(true);
        }
        else
        {
            TextManager.Instance.substituteStageText(isScaling);
			boxScaling.DestroyAllObjects();
			cockpit.transform.GetChild(0).gameObject.SetActive(true);
            boxScaling.enabled = false;
            menuScaleOptions.SetActive(false);
            menu.SetActive(true);
            menuTitleRenderer.enabled = true;

        }

    }

    public void SaveObjectsPropertiesIntoFile()
    {
        FileDataWriter.Instance.SaveObjectsProperties();
    }

    public void AddComponent()
    {
        Material material;
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject component = Instantiate(o);
        component.AddComponent<Interactible>();
        component.AddComponent<TapToPlace>().enabled = false;
        component.AddComponent<HoldToRotate>().enabled = false;
        component.AddComponent<TapToRemove>().enabled = false;
        component.AddComponent<NewParameterManager>();
        component.GetComponent<MeshRenderer>().enabled = true;
        material = Resources.Load("Cockpit_Buttons", typeof(Material)) as Material;
        component.GetComponent<MeshRenderer>().material = material;
        material = Resources.Load("PlaceableShadow", typeof(Material)) as Material;
        component.GetComponent<TapToPlace>().PlacementMaterial = material;
        component.GetComponent<HoldToRotate>().PlacementMaterial = material;

        component.name = "Obj_" + FileDataReader.Instance.GetNextObjectNumber();
        component.transform.position = new Vector3(0f, 0f, 0.5f);
        component.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        component.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        component.GetComponent<NewParameterManager>().setID(new ComponentsEnums.Components[] { ComponentsEnums.Components.None });
        component.GetComponent<NewParameterManager>().setDataType(new string[] { "double" });

        component.transform.SetParent(cockpit.transform);
        FileDataWriter.Instance.AddIntoList(component.name);

        Destroy(o);

        CloseMenu();
    }

    public void RemoveComponent()
    {
        if (isPlacing)
        {
            ObjectPlacement();
        }
        else if (isRotating)
        {
            ObjectRotation();
        }else if (isScaling)
        {
            BoxScale();
        }

        isRemoving = !isRemoving;

        if (isRemoving)
        {
            TextManager.Instance.substituteStageText(isRemoving, "Removing mode:\n Tap on a component to remove it from the cockpit.");
            menu.transform.GetChild(7).GetComponent<CompoundButtonText>().Text = "Stop Removing";
        }
        else
        {
            TextManager.Instance.substituteStageText(isRemoving);
            menu.transform.GetChild(7).GetComponent<CompoundButtonText>().Text = "Remove \nComponents";
        }

        TapToRemove tapToRemove;

        foreach (Transform component in cockpit.GetComponentsInChildren<Transform>())
        {
            tapToRemove = component.gameObject.GetComponent<TapToRemove>();

            if (tapToRemove != null)
            {
                if (isRemoving)
                {
                    tapToRemove.enabled = true;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    tapToRemove.enabled = false;
                    component.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }
}
