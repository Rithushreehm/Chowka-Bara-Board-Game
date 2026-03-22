using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainPanel;
    public GameObject GamePanel;
    public void OnMouseDown()
    {
        mainPanel.SetActive(true);
        GamePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
   
}
