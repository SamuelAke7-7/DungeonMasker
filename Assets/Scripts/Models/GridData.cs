using System;

[Serializable]
public class GridData
{
    public int width;
    public int height;
    public int[] cells; // Array plano: cells[y * width + x]
    public int startX;
    public int startY;
    public int endX;
    public int endY;
    public CellSecret[] cellSecret;
    public CellMonster[] cellMonster;
    public CellObject[] cellObject;
}