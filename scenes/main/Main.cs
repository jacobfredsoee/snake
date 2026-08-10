using System;
using Godot;

public partial class Main : Node2D
{
	#region Fields

	private SnakeGame _game;
	private Menu _menu;
	private Border _border;
	private Timer _moveTimer;
	private Timer _delayStartTimer;
	private int _cellSize;

	public GameState CurrentGameState { get; private set; } = GameState.None;

	#endregion

	#region Setup

	public override void _Ready()
	{
		_cellSize = (int)GetViewportRect().Size.X / Settings.CellNumber;

		_moveTimer = GetNode<Timer>("MoveTimer");
		_delayStartTimer = GetNode<Timer>("DelayStartTimer");
		_menu = GetNode<Menu>("Menu");

		_moveTimer.WaitTime = Settings.Speed;

		_menu.StartGame += OnStartButtonPressed;
		_menu.ExitGame += OnExitButtonPressed;
		_menu.ResumeGame += OnResumeButtonPressed;
		_menu.MainMenu += OnMainMenuButtonPressed;

		ChangeGameState(GameState.MainMenu);
	}

	#endregion

	#region State machine


	private void ChangeGameState(GameState newState)
	{
		if (newState == CurrentGameState)
		{
			return;
		}

		ExitGameState(CurrentGameState);
		CurrentGameState = newState;
		EnterGameState(CurrentGameState);
	}

	private void EnterGameState(GameState state)
	{
		switch (state)
		{
			case GameState.MainMenu:
				ClearBoard();
				_menu.ShowMainMenu();
				break;
			case GameState.Running:
				StartRound();
				break;
			case GameState.GameOver:
				_menu.ShowGameOverMenu();
				break;
		}

		QueueRedraw();
	}

	private void ExitGameState(GameState state)
	{
		switch (state)
		{
			case GameState.MainMenu:
				_menu.HideMenu();
				break;
			case GameState.Running:
				// Pause belongs to Running, so leaving clears it whatever the destination.
				GetTree().Paused = false;
				_delayStartTimer.Stop();
				_moveTimer.Stop();
				break;
			case GameState.GameOver:
				_menu.HideMenu();
				break;
		}
	}

	#endregion

	#region Round lifecycle

	private void StartRound()
	{
		_game = new SnakeGame(Settings.CellNumber, new Random());
		SetupBorder();
		_delayStartTimer.Start();
	}

	private void SetupBorder()
	{
		_border?.QueueFree();
		_border = GD.Load<PackedScene>("uid://cbd33asjyoy5k").Instantiate<Border>();
		AddChild(_border);
	}

	private void ClearBoard()
	{
		_game = null;
		_border?.QueueFree();
		_border = null;
	}

	#endregion

	#region Input

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause") && CurrentGameState == GameState.Running)
		{
			PauseGame();
		}

		if (CurrentGameState != GameState.Running || _game == null)
		{
			return;
		}

		Vector2I? newDirection = null;
		if (Input.IsActionPressed("move_up"))
		{
			newDirection = new Vector2I(0, -1);
		}
		if (Input.IsActionPressed("move_down"))
		{
			newDirection = new Vector2I(0, 1);
		}
		if (Input.IsActionPressed("move_left"))
		{
			newDirection = new Vector2I(-1, 0);
		}
		if (Input.IsActionPressed("move_right"))
		{
			newDirection = new Vector2I(1, 0);
		}

		if (newDirection.HasValue)
		{
			_game.TrySetDirection(newDirection.Value);
		}
	}

	#endregion

	#region Ticks

	public void OnDelayStartTimerTimeout()
	{
		_moveTimer.Start();
	}

	public void OnMoveTimerTimeout()
	{
		if (CurrentGameState != GameState.Running || _game == null)
		{
			return;
		}

		if (_game.Step() == MoveResult.Died)
		{
			ChangeGameState(GameState.GameOver);
			return;
		}

		QueueRedraw();
	}

	#endregion

	#region Drawing

	public override void _Draw()
	{
		if (_game == null)
		{
			return;
		}

		foreach (Vector2I cell in _game.Body)
		{
			DrawRect(new Rect2(cell.X * _cellSize, cell.Y * _cellSize, _cellSize, _cellSize), Colors.Green);
		}

		DrawRect(new Rect2(_game.Food.X * _cellSize, _game.Food.Y * _cellSize, _cellSize, _cellSize), Colors.Red);
	}

	#endregion

	#region Menu commands

	private void OnStartButtonPressed()
	{
		ChangeGameState(GameState.Running);
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void PauseGame()
	{
		_menu.ShowPauseMenu();
		GetTree().Paused = true;
	}

	private void OnResumeButtonPressed()
	{
		_menu.HideMenu();
		GetTree().Paused = false;
	}

	private void OnMainMenuButtonPressed()
	{
		ChangeGameState(GameState.MainMenu);
	}

	#endregion
}
