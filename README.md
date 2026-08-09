# Snake

Grid-based Snake built with **Godot 4 (.NET)** and **C#**.

Eat food to grow; hitting a wall or your own body ends the run.

## Controls

| Key | Action |
|-----|--------|
| Arrow keys | Turn |
| `Esc` | Pause |
| Arrow keys / `Enter` | Navigate and activate menu buttons |

## How it's put together

- **`game/SnakeGame.cs`** — the rules (board, body, food, one tick). Deliberately
  free of `Node` and scene-tree types, and takes its `Random` as a constructor
  argument, so a round can be played out in a test without starting the engine.
- **`scenes/main/Main.cs`** — the coordinator: a `MainMenu → Running → GameOver`
  state machine with an explicit legal-transition table, the tick timers, and
  rendering.
- **`ui/Menu.cs`** — main, pause and game-over panels in one scene, switched
  internally. Signals up (`StartGame`, `ExitGame`, `ResumeGame`, `MainMenu`) so
  it never needs to know about `GameState`.

Board size and tick speed live in `Settings.cs`.

## Running it

Open the project in the Godot **.NET** build (C# is not in the standard
download) with the .NET 8 SDK installed, then press play.
