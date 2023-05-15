using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBrickController : MonoBehaviour
{
    const float MAX_X = 3.1f;

    const float MIN_X = -3.1f;

    bool turn = true;

    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        if(turn){
            moveBrickRight();
        }
        if(!turn){
            moveBrickLeft();
        }
        if(transform.position.x >=MAX_X){
            turn = false;
        }
        if(transform.position.x <=MIN_X){
            turn = true;
        }
 
    }

    void moveBrickRight(){
        transform.Translate(speed*Time.deltaTime,0,0);
    }

    void moveBrickLeft(){
        transform.Translate(-speed*Time.deltaTime,0,0);
    }
}
