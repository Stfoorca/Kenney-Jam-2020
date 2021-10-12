using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TalkingSword : MonoBehaviour
{
    public List<SwordText> textLines;
    public TMP_Text talkText;

    public GameObject talkObject;

    public float textDuration;
    public Vector3 offset;
    public Transform buzkaTransform;
    
    public void ShowText()
    {
        talkText.text = textLines[Random.Range(0, textLines.Count)].text;
        
        Vector3 viewPortTalkObject = Camera.main.WorldToViewportPoint(buzkaTransform.transform.position);
        if (viewPortTalkObject.y > 0.5)
        {
            talkObject.transform.position = buzkaTransform.position + new Vector3(Random.Range(-0.05f, 0.05f)+offset.x, -offset.y, offset.z);
            talkObject.transform.localScale = new Vector3(1, -1, 1);
            talkText.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            talkObject.transform.position = buzkaTransform.position + new Vector3(Random.Range(-0.05f, 0.05f)+offset.x, offset.y, offset.z);
            talkObject.transform.localScale = new Vector3(1, 1, 1);
            talkText.transform.localScale = new Vector3(1, 1, 1);
        }

        StartCoroutine(SetActiveForSeconds(textDuration));
    }

    private IEnumerator SetActiveForSeconds(float time)
    {
        talkObject.SetActive(true);
        yield return new WaitForSeconds(time);
        talkObject.SetActive(false);
    }
}
