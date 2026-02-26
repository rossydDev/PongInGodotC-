using Godot;

public partial class Actor : Node2D
{
  [Export] private Sprite2D sprite2D;

  protected float spriteHeihgt;

  public override void _Ready()
  {
    Vector2 size = sprite2D.Texture.GetSize();

    spriteHeihgt = size.X * sprite2D.Scale.Y;

    spriteHeihgt *= 0.5f;
  }
}