using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneManager : MonoBehaviour
{
    public enum UIState
    {
        EggCrack,
        EggMachine,
        Recipe
    }

    public static MainSceneManager Instance = null;

    [SerializeField] GameObject eggPrefab;
    [SerializeField] Transform eggPool;
    [SerializeField] GameObject crackedEggPrefab;
    [SerializeField] Transform crackedEggPool;
    [SerializeField] EggCounter eggCounter;

    [SerializeField] RectTransform mainPanelOfCvsEggCrack;
    [SerializeField] RectTransform mainPanelOfCvsEggMachine;

    [SerializeField] Button zoneMakeEgg;

    UIState uiState = UIState.EggCrack;

    private bool isChangingUI;

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
        zoneMakeEgg.onClick.AddListener(() =>MakeEgg(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (uiState)
            {
                case UIState.EggCrack:
                    ChangeCvsEggCrackToEggMachine(0.5f);
                    break;
                case UIState.EggMachine:
                    ChangeCvsEggMachineToEggCrack(0.5f);
                    break;
                case UIState.Recipe:
                    break;
                default:
                    break;
            }
        }
    }
#endif

    public void ChangeCvsEggCrackToEggMachine(float changeTime)
    {
        if (isChangingUI) return;

        Sequence seq = DOTween.Sequence();

        mainPanelOfCvsEggCrack.anchoredPosition = new Vector2(0, 0);
        mainPanelOfCvsEggMachine.anchoredPosition = new Vector2(0, -1920);

        mainPanelOfCvsEggCrack.gameObject.SetActive(true);
        mainPanelOfCvsEggMachine.gameObject.SetActive(true);

        eggCounter.gameObject.SetActive(false);
        crackedEggPool.gameObject.SetActive(false);
        isChangingUI = true;

        seq.Append(mainPanelOfCvsEggCrack.DOAnchorPosY(1920, changeTime).SetEase(Ease.Linear));
        seq.Join(mainPanelOfCvsEggMachine.DOAnchorPosY(0, changeTime).SetEase(Ease.Linear));
        seq.OnComplete(() => {
            mainPanelOfCvsEggCrack.gameObject.SetActive(false);
            uiState = UIState.EggMachine;
            isChangingUI = false;
        });
    }

    public void ChangeCvsEggMachineToEggCrack(float changeTime)
    {
        if (isChangingUI) return;

        Sequence seq = DOTween.Sequence();

        mainPanelOfCvsEggCrack.anchoredPosition = new Vector2(0, 1920);
        mainPanelOfCvsEggMachine.anchoredPosition = new Vector2(0, 0);

        mainPanelOfCvsEggCrack.gameObject.SetActive(true);
        mainPanelOfCvsEggMachine.gameObject.SetActive(true);
        isChangingUI = true;

        seq.Append(mainPanelOfCvsEggCrack.DOAnchorPosY(0, changeTime).SetEase(Ease.Linear));
        seq.Join(mainPanelOfCvsEggMachine.DOAnchorPosY(-1920, changeTime).SetEase(Ease.Linear));
        seq.OnComplete(() => {
            crackedEggPool.gameObject.SetActive(true);
            eggCounter.gameObject.SetActive(true);
            mainPanelOfCvsEggMachine.gameObject.SetActive(false);
            uiState = UIState.EggCrack;
            isChangingUI = false;
        });

    }

    public void MakeEgg(Vector2 makePos)
    {
        if (isChangingUI)
            return;

        GameObject egg = PoolManager.GetItem<EggScript>().gameObject;
        egg.transform.position = makePos;
    }

    public void MakeCrackedEgg(Vector2 makePos)
    {
        GameObject egg = PoolManager.GetItem<BrokenEgg>().gameObject;
        egg.transform.position = makePos;
    }

}
