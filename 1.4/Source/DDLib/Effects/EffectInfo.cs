using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD
{
    public struct EffectInfo
    {
        private Effect effect;

        private EffectDef def;
        private float size;

        public EffectInfo Invalid => new EffectInfo()
        {
            effect = null,
            def = null,
            size = -1
        };

        public EffectInfo(Effect effect) : this()
        {
            this.effect = effect;
        }

        public EffectInfo(EffectDef def, float size) : this()
        {
            this.def = def;
            this.size = size;
        }

        public bool HasEffect => effect != null;
        public Effect Effect => effect;

        public EffectDef Def => HasEffect ? Effect.EffectDef : def;
        public float Size => HasEffect ? Effect.Size : size;
    }
}
