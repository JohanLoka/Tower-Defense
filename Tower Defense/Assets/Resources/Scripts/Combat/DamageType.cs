using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageType
{
    public enum Type
    {
        Fire,
        Frost,
        Lightning,
        Physical,
        Chaos
    }
    public Type typeOfDamge;
}
