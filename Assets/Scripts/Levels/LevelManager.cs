using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> carts = new List<GameObject>();

    private void Start()
    {
        GameManager.OnLevelFinished += GameManager_OnLevelFinished;
        GameManager.OnChangeLevel += GameManager_OnChangeLevel;
    }

    private void GameManager_OnChangeLevel()
    {
        for(int i = 0; i < carts.Count; i++)
        {
            for(int j = 0; j < carts[i].transform.childCount; j++)
            {
                Debug.Log("Set car " + i + "passenger " + j);
                carts[i].transform.GetChild(j).gameObject.SetActive(GameManager.Instance._levels[GameManager.Instance.currentLevel].passengers[i].myList[j]);
            }
        }
    }

    private void GameManager_OnLevelFinished()
    {
        if (GameManager.Instance.currentLevel + 1 >= GameManager.Instance._levels.Count) return;
    }

}
