using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FeedbackGroup
{

    public string sequence;
    public List<CockpitFeedback> objectIndicated;
    public bool showBox;
    public String adviceText;
    public String parameterText;
    public String stageText;
    public String textSpeak;
    public bool showParamInAdviceText;
    public bool isOnOhPanel;

    public FeedbackGroup()
    {

    }

    /**
     * This constructor contains the following parameters:
     * sequence: ID string representing the sequence.
     * objectIndicated: list of objects inside the cockpit connected with this sequence.
     * showBox: boolean specifying if the container box should be rendered or not.
     * adviceText: the text displayed inside the background, which gives advices to the user.
     * parameterText: the text displayed near the direction indicator, indicating which objects the arrow is pointing at.
     * stageText: the text displayed in the upper-left of the screen, indicating the current stage.
     * textSpeak: the text dictated by the speaker.
     * showParamInAdviceText: boolean indicating if there are additional text to be displayed inside adviceText.
     * isOnOhPanel: boolean specifying if the objectIndicated are positioned on the OhPanel (upper space of the cockpit) or not.
     **/
    public FeedbackGroup(string seq, CockpitFeedback[] objs, bool box, String advice, String param, String stage, String speak, bool showParam, bool ohPanel)
    {
        objectIndicated = new List<CockpitFeedback>();

        sequence = seq;

        foreach (CockpitFeedback obj in objs)
        {
            objectIndicated.Add(obj);
        }

        showBox = box;
        adviceText = advice;
        parameterText = param;
        stageText = stage;
        textSpeak = speak;
        showParamInAdviceText = showParam;
        isOnOhPanel = ohPanel;
    }
}
