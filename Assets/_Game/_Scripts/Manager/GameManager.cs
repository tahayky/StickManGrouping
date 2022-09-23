using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Centurion.CGSHC01.Game
{
    public class GameManager : StaticInstance<GameManager>
    {
        public delegate void OnStateChanged(GameState current_state);
        public event OnStateChanged on_state_changed;
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
            if (on_state_changed != null)
                on_state_changed(new_state);
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

