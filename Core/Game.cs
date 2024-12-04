public enum GameState
{
    Menu,
    InGame,
    GameOver,
    Paused,
    Options,
    Lobby,
    Win
}

public class Game
{
    public Frame frame;
    public Omar omar;
    public GameState currentState; 
    private System.Windows.Forms.Timer gameTimer;
    private HashSet<Keys> pressedKeys;
    public Map map;
    public bool isFullScreen;
    private const int GameWidth = 800;
    private const int GameHeight = 600;
    public Menu menu;
    public PauseScreen pauseScreen;
    public GameOverScreen gameOverScreen;
    public LobbyScreen lobbyScreen;
    public WinScreen winScreen;
    private InputHandler inputHandler;
    private Wave currentWave; 



    public Game()
    {
        frame = new Frame();
        omar = new Omar(400, 290, 40); 
        map = new Map(omar); 
        pressedKeys = new HashSet<Keys>(); 
        isFullScreen = false; 

        menu = new Menu();
        pauseScreen = new PauseScreen();
        gameOverScreen = new GameOverScreen();
        lobbyScreen = new LobbyScreen();
        winScreen = new WinScreen();
        currentState = GameState.Menu;

        currentWave = new Wave(1, TimeSpan.FromSeconds(10)); 
        inputHandler = new InputHandler(this);

        frame.KeyDown += new KeyEventHandler(inputHandler.OnKeyDown);
        frame.KeyUp += new KeyEventHandler(inputHandler.OnKeyUp);
        frame.KeyPreview = true;
        frame.Paint += new PaintEventHandler(FramePaint);

        gameTimer = new System.Windows.Forms.Timer();
        gameTimer.Interval = 16; 
        gameTimer.Tick += GameTimer_Tick;   
    }

     public void ShowMenu()
    {
        currentState = GameState.Menu;
        frame.BackColor = Color.White;
        gameTimer.Stop();
        frame.Invalidate();
    }
    
    public void StartGame()
    {
        currentState = GameState.InGame;
        frame.BackColor = Color.Gray;
        gameTimer.Start();
        frame.Invalidate();
    }

    public void RestartGame()
    {
        omar = new Omar(400, 290, 40);
        map = new Map(omar);
        StartGame();
        inputHandler.ResetInputHandler(omar);
    }

    public void GoToLobby(){
         if (currentWave.WaveNumber == 10) 
        {
            currentState = GameState.Win; 
        } else {
            currentState = GameState.Lobby;
        }
        gameTimer.Stop();
        frame.Invalidate();
    }
    
    public void ResetGameForLobby()
    {
        map.ClearObjects();
        omar.ResetPosition();
        currentState = GameState.InGame;
        frame.BackColor = Color.Gray;
        currentWave = new Wave(currentWave.WaveNumber + 1, TimeSpan.FromSeconds(10)); 
        gameTimer.Start();
        frame.Invalidate();
    }


    public void PauseGame()
    {
        currentState = GameState.Paused;
        frame.BackColor = Color.White;
        currentWave.Pause();
        gameTimer.Stop();
        frame.Invalidate();
    }

    public void ResumeGame()
    {
        currentState = GameState.InGame;
        frame.BackColor = Color.Gray;
        currentWave.Resume();
        gameTimer.Start();
        frame.Invalidate();
    }

     public void ShowWinScreen()
    {
        currentState = GameState.Win;
        frame.BackColor = Color.LightGreen;
        gameTimer.Stop();
        frame.Invalidate();
    }


    private void GameTimer_Tick(object? sender, EventArgs e)
    {   
        switch (currentState)
        {
            case GameState.InGame:
                map.update();
                frame.Invalidate();
                int timeLeft = currentWave.GetTimeLeft();
                if (timeLeft == 0)
                {
                    GoToLobby();
                }
                if (omar.HP <= 0)
                {
                    currentState = GameState.GameOver;
                }  if (currentWave.WaveNumber == 10)
                {
                    ShowWinScreen();
                }
                break;
            case GameState.Paused:
                break;
        }
    }
    

    private void FramePaint(object? sender, PaintEventArgs e)
    {
        switch (currentState)
        {
            case GameState.InGame:
                map.Draw(e.Graphics);
                omar.Draw(e.Graphics);
                frame.DrawStatistics(e.Graphics, omar);
                frame.DrawTimer(e.Graphics,  currentWave.GetTimeLeft(), currentWave.WaveNumber);
                break;
            case GameState.Menu:
                menu.Draw(e.Graphics, frame.ClientSize);
                break;
            case GameState.Paused:
                pauseScreen.Draw(e.Graphics, frame.ClientSize);
                break;
            case GameState.GameOver:
                gameOverScreen.Draw(e.Graphics, frame.ClientSize);
                break;
            case GameState.Lobby:
                lobbyScreen.Draw(e.Graphics, frame.ClientSize);
                break;
            case GameState.Win:
                winScreen.Draw(e.Graphics, frame.ClientSize);
                break;
            }
    }

    public static void Main(string[] args)
    {
        Game game = new Game();
        game.ShowMenu(); 
        Application.Run(game.frame);
    }
}
