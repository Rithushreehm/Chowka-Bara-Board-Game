using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPice1 : MonoBehaviour
{
    public bool isReady = true;
    public bool Ready;
    public int numberOfStepsAlreadyMove;
    public int numberOfStepsToMove;
    public PathObjectParent pathParent;
    public string playerID;
    public string playerColor; 
    Coroutine playerMovement;
    public PathPoint previousPathPoint;
    public PathPoint currentPathPoint;
    public List<PathPoint> movementHistory = new List<PathPoint>();


    private void Awake()
    {
        pathParent = FindObjectOfType<PathObjectParent>();
        if (pathParent == null)
        {
            Debug.LogError("PathObjectParent not found in scene!");
        }
    }
    public PathPoint[] GetMyPath()
{
    switch (playerColor)
    {
        case "Red": return pathParent.RedPathPoint;
        case "Green": return pathParent.GreenPathPoint;
        case "Blue": return pathParent.BluePathPoint;
        case "Yellow": return pathParent.YellowPathPoint;
    }

    Debug.LogError("Invalid playerColor on pawn: " + playerColor);
    return null;
}

    // inside PlayerPice1 class (make these public)
  public bool CanMoveStepsPublic(int steps)
{
    return isPathPointAvailableToMove(
        steps,
        numberOfStepsAlreadyMove,
        GetMyPath()
    );
}


public void MoveStepsPublic(int steps)
{
    numberOfStepsToMove = steps;
    MovePlayer(GetMyPath());
}

IEnumerator AIMoveAndNotify()
{
    GameManager.gm.isPawnMoving = true;
    yield return StartCoroutine(MoveSteps(GetMyPath()));
    GameManager.gm.isPawnMoving = false;
    GameManager.gm.OnPawnMoved();
}


    public void MakePlayerReadyToMove(PathPoint[] pathParent_)
    {
        if (pathParent_ == null || pathParent_.Length == 0)
        {
            Debug.LogError("Invalid pathParent_ passed to MakePlayerReadyToMove");
            return;
        }

        Ready = true;
        numberOfStepsAlreadyMove = 1;

        previousPathPoint = pathParent_[0];
        currentPathPoint = pathParent_[0];

        if (currentPathPoint != null)
        {
            currentPathPoint.AddPlayerPice1(this);
            GameManager.gm.AddPathPoint(currentPathPoint);
        }
        else
        {
            Debug.LogError("currentPathPoint is null in MakePlayerReadyToMove");
        }
    }

    public void MovePlayer(PathPoint[] pathParent_)
    {
        if (pathParent_ == null || pathParent_.Length == 0)
        {
            Debug.LogError("Invalid pathParent_ passed to MovePlayer");
            return;
        }

        playerMovement = StartCoroutine(MoveSteps(pathParent_));
    }

   IEnumerator MoveSteps(PathPoint[] pathParent_)
{
    // LOCK dice when pawn starts moving
    GameManager.gm.isPawnMoving = true;
    GameManager.gm.canDiceRoll = false;
    GameManager.gm.canPlayerMove = false;

    numberOfStepsToMove = GameManager.gm.numberOfStepsToMove;

    if (pathParent_ == null)
    {
        Debug.LogError("pathParent_ is null in MoveSteps");
        // ensure we release locks & transfer turn so game doesn't hang
        GameManager.gm.isPawnMoving = false;
        GameManager.gm.canDiceRoll = true;
        GameManager.gm.canPlayerMove = false;
        yield break;
    }

    // If the pawn CANNOT move at all with this roll (overshoot),
    // immediately transfer the turn instead of silently exiting.
    if (!isPathPointAvailableToMove(numberOfStepsToMove, numberOfStepsAlreadyMove, pathParent_))
    {
        Debug.Log("Pawn cannot move (overshoot). Transferring turn.");
        GameManager.gm.OnPawnMoved();             // triggers transfer logic
        GameManager.gm.isPawnMoving = false;
        GameManager.gm.canDiceRoll = true;
        GameManager.gm.canPlayerMove = false;
        yield break;
    }

    for (int i = numberOfStepsAlreadyMove; i < (numberOfStepsAlreadyMove + numberOfStepsToMove); i++)
    {
        // Re-check safety inside loop in case something changed
        if (!isPathPointAvailableToMove(numberOfStepsToMove, numberOfStepsAlreadyMove, pathParent_))
        {
            Debug.LogWarning("Path not available to move at index " + i + " — transferring turn.");
            GameManager.gm.OnPawnMoved();
            GameManager.gm.isPawnMoving = false;
            GameManager.gm.canDiceRoll = true;
            GameManager.gm.canPlayerMove = false;
            yield break;
        }
        

        if (pathParent_[i] == null)
        {
            Debug.LogError("PathPoint at index " + i + " is null!");
            // transfer turn so game continues instead of hanging.
            GameManager.gm.OnPawnMoved();
            GameManager.gm.isPawnMoving = false;
            GameManager.gm.canDiceRoll = true;
            GameManager.gm.canPlayerMove = false;
            yield break;
        }

        transform.position = pathParent_[i].transform.position;

        if (GameManager.gm.sound)
        {
            GameManager.gm.ads.Play();
        }

        movementHistory.Add(pathParent_[i]);
        yield return new WaitForSeconds(0.35f);
    }

    // Normal finish of movement
    if (isPathPointAvailableToMove(numberOfStepsToMove, numberOfStepsAlreadyMove, pathParent_))
    {
        numberOfStepsAlreadyMove += numberOfStepsToMove;
        GameManager.gm.numberOfStepsToMove = 0;
        
        // After movement is finished


// Tell GameManager the pawn move finished (this will handle extraChance/transfer)


        if (previousPathPoint != null)
        {
            GameManager.gm.RemovePathPoint(previousPathPoint);
            previousPathPoint.RemovePlayerPice1(this);
        }
        else
        {
            Debug.LogWarning("previousPathPoint is null before removal");
        }

        int targetIndex = numberOfStepsAlreadyMove - 1;
        if (targetIndex >= 0 && targetIndex < pathParent_.Length)
        {
            currentPathPoint = pathParent_[targetIndex];
        }
        else
        {
            Debug.LogError("Invalid target index: " + targetIndex);
            // transfer turn to avoid hang
            GameManager.gm.OnPawnMoved();
            GameManager.gm.isPawnMoving = false;
            GameManager.gm.canDiceRoll = true;
            GameManager.gm.canPlayerMove = false;
            yield break;
        }

        if (currentPathPoint != null)
        {
            GameManager.gm.AddPathPoint(currentPathPoint);
            currentPathPoint.AddPlayerPice1(this);
            previousPathPoint = currentPathPoint;
        }
        else
        {
            Debug.LogError("currentPathPoint is null after movement");
            GameManager.gm.OnPawnMoved();
            GameManager.gm.isPawnMoving = false;
            GameManager.gm.canDiceRoll = true;
            GameManager.gm.canPlayerMove = false;
            yield break;
        }
    }
// ==== CHECK IF THIS PAWN FINISHED ==== 

var myPath = GetMyPath();
if (myPath == null) yield break;

if (numberOfStepsAlreadyMove == GetMyPath().Length)
{
    GameManager.gm.finishOrder++;
    GameManager.gm.OnPlayerFinished(playerColor);   // show crown

    if (playerColor == "Red")
    {
        GameManager.gm.redCompletePlayer++;
        if (GameManager.gm.redCompletePlayer == 4)
            GameManager.gm.RedFinished = true;
    }

    if (playerColor == "Green")
    {
        GameManager.gm.greenCompletePlayer++;
        if (GameManager.gm.greenCompletePlayer == 4)
            GameManager.gm.GreenFinished = true;
    }

    if (playerColor == "Blue")
    {
        GameManager.gm.blueCompletePlayer++;
        if (GameManager.gm.blueCompletePlayer == 4)
            GameManager.gm.BlueFinished = true;
    }

    if (playerColor == "Yellow")
    {
        GameManager.gm.yellowCompletePlayer++;
        if (GameManager.gm.yellowCompletePlayer == 4)
            GameManager.gm.YellowFinished = true;
    }

    // STOP all gameplay flow here
    GameManager.gm.isPawnMoving = false;
    GameManager.gm.canDiceRoll = false;
    GameManager.gm.canPlayerMove = false;

    yield break;
}

// Tell GameManager the pawn move finished
GameManager.gm.OnPawnMoved();

// ==== MOVEMENT FINISHED ====
GameManager.gm.isPawnMoving = false;
GameManager.gm.canDiceRoll = true;
GameManager.gm.canPlayerMove = false;

if (playerMovement != null)
{
    StopCoroutine("MoveSteps");
}
}
    public IEnumerator ReverseToHome(PathPoint[] path)
{
    // Safety check
    if (path == null || path.Length == 0)
    {
        Debug.LogError("ReverseToHome FAILED → path array is NULL or EMPTY for player: " + playerColor);
        yield break;
    }

    // -------------------------------------
    // Move backward along the movement path
    // -------------------------------------
    for (int i = movementHistory.Count - 1; i >= 0; i--)
    {
        transform.position = movementHistory[i].transform.position;
        if(GameManager.gm.sound )
            {
                GameManager.gm.ads.Play();
            }
        yield return new WaitForSeconds(0.15f); // animation speed
    }

    // ----------------------------------------------------
    // Each pawn must return to its SPECIFIC base position
    // (pawn 0 → base 0, pawn 1 → base 1, pawn 2 → base 2...)
    // ----------------------------------------------------
    int pawnIndex = transform.GetSiblingIndex();   // returns 0,1,2,3

    PathPoint homePoint = pathParent.GetBasePoint(playerColor, pawnIndex);

    if (homePoint == null)
    {
        Debug.LogError("HomePoint is NULL for " + playerColor + " pawn = " + pawnIndex);
        yield break;
    }

    // Move pawn EXACTLY to its personal base point
    transform.position = homePoint.transform.position;

    currentPathPoint = homePoint;
    previousPathPoint = homePoint;

    // Reset pawn data
    Ready = false;
    numberOfStepsAlreadyMove = 0;
    movementHistory.Clear();

    yield break;
}


    bool isPathPointAvailableToMove(int numberOfStepsToMove, int numberOfStepsAlreadyMove, PathPoint[] pathParent_)
    {
        if (numberOfStepsToMove == 0 || pathParent_ == null)
        {
            return false;
        }

        int leftNumberOfPath = pathParent_.Length - numberOfStepsAlreadyMove;
        return leftNumberOfPath >= numberOfStepsToMove;
    }
}