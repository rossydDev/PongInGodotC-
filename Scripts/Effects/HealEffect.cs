// HealEffect.cs
using Godot;

public partial class HealEffect : Node2D
{
  private GpuParticles2D particles;
  private Paddle target;

  public override void _Ready()
  {
    particles = new GpuParticles2D();
    AddChild(particles);
    ConfigureParticles();
  }

  public override void _Process(double delta)
  {
    if (target != null && IsInstanceValid(target))
      GlobalPosition = target.GlobalPosition;
  }

  public void Play(Paddle paddle)
  {
    target = paddle;
    GlobalPosition = paddle.GlobalPosition;

    Vector2 size = paddle.GetNode<CollisionShape2D>("CollisionShape2D")
                         .Shape.GetRect().Size;

    var mat = (ParticleProcessMaterial)particles.ProcessMaterial;
    mat.EmissionBoxExtents = new Vector3(size.X * 0.45f, 1f, 0f);

    particles.Restart();

    float lifetime = (float)particles.Lifetime + 0.3f;
    GetTree().CreateTimer(lifetime).Timeout += QueueFree;
  }

  private void ConfigureParticles()
  {
    particles.Amount = 10;
    particles.Lifetime = 0.5f;
    particles.OneShot = true;
    particles.Explosiveness = 0f;
    particles.SpeedScale = 1.0f;
    particles.LocalCoords = false;

    particles.Texture = CreateSoftCircleTexture(6);

    var mat = new ParticleProcessMaterial();

    // Emite numa linha plana (largura ajustada no Play)
    mat.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Box;
    mat.EmissionBoxExtents = new Vector3(40f, 1f, 0f);

    mat.Direction = new Vector3(0f, -1f, 0f);
    mat.Spread = 8f;
    mat.InitialVelocityMin = 60f;   // mais rápido
    mat.InitialVelocityMax = 100f;  // mais rápido

    mat.Gravity = Vector3.Zero;

    mat.ScaleMin = 0.25f;  // bem pequeno
    mat.ScaleMax = 0.75f;  // bem pequeno

    // Aparece e some suavemente
    var alphaCurve = new Curve();
    alphaCurve.AddPoint(new Vector2(0f, 0f));
    alphaCurve.AddPoint(new Vector2(0.2f, 1f));
    alphaCurve.AddPoint(new Vector2(1f, 0f));
    var alphaTexture = new CurveTexture();
    alphaTexture.Curve = alphaCurve;

    // Verde simples
    var gradient = new Gradient();
    gradient.SetColor(0, new Color(0.6f, 1f, 0.65f, 1f));
    gradient.SetColor(1, new Color(0.2f, 0.9f, 0.4f, 1f));
    var colorRamp = new GradientTexture1D();
    colorRamp.Gradient = gradient;
    mat.ColorRamp = colorRamp;

    particles.ProcessMaterial = mat;

    // Blend normal — sem brilho excessivo
    var canvasMat = new CanvasItemMaterial();
    canvasMat.BlendMode = CanvasItemMaterial.BlendModeEnum.Mix;
    particles.Material = canvasMat;
  }

  private ImageTexture CreateSoftCircleTexture(int size)
  {
    var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
    float center = size * 0.5f;

    for (int x = 0; x < size; x++)
    {
      for (int y = 0; y < size; y++)
      {
        float dist = new Vector2(x - center, y - center).Length();
        float alpha = Mathf.Clamp(1f - (dist / center), 0f, 1f);
        image.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
      }
    }

    return ImageTexture.CreateFromImage(image);
  }
}