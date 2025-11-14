public class ScorePresenter
{
    private GameState _gameState;
    private ScoreView _scoreView;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public ScorePresenter(GameState gameState, ScoreView scoreView,
                                    ModelCollection modelCollection, CanvasView canvasView)
    {
        _gameState = gameState;
        _scoreView = scoreView;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnScoreChanged(int score)
    {
        _scoreView.UpdateScore(score);
    }

    private void OnItemAdded(Item item, Pair position)
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
