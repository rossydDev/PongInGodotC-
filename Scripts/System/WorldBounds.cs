using Godot;

/// <summary>
/// Responsável por guardar e calcular os limites físicos da tela.
/// Inicializado pelo GameManager no _Ready. Usado por Paddle, BallBase e IA.
/// </summary>
public static class WorldBounds
{
  public static float Width { get; private set; }

  public static void Initialize(float width)
  {
    Width = width;
  }

  public static bool IsOutOfBounds(float x, float halfWidth = 0f)
  {
    return x - halfWidth < 0f || x + halfWidth > Width;
  }

  public static float ClampX(float x, float halfWidth)
  {
    return Mathf.Clamp(x, halfWidth, Width - halfWidth);
  }
}
