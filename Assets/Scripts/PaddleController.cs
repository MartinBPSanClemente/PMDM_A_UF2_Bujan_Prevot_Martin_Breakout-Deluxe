using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    //MARTIN sistema pills
    AudioSource sfx;
    SpriteRenderer sr;
    static public bool pillPurpleIsActive = false;
    
    const float MAX_X = 3.6f;
    const float MIN_X = -3.6f;
    //const float MAX_X = 3.1f;
    //const float MIN_X = -3.1f;

    [SerializeField] float speed;
    //MARTIN Ruido pills
    [SerializeField] AudioClip sfxPillPicked;
    //MARTIN Asociando con el script de la pelota para tener acceso a componentes para las pills
    [SerializeField] BallController ballControllerScript;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        float center = transform.position.x;

        float left = center-(sr.bounds.size.x)/2;
        float right = center+(sr.bounds.size.x)/2;

        if(left > MIN_X && Input.GetKey("left"))
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        else if (right < MAX_X && Input.GetKey("right"))
            transform.Translate(speed * Time.deltaTime, 0, 0);

        /*float x = transform.position.x;
        if(x > MIN_X && Input.GetKey("left"))
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        else if (x < MAX_X && Input.GetKey("right"))
            transform.Translate(speed * Time.deltaTime, 0, 0);*/        
    }

    void OnTriggerEnter2D(Collider2D other){

        if(other.tag == "pill-b"){
            sfx.clip = sfxPillPicked;
            sfx.Play();
            DestroyPill(other.gameObject);
            EffectBluePill();
        }

        if(other.tag == "pill-g"){
            sfx.clip = sfxPillPicked;
            sfx.Play();
            DestroyPill(other.gameObject);
            EffectGreenPill();
        }

        if(other.tag == "pill-p"){
            sfx.clip = sfxPillPicked;
            sfx.Play();
            DestroyPill(other.gameObject);
            EffectPurplePill();
        }
    }

    public static void DestroyPill(GameObject obj)
    {
        Destroy(obj);
    }

    public static void EffectBluePill()
    {
        GameController.UpdateScore(100);
        GameController.isDisplayingExtraLifeText = true;
        GUIController.UpdateScreenTextMessage("EXTRA POINTS!!!");
    }

    void EffectGreenPill()
    {
        ballControllerScript.RestartBallVelocity();
        GameController.isDisplayingExtraLifeText = true;
        GUIController.UpdateScreenTextMessage("SLOW BALL!!!");
    }

    void EffectPurplePill()
    {
        if(pillPurpleIsActive == false){
        pillPurpleIsActive = true;
        ballControllerScript.DoublePaddle();
        GameController.isDisplayingExtraLifeText = true;
        GUIController.UpdateScreenTextMessage("BIG PADDLE");
        }
        else
        {
        EffectBluePill();
        }
    }
}
