using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEgg : MonoBehaviour
{

    private void OnEnable()
    {
        MainSceneManager.Instance.CrackedEggCount++;
    }

    private void OnDisable()
    {
        MainSceneManager.Instance.CrackedEggCount--;
    }

}
