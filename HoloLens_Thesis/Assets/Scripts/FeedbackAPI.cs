using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackAPI : MonoBehaviour {

    //Creates a new instance of FeedbackGroup and then returns it.
	public FeedbackGroup CreateFeedbackGroup(string seq_id, List<CockpitFeedback> cf, bool box = false, string advice_text = "", string parameter_text = "", string stage_text = "", string speak_text = "", bool show_param_text = false, bool oh_panel = false)
    {
        return new FeedbackGroup(seq_id, cf.ToArray(), box, advice_text, parameter_text, stage_text, speak_text, show_param_text, oh_panel);
    }

    //Adds a FeedbackGroup object in the list.
    public void AddFeedbackGroup(FeedbackGroup fg, int idx)
    {
        InputSequence.Instance.AddSequenceObject(fg, idx);
    }

    /*Deletes the FeedbackGroup instance with id "seq_id".
     *Returns true if it is successfully deleted, false otherwise.
     */
    public bool DeleteFeedbackGroup(string seq_id)
    {
        return InputSequence.Instance.RemoveSequence(seq_id);
    }

    //Gets a FeedbackGroup instance from its id string.
    public FeedbackGroup GetFeedbackGroup(string seq_id)
    {
        return InputSequence.Instance.GetSequence(seq_id);
    }

    //Sets the specified values in the FeedbackGroup instance having id "seq_id".
    public bool UpdateFeedbackGroup(string seq_id, List<CockpitFeedback> cf, bool box, string advice_text, string parameter_text, string stage_text, string speak_text, bool show_param_text, bool oh_panel)
    {
        FeedbackGroup feedGrp = CreateFeedbackGroup(seq_id, cf, box, advice_text, parameter_text, stage_text, speak_text, show_param_text, oh_panel);

        return InputSequence.Instance.UpdateSequence(seq_id, feedGrp);
    }

    //Creates a new instance of CockpitFeedback and then returns it.
    public CockpitFeedback CreateCockpitFeedback(string name, bool ring, bool box, bool checkMark, bool loading, bool[] sub_param, double expectedValue)
    {
        return new CockpitFeedback(name, ring, box, checkMark, loading, sub_param, expectedValue);
    }

    //Adds a CockpitFeedback object in the list of the corresponding FeedbackGroup.
    public void AddCockpitFeedback(CockpitFeedback cf, FeedbackGroup fg)
    {
        fg.objectIndicated.Add(cf);
    }

    /*Deletes the CockpitFeedback instance with name "name".
     *Returns true if it is successfully deleted, false otherwise.
     */
    public int DeleteCockpitFeedback(string seq_id, string name)
    {
        return GetFeedbackGroup(seq_id).objectIndicated.RemoveAll(n => n.name == name);
    }

    //Gets a CockpitFeedback instance from its name.
    public CockpitFeedback GetCockpitFeedback(string seq_id, string name)
    {
        return GetFeedbackGroup(seq_id).objectIndicated.Find(n => n.name == name);
    }

    //Updates an instance of CockpitFeedback having name "name".
    public void UpdateCockpitFeedback(string seq_id, string name, bool ring, bool box, bool checkMark, bool loading, bool[] sub_param, double value)
    {
        CockpitFeedback cf = GetFeedbackGroup(seq_id).objectIndicated.Find(n => n.name == name);

        cf.showRing = ring;
        cf.insideBox = box;
        cf.showCheckMark = checkMark;
        cf.showLoading = loading;
        cf.sub_parameters = sub_param;
        cf.requiredValue = value;
    }
}
