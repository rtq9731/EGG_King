using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance = null;

    [SerializeField] GameObject eggPrefab;
    [SerializeField] Transform eggPool;
    [SerializeField] GameObject crackedEggPrefab;
    [SerializeField] Transform crackedEggPool;
    [SerializeField] EggCounter eggCounter;

    [SerializeField] RectTransform mainPanelOfCvsEggCrack;
    [SerializeField] RectTransform mainPanelOfCvsEggMachine;

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

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeCvsEggCrackToEggMachine(0.5f);
    }
#endif

    public void ChangeCvsEggCrackToEggMachine(float changeTime)
    {
        Sequence seq = DOTween.Sequence();

        mainPanelOfCvsEggCrack.anchoredPosition = new Vector2(0, 0);
        mainPanelOfCvsEggMachine.anchoredPosition = new Vector2(0, -1920);
        mainPanelOfCvsEggCrack.gameObject.SetActive(true);
        mainPanelOfCvsEggMachine.gameObject.SetActive(true);

        eggCounter.gameObject.SetActive(false);
        seq.Append(mainPanelOfCvsEggCrack.DOAnchorPosY(1920, changeTime).SetEase(Ease.Linear));
        seq.Join(mainPanelOfCvsEggMachine.DOAnchorPosY(0, changeTime).SetEase(Ease.Linear));
        seq.OnComplete(() => mainPanelOfCvsEggCrack.gameObject.SetActive(false));
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
