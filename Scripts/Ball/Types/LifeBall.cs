using System.Diagnostics;
using Godot;

public partial class LifeBall : BallBase
{
  protected override void OnAreaEntered(Area2D area)
  {
    var owner = area.Owner;

    if (owner is Paddle)
    {
      PlayerController playerController = owner.GetNodeOrNull<PlayerController>("PlayerController");

      playerController.HealthComponent.Heal(1);

      SpawnExplosion();
      QueueFree();
    }
  }
}