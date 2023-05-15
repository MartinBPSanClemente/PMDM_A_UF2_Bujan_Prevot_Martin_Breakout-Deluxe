using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource sfx;

    int hitCount = 0;
    int brickCount;
    int sceneId;

    GameObject paddle;
    bool halved;

    [SerializeField] float force;
    [SerializeField] float forceInc;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;

    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;
    [SerializeField] AudioClip sfxGameOver;
    [SerializeField] AudioClip sfxNextLevel;

    //MARTIN: Sistema de pills
    [SerializeField] GameObject pillBlue;
    [SerializeField] GameObject pillGreen;
    [SerializeField] GameObject pillPurple;

    //Sistema partículas bloques
    [SerializeField] GameObject brickParticles;

    Dictionary<string, int> bricks = new Dictionary<string, int>{
        {"brick-y", 10},
        {"brick-g", 15},
        {"brick-a", 20},
        {"brick-r", 25},
        {"brick-pass", 25},
        //MARTIN
        {"brick-hard", 50}
    };


    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;

        sfx = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();

        GameObject.FindGameObjectWithTag("paddle");

        paddle = GameObject.FindWithTag("paddle");

        Invoke("LaunchBall", delay);
    }

    private void LaunchBall()
    {
        //reset position and velocity
        transform.position = Vector3.zero;
        rb.velocity = Vector2.zero;

        // get random direction
        float dirX, dirY = -1;
        dirX = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector2 dir = new Vector2(dirX, dirY);
        dir.Normalize();
        // aply force
        rb.AddForce(dir * force, ForceMode2D.Impulse);

    }

    //MARTIN: Para efecto pillGreen
    public void RestartBallVelocity()
    {
        Vector2 direction = rb.velocity.normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        hitCount = 0;
        Debug.Log("Restart speed");
    }

    private void OnCollisionEnter2D(Collision2D other){
        string tag = other.gameObject.tag;

        //MARTIN: Añadida 2a condicion para HardBrick
        if(bricks.ContainsKey(tag))
        {
            //MARTIN: Referenciando el script HardBrickController, ya que cada script es único en Unity y sacar el valor que interesa
            HardBrickController hitHardBrick = other.gameObject.GetComponent<HardBrickController>();
            if(tag == "brick-hard" && hitHardBrick.hits < 3){
                hitHardBrick.IncreaseHit();
                sfx.clip = sfxWall;
                sfx.Play();
            }
            else{
                DestroyBrick(other.gameObject);
            }
            
        }

        else if(tag == "paddle")
        {
            sfx.clip = sfxPaddle;
            sfx.Play();
            //paddle position
            Vector3 paddle = other.gameObject.transform.position;

            // get the contact point
            Vector2 contact = other.GetContact(0).point;

            if((rb.velocity.x < 0 && contact.x > (paddle.x + hitOffset)) ||
                (rb.velocity.x > 0 && contact.x < (paddle.x + hitOffset)))
                {
                    rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                }

                // increment the hit count
                hitCount++;
                if(hitCount %4 == 0)
                {
                    Debug.Log(hitCount + " -> Incremento velocidad");
                    rb.AddForce(rb.velocity.normalized * forceInc, ForceMode2D.Impulse);
                }
        }      

        else if(tag == "wall-top" || tag == "wall-lateral" || tag == "brick-rock")
        {
            sfx.clip = sfxWall;
            sfx.Play();

            if(!halved && tag == "wall-top")
            {
                //Reducir el tamaño de la pala
                HalvePaddle();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    
        //MARTIN: Separado if para parte de Game Over
        if(other.tag == "wall-bottom"){
            GameController.UpdateLifes(-1);
        }
        
        //MARTIN: Añadido && GameController.lifes > 0 para parte de Game Over
        if(other.tag == "wall-bottom" && GameController.lifes > 0)
        {
            sfx.clip = sfxFail;
            sfx.Play();

            //Restaurar la pala a su tamaño original
            Vector3 scale = paddle.transform.localScale;
            paddle.transform.localScale = new Vector3(1, scale.y, scale.z);
            halved = false;
            PaddleController.pillPurpleIsActive = false;
            
            Invoke("LaunchBall", delay);
        }

        //MARTIN: Para hacer el sonido de Game over
        if(other.tag == "wall-bottom" && GameController.lifes == 0)
        {
            sfx.clip = sfxGameOver;
            sfx.Play();
        }

        else if(other.tag == "brick-pass")
        {
            DestroyBrick(other.gameObject);
        }
    }

    void DestroyBrick(GameObject obj)
    {
            sfx.clip = sfxBrick;
            sfx.Play();

            //MARTIN: Sistema Pills
            float pillProb = Random.Range(1, 11);
            Debug.Log("Probabilidad Pill => " + pillProb*10 + "%");

            //update player score
            GameController.UpdateScore(bricks[obj.tag]);

            //MARTIN: Sistema Pills
            if(pillProb == 1){
                DropPill();
            }
            
            SpriteRenderer brickSR = obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Vector3 position = obj.transform.position;
            GameObject brickDestroyed = Instantiate(brickParticles, position, Quaternion.identity);
            var particleSystemMain = brickDestroyed.GetComponent<ParticleSystem>().main;
            particleSystemMain.startColor = brickSR.color;

            Destroy(obj);

            ++brickCount;
            if(brickCount == GameController.totalBricks[sceneId])
            {
                sfx.clip = sfxNextLevel;
                sfx.Play();

                rb.velocity = Vector2.zero;
                GetComponent<SpriteRenderer>().enabled = false;

                Invoke("NextScene", 3);
            }
    }

    void HalvePaddle()
    {
        //halved = halve;

        Vector3 scale = paddle.transform.localScale;
        if(halved==false){
            halved = true;
            paddle.transform.localScale = new Vector3(scale.x * 0.5f, scale.y, scale.z);
        }
    
    /*paddle.transform.localScale = halved ? 
        new Vector3(scale.x * 0.5f, scale.y, scale.z):
        new Vector3(scale.x * 2f, scale.y, scale.z);*/
    }

    public void DoublePaddle(){
        Vector3 scale = paddle.transform.localScale;
        paddle.transform.localScale = new Vector3(2, scale.y, scale.z);
        halved = false;
        //paddle.transform.localScale = new Vector3(scale.x * 2f, scale.y, scale.z);
    }

    void NextScene()
    {
        //MARTIN: Ahora que hay una escena final se puede volver al método original
        SceneManager.LoadScene(sceneId+1);

        /*int nextId = sceneId + 1;
        if(nextId == GameController.totalBricks.Count)
            nextId = 0;
        SceneManager.LoadScene(nextId);*/
    }

    //MARTIN: Sistema pills
    void DropPill(){
        Vector3 position = transform.position;

        float pillColorProb = Random.Range(1, 10);

        if(pillColorProb>=1 && pillColorProb<4){
            GameObject generatedPill = Instantiate(pillBlue, position, pillBlue.transform.rotation);
        }

        if(pillColorProb>=4 && pillColorProb<7){
            GameObject generatedPill = Instantiate(pillGreen, position, pillGreen.transform.rotation);
        }

        if(pillColorProb>=6 && pillColorProb<10){
            GameObject generatedPill = Instantiate(pillPurple, position, pillPurple.transform.rotation);
        }
    }
}
