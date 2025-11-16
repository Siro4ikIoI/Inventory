public class ScorePresenter
{
    private GameState _gameState;
    private ScoreView _scoreView;

    private ModelCollection _modelCollection;

    public ScorePresenter(GameState gameState, ScoreView scoreView, ModelCollection modelCollection)
    {
        _gameState = gameState;
        _scoreView = scoreView;

        _modelCollection = modelCollection;
    }

    private void OnScoreChanged(int score)
    {
        _scoreView.UpdateScore(score);
    }

    private void OnItemAdded(ItemModel item, (int row, int col) position)
    {
        _gameState.AddScore(item);
    }

    public void Enable()
    {
        _modelCollection.GetInventory(InventoryType.INVENTARY).ItemAdded += OnItemAdded;
        _gameState.ScoreChanged += OnScoreChanged;
    }

    public void Disable()
    {
        _gameState.ScoreChanged -= OnScoreChanged;
        _modelCollection.GetInventory(InventoryType.INVENTARY).ItemAdded -= OnItemAdded;
    }
}
