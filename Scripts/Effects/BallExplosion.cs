// BallExplosion.cs
using Godot;

public partial class BallExplosion : Node2D
{
  [Export] GpuParticles2D particles;

  public void Explode(Vector2 pos, Color color)
  {
    Position = pos;

    // Configura o material das partículas via código
    var material = new ParticleProcessMaterial();
    material.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere;
    material.EmissionSphereRadius = 4f;
    material.Direction = new Vector3(0, 0, 0);
    material.Spread = 180f;
    material.InitialVelocityMin = 80f;
    material.InitialVelocityMax = 220f;
    material.Gravity = new Vector3(0, 120, 0);
    material.ScaleMin = 0.4f;
    material.ScaleMax = 1.2f;
    material.Color = color;

    // Fade out das partículas
    var gradient = new Gradient();
    gradient.SetColor(0, color);
    gradient.SetColor(1, new Color(color.R, color.G, color.B, 0f));
    var gradientTexture = new GradientTexture1D();
    gradientTexture.Gradient = gradient;
    material.ColorRamp = gradientTexture;

    particles.ProcessMaterial = material;
    particles.Amount = 40;
    particles.Lifetime = 0.8f;
    particles.OneShot = true;
    particles.Explosiveness = 1.0f;
    particles.Emitting = true;

    material.ScaleMin = 1.0f;
    material.ScaleMax = 2.5f;
    material.InitialVelocityMin = 150f;
    material.InitialVelocityMax = 400f;
    particles.Amount = 60;
    particles.Lifetime = 1.0f;

    // Auto-destrói após as partículas sumirem
    GetTree().CreateTimer(1.2f).Connect(
        SceneTreeTimer.SignalName.Timeout,
        Callable.From(QueueFree)
    );
  }
}