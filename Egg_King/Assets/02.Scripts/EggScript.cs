using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{

    [SerializeField] Transform eggMakeTr;
    public void CreateCrackedEgg()
    {
        MainSceneManager.Instance.MakeCrackedEgg(eggMakeTr.position);
    }

    public void ActFalse()
    {
        this.gameObject.SetActive(false);
    }

}
