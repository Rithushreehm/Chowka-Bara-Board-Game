using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathObjectParent : PathPoint
{
    // Start is called before the first frame update
    public PathPoint[] RedPathPoint;
    public PathPoint[] GreenPathPoint;
    public PathPoint[] BluePathPoint;
    public PathPoint[] YellowPathPoint;

    public float[] scales;
    public float[] positionDifference;
    public PathPoint[] BasePoint;
    public PathPoint[] RedBase;
    public PathPoint[] GreenBase;
    public PathPoint[] BlueBase;
    public PathPoint[] YellowBase;

    public List<PathPoint> safePoints;
    public PathPoint startPoint;

    void Start()
    {
        for (int i = 0; i < BasePoint.Length; i++)
    {
        BasePoint[i].baseIndex = i;
    } 
    }
       public int GetBasePointIndex(string playerColor)
    {
        for (int i = 0; i < BasePoint.Length; i++)
        {
            if (BasePoint[i].ownerPlayerID == playerColor)
            {
                return i;
            }
        }

        Debug.LogError("BasePoint not found for playerID = " + playerColor);
        return -1;
    }

    // ⭐⭐⭐ ADD THIS FUNCTION HERE ⭐⭐⭐
    public PathPoint GetBasePoint(string playerColor, int pawnIndex)
    {
        // BasePoint[] must follow order:
        // 0-3 Red, 4-7 Green, 8-11 Blue, 12-15 Yellow

        switch (playerColor)
        {
            case "Red": return BasePoint[pawnIndex];
            case "Green": return BasePoint[4 + pawnIndex];
            case "Blue": return BasePoint[8 + pawnIndex];
            case "Yellow": return BasePoint[12 + pawnIndex];
        }

        Debug.LogError("Invalid color: " + playerColor);
        return null;
    }
}
