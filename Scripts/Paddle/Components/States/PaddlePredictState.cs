using Godot;

public partial class PaddlePredictState : PaddleState
{
  public override void Update(float delta)
  {
    Ball ball = GameManager.Instance.CurrentBall;

    Vector2 ballPos = ball.GlobalPosition;
    Vector2 ballVel = ball.Velocity;

    float paddleY = paddle.GlobalPosition.Y;

    if (Mathf.Sign(ballVel.Y) != Mathf.Sign(paddleY - ballPos.Y))
    {
      paddle.MoveTowards(GameManager.Instance.ScreenWidth / 2f, delta);
      return;
    }

    if (Mathf.Abs(ballVel.Y) < 0.001f) return;

    float time = (paddleY - ballPos.Y) / ballVel.Y;
    float predictedX = ballPos.X + ballVel.X * time;
    float width = GameManager.Instance.ScreenWidth;

    predictedX = ReflectX(predictedX, width);


    paddle.MoveTowards(predictedX, delta);
  }

  private float ReflectX(float x, float width)
  {
    while (x < 0 || x > width)
    {
      if (x < 0)
        x = -x;
      else if (x > width)
        x = width - (x - width);
    }

    return x;
  }
}