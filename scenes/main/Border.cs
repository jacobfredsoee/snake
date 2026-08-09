using Godot;
using System;

public partial class Border : Node2D
{
	public int CellNumber = 32;
	public int CellSize;
	public int ViewHeight;
	public int ViewWidth;

	public override void _Ready()
	{
		ViewWidth = (int)GetViewportRect().Size.X;
		ViewHeight = (int)GetViewportRect().Size.Y;
		CellSize = ViewWidth / CellNumber;
	}

	public override void _Draw()
	{
		DrawRect(new Rect2(0, 0, ViewWidth, CellSize), Colors.Black); // Top border
		DrawRect(new Rect2(0, ViewHeight - CellSize, ViewWidth, CellSize), Colors.Black); // Bottom border
		DrawRect(new Rect2(0, 0, CellSize, ViewHeight), Colors.Black); // Left border
		DrawRect(new Rect2(ViewWidth - CellSize, 0, CellSize, ViewHeight), Colors.Black); // Right border

	}
}
