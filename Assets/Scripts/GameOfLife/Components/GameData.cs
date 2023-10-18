using System;
using Arch.Core;
using Arch.Core.Extensions;
using UnityEngine;

namespace GameOfLife.Components
{
    public class GameData
    {
        public readonly GameConfig Config;

        public GameData(GameConfig config)
        {
            Config = config;
        }
    }
}