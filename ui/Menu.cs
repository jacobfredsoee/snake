using Godot;
using System;

public partial class Menu : Control
{
	[Signal]
	public delegate void StartGameEventHandler();
	[Signal]
	public delegate void ExitGameEventHandler();
	[Signal]
	public delegate void ResumeGameEventHandler();
	[Signal]
	public delegate void MainMenuEventHandler();

	private Button _startButton;
	private Button _resumeButton;
	private Button _restartGameButton;

	private Control _mainPanel;
	private Control _pausePanel;
	private Control _gameOverPanel;

	public override void _Ready()
	{
		_startButton = GetNode<Button>("%StartButton");
		_resumeButton = GetNode<Button>("%ResumeButton");
		_restartGameButton = GetNode<Button>("%RestartButton");

		_mainPanel = GetNode<Control>("MainMenu");
		_pausePanel = GetNode<Control>("PauseMenu");
		_gameOverPanel = GetNode<Control>("GameOverMenu");
		ShowMainMenu();
	}

	public void ShowMainMenu()
	{
		_mainPanel.Visible = true;
		_pausePanel.Visible = false;
		_gameOverPanel.Visible = false;
		_startButton.GrabFocus();
	}

	public void ShowPauseMenu()
	{
		_mainPanel.Visible = false;
		_pausePanel.Visible = true;
		_gameOverPanel.Visible = false;
		_resumeButton.GrabFocus();
	}

	public void ShowGameOverMenu()
	{
		_mainPanel.Visible = false;
		_pausePanel.Visible = false;
		_gameOverPanel.Visible = true;
		_restartGameButton.GrabFocus();
	}

	public void HideMenu()
	{
		_mainPanel.Visible = false;
		_pausePanel.Visible = false;
		_gameOverPanel.Visible = false;
	}

	public void OnStartButtonPressed()
	{
		EmitSignal(SignalName.StartGame);
	}
	public void OnExitButtonPressed()
	{
		EmitSignal(SignalName.ExitGame);
	}
	public void OnResumeButtonPressed()
	{
		EmitSignal(SignalName.ResumeGame);
	}
	public void OnMainMenuButtonPressed()
	{
		EmitSignal(SignalName.MainMenu);
	}
}
