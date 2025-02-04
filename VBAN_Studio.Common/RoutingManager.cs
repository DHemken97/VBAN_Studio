using System.IO;

namespace VBAN_Studio.Common.Audio
{
    public interface IRoutingManager : IDisposable
    {
        void Map(IAudioInput input, IAudioOutput output);
        void UnMap(IAudioInput input, IAudioOutput output);
        void UnMap(int streamIndex);
        void Start();
        void Stop();
    }
    public class RoutingManager : IRoutingManager
    {
        public readonly List<AudioStream> AudioStreams = new List<AudioStream>();
        private bool isRoutingActive;
        public void Map(Type inputType, List<string> inputParams, Type outputType, List<string> outputParams)
        {
            if (inputType == null || outputType == null)
                throw new ArgumentNullException("Input or Output type cannot be null.");

            // Create instances of input and output with proper type conversion
            IAudioInput inputInstance = (IAudioInput)CreateInstanceWithParams(inputType, inputParams);
            IAudioOutput outputInstance = (IAudioOutput)CreateInstanceWithParams(outputType, outputParams);

            if (inputInstance is not IAudioInput input)
                throw new InvalidOperationException("The provided input type does not implement IAudioInput.");

            if (outputInstance is not IAudioOutput output)
                throw new InvalidOperationException("The provided output type does not implement IAudioOutput.");

            // Perform the actual mapping logic
            var stream = new AudioStream(input, output);
            AudioStreams.Add(stream); // Assuming _streams is a list of active AudioStreams

            Stop();
            Start();
        }

        /// <summary>
        /// Creates an instance of the specified type by converting the input parameters to match the constructor's expected types.
        /// </summary>
        private object CreateInstanceWithParams(Type type, List<string> parameters)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var paramInfos = constructor.GetParameters();
                if (paramInfos.Length != parameters.Count)
                    continue; // Skip constructors that don't match the parameter count

                object[] convertedParams = new object[parameters.Count];

                for (int i = 0; i < parameters.Count; i++)
                {
                    var expectedType = paramInfos[i].ParameterType;
                    convertedParams[i] = Convert.ChangeType(parameters[i], expectedType);
                }

                return Activator.CreateInstance(type, convertedParams);
            }

            throw new InvalidOperationException($"No matching constructor found for {type.Name} with {parameters.Count} parameters.");
        }

        public void Map(IAudioInput input, IAudioOutput output)
        {
            if (input == null || output == null)
                throw new ArgumentNullException("Input and Output cannot be null");

            var audioStream = new AudioStream(input, output);
            AudioStreams.Add(audioStream);
        }

        public void UnMap(IAudioInput input, IAudioOutput output)
        {
            var audioStream = AudioStreams.FirstOrDefault(s => s.Input == input && s.Output == output);
            if (audioStream != null)
            {
                audioStream.Dispose();
                AudioStreams.Remove(audioStream);
            }
        }

        public void UnMap(int streamIndex)
        {
            if (streamIndex < 0 || streamIndex >= AudioStreams.Count)
                throw new ArgumentOutOfRangeException(nameof(streamIndex), "Invalid stream index.");

            var audioStream = AudioStreams[streamIndex];
            audioStream.Dispose();
            AudioStreams.RemoveAt(streamIndex);
        }

        public void Start()
        {
            if (isRoutingActive) return;

            foreach (var stream in AudioStreams)
            {
                stream.Start();
            }
            isRoutingActive = true;
        }

        public void Stop()
        {
            if (!isRoutingActive) return;

            foreach (var stream in AudioStreams)
            {
                stream.Stop();
            }
            isRoutingActive = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
