using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Text;
using System.IO;

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

    PlayerData data = new PlayerData();

    private bool isChangingUI;

    short crackedEggCount = 0;
    public short CrackedEggCount { get { return crackedEggCount; } set { crackedEggCount = value; eggCounter.UpdateText(crackedEggCount); } }

    string FilePath;

    private void Awake()
    {

#if UNITY_EDITOR
        Debug.Log(Application.persistentDataPath);
#endif

        // ������ ���� ��� ����
        FilePath = Application.persistentDataPath + "/data.sav";

        // �̱����� ���� Instance ����
        Instance = this;

        // Ǯ ����
        PoolManager.CreatePool<EggScript>(eggPrefab, eggPool);
        PoolManager.CreatePool<BrokenEgg>(crackedEggPrefab, crackedEggPool);
    }
    private void OnDestroy()
    {
        // ������ �� �̱��Ͽ��� ����
        Instance = null;
    }

    private void Start()
    {
        // ���� ����� �������ִ� ��ư �ʱ�ȭ
        zoneMakeEgg.onClick.AddListener(() =>MakeEgg(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }

    /// <summary>
    /// ���� MainSceneManager�� ��� PlayerData ���� 
    /// </summary>
    public void SaveGame()
    {
        string jsonString = JsonUtility.ToJson(data);
        FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

        sw.Write(jsonString);

        sw.Close();
        fs.Close();
    }

    /// <summary>
    /// ���� ���� ��ο��ִ� PlayerData �ҷ�����
    /// </summary>
    public void LoadGame()
    {
        if(!File.Exists(FilePath))
        {
            SaveGame();
            return;
        }

        FileStream fs = new FileStream(FilePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

#if UNITY_EDITOR
        Debug.Log(sr.ReadToEnd());
#endif

        JsonUtility.FromJson<PlayerData>(sr.ReadToEnd());

        sr.Close();
        fs.Close();
    }


#if UNITY_EDITOR // ���� �����Ϳ��� �׽�Ʈ�� �ڵ�
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

        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
            Debug.Log(JsonUtility.ToJson(data));
        }
        else if( Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }
#endif

    /// <summary>
    /// EggCrackCvs���� EggMachineCvs�� �Ѱ��ִ� �Լ�
    /// </summary>
    /// <param name="changeTime"></param>
    public void ChangeCvsEggCrackToEggMachine(float changeTime)
    {
        if (isChangingUI) return;

        Sequence seq = DOTween.Sequence();

        zoneMakeEgg.interactable = false;

        mainPanelOfCvsEggCrack.anchoredPosition = new Vector2(0, 0);
        mainPanelOfCvsEggMachine.anchoredPosition = new Vector2(0, -1920);

        mainPanelOfCvsEggCrack.gameObject.SetActive(true);
        mainPanelOfCvsEggMachine.gameObject.SetActive(true);

        eggCounter.gameObject.SetActive(false);
        crackedEggPool.gameObject.SetActive(false);
        eggPool.gameObject.SetActive(false);
        isChangingUI = true;

        seq.Append(mainPanelOfCvsEggCrack.DOAnchorPosY(1920, changeTime).SetEase(Ease.Linear));
        seq.Join(mainPanelOfCvsEggMachine.DOAnchorPosY(0, changeTime).SetEase(Ease.Linear));
        seq.OnComplete(() => {
            mainPanelOfCvsEggCrack.gameObject.SetActive(false);
            uiState = UIState.EggMachine;
            isChangingUI = false;
        });
    }

    /// <summary>
    /// EggMachineCvs���� EggCrackCvs�� �Ѱ��ִ� �Լ�
    /// </summary>
    /// <param name="changeTime"></param>
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
            zoneMakeEgg.interactable = true;
            eggPool.gameObject.SetActive(true);
        });

    }

    /// <summary>
    /// makePos�� ��ġ�� ���� ����� ������ݴϴ�.
    /// </summary>
    /// <param name="makePos"></param>
    public void MakeEgg(Vector2 makePos)
    {
        if (isChangingUI)
            return;

        GameObject egg = PoolManager.GetItem<EggScript>().gameObject;
        egg.transform.position = makePos;
    }

    /// <summary>
    /// makePos�� ��ġ�� �̹� ���� ����� ������ݴϴ�.
    /// </summary>
    /// <param name="makePos"></param>
    public void MakeCrackedEgg(Vector2 makePos)
    {
        GameObject egg = PoolManager.GetItem<BrokenEgg>().gameObject;
        egg.transform.position = makePos;
    }

    /// <summary>
    /// ���� CrackedEgg( -> ����ִ� ���� ��� )�� EggMachine���� �Ű��ݴϴ�.
    /// </summary>
    /// <param name="CrackedEggCount"></param>
    public void CrackedEggToEggMachine(int CrackedEggCount)
    {

    }
}
