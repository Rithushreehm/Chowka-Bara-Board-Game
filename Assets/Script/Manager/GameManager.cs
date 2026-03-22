using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public int numberOfStepsToMove;
    public RollingDice rollingDice;
    public bool canPlayerMove = true;
    public string currentDiceBoardColor;
    public List<string> diceColors = new List<string> { "Red", "Green", "Blue", "Yellow" };
    List<PathPoint> playerPahPointList = new List<PathPoint>();

    public bool canDiceRoll = true;
    public bool canDiceTransfer = false;
    public List<RollingDice> rollingDices;
    public bool hasExtraChance = false;
    public int redCompletePlayer;
    public int greenCompletePlayer;
    public int blueCompletePlayer;
    public int yellowCompletePlayer;
    public int totalPlayerCanPlay;
    public bool isBlueAI = false;        // Is blue an AI player?
    public RollingDice blueAIDice;       // Assign Blue AI's dice in Inspector

    public List<GameObject> playerHomes;
    public List<PlayerPice1> redPlayerPices1;
    public List<PlayerPice1> greenPlayerPices1;
    public List<PlayerPice1> bluePlayerPices1;
    public List<PlayerPice1> yellowPlayerPices1;
    public AudioSource ads;
    public bool sound =true;
    public bool isPawnMoving = false;
   public Image redCrown;
    public Image greenCrown;
    public Image blueCrown;
    public Image yellowCrown;

    public Sprite crown1;
    public Sprite crown2;
    public Sprite crown3;
    public Sprite crown4;
    int currentRank = 1;

     public Transform redHomesPosition;
    public Transform greenHomesPosition;
    public Transform blueHomesPosition;
    public Transform yellowHomesPosition;


public int finishOrder = 0;

    public bool RedFinished, GreenFinished, BlueFinished, YellowFinished;


    private void Awake()
    {
        gm = this;
        ads = GetComponent<AudioSource>();
    }

    private void Start()
{
    
        currentDiceBoardColor = diceColors[0];
        rollingDice = rollingDices[0];
}

    public void AddPathPoint(PathPoint pathPoint)
    {
        playerPahPointList.Add(pathPoint);
    }

    public void RemovePathPoint(PathPoint pathPoint)
    {
        if (playerPahPointList.Contains(pathPoint))
        {
            playerPahPointList.Remove(pathPoint);
        }
    }

public void OnPawnMoved()
{
    Debug.Log($"[OnPawnMoved] called | hasExtraChance={hasExtraChance} currentDiceBoardColor={currentDiceBoardColor}");

    // 1) If pawn earned extra chance
    if (hasExtraChance)
    {
        bool isBlueAITurn = isBlueAI && currentDiceBoardColor == "Blue";

        if (isBlueAITurn && blueAIDice != null)
        {
            // Important: do NOT auto-roll before movement; here movement has finished,
            // so it's OK to schedule the AI's next roll.
            Debug.Log("[OnPawnMoved] Blue AI has extra chance — scheduling autoRole()");
             canDiceRoll = false;
        canPlayerMove = false;

        CancelInvoke(nameof(role));
        Invoke(nameof(role), 0.45f);
        }
        else
        {
            Debug.Log("[OnPawnMoved] Human has extra chance — allowing dice roll");
            canDiceRoll = true;
        }

        hasExtraChance = false; // reset after using
        return; // do not continue to change turn
    }

    // 2) No extra chance → simply change turn
    canDiceTransfer = true;
    // Use our robust ChangeTurn() to transfer turn
    ChangeTurn();

    // 3) Disable movement until next roll (explicit)
    canPlayerMove = false;
}

 
   public void rollingDiceTransfer()
{
    ChangeTurn();
}
void role()
{
    if (isBlueAI && blueAIDice != null)
    {
        blueAIDice.autoRole();
    }
}    


// Replace existing AutoMoveForAI() + DoAutoMoveForAI() with these:

public void AutoMoveForAI()
{
    // Basic guards
    if (!isBlueAI) return;
    if (currentDiceBoardColor != "Blue") return;

    // If a movement is already running, wait (don't force another).
    if (isPawnMoving)
    {
        Debug.Log("[AutoMoveForAI] Aborting: isPawnMoving == true");
        return;
    }

    // Prepare flags so MoveSteps() runs correctly when invoked.
    canPlayerMove = true;      // allow AI to move
    canDiceRoll = false;       // block new rolls during movement

    // Small delay to let UI/dice finish animation before movement starts
    CancelInvoke(nameof(DoAutoMoveForAI));
    Invoke(nameof(DoAutoMoveForAI), 0.12f);
}

private void DoAutoMoveForAI()
{
    if (!isBlueAI) return;
    if (currentDiceBoardColor != "Blue") return;
    if (isPawnMoving)
    {
        Debug.Log("[DoAutoMoveForAI] Aborting: isPawnMoving == true");
        return;
    }

    int steps = numberOfStepsToMove;
    bool moved = false;

    Debug.Log($"[DoAutoMoveForAI] trying to move with steps={steps}");

    // 1) Prefer moving an already-on-board pawn that can move
    foreach (var pawn in bluePlayerPices1)
    {
        if (pawn == null) continue;
        if (pawn.Ready && pawn.CanMoveStepsPublic(steps))
        {
            Debug.Log("[DoAutoMoveForAI] moving existing pawn");
            pawn.MoveStepsPublic(steps);
            moved = true;
            break;
        }
    }

    // 2) If none can move, try to bring a pawn out from home (MakePlayerReadyToMove),
    //    then move remaining steps (steps-1) if applicable.
    if (!moved)
    {
        foreach (var pawn in bluePlayerPices1)
        {
            if (pawn == null) continue;
            if (!pawn.Ready)
            {
                Debug.Log("[DoAutoMoveForAI] bringing pawn out from home");
                pawn.MakePlayerReadyToMove(pawn.GetMyPath());

                // If roll > 1, move further steps after entering
                if (steps > 1)
                {
                    // Note: MoveStepsPublic will read numberOfStepsToMove from GameManager
                    // so ensure we set it for the remaining steps.
                    pawn.MoveStepsPublic(steps - 1);
                    Debug.Log($"[DoAutoMoveForAI] after entering, moving remaining steps = {steps - 1}");
                }

                moved = true;
                break;
            }
        }
    }

    // 3) If still nothing moved, then there's no legal move — change turn.
    if (!moved)
    {
        Debug.Log("[DoAutoMoveForAI] No valid AI moves available — changing turn.");
        ChangeTurn();
    }

    // If moved: MoveSteps coroutine (in PlayerPice1) will call GameManager.OnPawnMoved()
    // when movement finishes; that handles hasExtraChance / further rolls / turn transfer.
}

// Put this inside GameManager class (replace/ignore transferRollingDice implementation)
public void ChangeTurn()
{
    // Safety checks
    if (totalPlayerCanPlay <= 0 || diceColors == null || rollingDices == null)
    {
        Debug.LogError("[ChangeTurn] Invalid state for turn change.");
        return;
    }

    // Find the current index strictly from currentDiceBoardColor (safer than using rollingDice ref)
    int currentIndex = diceColors.IndexOf(currentDiceBoardColor);
    if (currentIndex == -1)
    {
        // fallback: try to find current rollingDice in the list
        currentIndex = rollingDices.IndexOf(rollingDice);
        if (currentIndex == -1)
        {
            Debug.LogWarning("[ChangeTurn] Could not find current index from color or dice; defaulting to 0.");
            currentIndex = 0;
        }
    }

   int nextIndex = currentIndex;

for (int i = 0; i < totalPlayerCanPlay; i++)
{
    nextIndex = (nextIndex + 1) % totalPlayerCanPlay;

    if (!IsPlayerFinished(diceColors[nextIndex]))
        break; // found valid player
}


    Debug.Log($"[ChangeTurn] currentIndex={currentIndex} ({currentDiceBoardColor}) -> nextIndex={nextIndex} ({diceColors[nextIndex]})");

    // Deactivate old dice, activate new one (guard nulls)
    if (currentIndex >= 0 && currentIndex < rollingDices.Count && rollingDices[currentIndex] != null)
        rollingDices[currentIndex].gameObject.SetActive(false);

    if (nextIndex >= 0 && nextIndex < rollingDices.Count && rollingDices[nextIndex] != null)
        rollingDices[nextIndex].gameObject.SetActive(true);

    // Update references
    rollingDice = rollingDices[nextIndex];
    currentDiceBoardColor = diceColors[nextIndex];

    // Reset transfer/dice flags (explicit)
    canDiceRoll = true;
    canDiceTransfer = false;
    canPlayerMove = false;

    Debug.Log($"[ChangeTurn] Turn changed to: {currentDiceBoardColor} | canDiceRoll={canDiceRoll} | canPlayerMove={canPlayerMove}");

    // If the next player is Blue and Blue is AI, let Blue auto-roll (only when appropriate)
    bool nextIsBlueAI = isBlueAI && currentDiceBoardColor == "Blue";
    if (nextIsBlueAI)
    {
        // Use invoke so UI settles before auto roll
        CancelInvoke(nameof(role));
        Invoke(nameof(role), 0.45f);
        Debug.Log("[ChangeTurn] Scheduling AI auto roll for Blue.");
    }
}
bool IsPlayerFinished(string color)
{
    return (color == "Red" && RedFinished) ||
           (color == "Green" && GreenFinished) ||
           (color == "Blue" && BlueFinished) ||
           (color == "Yellow" && YellowFinished);
}
public void OnPlayerFinished(string color)
{
    Sprite selected = null;

    if(currentRank == 1) selected = crown1;
    else if(currentRank == 2) selected = crown2;
    else if(currentRank == 3) selected = crown3;
    else if(currentRank == 4) selected = crown4;

    if(color == "Red")
    {
        redCrown.sprite = selected;
        redCrown.gameObject.SetActive(true);
    }

    if(color == "Green")
    {
        greenCrown.sprite = selected;
        greenCrown.gameObject.SetActive(true);
    }

    if(color == "Blue")
    {
        blueCrown.sprite = selected;
        blueCrown.gameObject.SetActive(true);
    }

    if(color == "Yellow")
    {
        yellowCrown.sprite = selected;
        yellowCrown.gameObject.SetActive(true);
    }

    currentRank++;
}

}