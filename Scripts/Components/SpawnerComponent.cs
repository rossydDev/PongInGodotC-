using Godot;

public partial class SpawnerComponent : Node2D
{
  [Export] private bool oneShot;
  [Export] private float minTimeout = 1f;
  [Export] private float maxTimeout = 3f;
  [Export] private PackedScene[] scenes;
  [Export] private CollisionShape2D spawnArea;

  private float _timeout;
  private bool _hasFired;
  private RandomNumberGenerator _rng = new RandomNumberGenerator();
  private Godot.Collections.Array<Node> _spawnedItems = new();

  public override void _Ready()
  {
    if (spawnArea == null)
    {
      GD.PrintErr("SpawnerComponent: spawnArea não foi definido!");
      return;
    }

    _rng.Randomize();
    ResetTimeout();

    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  public override void _ExitTree()
  {
    if (GameManager.Instance != null)
      GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
  }

  public override void _Process(double delta)
  {
    if (GameManager.Instance.CurrentState != GameState.Start) return;
    if (oneShot && _hasFired) return;
    if (scenes == null || scenes.Length == 0) return;

    _timeout -= (float)delta;

    if (_timeout <= 0f)
    {
      Spawn();
      if (oneShot)
        _hasFired = true;
      else
        ResetTimeout();
    }
  }

  private void OnGameStateChanged()
  {
    var state = GameManager.Instance.CurrentState;

    bool shouldClear =
        state == GameState.PlayerScore ||
        state == GameState.EnemyScore ||
        state == GameState.PlayerWin ||
        state == GameState.PlayerLoser;

    if (shouldClear)
      ClearSpawnedItems();

    // Reinicia o oneShot quando voltar ao Start
    if (state == GameState.Start)
    {
      _hasFired = false;
      ResetTimeout();
    }
  }

  private void Spawn()
  {
    var scene = scenes[_rng.RandiRange(0, scenes.Length - 1)];
    if (scene == null) return;

    var instance = scene.Instantiate<Node2D>();
    instance.GlobalPosition = GetRandomPositionInShape();

    GetParent().AddChild(instance);
    _spawnedItems.Add(instance);
  }

  private void ClearSpawnedItems()
  {
    foreach (var item in _spawnedItems)
    {
      if (IsInstanceValid(item))
        item.QueueFree();
    }
    _spawnedItems.Clear();
  }

  private Vector2 GetRandomPositionInShape()
  {
    if (spawnArea?.Shape == null)
      return GlobalPosition;

    var shape = spawnArea.Shape;
    var origin = spawnArea.GlobalPosition;

    if (shape is RectangleShape2D rect)
    {
      var size = rect.Size;
      return origin + new Vector2(
          _rng.RandfRange(-size.X / 2f, size.X / 2f),
          _rng.RandfRange(-size.Y / 2f, size.Y / 2f)
      );
    }

    if (shape is CircleShape2D circle)
    {
      float angle = _rng.RandfRange(0f, Mathf.Tau);
      float radius = _rng.RandfRange(0f, circle.Radius);
      return origin + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

    if (shape is CapsuleShape2D capsule)
    {
      return origin + new Vector2(
          _rng.RandfRange(-capsule.Radius, capsule.Radius),
          _rng.RandfRange(-capsule.Height / 2f, capsule.Height / 2f)
      );
    }

    return origin;
  }

  private void ResetTimeout()
  {
    _timeout = _rng.RandfRange(minTimeout, maxTimeout);
  }

  public void Reset()
  {
    _hasFired = false;
    ResetTimeout();
  }
}