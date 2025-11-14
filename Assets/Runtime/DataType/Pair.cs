using System;

public struct Pair
{
    public int Row { get; private set; } 
    public int Col { get; private set; }

    public Pair(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override bool Equals(object obj)
    {
        return obj is Pair pair &&
               Row == pair.Row &&
               Col == pair.Col;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Col);
    }

    public static bool operator ==(Pair pair1, Pair pair2)
    {
        return pair1.Equals(pair2);
    }

    public static bool operator !=(Pair pair1, Pair pair2)
    {
        return !(pair1 == pair2);
    }
}
