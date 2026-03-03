using System;
using Godot;

public partial class Main : Node2D
{
  [Export] PackedScene campScene;
  [Export] PackedScene playerPaddleScene;

  Camp currentCamp;

  public Camp CurrentCamp => currentCamp;

  public override void _Ready()
  {
    currentCamp = campScene.Instantiate<Camp>();

    AddChild(currentCamp);

    currentCamp.OnCampReady += OnCurrentCampReady;

    currentCamp.Initializer(playerPaddleScene);
  }

  private void OnCurrentCampReady()
  {
    GameManager.Instance.SwitchState(GameState.Start);
  }

}
