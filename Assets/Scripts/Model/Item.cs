public class Item : IMatrix
{
    private Matrix _blocks;

    public int Id { get; private set; }

    public Item(int id, int[,] blocks)
    {
        Id = id;
        _blocks = new(blocks);
    }

    public Matrix ToMatrix()
    {
        return _blocks;
    }
}
