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

        // 데이터 저장 경로 지정
        FilePath = Application.persistentDataPath + "/data.sav";

        // 싱글턴을 위한 Instance 생성
        Instance = this;

        // 풀 생성
        PoolManager.CreatePool<EggScript>(eggPrefab, eggPool);
        PoolManager.CreatePool<BrokenEgg>(crackedEggPrefab, crackedEggPool);
    }
    private void OnDestroy()
    {
        // 지워질 땐 싱글턴에서 삭제
        Instance = null;
    }

    private void Start()
    {
        // 깨진 계란을 생성해주는 버튼 초기화
        zoneMakeEgg.onClick.AddListener(() =>MakeEgg(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }

    /// <summary>
    /// 현재 MainSceneManager에 담긴 PlayerData 저장 
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
    /// 현재 저장 경로에있는 PlayerData 불러오기
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


#if UNITY_EDITOR // 전부 에디터에서 테스트용 코드
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
    /// EggCrackCvs에서 EggMachineCvs로 넘겨주는 함수
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
    /// EggMachineCvs에서 EggCrackCvs로 넘겨주는 함수
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
    /// makePos의 위치에 깨질 계란을 만들어줍니다.
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
    /// makePos의 위치에 이미 깨진 계란을 만들어줍니다.
    /// </summary>
    /// <param name="makePos"></param>
    public void MakeCrackedEgg(Vector2 makePos)
    {
        GameObject egg = PoolManager.GetItem<BrokenEgg>().gameObject;
        egg.transform.position = makePos;
    }

    /// <summary>
    /// 지금 CrackedEgg( -> 담겨있는 깨진 계란 )을 EggMachine으로 옮겨줍니다.
    /// </summary>
    /// <param name="CrackedEggCount"></param>
    public void CrackedEggToEggMachine(int CrackedEggCount)
    {

    }
}
