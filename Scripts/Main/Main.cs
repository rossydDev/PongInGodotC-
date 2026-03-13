using Godot;

public partial class Main : Node2D
{
  [Export] private PackedScene campScene;
  [Export] private PackedScene playerPaddleScene;
  [Export] private VictoryScreen victoryScreen;
  [Export] private GameOverScreen gameOverScreen;

  private Camp currentCamp;
  public Camp CurrentCamp => currentCamp;

  public override void _Ready()
  {
    victoryScreen.OnContinue += OnContinue;
    gameOverScreen.OnRetry += OnRetry;
    gameOverScreen.OnQuit += OnQuit;

    LoadCamp(campScene);
  }

  private void LoadCamp(PackedScene scene)
  {
    if (currentCamp != null)
    {
      currentCamp.GetNode<CampInitializer>("System/CampInitializer").Cleanup();
      currentCamp.QueueFree();
      currentCamp = null;
    }

    currentCamp = scene.Instantiate<Camp>();
    AddChild(currentCamp);

    // Escuta os sinais de fim de partida do Camp (após animação do ScoreHud)
    currentCamp.OnCampReady += OnCurrentCampReady;
    currentCamp.OnPlayerWin += OnPlayerWin;
    currentCamp.OnPlayerLoser += OnPlayerLoser;

    currentCamp.Initializer(playerPaddleScene);
    DialogueManager.Instance.RegisterGameLayer(currentCamp);
  }

  private void OnCurrentCampReady()
  {
    GameManager.Instance.SwitchState(GameState.Intro);
  }

  private void OnPlayerWin()
  {
    GD.Print("Testando a vitoria!");
    victoryScreen.ShowVictory();
  }

  private void OnPlayerLoser()
  {
    GD.Print("Testando a derrota!");
    gameOverScreen.ShowGameOver();
  }

  private async void OnContinue()
  {
    if (TransitionManager.Instance != null)
      await TransitionManager.Instance.PlayTransition(() => LoadCamp(campScene));
    else
      LoadCamp(campScene);
  }

  private async void OnRetry()
  {
    if (TransitionManager.Instance != null)
      await TransitionManager.Instance.PlayTransition(() => LoadCamp(campScene));
    else
      LoadCamp(campScene);
  }

  private void OnQuit()
  {
    GetTree().Quit();
  }
}