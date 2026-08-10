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
	/// <summary>How many turns may be buffered ahead. Two is enough for a U-turn
	/// (round the corner, then back) without letting mashing queue up a long tail.</summary>
	private const int MaxBufferedTurns = 2;

	private readonly int _cellNumber;
	private readonly Random _rng;
	private readonly Queue<Vector2I> _turns = new();

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
		_turns.Clear();
		Walls = BuildWalls();
		Body = BuildBody();
		Direction = new Vector2I(1, 0);
		Food = PickFreeCell();
	}

	public MoveResult Step()
	{
		ApplyNextTurn();

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

	/// <summary>
	/// Records an intended turn. Deliberately does not validate: whether a turn is
	/// legal depends on where the body is *when the tick lands*, so judging it at
	/// input time throws away turns that would have been fine a moment later.
	/// </summary>
	public void EnqueueTurn(Vector2I direction)
	{
		if (_turns.Count < MaxBufferedTurns)
		{
			_turns.Enqueue(direction);
		}
	}

	/// <summary>
	/// Takes the first buffered turn that isn't a reversal into the neck. Reversals
	/// are discarded rather than kept, since they can never become legal from here.
	/// </summary>
	private void ApplyNextTurn()
	{
		while (_turns.Count > 0)
		{
			Vector2I turn = _turns.Dequeue();
			if (Body.Last.Value + turn != Body.Last.Previous.Value)
			{
				Direction = turn;
				return;
			}
		}
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
