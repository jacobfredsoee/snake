using System;
using System.Collections.Generic;
using Godot;

public enum MoveResult
{
	Moved,
	Ate,
	Died,
	Won,
}

/// <summary>
/// The snake rules: the board, the body, the food, and what one tick does.
/// Holds no Node or scene-tree types, so a round can be played out in a test
/// without starting the engine. Randomness is injected for the same reason.
/// </summary>
public class SnakeGame
{
	private readonly int _cellNumber;
	private readonly Random _rng;

	public LinkedList<Vector2I> Body { get; private set; }
	public HashSet<Vector2I> Walls { get; private set; }
	public Vector2I Food { get; private set; }
	public Vector2I Direction { get; private set; }

	public SnakeGame(int cellNumber, Random rng)
	{
		_cellNumber = cellNumber;
		_rng = rng;
		Reset();
	}

	public void Reset()
	{
		Walls = BuildWalls();
		Body = BuildBody();
		Direction = new Vector2I(0, 1);
		Food = PickFreeCell();
	}

	public MoveResult Step()
	{
		Vector2I head = Body.Last.Value + Direction;

		if (Walls.Contains(head) || Body.Contains(head))
		{
			return MoveResult.Died;
		}

		Body.AddLast(head);

		if (head == Food)
		{
			if (Body.Count >= _cellNumber * _cellNumber - Walls.Count)
			{
				return MoveResult.Won;
			}
			Food = PickFreeCell();
			return MoveResult.Ate;
		}

		Body.RemoveFirst();
		return MoveResult.Moved;
	}

	public void TrySetDirection(Vector2I direction)
	{
		// If we try to move into the neck (i.e. backwards), do nothing.
		if (Body.Last.Value + direction == Body.Last.Previous.Value)
		{
			return;
		}

		Direction = direction;
	}

	private LinkedList<Vector2I> BuildBody()
	{
		int center = _cellNumber / 2;
		return new LinkedList<Vector2I>([
			new Vector2I(center - 1, center),
			new Vector2I(center, center),
			new Vector2I(center + 1, center),
		]);
	}

	private HashSet<Vector2I> BuildWalls()
	{
		var walls = new HashSet<Vector2I>();
		for (int i = 0; i < _cellNumber; i++)
		{
			walls.Add(new Vector2I(i, 0));
			walls.Add(new Vector2I(i, _cellNumber - 1));
			walls.Add(new Vector2I(0, i));
			walls.Add(new Vector2I(_cellNumber - 1, i));
		}

		return walls;
	}

	private Vector2I PickFreeCell()
	{
		Vector2I cell;
		do
		{
			cell = new Vector2I(_rng.Next(_cellNumber), _rng.Next(_cellNumber));
		}
		while (Walls.Contains(cell) || Body.Contains(cell));

		return cell;
	}
}
