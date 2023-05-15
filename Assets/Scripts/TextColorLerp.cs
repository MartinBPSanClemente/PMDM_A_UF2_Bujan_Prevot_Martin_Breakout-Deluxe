using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorLerp : MonoBehaviour
{
    [SerializeField] Text msg;
    [SerializeField] float duration;

    void Start()
    {
        StartCoroutine("ChangeColor");
    }

    IEnumerator ChangeColor()
    {
        float t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;
            msg.color = Color.Lerp(Color.black, Color.white, t/duration);
            yield return null;
        }
        // reiniciar corutina
        StartCoroutine("ChangeColor");
    }
}
