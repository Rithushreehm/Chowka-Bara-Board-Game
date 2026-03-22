using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingDice : MonoBehaviour
{
    [SerializeField] Sprite[] numberSprite;
    [SerializeField] SpriteRenderer numberSpriteHolder;
     [SerializeField] SpriteRenderer rollingDiceAnimation;
    [SerializeField] int numberGot;

    Coroutine generateRandomNumberDice;
    public bool canDiceRoll = true;
     int fourCount = 0;     // max 4
    int eightCount = 0;    // max 3
    int lastNumber = -1;
    public Dice diceSound;
    
    private void OnMouseDown()
    {
        if (GameManager.gm.isPawnMoving) return;  // <--- ADD THIS
        if (GameManager.gm.canDiceRoll)
        {
            generateRandomNumberDice = StartCoroutine(rollingDice());
        }
    }
      public void autoRole()
{
    if (GameManager.gm.isPawnMoving) return; // <--- ADD THIS
    if (GameManager.gm.canDiceRoll)
    {
        generateRandomNumberDice = StartCoroutine(rollingDice());
    }
}


    IEnumerator rollingDice()
{
    yield return new WaitForEndOfFrame();

    if (GameManager.gm.canDiceRoll)
    {
        GameManager.gm.canDiceRoll = false;
        diceSound.playSound();

        numberSpriteHolder.gameObject.SetActive(false);
        rollingDiceAnimation.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        int[] allowed = { 1, 2, 3, 4, 8 };
        int temp;
        
        do
        {
            temp = allowed[Random.Range(0, allowed.Length)];

            // break if invalid
            if (temp == 4 && fourCount >= 3) continue;  
            if (temp == 8 && eightCount >= 2) continue;
            if (temp == lastNumber && (temp == 4 || temp == 8)) continue;

            break;

        } while (true);


        // update counters
        if (temp == 4) fourCount++;
        if (temp == 8) eightCount++;

        lastNumber = temp;
        numberGot = temp;


        // sprite switch
        int spriteIndex = temp switch
        {
            1 => 0,
            2 => 1,
            3 => 2,
            4 => 3,
            8 => 4,
            _ => 0
        };

        numberSpriteHolder.sprite = numberSprite[spriteIndex];
        GameManager.gm.numberOfStepsToMove = temp;

        rollingDiceAnimation.gameObject.SetActive(false);
        numberSpriteHolder.gameObject.SetActive(true);

        GameManager.gm.rollingDice = this;

        bool isBlueAITurn =
            GameManager.gm.isBlueAI &&
            GameManager.gm.currentDiceBoardColor == "Blue";


       

        // extra chance
        // extra chance
if (temp == 4 || temp == 8)
{
    GameManager.gm.hasExtraChance = true;

    if (isBlueAITurn)
    {
        // AI should MOVE first
        GameManager.gm.AutoMoveForAI();
    }
    else
    {
        GameManager.gm.canDiceRoll = true;
    }
}
else
{
    GameManager.gm.hasExtraChance = false;

    if (isBlueAITurn)
        GameManager.gm.AutoMoveForAI();
    else
        GameManager.gm.canPlayerMove = true;
}



        if (generateRandomNumberDice != null)
        {
            StopCoroutine(generateRandomNumberDice);
            generateRandomNumberDice = null;
        }
    }
}

     int GetSafeNumber()
    {
        int[] safe = { 1, 2, 3 };
        return safe[Random.Range(0, safe.Length)];
    }
}
