public class BluePlayerPice1 : PlayerPice1
{
    void Start()
    {
        playerID = "Blue";
        playerColor = "Blue";
    }

    private void OnMouseDown()
    {
        if (GameManager.gm.currentDiceBoardColor != "Blue")
            return;

        if (!Ready)
        {
            MakePlayerReadyToMove(pathParent.BluePathPoint);
            return;
        }
        MovePlayer(pathParent.BluePathPoint);
    }
}
