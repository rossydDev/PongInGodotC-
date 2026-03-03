using Godot;
using System;

public partial class Camp : Node2D
{
  [Signal]
  public delegate void OnCampReadyEventHandler();

  [ExportGroup("Enemy Scene")]
  [Export] PackedScene paddleEnemyScene;

  [ExportGroup("Paddle Positions")]
  [Export] Node2D playerSpawnPosition;
  [Export] Node2D enemySpawnPosition;

  [ExportGroup("Gols")]
  [Export] Gol enemyGol;
  [Export] Gol playerGol;


  private Paddle paddleEnemy;
  private Paddle playerPaddle;

  public override void _Ready()
  {
    EmitSignal(SignalName.OnCampReady);
  }


  public void Initializer(PackedScene playerScene)
  {
    if (paddleEnemy == null)
    {
      CreatePaddleEnemy();
    }

    playerPaddle = playerScene.Instantiate<Paddle>();

    AddChild(playerPaddle);

    playerPaddle.GlobalPosition = playerSpawnPosition.GlobalPosition;
    enemyGol.SetAdversaryPaddle(playerPaddle);

    GD.Print(playerPaddle.GlobalPosition);
  }

  private void CreatePaddleEnemy()
  {
    Paddle _enemy = paddleEnemyScene.Instantiate<Paddle>();

    if (_enemy == null) return;

    paddleEnemy = _enemy;
    AddChild(paddleEnemy);

    paddleEnemy.GlobalPosition = enemySpawnPosition.GlobalPosition;
    playerGol.SetAdversaryPaddle(paddleEnemy);
  }

  public void EmitCampReady()
  {
    EmitSignal(SignalName.OnCampReady);
  }

}
