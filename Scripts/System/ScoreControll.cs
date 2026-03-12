using Godot;

/// <summary>
/// Responsável apenas por contar e expor os scores.
/// Listener passivo: reage a PlayerScore/EnemyScore confirmados pelo pipeline.
/// Não toma decisões — não dispara vitória, não dispara diálogo.
/// </summary>
public partial class ScoreControll : Node
{
  public static ScoreControll Instance { get; private set; }

  [Signal]
  public delegate void ScoreUpdateEventHandler(int playerScore, int enemyScore);

  public int PlayerScore { get; private set; }
  public int EnemyScore { get; private set; }

  public override void _Ready()
  {
    Instance = this;
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  public void ResetScore()
  {
    PlayerScore = 0;
    EnemyScore = 0;
    EmitSignal(SignalName.ScoreUpdate, PlayerScore, EnemyScore);
  }

  private void OnGameStateChanged()
  {
    switch (GameManager.Instance.CurrentState)
    {
      case GameState.PlayerScore:
        PlayerScore++;
        EmitSignal(SignalName.ScoreUpdate, PlayerScore, EnemyScore);
        break;

      case GameState.EnemyScore:
        EnemyScore++;
        EmitSignal(SignalName.ScoreUpdate, PlayerScore, EnemyScore);
        break;
    }
  }
}