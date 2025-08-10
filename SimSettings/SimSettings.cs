using System.Collections.Generic;

[System.Serializable]
public class SimSettings
{
    public int numberOfRovers;
    public int gridMapRows;
    public int gridMapCols;
    public int numTrashItems;
    public List<(int start, int end)> assignments;
}
