using Godot;

public partial class Actor : Node2D
{
  [Export] private Sprite2D sprite2D;

  protected float spriteWidth;

  public override void _Ready()
  {
    Vector2 size = sprite2D.Texture.GetSize();

    spriteWidth = size.X * sprite2D.Scale.X;

    spriteWidth *= 0.5f;

    GD.Print(spriteWidth);
  }
}