using Godot;

public partial class PaddleApproachState : PaddleState
{
  private AIDifficultySettings _settings;
  private float _recalculateTimer;
  private const float RecalculateInterval = 0.4f;

  public override void Enter()
  {
    _settings = paddle.Controller.CurrentSettings;
    _recalculateTimer = RecalculateInterval;
  }

  public override void Update(float delta)
  {
    float target = paddle.Controller.CurrentTarget;
    float distanceToTarget = Mathf.Abs(paddle.GlobalPosition.X - target);

    // Chegou perto o suficiente — para de se mover (humanos não ficam ajustando pixel a pixel)
    if (distanceToTarget < 8f)
    {
      stateMachine.SwitchState<PaddleIdleState>();
      return;
    }

    // Recalcula de tempos em tempos enquanto se move
    _recalculateTimer -= delta;
    if (_recalculateTimer <= 0f)
    {
      _recalculateTimer = RecalculateInterval;
      stateMachine.SwitchState<PaddleReactState>();
      return;
    }

    paddle.MoveTowards(target, delta);
  }
}