# Snake

Grid-based Snake built with **Godot 4 (.NET)** and **C#**.

Eat food to grow; hitting a wall or your own body ends the run.

<img width="340" height="330" alt="image" src="https://github.com/user-attachments/assets/21fd3213-9e05-4a2e-a22c-c6ea21591660" />

## Controls

| Key                  | Action                             |
| -------------------- | ---------------------------------- |
| Arrow keys           | Turn                               |
| `Esc`                | Pause                              |
| Arrow keys / `Enter` | Navigate and activate menu buttons |

## Game flow

`Main` runs a small finite state machine. It's a **Moore machine**: every action
hangs off a _state_, never off a transition. Entry actions establish what must be
true on arrival regardless of where you came from, and exit actions tear down what
the state owned regardless of where you're going.

```mermaid
stateDiagram-v2
    direction LR

    [*] --> MainMenu : _Ready

    MainMenu --> Running  : Start
    Running  --> GameOver : snake dies
    Running  --> MainMenu : Main menu, from pause
    GameOver --> Running  : Try again
    GameOver --> MainMenu : Main menu

    note right of Running
        Pause is not a state.
        The scene tree's paused flag is
        toggled inside Running, and
        leaving Running always clears it.
    end note
```
