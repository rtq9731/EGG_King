using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCounter : MonoBehaviour
{
    public void UpdateText(int count)
    {
        GetComponent<Text>().text = $"°è¶õ °¹¼ö : {count} / 30";
    }
}
