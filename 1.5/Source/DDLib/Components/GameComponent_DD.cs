using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class GameComponent_DD : GameComponent
    {
        private Game game;

        public GameComponent_DD(Game game)
        {
            this.game = game;
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            DraconicOverseer.Settings.Apply();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            DraconicOverseer.Settings.Apply();
        }
    }
}
