# Chowka Bara Board Game 🎲

Chowka Bara is a traditional Indian board game, recreated here in Unity.  
This project brings cultural heritage into a digital format with dice rolling, player pieces, and simple UI.

## Features
- 🎲 Dice rolling with animation and sound
- 🟢🔴🟡🔵 Player pieces for four players
- 🏠 Home squares for each color
- 🎮 Game Manager to control flow and rules
- 🎵 Integrated audio feedback
- 🖥️ Simple UI for interaction

## Project Structure
Key scripts inside `Assets/Script/`:
- **Dice.cs** – Handles dice logic
- **RollingDice.cs** – Manages dice animations
- **GameManager.cs** – Controls game flow
- **UIManager.cs** – Manages user interface
- **Sound.cs** – Plays audio effects
- **PathPoint.cs / PathObjectParent.cs** – Defines movement paths
- **Home.cs & Color-specific Home scripts** – Manage player home squares
- **PlayerPice scripts** – Control individual player tokens

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/Rithushreehm/Chowka-Bara-Board-Game.git

## How to Play
- Select the number of players (2, 3, or 4).
- Each player rolls the dice and moves tokens according to Chowka Bara rules.
- The goal is to move all tokens around the board and reach the home square.
- First player to bring all tokens home wins.

## Acknowledgements
- Inspired by the traditional Indian board game *Chowka Bara*.
- Built using Unity Engine.

## License

This project is licensed under a **Non‑Commercial Use License**.  
You may use, copy, and modify the code for personal, educational, or research purposes only.  
Commercial use is strictly prohibited.  

See the [LICENSE](LICENSE) file for full details.