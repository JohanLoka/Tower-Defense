using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebuffType
{
    public enum Type
    {
        Slow,
        DoT,
        Stun,
        Amplify,
		Splash,
        Nothing
    }
    public Type typeOfDebuff;

    public float value;
    public float duration;
}
