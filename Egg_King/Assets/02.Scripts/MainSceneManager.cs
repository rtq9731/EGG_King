using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance = null;

    [SerializeField] GameObject eggPrefab;
    [SerializeField] Transform eggPool;
    [SerializeField] GameObject crackedEggPrefab;
    [SerializeField] Transform crackedEggPool;
    [SerializeField] EggCounter eggCounter;

    [SerializeField] Button zoneMakeEgg;

    short crackedEggCount = 0;
    public short CrackedEggCount { get { return crackedEggCount; } set { crackedEggCount = value; eggCounter.UpdateText(crackedEggCount); } }

    private void Awake()
    {
        Instance = this;
        PoolManager.CreatePool<EggScript>(eggPrefab, eggPool);
        PoolManager.CreatePool<BrokenEgg>(crackedEggPrefab, crackedEggPool);
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        zoneMakeEgg.onClick.AddListener(() => MakeEgg(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }

    public void MakeEgg(Vector2 makePos)
    {
        GameObject egg = PoolManager.GetItem<EggScript>().gameObject;
        egg.transform.position = makePos;
    }

    public void MakeCrackedEgg(Vector2 makePos)
    {
        GameObject egg = PoolManager.GetItem<BrokenEgg>().gameObject;
        egg.transform.position = makePos;
    }

}
