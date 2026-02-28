using Godot;

public partial class GameManager : Node
{
  [Signal]
  public delegate void OnGameStateChangedEventHandler();

  public static GameManager Instance { get; private set; }

  [Export] private GameState currentState = GameState.Freeze;
  private float screenWidth;
  private Timer startTimer;

  public float ScreenWidth => screenWidth;
  public GameState CurrentState => currentState;

  public override void _Ready()
  {
    Instance = this;
    screenWidth = GetTree().Root.GetVisibleRect().Size.X;

    EmitSignal(SignalName.OnGameStateChanged);

    startTimer = GetNode<Timer>("Timer");
    startTimer.Timeout += () => SwitchState(GameState.Start);
  }

  public bool IsOutOfBounds(Vector2 position, float halfWidth = 0)
  {
    return position.X - halfWidth < 0
           || position.X + halfWidth > screenWidth;
  }

  public float ClampY(float y, float halfWidth)
  {
    return Mathf.Clamp(y, halfWidth, screenWidth - halfWidth);
  }

  public void SwitchState(GameState newState)
  {
    if (newState == currentState) return;

    currentState = newState;

    EmitSignal(SignalName.OnGameStateChanged);
  }
}