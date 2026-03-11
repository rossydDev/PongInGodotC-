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
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    DialogueManager.Instance.RegisterGameLayer(this);
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState == GameState.Intro)
      initializer.BeginIntro();
  }

  public void Initializer(PackedScene playerScene)
  {
    initializer.Setup(playerScene);
    EmitCampReady();
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

