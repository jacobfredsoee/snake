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
	private Button _exitButton;
	private Button _resumeButton;
	private Button _mainMenuButton;

	private Control _mainPanel;
	private Control _pausePanel;

	public override void _Ready()
	{
		_mainPanel = GetNode<Control>("MainMenu");
		_pausePanel = GetNode<Control>("PauseMenu");
		ShowMainMenu();
	}

	public void ShowMainMenu()
	{
		_mainPanel.Visible = true;
		_pausePanel.Visible = false;
	}

	public void ShowPauseMenu()
	{
		_mainPanel.Visible = false;
		_pausePanel.Visible = true;
	}

	public void HideMenu()
	{
		_mainPanel.Visible = false;
		_pausePanel.Visible = false;
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
