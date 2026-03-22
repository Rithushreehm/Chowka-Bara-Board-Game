using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    AudioSource ads;
    void Start()
    {
        ads = GetComponent<AudioSource>();
    }
    
    public void playSound()
    {
        if(GameManager.gm.sound )
        {
        ads.Play();
        }
    }
   
}
