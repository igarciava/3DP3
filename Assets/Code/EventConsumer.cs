using UnityEngine;

public class EventConsumer : MonoBehaviour
{
    public void Step(string FootName)
    {
        Debug.Log("step with foot " + FootName);
    }
    public void PunchSound1(AnimationEvent _AnimationEvent)
    {
        string l_StringParameter = _AnimationEvent.stringParameter;
        float l_FloatParameter = _AnimationEvent.floatParameter;
        int l_IntParameter = _AnimationEvent.intParameter;
        Object l_Object = _AnimationEvent.objectReferenceParameter;

        Debug.Log("event punchsound1 " + l_StringParameter + " f " + l_FloatParameter + " i " + l_IntParameter + " o " + l_Object.name);
    }
}
