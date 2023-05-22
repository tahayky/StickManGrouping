using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Centurion.CGSHC01.Game
{
    public class GameManager : StaticInstance<GameManager>
    {
        public Action<GameState> on_state_changed;
        public GameState game_state { get; private set; }

        void Start() => ChangeState(GameState.Starting);

        public void ChangeState(GameState new_state)
        {
            //if (new_state == game_state) return;
            game_state = new_state;
            switch (new_state)
            {
                case GameState.Starting:
                    HandleStarting();
                    break;
                case GameState.Menu:
                    HandleMenu();
                    break;
                case GameState.Level:
                    HandleLevel();
                    break;
                case GameState.Win:
                    HandleWin();
                    break;
                case GameState.Lose:
                    HandleLose();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(new_state), new_state, null);
            }
                on_state_changed?.Invoke(new_state);
        }
        void HandleStarting()
        {
            
        }
        void HandleMenu()
        {

        }
        void HandleLevel()
        {

        }
        void HandleWin()
        {

        }
        void HandleLose()
        {

        }

    }
}

