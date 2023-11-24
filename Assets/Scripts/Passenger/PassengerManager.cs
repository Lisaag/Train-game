using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    [SerializeField] PassengerProperties _passengerPropertiesSerialized = null;
    public static PassengerProperties passengerProperties = null;

    private void Awake()
    {
        passengerProperties = _passengerPropertiesSerialized; 
    }
}
