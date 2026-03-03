using Godot;
using System;

public partial class ScoreControll : Node
{
  [Signal]
  public delegate void ScoreUpdateEventHandler(int playerScore, int enemyScore);

  private Timer timer;

  public int playerScore;
  public int enemyScore;

  public void Initializer()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

    playerScore = 0;
    enemyScore = 0;
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState == GameState.PlayerScore)
    {
      playerScore++;
      GD.Print($"{playerScore} : {enemyScore}");
      EmitSignal(SignalName.ScoreUpdate, playerScore, enemyScore);
    }
    else if (GameManager.Instance.CurrentState == GameState.EnemyScore)
    {
      enemyScore++;
      GD.Print($"{playerScore} : {enemyScore}");
      EmitSignal(SignalName.ScoreUpdate, playerScore, enemyScore);
    }
  }
}
