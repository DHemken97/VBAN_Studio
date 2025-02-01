using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBAN_Studio.Common.AudioModifiers
{
    public class VolumeModifier : AudioModifier
    {
        public static float MasterVolume = 1;
        public float Volume = 1;
        public override byte[] Apply(byte[] data)
        {
            return AdjustVolume(data, MasterVolume * Volume);
        }
        public static byte[] AdjustVolume(byte[] audioData, float volumeFactor)
        {
            if (audioData.Length % 2 != 0)
                throw new ArgumentException("Byte array length must be even.");

            int sampleCount = audioData.Length / 2;
            short[] samples = new short[sampleCount];
            byte[] adjustedAudioData = new byte[audioData.Length];

            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = BitConverter.ToInt16(audioData, i * 2);
            }

            for (int i = 0; i < sampleCount; i++)
            {
                float adjustedSample = samples[i] * volumeFactor;
                adjustedSample = Math.Max(Math.Min(adjustedSample, short.MaxValue), short.MinValue);
                byte[] adjustedBytes = BitConverter.GetBytes((short)adjustedSample);
                adjustedAudioData[i * 2] = adjustedBytes[0];
                adjustedAudioData[i * 2 + 1] = adjustedBytes[1];
            }

            return adjustedAudioData;
        }
    }



}
