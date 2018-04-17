using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }

    private TextMesh stageText;
    private string oldStageText = null;
    private TextMesh parametersText;
    private MeshRenderer parametersRenderer;
    private TextMesh adviceText;
    private GameObject background;
    private MeshRenderer textRenderer;
    private MeshRenderer backgroundRenderer;
    private Bounds textBounds;
    private Quaternion DefaultRotation;
    private Vector3 defaultTextScale;
    private Vector3 defaultBackgroundScale;
    private Interpolator backgroundInterpolator;
    private Interpolator textInterpolator;
    private Tagalong backgroundTagalong;
    private TextMesh menuTipText;
    private bool flag;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        GameObject go = GameObject.FindWithTag("AdviceText");
        adviceText = go.GetComponent<TextMesh>();
        adviceText.text = "";
        adviceText.color = Color.white;
        DefaultRotation = go.GetComponent<Billboard>().DefaultRotation;
        go = GameObject.FindWithTag("AdviceBackground");
        background = go.gameObject;
        textRenderer = adviceText.GetComponent<MeshRenderer>();
        backgroundRenderer = background.transform.GetComponent<MeshRenderer>();
        backgroundRenderer.material.color = Color.grey - new Color(0, 0, 0, 0.25f);
        defaultTextScale = adviceText.transform.localScale;
        defaultBackgroundScale = background.transform.localScale;
        textRenderer.enabled = false;
        backgroundRenderer.enabled = false;

        var interpolatorHolder = GameObject.FindWithTag("AdviceBackground");
        backgroundInterpolator = interpolatorHolder.EnsureComponent<Interpolator>();
        interpolatorHolder = GameObject.FindWithTag("AdviceText");
        textInterpolator = interpolatorHolder.EnsureComponent<Interpolator>();

        backgroundTagalong = background.GetComponent<Tagalong>();

        go = GameObject.FindWithTag("StageText");
        stageText = go.GetComponent<TextMesh>();
        stageText.text = "";
        stageText.color = Color.white;

        go = GameObject.FindWithTag("ParametersText");
        parametersText = go.GetComponent<TextMesh>();
        parametersText.text = "";
        parametersText.color = new Color(140, 50, 255, 100);
        parametersRenderer = parametersText.GetComponent<MeshRenderer>();
        parametersRenderer.enabled = false;

        go = GameObject.FindWithTag("MenuTipText");
        menuTipText = go.GetComponent<TextMesh>();
        menuTipText.text = "Say 'Open Menu' or tap and hold for some seconds to open the menu.";
        menuTipText.color = Color.white;

        flag = true;

        Invoke("DestroyMenuTipText", 8);
    }

    void Update()
    {
        stageText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.25f, 0.95f, 1f));
        stageText.transform.rotation = Camera.main.transform.rotation;

        menuTipText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.65f, 0.1f, 1f));
        menuTipText.transform.rotation = Camera.main.transform.rotation;
		
		/*stageText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.25f, 0.8f, 1f));
        stageText.transform.rotation = Camera.main.transform.rotation;

        menuTipText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.65f, 0.15f, 1f));
        menuTipText.transform.rotation = Camera.main.transform.rotation;*/

        if (!flag)
        {
            adjustTextSizeFromDistance();
        }

        if(backgroundTagalong.enabled && adviceText.transform.position != background.transform.position)
        {
            textInterpolator.SetTargetPosition(background.transform.position);
        }
        
    }

    public void ShowMenuTipText(string t)
    {
        menuTipText.text = t;

        Invoke("DestroyMenuTipText", 4);
    }

    private void DestroyMenuTipText()
    {
        menuTipText.text = "";

        CancelInvoke();
    }

    public void setFlag(bool value)
    {
        flag = value;
    }

    public void updateStageText(string t)
    {
        stageText.text = t;
    }

    public void substituteStageText(bool b, string t = null)
    {
        if(b && t == null)
        {
            Debug.Log("Error: the new string must not be null.");
            return;
        }

        if (b)
        {
            if(oldStageText == null)
            {
                oldStageText = stageText.text;
            }
            stageText.text = t;
            stageText.color = Color.red;
        }
        else
        {
            stageText.text = oldStageText;
            stageText.color = Color.white;
            oldStageText = null;
        }
    }

    public void updateParametersText(string t)
    {
        parametersText.text = t;
    }

    public void updateParameterTextPosition(Vector3 position)
    {
        parametersText.transform.position = position + new Vector3(0, 0.25f, 0);
    }

    public void enableParametersText(bool b)
    {
        parametersRenderer.enabled = b;
    }

    public void updateAdviceText(string t)
    {
        adviceText.text = t;
    }

    public void updateAdviceTextPosition(Vector3 position, int border = 0)
    {
        switch(border){
			case 0:
				//Place text according to its center.
                backgroundInterpolator.SetTargetPosition(position);
                textInterpolator.SetTargetPosition(position);
                break;
			case 1:
                //Place text according to its right border.
                backgroundInterpolator.SetTargetPosition(position + (background.transform.right * getBackgroundWidth() / 2));
                textInterpolator.SetTargetPosition(position + (background.transform.right * getBackgroundWidth() / 2));
                break;
			case 2:
                //Place text according to its left border.
                backgroundInterpolator.SetTargetPosition(position - (background.transform.right * getBackgroundWidth() / 2));
                textInterpolator.SetTargetPosition(position - (background.transform.right * getBackgroundWidth() / 2));
                break;
            case 3:
                //Place text according to its top border.
                backgroundInterpolator.SetTargetPosition(position - (background.transform.up * getBackgroundHeight() / 2));
                textInterpolator.SetTargetPosition(position - (background.transform.up * getBackgroundHeight() / 2));
                break;
            case 4:
                //Place text according to its bottom border.
                backgroundInterpolator.SetTargetPosition(position + (background.transform.up * getBackgroundHeight() / 2));
                textInterpolator.SetTargetPosition(position + (background.transform.up * getBackgroundHeight() / 2));
                break;
            default:
				break;
		}

    }

    public void enableAdviceText(bool b)
    {
        textRenderer.enabled = b;
        backgroundRenderer.enabled = b;
    }

    public void updateBackgroundSize()
    {
        adviceText.transform.rotation = DefaultRotation;
        textBounds = new Bounds(Vector3.zero, Vector3.zero);
        textBounds.Encapsulate(textRenderer.bounds);
        background.transform.localScale = new Vector3(textRenderer.bounds.size.x + 0.02f, textRenderer.bounds.size.y, 0.1f);

        //defaultTextScale = adviceText.transform.localScale;
        defaultBackgroundScale = background.transform.localScale;
    }

    public void resetTextSize()
    {
        textInterpolator.SetTargetLocalScale(defaultTextScale);
        backgroundInterpolator.SetTargetLocalScale(defaultBackgroundScale);
    }

    private void adjustTextSizeFromDistance()
    {
        if(Math.Abs(Vector3.Distance(Camera.main.transform.position, background.transform.position)) >= 0.6)
        {
            resetTextSize();
        }
        else
        {
            textInterpolator.SetTargetLocalScale(defaultTextScale * 0.8f);
            backgroundInterpolator.SetTargetLocalScale(defaultBackgroundScale * 0.8f);
        }
    }

    public float getBackgroundWidth()
    {
        return background.transform.localScale.x;
    }

    public float getBackgroundHeight()
    {
        return background.transform.localScale.y;
    }

    public bool IsTextVisible(Vector3 position, float TitleSafeFactor, int fromWhere)
    {
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(position);

        switch (fromWhere)
        {
            case 0:
                //Check if the text is fully visible inside the view frustum's of the camera.
                return (targetViewportPosition.x > TitleSafeFactor && targetViewportPosition.x < 1 - TitleSafeFactor &&
                    targetViewportPosition.y > TitleSafeFactor && targetViewportPosition.y < 1 - TitleSafeFactor);
            case 1:
                //Check if the text is fully visible from the right border of the camera.
                return targetViewportPosition.x < 1 - TitleSafeFactor;
            case 2:
                //Check if the text is fully visible from the left border of the camera.
                return targetViewportPosition.x > TitleSafeFactor;
            case 3:
                //Check if the text is fully visible from the bottom of the camera.
                return targetViewportPosition.y > TitleSafeFactor;
            case 4:
                //Check if the text is fully visible from the top of the camera.
                return targetViewportPosition.y < 1 - TitleSafeFactor;
            default:
                break;
        }

        return false;
    }
}
