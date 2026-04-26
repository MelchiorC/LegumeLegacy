using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMechanicCategory", menuName = "Mechanics/Category")]
public class MechanicCategory : ScriptableObject
{
    public string categoryName;
    public List<MechanicEntry> entries;
}