using Godot;

public partial class GameManager : Node
{
  public static GameManager Instance { get; private set; }

  private Ball currentBall;
  private float screenWidth;

  public float ScreenWidth => screenWidth;
  public Ball CurrentBall => currentBall;

  public override void _Ready()
  {
    Instance = this;
    screenWidth = GetTree().Root.GetVisibleRect().Size.X;
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

  public void SetCurrentBall(Ball newBall)
  {
    currentBall = newBall;
  }
}