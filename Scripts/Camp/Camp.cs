using Godot;

public partial class Camp : Node2D
{
  [Signal] public delegate void OnCampReadyEventHandler();
  [Signal] public delegate void OnPlayerWinEventHandler();
  [Signal] public delegate void OnPlayerLoserEventHandler();

  [Export] private CampInitializer initializer;
  [Export] private ScoreHud scoreHud;

  public override void _Ready()
  {
    scoreHud.OnScoreAnimationFinished += OnScoreHudAnimationFinished;
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    DialogueManager.Instance.RegisterGameLayer(this);

    TreeExiting += () => GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
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
    if (!IsInstanceValid(this)) return;

    var state = GameManager.Instance.CurrentState;

    scoreHud.HideScore();

    if (state == GameState.PlayerWin)
    {
      EmitSignal(SignalName.OnPlayerWin);
      return;
    }

    if (state == GameState.PlayerLoser)
    {
      EmitSignal(SignalName.OnPlayerLoser);
      return;
    }

    // Ponto normal — continua o fluxo de jogo
    GameManager.Instance.SwitchState(GameState.Scored);
  }

  public void EmitCampReady()
  {
    EmitSignal(SignalName.OnCampReady);
  }
}