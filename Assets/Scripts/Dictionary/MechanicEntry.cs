using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMechanicEntry", menuName = "Mechanics/Entry")]
public class MechanicEntry : ScriptableObject
{
    public string mechanicName;
    [TextArea] public string mechanicDescription;
    public Sprite mechanicImage;
}
