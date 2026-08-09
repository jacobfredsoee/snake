using System.Collections.Generic;
using Godot;

public enum CollisionType
{
	Border,
	Snake,
	Food,
	None
}

public enum GameState
{
	MainMenu,
	Running,
	GameOver
}

public partial class Main : Node2D
{
	public int CellSize;
	public int ViewHeight;
	public int ViewWidth;
	public List<Vector2> BorderCells;
	public LinkedList<Vector2> SnakeBody;
	public List<Vector2> FoodCells;
	public Timer MoveTimer;
	public Vector2 SnakeDirection;
	public GameState CurrentGameState;
	public Menu Menu;
	private Border _border;
	public override void _Ready()
	{
		ViewWidth = (int)GetViewportRect().Size.X;
		ViewHeight = (int)GetViewportRect().Size.Y;
		MoveTimer = GetNode<Timer>("MoveTimer");
		Menu = GetNode<Menu>("Menu");

		Menu.StartGame += OnStartButtonPressed;
		Menu.ExitGame += OnExitButtonPressed;
		Menu.ResumeGame += OnResumeButtonPressed;
		Menu.MainMenu += OnMainMenuButtonPressed;

		ChangeGameState(GameState.MainMenu);
	}

	private bool IsValidStateChange(GameState newState)
	{
		// Placeholder for now
		return true;
	}

	private void ChangeGameState(GameState newState)
	{
		if (!IsValidStateChange(newState))
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
				Menu.ShowMainMenu();
				break;
			case GameState.Running:
				InitializeGame();
				break;
		}
	}

	private void ExitGameState(GameState state)
	{
		switch (state)
		{
			case GameState.MainMenu:
				Menu.HideMenu();
				break;
			case GameState.Running:
				_border.QueueFree();
				_border = null;
				MoveTimer.Stop();
				break;
		}
	}

	private void OnStartButtonPressed()
	{
		ChangeGameState(GameState.Running);
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnPauseButtonPressed()
	{
		Menu.ShowPauseMenu();
		GetTree().Paused = true;
	}

	private void OnResumeButtonPressed()
	{
		Menu.HideMenu();
		GetTree().Paused = false;
	}

	private void OnMainMenuButtonPressed()
	{
		ChangeGameState(GameState.MainMenu);
	}
	public void OnDelayStartTimerTimeout()
	{
		MoveTimer.Start();
	}

	private void InitializeGame()
	{
		CellSize = ViewWidth / Settings.CellNumber;
		MoveTimer.WaitTime = Settings.Speed;

		SpawnBorderCells();
		SpawnSnake();
		SpawnFood();
		GetNode<Timer>("DelayStartTimer").Start();
		QueueRedraw();
	}

	private void SpawnSnake()
	{
		SnakeBody = [];
		int center = Settings.CellNumber / 2;
		SnakeBody.AddLast(new Vector2(center, center - 1));
		SnakeBody.AddLast(new Vector2(center, center));
		SnakeBody.AddLast(new Vector2(center, center + 1));
		SnakeDirection = new Vector2(0, 1);
	}

	private void SpawnBorderCells()
	{
		BorderCells = [];
		for (int x = 0; x < Settings.CellNumber; x++)
		{
			BorderCells.Add(new Vector2(x, 0)); // Top border
			BorderCells.Add(new Vector2(x, Settings.CellNumber - 1)); // Bottom border
			BorderCells.Add(new Vector2(0, x)); // Left border
			BorderCells.Add(new Vector2(Settings.CellNumber - 1, x)); // Right border
		}

		_border = GD.Load<PackedScene>("uid://cbd33asjyoy5k").Instantiate<Border>();
		AddChild(_border);

	}

	private void SpawnFood()
	{
		FoodCells = [];
		Vector2 foodCell;
		do
		{
			foodCell = new Vector2(GD.RandRange(0, Settings.CellNumber - 1), GD.RandRange(0, Settings.CellNumber - 1));
		}
		while (BorderCells.Contains(foodCell) || SnakeBody.Contains(foodCell) || FoodCells.Contains(foodCell));

		FoodCells.Add(foodCell);
	}

	public override void _Draw()
	{
		if (CurrentGameState != GameState.Running)
		{
			return;
		}

		foreach (Vector2 bodyPart in SnakeBody)
		{
			DrawRect(new Rect2(bodyPart.X * CellSize, bodyPart.Y * CellSize, CellSize, CellSize), Colors.Green);
		}
		foreach (Vector2 foodCell in FoodCells)
		{
			DrawRect(new Rect2(foodCell.X * CellSize, foodCell.Y * CellSize, CellSize, CellSize), Colors.Red);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("pause"))
		{
			OnPauseButtonPressed();
		}



		if (CurrentGameState != GameState.Running)
		{
			return;
		}

		var newDirection = SnakeDirection;
		if (Input.IsActionPressed("move_up"))
		{
			newDirection = new Vector2(0, -1);
		}
		if (Input.IsActionPressed("move_down"))
		{
			newDirection = new Vector2(0, 1);
		}
		if (Input.IsActionPressed("move_left"))
		{
			newDirection = new Vector2(-1, 0);
		}
		if (Input.IsActionPressed("move_right"))
		{
			newDirection = new Vector2(1, 0);
		}

		if (IsValidNewDirection(newDirection))
		{
			SnakeDirection = newDirection;
		}
	}

	private bool IsValidNewDirection(Vector2 newDirection)
	{
		// If the new cell is the same as the second to last cell, we can't go in that direction
		if (SnakeBody.Last.Value + newDirection == SnakeBody.Last.Previous.Value)
		{
			return false;
		}
		return true;
	}

	public void OnMoveTimerTimeout()
	{
		if (CurrentGameState != GameState.Running || SnakeBody == null)
		{
			return;
		}
		MoveSnake();
		QueueRedraw();
	}

	private CollisionType IsMoveCollision(Vector2 newCell)
	{
		if (SnakeBody.Contains(newCell))
		{
			return CollisionType.Snake;
		}
		if (BorderCells.Contains(newCell))
		{
			return CollisionType.Border;
		}
		if (FoodCells.Contains(newCell))
		{
			return CollisionType.Food;
		}
		return CollisionType.None;
	}

	private void MoveSnake()
	{
		var newCell = SnakeBody.Last.Value + SnakeDirection;
		var collisionType = IsMoveCollision(newCell);
		if (collisionType != CollisionType.None)
		{
			GD.Print("Move collision");
		}
		SnakeBody.AddLast(newCell);
		if (collisionType == CollisionType.Food)
		{
			FoodCells.Remove(newCell);
			SpawnFood();
		}
		else
		{
			SnakeBody.RemoveFirst();
		}
	}
}
