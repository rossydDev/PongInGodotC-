// BallExplosion.cs
using Godot;

public partial class BallExplosion : Node2D
{
  [Export] GpuParticles2D particles;

  public void Explode(Vector2 pos, Color color, float scale = 1f)
  {
    Position = pos;

    var material = new ParticleProcessMaterial();
    material.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere;
    material.EmissionSphereRadius = 4f * scale;
    material.Spread = 180f;
    material.InitialVelocityMin = 150f * scale;
    material.InitialVelocityMax = 400f * scale;
    material.Gravity = new Vector3(0, 120, 0);
    material.ScaleMin = 1.0f * scale;
    material.ScaleMax = 2.5f * scale;
    material.Color = color;

    var gradient = new Gradient();
    gradient.SetColor(0, color);
    gradient.SetColor(1, new Color(color.R, color.G, color.B, 0f));
    var gradientTexture = new GradientTexture1D();
    gradientTexture.Gradient = gradient;
    material.ColorRamp = gradientTexture;

    particles.ProcessMaterial = material;
    particles.Amount = (int)(60 * scale);
    particles.Lifetime = 1.0f;
    particles.OneShot = true;
    particles.Explosiveness = 1.0f;
    particles.Emitting = true;

    GetTree().CreateTimer(1.5f).Connect(
        SceneTreeTimer.SignalName.Timeout,
        Callable.From(QueueFree)
    );
  }
}