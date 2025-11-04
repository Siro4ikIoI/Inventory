public class Item : IMatrix
{
    private Matrix _blocks;

    public Item(int[,] blocks)
    {
        _blocks = new(blocks);
    }

    public Matrix ToMatrix()
    {
        return _blocks;
    }
}
