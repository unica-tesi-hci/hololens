using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class InputSequence : Singleton<InputSequence>
{
    //aggiungi GIOVANNI
    private string subRoutineName = "BRAKE_RELEASE";
    //private bool order = false;
    private List<Task> tasks;
    private int task_index = 0;
    private List<string> sequence;
    private int sequenceIndex;
    private IPAddress interlocutorAddress;
    private Socket listener;
    private bool engineOK = true;
    private Thread tid1;
    private bool tid1Running = false;
    private IPEndPoint senderEndPoint;

    private List<List<FeedbackGroup>> seqsFromFile = new List<List<FeedbackGroup>>();
    private List<List<Task>> tasksList = new List<List<Task>>();
    private List<JsonSaveObject> routinesList = new List<JsonSaveObject>();

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

    //modificato in List
    private FeedbackGroup currentSequence = null;
    private Dictionary<GameObject, NewParameterManager> paramMap;
    [HideInInspector]
    public bool nextTask;
    [HideInInspector]
    public bool flag;
    [HideInInspector]
    public bool[] isObjectInCorrectState;

    private void SelectSeqByIndex()
    {
        seqFromFile = routinesList[sequenceIndex].feedback_group;
        tasks = routinesList[sequenceIndex].tasks;
    }

    private void SelectSeqByTaskName(string task)
    {
        sequenceIndex = sequence.IndexOf(task);

        seqFromFile = routinesList[sequenceIndex].feedback_group;
        tasks = routinesList[sequenceIndex].tasks;
    }

    public void engineStartProcedure()
    {
        //flag = false;
        CockpitObjectsAPI coAPI = new CockpitObjectsAPI();
        
        coAPI.SetSimulationParameterValue("/controls/electric/external-power", 1.0);                //    "EXT_PWR_ON"
        coAPI.SetSimulationParameterValue("/controls/electric/battery-switch", 1.0);                //    "BAT_ON"
        coAPI.SetSimulationParameterValue("/controls/APU/master-switch", 1.0);                      //    "APU_MASTER_ON"
        coAPI.SetSimulationParameterValue("/controls/APU/starter", 1.0);                            //    "APU_START_ON"
        coAPI.SetSimulationParameterValue("/controls/electric/APU-generator", 1.0);                 //    "APU_GEN_ON"
        coAPI.SetSimulationParameterValue("/controls/electric/external-power", 0.0);                //    "EXT_PWR_OFF"
        coAPI.SetSimulationParameterValue("/controls/pneumatic/APU-bleed", 1.0);                    //    "APU_BLEED_ON"

        coAPI.SetSimulationParameterValue("/consumables/fuel/tank/selected", 1.0);                  //    "PUMPS_ON"
        coAPI.SetSimulationParameterValue("/consumables/fuel/tank[1]/selected", 1.0);
        coAPI.SetSimulationParameterValue("/consumables/fuel/tank[3]/selected", 1.0);
        coAPI.SetSimulationParameterValue("/consumables/fuel/tank[4]/selected", 1.0);
        coAPI.SetSimulationParameterValue("/consumables/fuel/tank[5]/selected", 1.0);

        coAPI.SetSimulationParameterValue("/controls/engines/engine-start-switch", 2.0);            //    "ENG_SWITCH_START"

        coAPI.SetSimulationParameterValue("/controls/engines/engine/cutoff-switch", 0.0);           //    "ENG_ON"
        coAPI.SetSimulationParameterValue("/controls/engines/engine[1]/cutoff-switch", 0.0);

        System.Threading.Thread.Sleep(25000);                                                       //    "ENG_LOAD"

        coAPI.SetSimulationParameterValue("/controls/engines/engine-start-switch", 2.0);            //    "ENG_START_SWITCH"
        coAPI.SetSimulationParameterValue("/controls/electric/engine/generator", 1.0);              //    "GENERATOR_ON"
        coAPI.SetSimulationParameterValue("/controls/electric/engine[1]/generator", 1.0);

        coAPI.SetSimulationParameterValue("/controls/pneumatic/APU-bleed", 0.0);                    //    "APU_BLEED_OFF"
        coAPI.SetSimulationParameterValue("/controls/pressurization/pack/pack-on", 1.0);            //    "PACK_ON"
        coAPI.SetSimulationParameterValue("/controls/pressurization/pack[1]/pack-on", 1.0);

        coAPI.SetSimulationParameterValue("/controls/APU/starter", 0.0);                            //    "APU_OFF"
        coAPI.SetSimulationParameterValue("/controls/electric/APU-generator", 0.0);
        coAPI.SetSimulationParameterValue("/controls/APU/master-switch", 0.0);

        engineOK = true;
    }

    private void Start()
    {
        //sequence = new List<string>
        //{
        //    "EXT_PWR_ON",
        //    "BAT_ON",
        //    "APU_MASTER_ON",
        //    "APU_START_ON",
        //    "APU_GEN_ON",
        //    "EXT_PWR_OFF",
        //    "APU_BLEED_ON",
        //    "PUMPS_ON",
        //    "ENG_SWITCH_START",
        //    "ENG_ON",
        //    "ENG_LOAD",
        //    "ENG_SWITCH_NORM",
        //    "GENERATOR_ON",
        //    "APU_BLEED_OFF",
        //    "PACK_ON",
        //    "APU_OFF",
        //    "BRAKE_RELEASE",                  //RELEASE BRAKE
        //    "AUTOBRAKE_MAX",                  // TO CREATE: RELEASE BRAKE 2
        //    "SPOILER_ARM",                    //  ????
        //    "FLAPS",                          //  TO CREATE: FLAPS to 2
        //    "PACK_OFF",                       //  ???? 
        //    "THROTTLE_FULL",                  //RELEASE THR LEVERS
        //    "ACCELERATION",                   //  ????
        //    "TAKEOFF",                        //  ???? non vi sono controlli sull'altitudine
        //    "GEAR_OFF",                       //GEAR UP
        //    "VELOCITY",                       //REACH 80 KNOTS
        //    "SPOILER_DISARM_AND_FLAPS",       //GROUND SPOILER + FLAPS 1
        //    "VELOCITY2",                      //CHECK 100 KNOTS
        //    "FLAPS2",                         //FLAPS 0
        //    "END"
        //};

        sequence = new List<string>
        {
            "BRAKE_RELEASE",
            "RELEASE_THR_LEVERS",
            "ACCELERATION",
            "GAIN ALTITUDE",
            "GEAR_UP",
            "CHECK_80_KNOTS",
            "FLAPS_1",
            "CHECK_100_KNOTS",
            "FLAPS_0",
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

        SelectSeqByTaskName(subRoutineName);

        //procedura preliminare di avvio del motore
        engineStartProcedure();

        //setto il server socket
        setServer();

        //tid1 = new Thread(new ThreadStart(waitResponse));
        //tid1.Start();
    }

    //creo l'interlocutore socket
    public void setServer()
    {
        IPHostEntry ipHostInfo = null;
        try
        {
            ipHostInfo = Dns.GetHostEntry("127.0.0.1");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine(e.StackTrace.ToString());
        }
        interlocutorAddress = ipHostInfo.AddressList[0];

        listener = new Socket(interlocutorAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        senderEndPoint = new IPEndPoint(interlocutorAddress, 12000);
    }


    //SOCKET CLIENT
    public void sendInput(String text)
    {
        byte[] bytes = new byte[1024];

        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(interlocutorAddress, 11000);
            Socket sender = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //connetto al server
                sender.Connect(remoteEndPoint);

                byte[] msg = Encoding.ASCII.GetBytes(text);
                sender.Send(msg);
                int bytesRec = sender.Receive(bytes);
                Debug.Log(Encoding.ASCII.GetString(bytes, 0, bytesRec));

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }

    //RESPONSE
    public void waitResponse()
    {
        byte[] bytes = new Byte[1024];
        listener.Bind(senderEndPoint);

        Debug.Log("Waiting for Connection");
        try
        {
            listener.Listen(1);
            Debug.Log("Client Connected");

            //attendo connessioni 
            while (true)
            {
                Socket handler = listener.Accept();
                String data = null;
                data += Encoding.ASCII.GetString(bytes, 0, handler.Receive(bytes));

                Debug.Log("TaskTree notification: " + data + " Completed");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();


                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Update()
    {
        //finche i dati tra flightgear e hololens non si sincronizzano non faccio nulla
        if (!Parameters.Instance.isAllZero())
        {
            if (engineOK) //se il motore è acceso
            {
                updateTaskStatus();
                if (nextTask && task_index == 0)
                {
                    nextTask = false;
                    nextSeq();
                }
            }
        }
        //waitResponse();
    }

    private void updateTaskStatus()
    {
        String messageEnding = " DONE";
        String lastMessageEnding = " DONE <EOF>";
        if (tasks != null)
        {
            if (tasks[task_index].type.Equals("output"))
            {
                if (tasks[task_index].name.Equals("Display 3D confirmation"))
                {
                    //Debug.Log("TASK #" + task_index + " completed");
                    sendInput(tasks[task_index].x + ";" + tasks[task_index].y + messageEnding);
                    task_index++;

                    for (int i = 0; i < objectIndicated.Length; i++)
                    {
                        if (objsData[i].correctPosition)
                        {
                            double requiredValue = objsData[i].requiredValue;
                            Parameters.Instance.UpdateValue(objsData[i].name, requiredValue, objsData[i].GetType().ToString());
                        }
                    }
                }
                else
                {
                    //Debug.Log("TASK #" + task_index + " completed");
                    sendInput(tasks[task_index].x + ";" + tasks[task_index].y + messageEnding);
                    task_index++;
                }
            }
            else if (tasks[task_index].type.Equals("input"))
            {
                //wait for input
                if (nextTask)
                {
                    //Debug.Log("TASK #" + task_index + " completed");
                    sendInput(tasks[task_index].x + ";" + tasks[task_index].y + messageEnding);
                    task_index++;
                }
            }
            else if (tasks[task_index].type.Equals("sight"))
            {
                //Debug.Log("TASK #" + task_index + " completed");
                sendInput(tasks[task_index].x + ";" + tasks[task_index].y + lastMessageEnding);
                if (nextTask)
                    task_index = 0;
            }
            else if (tasks[task_index].type.Equals("user") || tasks[task_index].type.Equals("cognitive"))
            {
                //wait for input
                if (nextTask)
                {
                    //Debug.Log("TASK #" + task_index + " completed");
                    sendInput(tasks[task_index].x + ";" + tasks[task_index].y + messageEnding);
                    task_index++;
                }
            }
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
        correctSelection();

        if (sequence[sequenceIndex + 1] != "END")
        {
            task_index = 0;
            ++sequenceIndex;
            SelectSeqByIndex();
        }
        Invoke("updateComponents", 3);
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
        String completeFeedback = tasks[tasks.Count - 1].name;
        //String completeFeedback = "GOOD";

        TextManager.Instance.setFlag(true);
        adviceText = completeFeedback;
        billboardText.enabled = false;
        TextManager.Instance.resetTextSize();
        TextManager.Instance.updateAdviceText(adviceText);
        TextManager.Instance.updateBackgroundSize();
        billboardText.enabled = true;
        TextManager.Instance.enableParametersText(false);
        textSpeak = completeFeedback;
        TextManager.Instance.setFlag(false);
        Speak_Text();
    }

    //aggiunto da Giovanni
    private String feedback()
    {
        int inputIndex = 0;
        foreach (Task t in tasks) {
            if (t.type.Equals("input") || t.type.Equals("user") || t.type.Equals("cognitive"))
                break;
            inputIndex++;
        }
        if (tasks[inputIndex].type.Equals("user"))
            return tasks[inputIndex].name + ": " + currentSequence.adviceText;
        else
            return tasks[inputIndex].name;

    }

    private void setAdvice()
    {
        //adviceText = currentSequence.adviceText;
        //if (currentSequence.showParamInAdviceText)
        //{
        //    String options = "";
        //    objsData = currentSequence.objectIndicated.ToArray();
        //    for (int i = 0; i < objsData.Length; i++)
        //    {
        //        if (!isObjectInCorrectState[i])
        //        {
        //            options = options + "\n<b>• " + objsData[i].name + ";</b>";
        //        }
        //    }

        //    adviceText = adviceText + "\n" + options;
        //}
        //textSpeak = currentSequence.textSpeak;
        //parameterText = currentSequence.parameterText;
        //stageText = currentSequence.stageText;

        String adviceText = feedback();

        textSpeak = adviceText;
        parameterText = currentSequence.parameterText;
        stageText = tasks[task_index].name;

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

        for (i = 0; i < objsData.Length; i++)
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
        if (idx < 0)
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
        if (currentSequence.sequence == seq_id)
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
                if (!seq.objectIndicated[0].correctPosition)
                    return seq;
            }
        }

        return null;
    }

    public bool UpdateSequence(string seq_id, FeedbackGroup feed_grp)//, string suggestion)
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

    public void InitSequenceObjects(List<List<FeedbackGroup>> seqs_objs, List<JsonSaveObject> sequences)
    {
        seqsFromFile = seqs_objs;
        routinesList = sequences;
    }

    //public void InitSequenceObjects(List<FeedbackGroup> seq_objs, string _name, List<Task> _tasks)
    //{
    //    seqFromFile = seq_objs;

    //    //aggiunto da GIOVANNI
    //    subRoutineName = _name;
    //    tasks = _tasks;
    //}

    private void getObjectsFromSequence(out GameObject[] objectIndicated, out CockpitFeedback[] objectData)
    {
        GameObject[] objects = null;
        if (currentSequence == null || currentSequence.objectIndicated == null)
        {
            objectIndicated = null;
            objectData = null;
            return;
        }
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
        if (objectIndicated != null)
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

            if (objectIndicated.Length > 0)// && (tasks[task_index].type.Equals("input") || tasks[task_index].type.Equals("user")))
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
            switch(sequenceIndex)
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

                if(path[i] == "/velocities/airspeed-kt" || path[i] == "/position/altitude-ft") //aggiunto giovanni
                {
                    result = result && param >= requiredValue;
                }
                else
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

        foreach (GameObject obj in objectIndicated)
        {
            foreach (ComponentsEnums.Components id in paramMap[obj].getID())

                if (value == id)
                {
                    return true;
                }
        }

        return false;
    }
}
