using Buildron.ClassicMods.SoundMod.Controllers;
using Buildron.Domain.Mods;

namespace Buildron.ClassicMods.SoundMod
{
    public class Mod : IMod
    {
        public static IModContext Context { get; private set; }

        public void Initialize(IModContext context)
        {
            Context = context;
            context.GameObjects.Create<BuildSoundEffectController>();
        }
    }
}
