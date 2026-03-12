using Godot;
using System;

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
    if (GameManager.Instance.CurrentState == GameState.PlayerScore)
    {
      PlayerScore++;
      EmitSignal(SignalName.ScoreUpdate, PlayerScore, EnemyScore);
    }
    else if (GameManager.Instance.CurrentState == GameState.EnemyScore)
    {
      EnemyScore++;
      EmitSignal(SignalName.ScoreUpdate, PlayerScore, EnemyScore);
    }
  }

}
