using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCounter : MonoBehaviour
{
    public void UpdateText(int count)
    {
        GetComponent<Text>().text = $"��� ���� : {count} / 30";
    }
}
