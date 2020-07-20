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
            CompatibilityPatcher.Patch();
            ResetHediffGivers();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            CompatibilityPatcher.Patch();
            ResetHediffGivers();
        }

        private void ResetHediffGivers()
        {
            foreach (HediffGiverSetDef def in DefDatabase<HediffGiverSetDef>.AllDefs)
            {
                foreach (HediffGiver_Ferocity giver in def.hediffGivers.OfType<HediffGiver_Ferocity>())
                {
                    giver.Clear();
                }
            };
        }
    }
}
