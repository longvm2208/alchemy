using System;
using UnityEngine;

[Serializable]
public class Grid2D<T>
{
    [SerializeField] Vector2Int size;
    public Vector2Int Size => size;
    [SerializeField] T[] cells;
    public bool IsEmpty => cells == null || cells.Length == 0;

    public T this[int x, int y]
    {
        get => cells[y * size.x + x];
        set => cells[y * size.x + x] = value;
    }

    public T this[Vector2Int c]
    {
        get => this[c.x, c.y];
        set => this[c.x, c.y] = value;
    }

    public Grid2D(int x, int y)
    {
        size = new(x, y);
        cells = new T[x * y];
    }

    public Grid2D(Vector2Int size)
    {
        this.size = size;
        cells = new T[size.x * size.y];
    }

    public bool AreValidCoord(Vector2Int c)
    {
        return AreValidCoord(c.x, c.y);
    }

    public bool AreValidCoord(int x, int y)
    {
        return 0 <= x && x < size.x && 0 <= y && y < size.y;
    }

    public void Swap(Vector2Int a, Vector2Int b)
    {
        (this[a], this[b]) = (this[b], this[a]);
    }

    public void Shuffle()
    {
        cells.Shuffle();
    }

    public void AddRow()
    {
        if (IsEmpty) return;

        size.y += 1;

        T[] newCells = new T[size.x * size.y];

        Array.Copy(cells, 0, newCells, 0, cells.Length);

        cells = newCells;
    }
}
