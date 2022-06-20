using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class HostileResponseOption
    {
        public HostilityResponseType type;
        public string label;
        public string description;
        public string iconPath;
        public string disableMessage;

        private Texture2D iconTexture;
        public Texture2D Texture
        {
            get
            {
                if(iconTexture == null && !iconPath.NullOrEmpty())
                {
                    iconTexture = ContentFinder<Texture2D>.Get(iconPath);
                }
                return iconTexture;
            }
        }
    }
}
