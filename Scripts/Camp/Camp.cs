using Godot;

public partial class Camp : Node2D
{
  [Signal]
  public delegate void OnCampReadyEventHandler();

  [Export] private CampInitializer initializer;
  [Export] private ScoreHud scoreHud;

  public override void _Ready()
  {
    scoreHud.OnScoreAnimationFinished += OnScoreHudAnimationFinished;
    EmitSignal(SignalName.OnCampReady);
  }

  public void Initializer(PackedScene playerScene)
  {
    initializer.Setup(playerScene);
  }

  private async void OnScoreHudAnimationFinished()
  {
    await ToSignal(GetTree().CreateTimer(1.7f), SceneTreeTimer.SignalName.Timeout);
    scoreHud.HideScore();
    GameManager.Instance.SwitchState(GameState.Scored);
  }

  public void EmitCampReady()
  {
    EmitSignal(SignalName.OnCampReady);
  }
}

