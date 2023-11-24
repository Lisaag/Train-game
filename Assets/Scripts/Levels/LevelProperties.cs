using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelProperties", menuName = "ScriptableObjects/LevelPropertiesScriptableObject", order = 1)]
public class LevelProperties : ScriptableObject
{
    [SerializeField] public float levelDuration = 0f;

    [SerializeField] public List<ListWrapper<bool>> passengers = new List<ListWrapper<bool>>();

    [System.Serializable]
    public class ListWrapper<T>
    {
        public List<T> myList;
    }
}
