using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardBrickController : MonoBehaviour
{
    
    public int hits = 0;

    //MARTIN: Así no porque afecta a todos los objetos
    //public static int hits = 0;

    //MARTIN: Hecho así porque no encontré la forma de hacerlo correctamente con GetComponentsInChildren
    private Transform spriteChild;
    private SpriteRenderer childRenderer;

    public Color colorFirstHit;
    public Color colorSecondHit;
    public Color colorThirdHit;

    void Start()
    {
        spriteChild = this.gameObject.transform.GetChild(0);
        childRenderer = spriteChild.GetComponent<SpriteRenderer>();
        
    }

    void Update()
    {

        if(hits == 1)
    {
        childRenderer.color = colorFirstHit;
    }

    if(hits == 2)
    {
        childRenderer.color = colorSecondHit;
    }

    if(hits == 3)
    {
        childRenderer.color = colorThirdHit;
    }
    }

    public void IncreaseHit()
    {
        hits++;
    }

}
