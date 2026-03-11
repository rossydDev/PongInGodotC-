using Godot;

/// <summary>
/// Responsável por conectar todas as peças da arena após a cena estar pronta.
/// Camp.cs vira apenas o nó raiz da cena — CampInitializer cuida do setup.
/// Para criar uma arena de Boss: faça uma cena nova, adicione CampInitializer
/// e configure os [Export] no Inspector sem precisar herdar Camp.
/// </summary>
public partial class CampInitializer : Node
{
  [ExportGroup("Paddles")]
  [Export] private PackedScene paddleEnemyScene;
  [Export] private Node2D playerSpawnPosition;
  [Export] private Node2D enemySpawnPosition;
  [Export] private Node2D ballSpawnPosition;

  [ExportGroup("Gols")]
  [Export] private Gol enemyGol;
  [Export] private Gol playerGol;

  [ExportGroup("Systems")]
  [Export] private ScoreControll scoreControll;

  [ExportGroup("HUD")]
  [Export] private ScoreHud scoreHud;
  [Export] private HeartHudComponent heartHudComponent;

  [ExportGroup("Dialogue")]
  [Export] private DialogueTriggerComponent dialogueTrigger;
  [Export] private MatchIntroComponent matchIntro;

  private Paddle paddleEnemy;
  private Paddle playerPaddle;

  public void Setup(PackedScene playerScene)
  {
    CreatePaddleEnemy();
    CreatePlayer(playerScene);
    SetupScore();
    dialogueTrigger?.Initialize();
  }

  // Chamado pelo Camp quando GameState.Intro é emitido
  public void BeginIntro()
  {
    matchIntro?.Begin();
  }

  private void CreatePlayer(PackedScene playerScene)
  {
    // Informa ao BallController onde spawnar a bola nesta arena
    BallController.Instance.SetSpawnPosition(ballSpawnPosition.GlobalPosition);

    playerPaddle = playerScene.Instantiate<Paddle>();
    GetParent().AddChild(playerPaddle);
    playerPaddle.GlobalPosition = playerSpawnPosition.GlobalPosition;

    enemyGol.SetAdversaryPaddle(playerPaddle);

    var playerController = playerPaddle.GetNode<PlayerController>("PlayerController");
    heartHudComponent.Initializer(playerController.HealthComponent);
    heartHudComponent.ConnectAbility(playerController.CurrentAbility);

    var life = playerPaddle.GetNodeOrNull<LifeComponent>("LifeComponent");
    life?.SetSpawnPosition(playerSpawnPosition.GlobalPosition);
  }

  private void CreatePaddleEnemy()
  {
    if (paddleEnemyScene == null) return;

    paddleEnemy = paddleEnemyScene.Instantiate<Paddle>();
    GetParent().AddChild(paddleEnemy);
    paddleEnemy.GlobalPosition = enemySpawnPosition.GlobalPosition;

    playerGol.SetAdversaryPaddle(paddleEnemy);
  }

  private void SetupScore()
  {
    scoreControll.Initializer();
    scoreControll.ScoreUpdate += (playerScore, enemyScore) =>
      scoreHud.SetScore(playerScore, enemyScore);
  }
}
