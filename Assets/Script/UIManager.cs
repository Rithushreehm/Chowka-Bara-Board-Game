using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject GamePanel;
    public List<GameObject> playerHomes;


    public void Game1()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);

        GameManager gm = GameManager.gm;

        gm.totalPlayerCanPlay = 2;

        // Store original references
        var red   = gm.rollingDices[0];
        var blue  = gm.rollingDices[2];

        // Enable only Red + Blue
        red.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);

        gm.rollingDices[1].gameObject.SetActive(false);
        gm.rollingDices[3].gameObject.SetActive(false);

        gm.playerHomes[0].SetActive(true);
        gm.playerHomes[2].SetActive(true);

        gm.playerHomes[1].SetActive(false);
        gm.playerHomes[3].SetActive(false);

        gm.rollingDices = new List<RollingDice>() { red, blue };
        gm.diceColors   = new List<string>() { "Red", "Blue" };

        gm.isBlueAI = false;
        gm.blueAIDice = null;

        gm.rollingDice = red;
        gm.currentDiceBoardColor = "Red";
    }

    public void Game2()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);

        GameManager gm = GameManager.gm;

        gm.totalPlayerCanPlay = 3;

        // Store original references
        var red   = gm.rollingDices[0];
        var green = gm.rollingDices[1];
        var blue  = gm.rollingDices[2];

        // Enable them
        red.gameObject.SetActive(true);
        green.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);

        // Disable yellow
        gm.rollingDices[3].gameObject.SetActive(false);
        gm.playerHomes[3].SetActive(false);

        gm.rollingDices = new List<RollingDice>() { red, green, blue };
        gm.diceColors = new List<string>() { "Red", "Green", "Blue" };

        gm.isBlueAI = false;
        gm.blueAIDice = null;

        gm.rollingDice = red;
        gm.currentDiceBoardColor = "Red";
    }

    public void Game3()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);

        GameManager gm = GameManager.gm;

        gm.totalPlayerCanPlay = 4;

        var red    = gm.rollingDices[0];
        var green  = gm.rollingDices[1];
        var blue   = gm.rollingDices[2];
        var yellow = gm.rollingDices[3];

        red.gameObject.SetActive(true);
        green.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);
        yellow.gameObject.SetActive(true);

        gm.rollingDices = new List<RollingDice>() { red, green, blue, yellow };
        gm.diceColors = new List<string>() { "Red", "Green", "Blue", "Yellow" };

        gm.isBlueAI = false;
        gm.blueAIDice = null;

        gm.rollingDice = red;
        gm.currentDiceBoardColor = "Red";
    }

    public void Game4()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);

        GameManager gm = GameManager.gm;

        gm.totalPlayerCanPlay = 2;  // FIXED — must be 2 (Red + Blue AI)

        var red  = gm.rollingDices[0];
        var blue = gm.rollingDices[2];

        red.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);

        gm.rollingDices[1].gameObject.SetActive(false);
        gm.rollingDices[3].gameObject.SetActive(false);

        gm.playerHomes[1].SetActive(false);
        gm.playerHomes[3].SetActive(false);

        gm.rollingDices = new List<RollingDice>() { red, blue };
        gm.diceColors = new List<string>() { "Red", "Blue" };

        gm.isBlueAI = true;
        gm.blueAIDice = blue;

        gm.rollingDice = red;
        gm.currentDiceBoardColor = "Red";
    }

}
