using Godot;

public partial class GameManager : Node
{
  public static GameManager Instance { get; private set; }

  private float screenHeight;

  public override void _Ready()
  {
    Instance = this;

    screenHeight = GetTree().Root.GetVisibleRect().Size.Y;
  }

  public bool IsOutOfBounds(Vector2 position, float halfHeight = 0)
  {
    return position.Y - halfHeight < 0
           || position.Y + halfHeight > screenHeight;
  }

  public float ClampY(float y, float halfHeight)
  {
    return Mathf.Clamp(y, halfHeight, screenHeight - halfHeight);
  }
}