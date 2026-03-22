public class YellowPlayerPice1 : PlayerPice1
{
    void Start()
    {
        playerID = "Yellow";
        playerColor = "Yellow";

    }

    private void OnMouseDown()
    {
        if (GameManager.gm.currentDiceBoardColor != "Yellow")
            return;

        if (!Ready)
        {
            MakePlayerReadyToMove(pathParent.YellowPathPoint);
            return;
        }
        MovePlayer(pathParent.YellowPathPoint);
    }
}
