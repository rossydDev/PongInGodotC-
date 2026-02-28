using Godot;

public partial class PaddleStateController : Node
{
  [Export] private State gameStart;
  [Export] private StateMachine stateMachine;

  [ExportGroup("AI Difficulty")]
  [Export] public AIDifficulty Difficulty { get; set; } = AIDifficulty.Medium;
  [Export] public AIDifficultySettings EasySettings = new() { ErrorMargin = 120f, ReactionDelay = 0.6f, MaxBounces = 1 };
  [Export] public AIDifficultySettings MediumSettings = new() { ErrorMargin = 60f, ReactionDelay = 0.3f, MaxBounces = 2 };
  [Export] public AIDifficultySettings HardSettings = new() { ErrorMargin = 20f, ReactionDelay = 0.1f, MaxBounces = 4 };
  [Export] public AIDifficultySettings ImpossibleSettings = new() { ErrorMargin = 0f, ReactionDelay = 0f, MaxBounces = 99 };

  public AIDifficultySettings CurrentSettings => Difficulty switch
  {
    AIDifficulty.Easy => EasySettings,
    AIDifficulty.Medium => MediumSettings,
    AIDifficulty.Hard => HardSettings,
    AIDifficulty.Impossible => ImpossibleSettings,
    _ => MediumSettings
  };

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    GameState state = GameManager.Instance.CurrentState;
    switch (state)
    {
      case GameState.Freeze:
        stateMachine.SwitchState<PaddleIdleState>();
        break;
      case GameState.Start:
        stateMachine.SwitchState(gameStart);
        break;
    }
  }
}