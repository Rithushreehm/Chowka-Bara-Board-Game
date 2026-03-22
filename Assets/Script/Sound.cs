using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public GameObject play;
    public GameObject mute;
    public void OnMouseDown()
    {
        if(GameManager.gm.ads)
        {
            GameManager.gm.sound = false;
            play.SetActive(false);
            mute.SetActive(true);

        }
        else
        {
             GameManager.gm.sound = true;
             play.SetActive(true);
             mute.SetActive(false);
        }
    }
}
