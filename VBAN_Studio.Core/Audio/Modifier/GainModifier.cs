using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.Audio.Modifier
{
    public class GainModifier : AudioModifier
    {
        public static float MasterGain = 1;
        public float Gain = 1;
        public override byte[] Apply(byte[] data)
        {
            return AdjustGain(data, MasterGain * Gain);
        }
        public static byte[] AdjustGain(byte[] audioData, float volumeFactor)
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
