using Godot;

public partial class GameOverScreen : Control
{
  [Export] private Label playerScoreLabel;
  [Export] private Label enemyScoreLabel;
  [Export] private Button retryButton;
  [Export] private Button quitButton;

  [Signal] public delegate void OnRetryEventHandler();
  [Signal] public delegate void OnQuitEventHandler();

  public override void _Ready()
  {
    Hide();
    retryButton.Pressed += OnRetryPressed;
    quitButton.Pressed += OnQuitPressed;
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState != GameState.PlayerLoser) return;
    ShowGameOver();
  }

  private void ShowGameOver()
  {
    playerScoreLabel.Text = ScoreControll.Instance.PlayerScore.ToString();
    enemyScoreLabel.Text = ScoreControll.Instance.EnemyScore.ToString();
    Show();
  }

  private void OnRetryPressed()
  {
    Hide();
    EmitSignal(SignalName.OnRetry);
  }

  private void OnQuitPressed()
  {
    Hide();
    EmitSignal(SignalName.OnQuit);
  }
}