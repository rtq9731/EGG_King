using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    public static Dictionary<string, object> pool = new Dictionary<string, object>();
    public static Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    /// <summary>
    /// 필요한 Pool을 만들어줍니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefab">만들 프리팹</param>
    /// <param name="parent">부모 Transform</param>
    /// <param name="count">기본적인 갯수</param>
    public static void CreatePool<T>(GameObject prefab, Transform parent, int count = 5)
    {
        Queue<T> q = new Queue<T>();
        for (int i = 0; i < count; i++)
        {
            GameObject temp = GameObject.Instantiate(prefab, parent);
            temp.SetActive(false); // 만들어진 오브젝트를 꺼진 상태로 만들어줌.
            q.Enqueue(temp.GetComponent<T>()); // Queue에 추가해줌. 
        }

        string key = typeof(T).ToString(); // 이 키에는 T 이름이 들어갈 것.
        pool.Add(key, q); // 딕셔너리에는 ( T 의 이름 ) 키 값으로 Queue<T>를 추가한다
        prefabDictionary.Add(key, prefab); // 그리고 추가적으로 더 만들 수도 있으니 prefab도 저장.
    }

    /// <summary>
    /// T를 가지고 있는 오브젝트를 알맞은 Pool에서 뽑아줍니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetItem<T>() where T : MonoBehaviour
    {
        string key = typeof(T).ToString(); // T에서 key값을 만들기 위해 T.ToString
        T item = null; // 돌려줄 item을 초기화한다.

        if (pool.ContainsKey(key)) // 만약 key값에 맞는 pool이 존재한다면
        {
            Queue<T> q = (Queue<T>)pool[key]; // pool Dictionary에서 Queue<T> 를 불러온다. 기본적으로 Object 형식의 Dictinary기 때문에, T로 형변환 해준다.
            T firstItem = q.Peek(); // pool의 첫번째 obj를 확인하기 위해 불러옴.

            if (firstItem.gameObject.activeSelf) // firstItem이 활성화 (사용중) 이라면 모든 아이템이 사용중인것.
            {
                GameObject prefab = prefabDictionary[key]; // 프리펩을 데려온다.
                GameObject g = GameObject.Instantiate(prefab, firstItem.transform.parent); // 프리펩을 만든다.
                item = g.GetComponent<T>(); // 돌려줄 item 에 게임오브젝트를 준다.
            }
            else // 그렇지 않다면 사용중이지 않은 것이니, 큐에서 빼서 돌려줌.
            {
                item = q.Dequeue();
                item.gameObject.SetActive(true); // 활성화
            }

            q.Enqueue(item); // 다시 큐의 맨 마지막으로 돌려줌.
        }

        return item; // 큐에서 빼서 돌려줌
    }

}
