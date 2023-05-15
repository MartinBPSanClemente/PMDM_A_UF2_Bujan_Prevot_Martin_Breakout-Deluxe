using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //MARTIN
using UnityEngine.SceneManagement; //MARTIN

public class GameController : MonoBehaviour
{
    //MARTIN: Añadido para Restart Game
    static AudioSource sfx;
    //MARTIN: Añadido para sonido Restart
    [SerializeField] AudioClip sfxRestart;
    [SerializeField] AudioClip sfxGameOver;
    [SerializeField] AudioClip sfxVictory;
    [SerializeField] AudioClip sfxExtraLife;
    [SerializeField] AudioClip sfxCountPoints;

    public static int score {get; private set; } = 0;
    public static int lifes {get; private set; } = 3;
    public static int extraLifes {get; private set; } = 0;
    public static int extraLifeTextTimer = 0;
    public static bool isDisplayingExtraLifeText = false;

    //MARTIN: Añadido un valor por cada nivel a List
    public static List<int> totalBricks = new List<int> { 0, 32, 32, 33, 17 };

    public static void UpdateLifes(int numLifes) {lifes += numLifes; }

    //MARTIN: Modificación para pausa
    bool paused;

    //MARTIN: Modificación para victoria
    int sceneId;

    //MARTIN: Método para el audio
    void Start()
    {
        sfx = GetComponent<AudioSource>();
        //MARTIN: Modificación para victoria
        sceneId = SceneManager.GetActiveScene().buildIndex;

        if(sceneId == 5)
        {
            sfx.clip = sfxVictory;
            sfx.Play();
            Invoke("RecursiveCountLifesForScore", sfxVictory.length);
            Invoke("FinalMessage", 7);
            Invoke("RecursiveCredits", 8);

        }
    }

    void RecursiveCountLifesForScore(){
        if(lifes > 0)
        {
            lifes--;
            score+=200;
            sfx.clip = sfxCountPoints;
            sfx.Play();
            if(lifes > 0)
            {
                Invoke("RecursiveCountLifesForScore", 1);
            }
        }
    }

    void FixedUpdate(){
        if(isDisplayingExtraLifeText == true){
            extraLifeTextTimer++;
            if(extraLifeTextTimer > 100){
                GUIController.UpdateScreenTextMessage("");
                isDisplayingExtraLifeText = false;
                extraLifeTextTimer = 0;
            }
        }
    }

    void Update(){
        //MARTIN: lifes > 0 para evitar su uso en Game Over
        if(Input.GetKeyDown(KeyCode.P) && lifes > 0 && sceneId!=5){
            if(paused)
                ResumeGame();

            else
                PauseGame();
        }
        //MARTIN: Modificacion para Game Over
        if(lifes == 0 && sceneId!=5)
        {
            GUIController.UpdateScreenTextMessage("GAME OVER\nPRESS <ENTER> TO RESTART\nPRESS <ESC> TO QUIT");
            if(Input.GetKeyDown(KeyCode.Return))
            {
                sfx.clip = sfxRestart;
                sfx.Play();
                score = 0;
                lifes = 3;
                extraLifes = 0;
                StartCoroutine("StartNextGame");
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
        //MARTIN: Hasta aquí modificación Game Over

        //MARTIN: Modificacion para Victoria
        if(sceneId == 5)
        {
            //GUIController.UpdateScreenTextMessage("YOU FINISHED BREAKOUT DELUXE\nYOUR SCORE:" + score + "\nPRESS <ENTER> TO TRY TO\n BEAT YOUR SCORE\nPRESS <ESC> TO QUIT");

            if(Input.GetKeyDown(KeyCode.Return))
            {
                sfx.clip = sfxRestart;
                sfx.Play();
                score = 0;
                lifes = 3;
                StartCoroutine("StartNextGame");
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    public static void UpdateScore(int points)
    {
        score += points;
        if(Mathf.Floor(score/500) > extraLifes)
        {
            ExtraLife();
        }
    }

    IEnumerator StartNextGame()
    {
        float t = 0;
        while(t < sfxRestart.length)
        {
            t += Time.deltaTime;
            
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
            
    //MARTIN: Modificación para pausa
    void PauseGame()
    {
        paused = true;
        GUIController.UpdateScreenTextMessage("PAUSED\nPRESS <P> TO RESUME");
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        paused = false;
        GUIController.UpdateScreenTextMessage("");
        Time.timeScale = 1;
    }
    //MARTIN: Hasta aquí modificacion para pausa
    
    static void ExtraLife()
    {
        lifes++;
        extraLifes++;
        //sfx.clip = sfxExtraLife;
        //sfx.Play();
        isDisplayingExtraLifeText = true;
        GUIController.UpdateScreenTextMessage("EXTRA LIFE!!!");
        
    }

    void FinalMessage()
    {
        GUIController.UpdateScreenTextMessage("YOU FINISHED BREAKOUT DELUXE\nYOUR SCORE:" + score + "\nPRESS <ENTER> TO TRY TO\n BEAT YOUR SCORE\nPRESS <ESC> TO QUIT");
    }

    void CreditsFirst()
    {
        GUIController.UpdateScreenTextCreditsMessage("BASED ON ARKANOID");
    }

    void CreditsSecond()
    {
        GUIController.UpdateScreenTextCreditsMessage("ORIGINALLY DEVELOPED BY TAITO");
    }

    void CreditsThird()
    {
        GUIController.UpdateScreenTextCreditsMessage("FIRST VERSION BY\n FRANCISCO GABRIEL MONTOIRO CABADA");
    }

    void CreditsFourth()
    {
        GUIController.UpdateScreenTextCreditsMessage("FINAL VERSION BY\n MARTIN BUJAN PREVOT");
    }

    void RecursiveCredits()
    {
        Invoke("CreditsFirst", 2);
        Invoke("CreditsSecond", 4);
        Invoke("CreditsThird", 6);
        Invoke("CreditsFourth", 8);
    }

    IEnumerator TimeScreenTextMessage()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime;
            
            yield return null;
        }

        GUIController.UpdateScreenTextMessage("");
    }
}

