using Godot;

public partial class PaddlePredictState : PaddleState
{
  private AIDifficultySettings _settings;
  private float _reactionTimer = 0f;
  private float _targetX = 0f;
  private bool _hasTarget = false;

  public override void Enter()
  {
    _settings = paddle.Controller.CurrentSettings;

    GD.Print(_settings);
  }

  public override void Update(float delta)
  {
    if (_settings == null) return;

    Ball ball = BallController.Instance.CurrentBall;
    Vector2 ballPos = ball.GlobalPosition;
    Vector2 ballVel = ball.Velocity;
    float paddleY = paddle.GlobalPosition.Y;

    _reactionTimer -= delta;

    if (_reactionTimer <= 0f)
    {
      _reactionTimer = _settings.ReactionDelay;

      if (Mathf.Sign(ballVel.Y) != Mathf.Sign(paddleY - ballPos.Y))
      {
        _targetX = GameManager.Instance.ScreenWidth / 2f;
      }
      else if (Mathf.Abs(ballVel.Y) >= 0.001f)
      {
        float time = (paddleY - ballPos.Y) / ballVel.Y;
        float predictedX = ballPos.X + ballVel.X * time;
        float width = GameManager.Instance.ScreenWidth;

        predictedX = ReflectX(predictedX, width, (int)_settings.MaxBounces);

        float error = (float)GD.RandRange(-_settings.ErrorMargin, _settings.ErrorMargin);
        _targetX = predictedX + error;
      }

      _hasTarget = true;
    }

    if (_hasTarget)
      paddle.MoveTowards(_targetX, delta);
  }

  private float ReflectX(float x, float width, int maxBounces)
  {
    int bounces = 0;
    while ((x < 0 || x > width) && bounces < maxBounces)
    {
      if (x < 0)
        x = -x;
      else if (x > width)
        x = width - (x - width);
      bounces++;
    }
    // Se passou do limite de bounces, retorna o centro
    if (x < 0 || x > width)
      return width / 2f;

    return x;
  }

  public override void Exit()
  {
    _settings = null;
  }
}