// WallPulse.cs
using Godot;

public partial class WallPulse : Node2D
{
  float radius = 0f;
  float maxRadius = 120f;
  float duration = 0.4f;
  float elapsed = 0f;
  Color color = Colors.White;

  public void Pulse(Vector2 pos, float maxR = 120f)
  {
    Position = pos;
    maxRadius = maxR;
    color = Colors.White;
  }

  public override void _Process(double delta)
  {
    elapsed += (float)delta;
    float t = elapsed / duration;

    if (t >= 1f)
    {
      QueueFree();
      return;
    }

    radius = Mathf.Lerp(0f, maxRadius, t);
    Modulate = new Color(1, 1, 1, 1f - t); // fade out
    QueueRedraw();
  }

  public override void _Draw()
  {
    // Anel: desenha círculo maior e sobrepõe com transparente
    DrawArc(Vector2.Zero, radius, 0f, Mathf.Tau, 32, color, 4f);
  }
}