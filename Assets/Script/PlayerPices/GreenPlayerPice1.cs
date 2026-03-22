public class GreenPlayerPice1 : PlayerPice1
{
    void Start()
    {
        playerID = "Green";
        playerColor = "Green";
        
    }

    private void OnMouseDown()
    {
        if (GameManager.gm.currentDiceBoardColor != "Green")
            return;

        if (!Ready)
        {
            MakePlayerReadyToMove(pathParent.GreenPathPoint);
            return;
        }
        MovePlayer(pathParent.GreenPathPoint);
    }
}
