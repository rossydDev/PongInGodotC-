using Godot;
using System;

public partial class ScoreControll : Node
{
  public static ScoreControll Instance { get; private set; }

  private Timer timer;

  public override void _Ready()
  {
    Instance = this;


    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState == GameState.PlayerScore ||
       GameManager.Instance.CurrentState == GameState.EnemyScore
    )
    {
      GD.Print("Foi pontuado!");

      BallController.Instance.SpawnBall();
    }
  }

}
