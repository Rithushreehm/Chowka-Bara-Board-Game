using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public PathObjectParent pathObjectParent;
    public List<PlayerPice1> playerPice1List = new List<PlayerPice1>();
    public PathPoint[] pathPointsToMoveOn;
    public string ownerPlayerID;  // red / blue / green / yellow
    public int baseIndex = -1;    // which BasePoint index this point represents

    Coroutine playerMovement;
    private void Start()
    {
        pathObjectParent = GetComponentInParent<PathObjectParent>();
    }

    public bool AddPlayerPice1(PlayerPice1 playerPice1_)
    {
        if(this.name == "CommonPathPoint")
        {
            AddPlayer(playerPice1_);
            complete(playerPice1_);
            return false;
        }
        
        if (!(pathObjectParent.safePoints.Contains(this)))
        {
        if (playerPice1List.Count == 1)
        {
            string prePlayerPice1Name = playerPice1List[0].name;
            string currentPlayerPice1Name = playerPice1_.name;
            currentPlayerPice1Name = currentPlayerPice1Name.Substring(0, currentPlayerPice1Name.Length - 4);
             if (!prePlayerPice1Name.Contains(currentPlayerPice1Name))
            {
                GameManager.gm.hasExtraChance = true;
                Debug.Log("KILL! Extra chance granted.");

                PlayerPice1 killedPawn = playerPice1List[0];

                killedPawn.Ready = false;
                PathPoint[] homePath = null;
                 switch (killedPawn.playerColor)
                {
                    case "Red":
                        homePath = killedPawn.pathParent.RedPathPoint;
                        break;
                    case "Green":
                        homePath = killedPawn.pathParent.GreenPathPoint;
                        break;
                    case "Blue":
                        homePath = killedPawn.pathParent.BluePathPoint;
                        break;
                    case "Yellow":
                        homePath = killedPawn.pathParent.YellowPathPoint;
                        break;
                }
                 StartCoroutine(killedPawn.ReverseToHome(homePath));


                killedPawn.numberOfStepsAlreadyMove = 0;

                RemovePlayerPice1(killedPawn);

// 🔥 After sending pawn home, check finish
            if (killedPawn.numberOfStepsAlreadyMove == killedPawn.GetMyPath().Length)
            {
                GameManager.gm.OnPlayerFinished(killedPawn.playerColor);
            }

            playerPice1List.Add(playerPice1_);
            return false;

             }
        }
    }
        AddPlayer(playerPice1_);
        return true;
    }
    IEnumerator revertOnStart(PlayerPice1 playerPice1)
    {
        int maxIndex = pathPointsToMoveOn.Length - 1;
        int steps = Mathf.Min(playerPice1.numberOfStepsAlreadyMove, pathPointsToMoveOn.Length);
        for (int i = steps - 1; i >= 0; i--)
    {
        if (i >= 0 && i <= maxIndex)
        {
            playerPice1.transform.position = pathPointsToMoveOn[i].transform.position;
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            Debug.LogWarning($"Step index {i} is out of bounds for pathPointsToMoveOn.");
        }
    }

    int baseIndex = basepointPosition(playerPice1.name);
    if (baseIndex >= 0 && baseIndex < pathObjectParent.BasePoint.Length)
    {
        playerPice1.transform.position = pathObjectParent.BasePoint[baseIndex].transform.position;
        playerPice1.previousPathPoint = pathObjectParent.BasePoint[baseIndex];
        playerPice1.currentPathPoint = pathObjectParent.BasePoint[baseIndex];
        playerPice1.Ready = false;
    }
    else
    {
        Debug.LogError($"BasePoint not found for player name: {playerPice1.name}");
    }

     if (playerMovement != null)
    {
        StopCoroutine(playerMovement);
        playerMovement = null;
    }
}
    void complete(PlayerPice1 playerPice1_)
    {
        int totalCompletePlayer;
        if(playerPice1_.name.Contains("Red")){totalCompletePlayer = GameManager.gm.redCompletePlayer += 1;}
        else if(playerPice1_.name.Contains("Green")){totalCompletePlayer = GameManager.gm.greenCompletePlayer += 1;}
        else if(playerPice1_.name.Contains("Blue")){totalCompletePlayer = GameManager.gm.blueCompletePlayer += 1;}
        else {totalCompletePlayer = GameManager.gm.yellowCompletePlayer += 1;}

        if(totalCompletePlayer == 4)
        {
            
        }
    }
    
    int basepointPosition(string name)
    {
        for (int i = 0; i < pathObjectParent.BasePoint.Length; i++)
        {
            if(pathObjectParent.BasePoint[i].name == name)
            {
                return i;
            }
        }
        return -1;
    }

    public void AddPlayer(PlayerPice1 playerPice1)
    {
        playerPice1List.Add(playerPice1);
        rescaleAndRepositingAllPlayerPice1();
    }

    public void RemovePlayerPice1(PlayerPice1 playerPice1)
    {
        if (playerPice1List.Contains(playerPice1))
        {
            playerPice1List.Remove(playerPice1);
            rescaleAndRepositingAllPlayerPice1();
        }
    }

    
        // set sprite order (defensive)
       public void rescaleAndRepositingAllPlayerPice1()
{
    int placeCount = playerPice1List.Count;

    if (placeCount == 0) return;   // FIX

    bool isOdd = (placeCount % 2) == 1;

    int extent = placeCount / 2;
    int counter = 0;
    int SpriteLayer = 0;

    if (isOdd)
    {
        for (int i = 0; i <= extent; i++)
        {
            playerPice1List[counter].transform.localScale =
                new Vector3(pathObjectParent.scales[placeCount - 1],
                            pathObjectParent.scales[placeCount - 1], 0f);

            playerPice1List[counter].transform.position =
                new Vector3(transform.position.x +
                            (i * pathObjectParent.positionDifference[placeCount - 1]),
                            transform.position.y, 0f);

            counter++;
        }
    }
    else
    {
        for (int i = 0; i < extent; i++)
        {
            playerPice1List[counter].transform.localScale =
                new Vector3(pathObjectParent.scales[placeCount - 1],
                            pathObjectParent.scales[placeCount - 1], 0f);

            playerPice1List[counter].transform.position =
                new Vector3(transform.position.x +
                            (i * pathObjectParent.positionDifference[placeCount - 1]),
                            transform.position.y, 0f);

            counter++;
        }
    }

    for (int i = 0; i < playerPice1List.Count; i++)
    {
        playerPice1List[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = SpriteLayer;
        SpriteLayer++;
    }
}


}