using Godot;

public partial class BallController : Node, IBallProvider
{
  public static BallController Instance { get; private set; }

  // ballScene é configurado no Inspector do Autoload (Project > Autoloads > editar)
  [Export] private PackedScene ballScene;

  private Vector2 spawnPosition;
  private BallBase currentBall;

  public BallBase CurrentBall => currentBall;

  public override void _Ready()
  {
    Instance = this;

    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  // Chamado pelo CampInitializer ao montar a arena
  public void SetSpawnPosition(Vector2 position)
  {
    spawnPosition = position;
  }

  private void OnGameStateChanged()
  {
    GameState currentState = GameManager.Instance.CurrentState;

    if (currentState == GameState.Start || currentState == GameState.Scored)
    {
      CreateBall();
    }
  }

  private void CreateBall()
  {
    if (currentBall != null)
    {
      currentBall.QueueFree();
      currentBall = null;
    }

    BallBase ball = ballScene.Instantiate<BallBase>();

    AddChild(ball);

    ball.GlobalPosition = spawnPosition;

    currentBall = ball;
  }
}