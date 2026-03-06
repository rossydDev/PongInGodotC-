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

  [ExportGroup("Systems")]
  [Export] ScoreControll scoreControll;
  [ExportGroup("HUD")]
  [Export] ScoreHud scoreHud;
  [Export] HeartHudComponent heartHudComponent;


  private Paddle paddleEnemy;
  private Paddle playerPaddle;

  public override void _Ready()
  {
    EmitSignal(SignalName.OnCampReady);

    scoreHud.OnScoreAnimationFinished += OnScoreHudAnimationFinished;
  }

  private async void OnScoreHudAnimationFinished()
  {
    await ToSignal(GetTree().CreateTimer(1.7f), SceneTreeTimer.SignalName.Timeout);

    scoreHud.HideScore();

    GameManager.Instance.SwitchState(GameState.Scored);
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

    heartHudComponent.Initializer(playerPaddle);
    AbilityComponent ability = playerPaddle.GetNode<PlayerController>("PlayerController").CurrentAbility;

    heartHudComponent.ConnectAbility(ability);

    var life = playerPaddle.GetNodeOrNull<LifeComponent>("LifeComponent");
    life?.SetSpawnPosition(playerSpawnPosition.GlobalPosition);

    scoreControll.Initializer();
    scoreControll.ScoreUpdate += OnScoreControllUpdate;
  }

  private void OnScoreControllUpdate(int playerScore, int enemyScore)
  {
    scoreHud.SetScore(playerScore, enemyScore);
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
