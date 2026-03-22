using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPlayerPice1 : PlayerPice1
{
    void Start()
    {
        playerID = "Red"; // IMPORTANT
        playerColor = "Red";
    }

    private void OnMouseDown()
    {
        // ❗ Block click if it's not Red's turn
        if (GameManager.gm.currentDiceBoardColor != "Red")
            return;

        // Ready check
        if (!Ready)
        {
            MakePlayerReadyToMove(pathParent.RedPathPoint);
            return;
        }

        MovePlayer(pathParent.RedPathPoint);
    }
}
