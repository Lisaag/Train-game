using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassengerProperties", menuName = "ScriptableObjects/PassengerPropertiesScriptableObject", order = 1)]
public class PassengerProperties : ScriptableObject
{
    [Header("Sprites")]
    [SerializeField] public Sprite confused;
    [SerializeField] public Sprite suspicious;
    [SerializeField] public Sprite highAlert;

}
