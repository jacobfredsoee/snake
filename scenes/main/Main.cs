using System.Collections.Generic;
using Godot;

public enum CollisionType
{
	Border,
	Snake,
	Food,
	None
}

public partial class Main : Node2D
{
	public int CellNumber = 32;
	public int CellSize;
	public int ViewHeight;
	public int ViewWidth;
	public List<Vector2> BorderCells = [];
	public LinkedList<Vector2> SnakeBody;
	public List<Vector2> FoodCells = [];
	public Timer MoveTimer;
	public Vector2 SnakeDirection = new Vector2(0, 1);

	public override void _Ready()
	{
		ViewWidth = (int)GetViewportRect().Size.X;
		ViewHeight = (int)GetViewportRect().Size.Y;
		CellSize = ViewWidth / CellNumber;
		MoveTimer = GetNode<Timer>("MoveTimer");
		MoveTimer.Start();

		SnakeBody = new LinkedList<Vector2>([
			new Vector2(10, 10),
			new Vector2(10, 11),
			new Vector2(10, 12),
			new Vector2(10, 13),
			new Vector2(10, 14)
		]);
		InitializeBorderCells();
		SpawnFood();
	}

	private void InitializeBorderCells()
	{
		for (int x = 0; x < CellNumber; x++)
		{
			BorderCells.Add(new Vector2(x, 0)); // Top border
			BorderCells.Add(new Vector2(x, CellNumber - 1)); // Bottom border
			BorderCells.Add(new Vector2(0, x)); // Left border
			BorderCells.Add(new Vector2(CellNumber - 1, x)); // Right border
		}
	}

	private void SpawnFood()
	{
		Vector2 foodCell;
		do
		{
			foodCell = new Vector2(GD.RandRange(0, CellNumber - 1), GD.RandRange(0, CellNumber - 1));
		}
		while (BorderCells.Contains(foodCell) || SnakeBody.Contains(foodCell) || FoodCells.Contains(foodCell));

		FoodCells.Add(foodCell);
	}

	public override void _Draw()
	{
		//This is where I will draw the food and snake cells
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
