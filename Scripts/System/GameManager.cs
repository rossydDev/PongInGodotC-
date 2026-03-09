using Godot;

public partial class GameManager : Node
{
  [Signal]
  public delegate void OnGameStateChangedEventHandler();

  public static GameManager Instance { get; private set; }

  private GameState currentState = GameState.Freeze;

  public GameState CurrentState => currentState;

  public override void _Ready()
  {
    Instance = this;
    WorldBounds.Initialize(GetTree().Root.GetVisibleRect().Size.X);

    EmitSignal(SignalName.OnGameStateChanged);
  }

  public void SwitchState(GameState newState)
  {
    if (newState == currentState) return;

    currentState = newState;

    EmitSignal(SignalName.OnGameStateChanged);
  }

  public void Scored(Paddle scoredPaddle)
  {
    BallController.Instance.CurrentBall.SpawnExplosion();

    if (scoredPaddle is PaddleIA)
    {
      SwitchState(GameState.EnemyScore);
    }
    else
    {
      SwitchState(GameState.PlayerScore);
    }
  }
}