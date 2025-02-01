using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBAN_Studio.Common.Audio
{
    public interface IAudioDevice : IDisposable
    {

    }
    public abstract class AudioDevice : IAudioDevice
    {
        public abstract void Dispose();

        public abstract string GetConfigCommand();
    }
}
