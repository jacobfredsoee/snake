public enum GameState
{
	/// <summary>Before the first transition. Keeps the enum's default value from
	/// silently counting as a real state — see the transition table in Main.</summary>
	None,
	MainMenu,
	Running,
	GameOver,
}
