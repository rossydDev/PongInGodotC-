using Godot;

public partial class PaddleReactState : PaddleState
{
  private AIDifficultySettings _settings;
  private float _reactionTimer;

  public override void Enter()
  {
    _settings = paddle.Controller.CurrentSettings;
    _reactionTimer = _settings.ReactionDelay;
  }

  public override void Update(float delta)
  {
    BallBase ball = BallController.Instance.CurrentBall;
    if (ball == null) return;

    float paddleY = paddle.GlobalPosition.Y;
    bool ballComingToMe = Mathf.Sign(ball.Velocity.Y) == Mathf.Sign(paddleY - ball.GlobalPosition.Y);

    // Bola foi embora enquanto esperava o delay — vai pro centro
    if (!ballComingToMe)
    {
      stateMachine.SwitchState<PaddleIdleState>();
      return;
    }

    _reactionTimer -= delta;
    if (_reactionTimer > 0f) return;

    // Delay passou — calcula target e vai se mover
    float predictedX = PredictBallX(ball);
    float error = (float)GD.RandRange(-_settings.ErrorMargin, _settings.ErrorMargin);
    paddle.Controller.SetTarget(predictedX + error);
    stateMachine.SwitchState<PaddleApproachState>();
  }

  private float PredictBallX(BallBase ball)
  {
    Vector2 pos = ball.GlobalPosition;
    Vector2 vel = ball.Velocity;
    float paddleY = paddle.GlobalPosition.Y;
    float width = GameManager.Instance.ScreenWidth;

    if (Mathf.Abs(vel.Y) < 0.001f) return pos.X;

    float time = (paddleY - pos.Y) / vel.Y;
    float predictedX = pos.X + vel.X * time;

    return ReflectX(predictedX, width, (int)_settings.MaxBounces);
  }

  private float ReflectX(float x, float width, int maxBounces)
  {
    int bounces = 0;
    while ((x < 0 || x > width) && bounces < maxBounces)
    {
      x = x < 0 ? -x : width - (x - width);
      bounces++;
    }
    return (x < 0 || x > width) ? width / 2f : x;
  }

  public override void Exit()
  {
    _settings = null;
    _reactionTimer = 0f;
  }
}