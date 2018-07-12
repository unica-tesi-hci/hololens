using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputSequence : Singleton<InputSequence>
{

    private List<string> sequence;
    private int sequenceIndex;
    HeadsUpDirectionIndicator dirInd;
    MarkerRing markRing;
    ContainerBox contBox;
    FeedbackHolograms feed;
    TextToSpeechManager textToSpeech;
    private string adviceText;
    private string parameterText;
    private string stageText;
    private string textSpeak;
    private GameObject DirectionalIndicator;
    private GameObject MarkerRing;
    private GameObject ContainerBox;
    private GameObject FeedbackHolograms;
    private GameObject[] objectIndicated;
    private CockpitFeedback[] objsData;
    private Billboard billboardText;
    private List<FeedbackGroup> seqFromFile = null;
    private FeedbackGroup currentSequence = null;
    private Dictionary<GameObject, NewParameterManager> paramMap;
    [HideInInspector]
    public bool nextTask;
    [HideInInspector]
    public bool flag;
    [HideInInspector]
    public bool[] isObjectInCorrectState;

    private void Start()
    {
        sequence = new List<string>
        {
            "EXT_PWR_ON",
            "BAT_ON",
            "APU_MASTER_ON",
            "APU_START_ON",
            "APU_GEN_ON",
            "EXT_PWR_OFF",
            "APU_BLEED_ON",
            "PUMPS_ON",
            "ENG_SWITCH_START",
            "ENG_ON",
            "ENG_LOAD",
            "ENG_SWITCH_NORM",
            "GENERATOR_ON",
            "APU_BLEED_OFF",
            "PACK_ON",
            "APU_OFF",
            "BRAKE_RELEASE",
            "AUTOBRAKE_MAX",
            "SPOILER_ARM",
            "FLAPS",
            "PACK_OFF",
            "THROTTLE_FULL",
            "ACCELERATION",
            "TAKEOFF",
            "GEAR_OFF",
            "VELOCITY",
            "SPOILER_DISARM_AND_FLAPS",
            "VELOCITY2",
            "FLAPS2",
            "END"
        };

        sequenceIndex = 0;
        flag = true;
        nextTask = false;

        DirectionalIndicator = GameObject.FindWithTag("DirectionalIndicator");
        MarkerRing = GameObject.FindWithTag("MarkerRing");
        ContainerBox = GameObject.FindWithTag("ContainerBox");
        FeedbackHolograms = GameObject.FindWithTag("FeedbackHolograms");
        if (DirectionalIndicator != null && MarkerRing != null && FeedbackHolograms != null)
        {
            dirInd = DirectionalIndicator.GetComponent<HeadsUpDirectionIndicator>();
            markRing = MarkerRing.GetComponent<MarkerRing>();
            contBox = ContainerBox.GetComponent<ContainerBox>();
            feed = FeedbackHolograms.GetComponent<FeedbackHolograms>();
        }
        if (dirInd == null || markRing == null || contBox == null || feed == null)
        {
            Debug.Log("Cannot find 'HeadsUpDirectionIndicator', 'MarkerRing', 'ContainerBox' and/or 'FeedbackHolograms' scripts.");
        }

        GameObject audioManager = GameObject.FindWithTag("AudioManager");
        if (audioManager != null)
        {
            textToSpeech = audioManager.GetComponent<TextToSpeechManager>();
        }
        if (textToSpeech != null)
        {
            textToSpeech.Voice = TextToSpeechVoice.Default;
            textToSpeech.SpeakText("Welcome aboard the Airbus A320!");
        }
        else
        {
            Debug.Log("Cannot find TextToSpeechManager script.");
        }
        billboardText = GameObject.FindWithTag("AdviceText").GetComponent<Billboard>();

        FileDataReader.Instance.LoadObjectsProperties();

        Invoke("updateComponents", 3);
    }

    private void Update()
    {
        if (nextTask)
        {
            nextTask = false;
            nextSeq();
        }
    }

    public string getSeq()
    {
        return sequence[sequenceIndex];
    }

    public string getSeq(int i)
    {
        return sequence[i];
    }

    public int getSeqLength()
    {
        return sequence.Count;
    }

    public FeedbackGroup getCurrentSequence()
    {
        return currentSequence;
    }

    public void InitMap(Dictionary<GameObject, NewParameterManager> map)
    {
        paramMap = map;
    }

    private void nextSeq()
    {
        if (sequence[sequenceIndex] != "END")
        {
            correctSelection();
            ++sequenceIndex;
            Invoke("updateComponents", 3);
        }

    }

    private void updateComponents()
    {
        DestroyIndicationObjects();
        setIndicationObjects();
        billboardText.enabled = false;
        setAdvice();
        billboardText.enabled = true;
        feed.InitializeTextPosition();
        Speak_Text();
        flag = false;
        CancelInvoke();
    }

    private void correctSelection()
    {
        TextManager.Instance.setFlag(true);
        adviceText = "Good!";
        billboardText.enabled = false;
        TextManager.Instance.resetTextSize();
        TextManager.Instance.updateAdviceText(adviceText);
        TextManager.Instance.updateBackgroundSize();
        billboardText.enabled = true;
        TextManager.Instance.enableParametersText(false);
        textSpeak = "Good!";
        TextManager.Instance.setFlag(false);
        Speak_Text();
    }

    private void setAdvice()
    {
        adviceText = currentSequence.adviceText;
        if (currentSequence.showParamInAdviceText)
        {
            String options = "";
            objsData = currentSequence.objectIndicated.ToArray();
            for (int i = 0; i < objsData.Length; i++)
            {
                if (!isObjectInCorrectState[i])
                {
                    options = options + "\n<b>• " + objsData[i].name + ";</b>";
                }
            }

            adviceText = adviceText + "\n" + options;
        }
        textSpeak = currentSequence.textSpeak;
        parameterText = currentSequence.parameterText;
        stageText = currentSequence.stageText;

        TextManager.Instance.setFlag(true);
        TextManager.Instance.resetTextSize();
        TextManager.Instance.updateAdviceText(adviceText);
        TextManager.Instance.updateBackgroundSize();
        TextManager.Instance.setFlag(false);
        TextManager.Instance.updateParametersText(parameterText);
        TextManager.Instance.updateStageText(stageText);

        if (getSeq().Equals("End"))
        {
            TextManager.Instance.enableAdviceText(false);
        }
    }

    public void Speak_Text()
    {
        if (!textSpeak.Equals(""))
        {
            textToSpeech.SpeakText(textSpeak);
        }
    }

    public void setIndicationObjects()
    {
        List<bool> list = new List<bool>();
        int i;

        objectIndicated = null;

        currentSequence = GetSequence(getSeq());

        getObjectsFromSequence(out objectIndicated, out objsData);

        //Make these objects as the next to be indicated and marked, if required.
        dirInd.SetObjectsIndicated(objectIndicated);

        for(i = 0; i < objsData.Length; i++)
        {
            list.Add(objsData[i].showRing);
        }
        markRing.SetObjectsIndicated(objectIndicated, list.ToArray());

        list.Clear();

        for (i = 0; i < objsData.Length; i++)
        {
			list.Add(objsData[i].insideBox);
        }
        contBox.SetObjectsIndicated(objectIndicated, list.ToArray(), currentSequence.showBox);

        list.Clear();

        feed.SetObjectsIndicated(objectIndicated, objsData);
    }

    public void DestroyIndicationObjects()
    {
        markRing.DestroyObjectsIndicated();
        contBox.DestroyObjectsIndicated();
        feed.DestroyObjectsIndicated();

        TextManager.Instance.enableAdviceText(false);
    }

    public List<FeedbackGroup> GetSequenceObjects()
    {
        return seqFromFile;
    }

    public void AddSequenceObject(FeedbackGroup feedGrp, int idx)
    {
        if(idx < 0)
        {
            return;
        }

        if (idx >= sequence.Count)
        {
            seqFromFile.Add(feedGrp);
        }
        else
        {
            seqFromFile.Insert(idx, feedGrp);

            if (idx <= sequenceIndex)
            {
                ++sequenceIndex;
            }
        }
    }

    public bool RemoveSequence(string seq_id)
    {
        if(currentSequence.sequence == seq_id)
        {
            return false;
        }

        foreach (FeedbackGroup seq in seqFromFile)
        {
            if (seq.sequence.Equals(seq_id))
            {
                seqFromFile.Remove(seq);

                return true;
            }
        }

        return false;
    }

    public FeedbackGroup GetSequence(string seq_id)
    {
        foreach (FeedbackGroup seq in seqFromFile)
        {
            if (seq.sequence.Equals(seq_id))
            {
                return seq;
            }
        }

        return null;
    }

    public bool UpdateSequence(string seq_id, FeedbackGroup feed_grp)
    {
        if (currentSequence.sequence == seq_id)
        {
            return false;
        }

        foreach (FeedbackGroup seq in seqFromFile)
        {
            if (seq.sequence.Equals(seq_id))
            {
                seq.objectIndicated = feed_grp.objectIndicated;
                seq.showBox = feed_grp.showBox;
                seq.adviceText = feed_grp.adviceText;
                seq.parameterText = feed_grp.parameterText;
                seq.stageText = feed_grp.stageText;
                seq.textSpeak = feed_grp.textSpeak;
                seq.showParamInAdviceText = feed_grp.showParamInAdviceText;
                seq.isOnOhPanel = feed_grp.isOnOhPanel;

                return true;
            }
        }

        return false;
    }

    public void InitSequenceObjects(List<FeedbackGroup> seq_objs)
    {
        seqFromFile = seq_objs;
    }

    private void getObjectsFromSequence(out GameObject[] objectIndicated, out CockpitFeedback[] objectData)
    {
        GameObject[] objects = null;
        objectData = currentSequence.objectIndicated.ToArray();

        if (objectData.Length > 0)
        {
            objects = new GameObject[objectData.Length];
            isObjectInCorrectState = new bool[objectData.Length];

            for (int i = 0; i < objectData.Length; i++)
            {
                objects[i] = gameObject.transform.Find(objectData[i].name).gameObject;
                isObjectInCorrectState[i] = false;
            }
        }

        objectIndicated = objects;
    }

    public GameObject[] getObjectsFromSequence(string seq)
    {
        GameObject[] objects = null;
        CockpitFeedback[] components = GetSequence(seq).objectIndicated.ToArray();

        if (components.Length > 0)
        {
            objects = new GameObject[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                objects[i] = gameObject.transform.Find(components[i].name).gameObject;
            }
        }

        return objects;
    }

    public bool isOnOhPanel()
    {
        return currentSequence.isOnOhPanel;
    }

    public void checkNextSeq()
    {
        if (getSeq().Equals("End"))
        {
            flag = true;
            textSpeak = "There is no other indication to give. The tutorial is finished.";
            Destroy(DirectionalIndicator);
            Destroy(MarkerRing);
            Destroy(FeedbackHolograms);
            Destroy(TextManager.Instance.gameObject);

            return;
        }

        if(objectIndicated.Length > 0)
        {
            double requiredValue;
            bool oldValue;
            bool updateAdvice = false;
            bool taskCompleted = true;

            for (int i = 0; i < objectIndicated.Length; i++)
            {
                requiredValue = objsData[i].requiredValue;
                oldValue = isObjectInCorrectState[i];
                
                isObjectInCorrectState[i] = CheckObjectState(paramMap[objectIndicated[i]].getPath(), currentSequence.objectIndicated[i].sub_parameters, paramMap[objectIndicated[i]].getDataType(), requiredValue);
                
                taskCompleted = taskCompleted && isObjectInCorrectState[i];

                updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[i]) ? true : false;
            }

            if (currentSequence.showParamInAdviceText && updateAdvice)
            {
                setAdvice();
            }

            if (taskCompleted)
            {
                flag = true;
                nextTask = true;
            }
        }

        /*switch (sequence[sequenceIndex])
        {
            case 0:
                if (Parameters.Instance.get_EXT_PWR() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 1:
                if (Parameters.Instance.get_BAT() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 2:
                if (Parameters.Instance.get_APU_M_SW() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 3:
                if (Parameters.Instance.get_APU_START() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 4:
                if (Parameters.Instance.get_APU_GEN() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 5:
                if (Parameters.Instance.get_EXT_PWR() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 6:
                if (Parameters.Instance.get_APU_BLEED() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 7:
                if (Parameters.Instance.get_LTK_PUMPS_1() == 1 && Parameters.Instance.get_LTK_PUMPS_2() == 1 && Parameters.Instance.get_PUMPS() == 1 && Parameters.Instance.get_RTK_PUMPS_1() == 1 && Parameters.Instance.get_RTK_PUMPS_2() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    isObjectInCorrectState[2] = true;
                    isObjectInCorrectState[3] = true;
                    isObjectInCorrectState[4] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_LTK_PUMPS_1() == 1 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_LTK_PUMPS_2() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    oldValue = isObjectInCorrectState[2];
                    isObjectInCorrectState[2] = Parameters.Instance.get_PUMPS() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[2]) ? true : false;

                    oldValue = isObjectInCorrectState[3];
                    isObjectInCorrectState[3] = Parameters.Instance.get_RTK_PUMPS_1() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[3]) ? true : false;

                    oldValue = isObjectInCorrectState[4];
                    isObjectInCorrectState[4] = Parameters.Instance.get_RTK_PUMPS_2() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[4]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 8:
                if (Parameters.Instance.get_ENG_START_SW() == 2)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 9:
                if (Parameters.Instance.get_ENG1() == 0 && Parameters.Instance.get_ENG2() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_ENG1() == 0 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_ENG2() == 0 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 10:
                if (Math.Abs(Parameters.Instance.get_ENG1_N1()) >= 30 && Math.Abs(Parameters.Instance.get_ENG2_N1()) >= 30)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                break;
            case 11:
                if (Parameters.Instance.get_ENG_START_SW() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 12:
                if (Parameters.Instance.get_ENG1_GEN() == 1 && Parameters.Instance.get_ENG2_GEN() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_ENG1_GEN() == 1 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_ENG2_GEN() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 13:
                if (Parameters.Instance.get_APU_BLEED() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 14:
                if (Parameters.Instance.get_PACK1() == 1 && Parameters.Instance.get_PACK2() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_PACK1() == 1 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_PACK2() == 1 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 15:
                if (Parameters.Instance.get_APU_GEN() == 0 && Parameters.Instance.get_APU_START() == 0 && Parameters.Instance.get_APU_M_SW() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    isObjectInCorrectState[2] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_APU_GEN() == 0 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_APU_M_SW() == 0 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    oldValue = isObjectInCorrectState[2];
                    isObjectInCorrectState[2] = Parameters.Instance.get_APU_START() == 0 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[2]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 16:
                if (Parameters.Instance.get_BRK_PRK() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 17:
                if (Parameters.Instance.get_AUTO_BRK() == 3)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 18:
                if (Parameters.Instance.get_Spoiler_ARM() == 1)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 19:
                if (Math.Abs(Parameters.Instance.get_flaps()) == 0.375)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 20:
                if (Parameters.Instance.get_PACK1() == 0 && Parameters.Instance.get_PACK2() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Parameters.Instance.get_PACK1() == 0 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_PACK2() == 0 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 21:
                if (Parameters.Instance.is_throttle1_max() && Parameters.Instance.is_throttle2_max())
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 22:
                if (Math.Abs(Parameters.Instance.get_velocity()) >= 145.0)
                {
                    ++sequenceIndex;
                    updateComponents();
                }
                break;
            case 23:
                if (Math.Abs(Parameters.Instance.get_altitude() - Parameters.Instance.get_initialAltitude()) >= 500.0)
                {
                    ++sequenceIndex;
                    updateComponents();
                }
                break;
            case 24:
                if (Parameters.Instance.get_gear() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            case 25:
                if (Math.Abs(Parameters.Instance.get_velocity()) > 85.0)
                {
                    ++sequenceIndex;
                    updateComponents();
                }
                break;
            case 26:
                if (Math.Abs(Parameters.Instance.get_flaps()) == 0.25 && Parameters.Instance.get_Spoiler_ARM() == 0)
                {
                    isObjectInCorrectState[0] = true;
                    isObjectInCorrectState[1] = true;
                    nextSeq();
                }
                else
                {
                    bool oldValue;
                    bool updateAdvice = false;

                    oldValue = isObjectInCorrectState[0];
                    isObjectInCorrectState[0] = Math.Abs(Parameters.Instance.get_flaps()) == 0.25 ? true : false;
                    updateAdvice = (oldValue != isObjectInCorrectState[0]) ? true : false;

                    oldValue = isObjectInCorrectState[1];
                    isObjectInCorrectState[1] = Parameters.Instance.get_Spoiler_ARM() == 0 ? true : false;
                    updateAdvice = updateAdvice || (oldValue != isObjectInCorrectState[1]) ? true : false;

                    if (updateAdvice)
                    {
                        setAdvice();
                    }
                }
                break;
            case 27:
                if (Math.Abs(Parameters.Instance.get_velocity()) > 110.0)
                {
                    ++sequenceIndex;
                    updateComponents();
                }
                break;
            case 28:
                if (Math.Abs(Parameters.Instance.get_flaps()) == 0.00)
                {
                    isObjectInCorrectState[0] = true;
                    nextSeq();
                }
                else
                {
                    isObjectInCorrectState[0] = false;
                }
                break;
            default:
                flag = true;
                textSpeak = "There is no other indication to give. The tutorial is finished.";
                Destroy(DirectionalIndicator);
                Destroy(MarkerRing);
                Destroy(FeedbackHolograms);
                Destroy(TextManager.Instance.gameObject);
                break;
        }*/
    }

    public bool CheckObjectState(string[] path, bool[] subParam, string[] dataType, double requiredValue)
    {
        bool result = true;

        for (int i = 0; i < path.Length; i++)
        {
            if (!subParam[i])
            {
                continue;
            }

            var param = dataType[i] == "double" ? 0.00 : 0;

            if (dataType[i] == "double")
            {
                if (path[i] == "/position/altitude-ft")
                {
                    param = Math.Abs(Parameters.Instance.GetValueDouble(path[i]) - Parameters.Instance.get_initialAltitude());
                }
                else
                {
                    param = Math.Abs(Parameters.Instance.GetValueDouble(path[i]));
                }

                result = result && param == requiredValue;
            }
            else
            {
                param = Parameters.Instance.GetValueInt(path[i]);

                result = result && param == Convert.ToInt32(requiredValue);
            }
        }

        return result;
    }

    public bool checkIfSeq(ComponentsEnums.Components value)
    {
        if (value == ComponentsEnums.Components.None)
        {
            return false;
        }

        if (getSeq().Equals("End"))
        {
            return true;
        }

        foreach(GameObject obj in objectIndicated)
        {
            foreach(ComponentsEnums.Components id in paramMap[obj].getID())

            if(value == id)
            {
                return true;
            }
        }

        return false;
    }

}
