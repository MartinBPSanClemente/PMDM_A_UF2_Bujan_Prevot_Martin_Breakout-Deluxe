using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{

    [SerializeField] Text txtScore;
    [SerializeField] Text txtLifes;
    [SerializeField] Text _txtMessage;
    [SerializeField] Text _txtCredits;
    public static Text txtMessage;
    public static Text txtCredits;

private void Awake(){
    txtMessage = _txtMessage;
    txtCredits = _txtCredits;
}

private void OnGUI()
    {
        txtScore.text = string.Format("{0,3:D3}", 
            GameController.score);
        txtLifes.text = GameController.lifes.ToString();
    }

public static void UpdateScreenTextMessage(string text){
    txtMessage.text = text;
}

public static void UpdateScreenTextCreditsMessage(string text){
    txtCredits.text = text;
}

}
