using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputSequence : Singleton<InputSequence>
{

    private int[] sequence;
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
    private Billboard billboardText;
    [HideInInspector]
    public bool flag;
    [HideInInspector]
    public bool[] isObjectInCorrectState;

    private void Start()
    {
        sequence = new int[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29
        };

        sequenceIndex = 0;
        flag = true;

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

    public int getSeq()
    {
        return sequence[sequenceIndex];
    }
	
	public int getSeqLength()
	{
		return sequence.Length;
	}

    private void nextSeq()
    {
        flag = true;

        if (sequence[sequenceIndex] < (sequence.Length - 1))
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
        setStage();
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

        if (getSeq() == 10)
        {
            feed.DestroyObjectsIndicated();
            feed.InstantiateCheckMark();
            feed.SetObjectsIndicated(objectIndicated);
        }
    }

    private void setAdvice()
    {
        string option1, option2, option3, option4, option5;

        switch (sequence[sequenceIndex])
        {
            case 0:
                adviceText = "Turn on EXT PWR.";
                parameterText = "EXT PWR";
                textSpeak = "Turn on the External Power to give us the electrical power.";
                break;
            case 1:
                adviceText = "Turn on BAT1 and BAT2.";
                parameterText = "BAT";
                textSpeak = "Turn on batteries 1 and 2.";
                break;
            case 2:
                adviceText = "Turn on MASTER SWITCH in APU.";
                parameterText = "APU MASTER SW";
                textSpeak = "Turn the APU Master switch on to start the engines.";
                break;
            case 3:
                adviceText = "Turn on START in APU.";
                parameterText = "APU START";
                textSpeak = "Turn the APU Start switch on.";
                break;
            case 4:
                adviceText = "Turn on APU GEN.";
                parameterText = "APU GEN";
                textSpeak = "Turn on the APU Generator to give the airplane the electrical power without external assistance.";
                break;
            case 5:
                adviceText = "Turn off EXT PWR.";
                parameterText = "EXT PWR";
                textSpeak = "Turn off the External Power, since we don't need it anymore.";
                break;
            case 6:
                adviceText = "Turn on APU BLEED.";
                parameterText = "APU BLEED";
                textSpeak = "Turn on the APU BLEED.";
                break;
            case 7:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• LTK PUMPS 1;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• LTK PUMPS 2;</b>\n";
                }

                if (isObjectInCorrectState[2])
                {
                    option3 = "";
                }
                else
                {
                    option3 = "<b>• PUMP 1 and PUMP 2;</b>\n";
                }

                if (isObjectInCorrectState[3])
                {
                    option4 = "";
                }
                else
                {
                    option4 = "<b>• RTK PUMPS 1;</b>\n";
                }

                if (isObjectInCorrectState[4])
                {
                    option5 = "";
                }
                else
                {
                    option5 = "<b>• RTK PUMPS 2;</b>\n";
                }

                adviceText = "Set on all the following fuel's PUMPS:\n\n" + option1 + option2 + option3 + option4 + option5;
                parameterText = "FUEL PUMPS";
                textSpeak = "Set on all the fuel's Pumps.";
                break;
            case 8:
                adviceText = "Set ENG SWITCH on START.";
                parameterText = "ENG SW";
                textSpeak = "Set the Engines Switch to the value 'START'.";
                break;
            case 9:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• ENG1;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• ENG2;</b>\n";
                }

                adviceText = "Switch on the following components:\n\n" + option1 + option2;
                parameterText = "ENG1 / ENG2";
                textSpeak = "Switch the engines master 1 and 2 to start the respective engines.";
                break;
            case 10:
                adviceText = "Wait until the engines reach the value 30% on the monitor.";
                textSpeak = "Now wait the engines to spool up to 30%, so that they become responsive.";
                break;
            case 11:
                adviceText = "Switch ENG SWITCH on NORM.";
                parameterText = "ENG SW";
                textSpeak = "Switch the Engine Switch back to the value 'NORM'.";
                break;
            case 12:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• GEN1;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• GEN2;</b>\n";
                }

                adviceText = "Turn on the following components:\n\n" + option1 + option2;
                parameterText = "GEN1 / GEN2";
                textSpeak = "Turn on the engine generators 1 and 2.";
                break;
            case 13:
                adviceText = "Turn off APU BLEED.";
                parameterText = "APU BLEED";
                textSpeak = "Turn off the APU BLEED.";
                break;
            case 14:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• PACK1;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• PACK2;</b>\n";
                }

                adviceText = "Turn on the following components:\n\n" + option1 + option2;
                parameterText = "PACK1 / PACK2";
                textSpeak = "Turn on Pack 1 and Pack 2.";
                break;
            case 15:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• APU GEN;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• APU MASTER SWITCH;</b>\n";
                }

                if (isObjectInCorrectState[2])
                {
                    option3 = "";
                }
                else
                {
                    option3 = "<b>• APU START;</b>\n";
                }

                adviceText = "Turn off the following components:\n\n" + option1 + option2 + option3;
                parameterText = "APU";
                textSpeak = "Since we are generating electrical power through the engines, turn off the APU Generator, APU Start and APU Master switches.";
                break;
            case 16:
                adviceText = "Release the parking brake.";
                parameterText = "BRAKE";
                textSpeak = "Release the parking brake.";
                break;
            case 17:
                adviceText = "Set the Auto Brake to MAX.";
                parameterText = "AUTO BRAKE";
                textSpeak = "Set the Auto Brake to MAX.";
                break;
            case 18:
                adviceText = "Set the spoiler armed.";
                parameterText = "SPOILER";
                textSpeak = "Set the spoiler armed.";
                break;
            case 19:
                adviceText = "Set the flaps to 2.";
                parameterText = "FLAPS";
                textSpeak = "Set the flaps to 2.";
                break;
            case 20:
                if (isObjectInCorrectState[0])
                {
                    option1 = "";
                }
                else
                {
                    option1 = "<b>• PACK1;</b>\n";
                }

                if (isObjectInCorrectState[1])
                {
                    option2 = "";
                }
                else
                {
                    option2 = "<b>• PACK2;</b>\n";
                }

                adviceText = "Turn off the following components:\n\n" + option1 + option2;
                parameterText = "PACK1 / PACK2";
                textSpeak = "Turn off Pack 1 and Pack 2.";
                break;
            case 21:
                adviceText = "Full in the Throttles to apply full power.";
                parameterText = "THROTTLE";
                textSpeak = "Full in the Throttles to apply full power.";
                break;
            case 22:
                adviceText = "Wait until the airplane gains sufficient speed.";
                parameterText = "";
                textSpeak = "Wait until the airplane gains sufficient speed.";
                break;
            case 23:
                adviceText = "Pull back the yoke to take off. Go up until you reach 500 foot.";
                parameterText = "";
                textSpeak = "Pull back the yoke to take off. Go up until you reach 500 foot.";
                break;
            case 24:
                adviceText = "Retract the landing gear.";
                parameterText = "GEAR";
                textSpeak = "Retract the landing gear.";
                break;
            case 25:
                adviceText = "";
                parameterText = "";
                textSpeak = "";
                break;
            case 26:
                adviceText = "";
                textSpeak = "";

                if (!isObjectInCorrectState[0] && !isObjectInCorrectState[1])
                {
                    adviceText = "Set the flaps to 1 and disarm the spoilers.";
                    textSpeak = "Set the flaps to 1 and disarm the spoilers.";
                }
                else if (!isObjectInCorrectState[0] && isObjectInCorrectState[1])
                {
                    adviceText = "Set the flaps to 1.";
                    textSpeak = "Set the flaps to 1.";
                }
                else if (isObjectInCorrectState[0] && !isObjectInCorrectState[1])
                {
                    adviceText = "Disarm the spoilers.";
                    textSpeak = "Disarm the spoilers.";
                }

                parameterText = "FLAPS/SPOILER";
                break;
            case 27:
                adviceText = "";
                parameterText = "";
                textSpeak = "";
                break;
            case 28:
                adviceText = "Set the flaps to 0.";
                parameterText = "FLAPS";
                textSpeak = "Set the flaps to 0.";
                break;
            default:
                adviceText = "";
                parameterText = "";
                textSpeak = "Tutorial completed!";
                TextManager.Instance.enableAdviceText(false);
                break;
        }

        TextManager.Instance.setFlag(true);
        TextManager.Instance.resetTextSize();
        TextManager.Instance.updateAdviceText(adviceText);
        TextManager.Instance.updateBackgroundSize();
        TextManager.Instance.setFlag(false);
        TextManager.Instance.updateParametersText(parameterText);
    }

    public void Speak_Text()
    {
        if (!textSpeak.Equals(""))
        {
            textToSpeech.SpeakText(textSpeak);
        }
    }

    private void setStage()
    {
        switch (sequence[sequenceIndex])
        {
            case 0:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 1:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 2:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 3:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 4:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 5:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 6:
                stageText = "1)Preliminary cockpit preparation";
                break;
            case 7:
                stageText = "2)Engine start";
                break;
            case 8:
                stageText = "2)Engine start";
                break;
            case 9:
                stageText = "2)Engine start";
                break;
            case 10:
                stageText = "2)Engine start";
                break;
            case 11:
                stageText = "2)Engine start";
                break;
            case 12:
                stageText = "2)Engine start";
                break;
            case 13:
                stageText = "2)Engine start";
                break;
            case 14:
                stageText = "2)Engine start";
                break;
            case 15:
                stageText = "2)Engine start";
                break;
            case 16:
                stageText = "3)Before taxi";
                break;
            case 17:
                stageText = "3)During taxi";
                break;
            case 18:
                stageText = "3)During taxi";
                break;
            case 19:
                stageText = "3)During taxi";
                break;
            case 20:
                stageText = "3)During taxi";
                break;
            case 21:
                stageText = "4)Takeoff";
                break;
            case 22:
                stageText = "4)Takeoff";
                break;
            case 23:
                stageText = "4)Takeoff";
                break;
            case 24:
                stageText = "4)Takeoff";
                break;
            case 25:
                stageText = "4)Takeoff";
                break;
            case 26:
                stageText = "4)Takeoff";
                break;
            case 27:
                stageText = "4)Takeoff";
                break;
            case 28:
                stageText = "4)Takeoff";
                break;
            case 29:
                stageText = "";
                break;
            default:
                break;
        }

        TextManager.Instance.updateStageText(stageText);
    }

    public void setIndicationObjects()
    {
        objectIndicated = null;

        getObjectsFromSequence(out objectIndicated);

        //Make these objects as the next to be indicated and marked, if required.
        dirInd.SetObjectsIndicated(objectIndicated);

        if (getSeq() != 10)
        {
            markRing.SetObjectsIndicated(objectIndicated);
        }

        if (getSeq() == 7 || getSeq() == 10 || getSeq() == 12 || getSeq() == 14 || getSeq() == 15 || getSeq() == 20 || getSeq() == 26)
        {
            contBox.SetObjectsIndicated(objectIndicated, true);
        }
        else
        {
            contBox.SetObjectsIndicated(objectIndicated, false);
        }

        if (getSeq() == 10)
        {
            feed.InstantiateWaitingObject();
        }
        feed.SetObjectsIndicated(objectIndicated);
    }

    public void DestroyIndicationObjects()
    {
        markRing.DestroyObjectsIndicated();
        contBox.DestroyObjectsIndicated();
        feed.DestroyObjectsIndicated();

        TextManager.Instance.enableAdviceText(false);
    }

    private void getObjectsFromSequence(out GameObject[] objectIndicated)
    {
        GameObject[] objects = null;

        switch (getSeq())
        {
            case 0:
                objects = new GameObject[] { gameObject.transform.Find("External Power").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 1:
                objects = new GameObject[] { gameObject.transform.Find("Battery").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 2:
                objects = new GameObject[] { gameObject.transform.Find("APU Master SW").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 3:
                objects = new GameObject[] { gameObject.transform.Find("APU Start").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 4:
                objects = new GameObject[] { gameObject.transform.Find("APU Gen").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 5:
                objects = new GameObject[] { gameObject.transform.Find("External Power").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 6:
                objects = new GameObject[] { gameObject.transform.Find("APU Bleed").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 7:
                objects = new GameObject[] { gameObject.transform.Find("LTK_PUMPS_1").gameObject, gameObject.transform.Find("LTK_PUMPS_2").gameObject, gameObject.transform.Find("Pumps").gameObject, gameObject.transform.Find("RTK_PUMPS_1").gameObject, gameObject.transform.Find("RTK_PUMPS_2").gameObject };
                isObjectInCorrectState = new bool[] { false, false, false, false, false };
                break;
            case 8:
                objects = new GameObject[] { gameObject.transform.Find("ENG Start Switch").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 9:
                objects = new GameObject[] { gameObject.transform.Find("Engine1").gameObject, gameObject.transform.Find("Engine2").gameObject };
                isObjectInCorrectState = new bool[] { false, false };
                break;
            case 10:
                objects = new GameObject[] { gameObject.transform.Find("Monitor").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 11:
                objects = new GameObject[] { gameObject.transform.Find("ENG Start Switch").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 12:
                objects = new GameObject[] { gameObject.transform.Find("Engine1 Gen").gameObject, gameObject.transform.Find("Engine2 Gen").gameObject };
                isObjectInCorrectState = new bool[] { false, false };
                break;
            case 13:
                objects = new GameObject[] { gameObject.transform.Find("APU Bleed").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 14:
                objects = new GameObject[] { gameObject.transform.Find("Pack1").gameObject, gameObject.transform.Find("Pack2").gameObject };
                isObjectInCorrectState = new bool[] { false, false };
                break;
            case 15:
                objects = new GameObject[] { gameObject.transform.Find("APU Gen").gameObject, gameObject.transform.Find("APU Master SW").gameObject, gameObject.transform.Find("APU Start").gameObject };
                isObjectInCorrectState = new bool[] { false, false, false };
                break;
            case 16:
                objects = new GameObject[] { gameObject.transform.Find("Brake Parking").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 17:
                objects = new GameObject[] { gameObject.transform.Find("Auto_Brake").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 18:
                objects = new GameObject[] { gameObject.transform.Find("Spoiler").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 19:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 20:
                objects = new GameObject[] { gameObject.transform.Find("Pack1").gameObject, gameObject.transform.Find("Pack2").gameObject };
                isObjectInCorrectState = new bool[] { false, false };
                break;
            case 21:
                objects = new GameObject[] { gameObject.transform.Find("Throttle").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 23:
                Parameters.Instance.set_initialAltitude(Parameters.Instance.get_altitude());
                break;
            case 24:
                objects = new GameObject[] { gameObject.transform.Find("Gears").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            case 26:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject, gameObject.transform.Find("Spoiler").gameObject };
                isObjectInCorrectState = new bool[] { false, false };
                break;
            case 28:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject };
                isObjectInCorrectState = new bool[] { false };
                break;
            default:
                break;
        }

        objectIndicated = objects;
    }
	
	public GameObject[] getObjectsFromSequence(int seq)
    {
        GameObject[] objects = null;

        switch (seq)
        {
            case 0:
                objects = new GameObject[] { gameObject.transform.Find("External Power").gameObject };
                break;
            case 1:
                objects = new GameObject[] { gameObject.transform.Find("Battery").gameObject };
                break;
            case 2:
                objects = new GameObject[] { gameObject.transform.Find("APU Master SW").gameObject };
                break;
            case 3:
                objects = new GameObject[] { gameObject.transform.Find("APU Start").gameObject };
                break;
            case 4:
                objects = new GameObject[] { gameObject.transform.Find("APU Gen").gameObject };
                break;
            case 5:
                objects = new GameObject[] { gameObject.transform.Find("External Power").gameObject };
                break;
            case 6:
                objects = new GameObject[] { gameObject.transform.Find("APU Bleed").gameObject };
                break;
            case 7:
                objects = new GameObject[] { gameObject.transform.Find("LTK_PUMPS_1").gameObject, gameObject.transform.Find("LTK_PUMPS_2").gameObject, gameObject.transform.Find("Pumps").gameObject, gameObject.transform.Find("RTK_PUMPS_1").gameObject, gameObject.transform.Find("RTK_PUMPS_2").gameObject };
                break;
            case 8:
                objects = new GameObject[] { gameObject.transform.Find("ENG Start Switch").gameObject };
                break;
            case 9:
                objects = new GameObject[] { gameObject.transform.Find("Engine1").gameObject, gameObject.transform.Find("Engine2").gameObject };
                break;
            case 10:
                objects = new GameObject[] { gameObject.transform.Find("Monitor").gameObject };
                break;
            case 11:
                objects = new GameObject[] { gameObject.transform.Find("ENG Start Switch").gameObject };
                break;
            case 12:
                objects = new GameObject[] { gameObject.transform.Find("Engine1 Gen").gameObject, gameObject.transform.Find("Engine2 Gen").gameObject };
                break;
            case 13:
                objects = new GameObject[] { gameObject.transform.Find("APU Bleed").gameObject };
                break;
            case 14:
                objects = new GameObject[] { gameObject.transform.Find("Pack1").gameObject, gameObject.transform.Find("Pack2").gameObject };
                break;
            case 15:
                objects = new GameObject[] { gameObject.transform.Find("APU Gen").gameObject, gameObject.transform.Find("APU Master SW").gameObject, gameObject.transform.Find("APU Start").gameObject };
                break;
            case 16:
                objects = new GameObject[] { gameObject.transform.Find("Brake Parking").gameObject };
                break;
            case 17:
                objects = new GameObject[] { gameObject.transform.Find("Auto_Brake").gameObject };
                break;
            case 18:
                objects = new GameObject[] { gameObject.transform.Find("Spoiler").gameObject };
                break;
            case 19:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject };
                break;
            case 20:
                objects = new GameObject[] { gameObject.transform.Find("Pack1").gameObject, gameObject.transform.Find("Pack2").gameObject };
                break;
            case 21:
                objects = new GameObject[] { gameObject.transform.Find("Throttle").gameObject };
                break;
            case 24:
                objects = new GameObject[] { gameObject.transform.Find("Gears").gameObject };
                break;
            case 26:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject, gameObject.transform.Find("Spoiler").gameObject };
                break;
            case 28:
                objects = new GameObject[] { gameObject.transform.Find("Flaps").gameObject };
                break;
            default:
                break;
        }

        return objects;
    }

    public bool isOnOhPanel()
    {
        return getSeq() == 0 || getSeq() == 1 || getSeq() == 2 || getSeq() == 3 || getSeq() == 4 || getSeq() == 5 || getSeq() == 6 || getSeq() == 7 || getSeq() == 12 || getSeq() == 13 || getSeq() == 14 || getSeq() == 15 || getSeq() == 20;
    }

    public void checkNextSeq()
    {
        switch (sequence[sequenceIndex])
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
        }
    }

    public bool checkIfSeq(ComponentsEnums.Components value)
    {
        if (sequence[sequenceIndex] >= (sequence.Length - 1))
        {
            return true;
        }

        switch (value)
        {
            case ComponentsEnums.Components.External_Power:
                if (sequence[sequenceIndex] == 0 || sequence[sequenceIndex] == 5)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Battery:
                if (sequence[sequenceIndex] == 1)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.APU_Gen:
                if (sequence[sequenceIndex] == 4)
                {
                    return true;
                }
                else if (sequence[sequenceIndex] == 15)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.APU_Start:
                if (sequence[sequenceIndex] == 3)
                {
                    return true;
                }
                else if (sequence[sequenceIndex] == 15)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.APU_Master_SW:
                if (sequence[sequenceIndex] == 2)
                {
                    return true;
                }
                else if (sequence[sequenceIndex] == 15)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.APU_Bleed:
                if (sequence[sequenceIndex] == 6 || sequence[sequenceIndex] == 13)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.ENG_Start_Switch:
                if (sequence[sequenceIndex] == 8 || sequence[sequenceIndex] == 11)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Engine1:
                if (sequence[sequenceIndex] == 9)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Engine2:
                if (sequence[sequenceIndex] == 9)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Brake_Parking:
                if (sequence[sequenceIndex] == 16)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Engine1_Gen:
                if (sequence[sequenceIndex] == 12)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Engine2_Gen:
                if (sequence[sequenceIndex] == 12)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Pack1:
                if (sequence[sequenceIndex] == 14 || sequence[sequenceIndex] == 20)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Pack2:
                if (sequence[sequenceIndex] == 14 || sequence[sequenceIndex] == 20)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.LTK_PUMPS_1:
                if (sequence[sequenceIndex] == 7)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.LTK_PUMPS_2:
                if (sequence[sequenceIndex] == 7)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Pumps:
                if (sequence[sequenceIndex] == 7)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.RTK_PUMPS_1:
                if (sequence[sequenceIndex] == 7)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.RTK_PUMPS_2:
                if (sequence[sequenceIndex] == 7)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Auto_Brake:
                if (sequence[sequenceIndex] == 17)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Spoiler:
                if (sequence[sequenceIndex] == 18 || sequence[sequenceIndex] == 26)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Flaps:
                if (sequence[sequenceIndex] == 19 || sequence[sequenceIndex] == 26 || sequence[sequenceIndex] == 28)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Throttle:
                if (sequence[sequenceIndex] == 21)
                {
                    return true;
                }
                break;
            case ComponentsEnums.Components.Gears:
                if (sequence[sequenceIndex] == 24)
                {
                    return true;
                }
                break;
            default:
                break;
        }

        return false;
    }

}
