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
        public readonly List<IAudioInput> AudioInputs = new List<IAudioInput>();
        public readonly List<IAudioOutput> AudioOutputs = new List<IAudioOutput>();   
        private bool isRoutingActive;
        public void Map(Type inputType, List<string> inputParams, Type outputType, List<string> outputParams)
        {
            if (inputType == null || outputType == null)
                throw new ArgumentNullException("Input or Output type cannot be null.");

            // Create instances of input and output with proper type conversion
            IAudioInput inputInstance = (IAudioInput)GetInputInstanceWithParams(inputType, inputParams);
            IAudioOutput outputInstance = (IAudioOutput)GetOutputInstanceWithParams(outputType, outputParams);

            if (inputInstance is not IAudioInput input)
                throw new InvalidOperationException("The provided input type does not implement IIAudioInput.");

            if (outputInstance is not IAudioOutput output)
                throw new InvalidOperationException("The provided output type does not implement IIAudioOutput.");

            if (AudioStreams.Any(x => x.Input == inputInstance && x.Output == outputInstance))
            {

                Console.WriteLine("Map already exists");
                return;
            }
            var stream = new AudioStream(inputInstance, outputInstance);


            AudioStreams.Add(stream); // Assuming _streams is a list of active AudioStreams
            if (!AudioInputs.Contains(inputInstance))
            AudioInputs.Add(inputInstance);
            if (!AudioOutputs.Contains(outputInstance))
            AudioOutputs.Add(outputInstance);
            Stop();
            Start();
            Console.WriteLine($"Mapped {input.Name} -> {output.Name}");

        }

        private IAudioOutput GetOutputInstanceWithParams(Type outputType, List<string> outputParams)
        {
            var instancesOfType = AudioOutputs.Where(t => t.GetType() == outputType).ToList();
            var newInstance = (IAudioOutput)CreateInstanceWithParams(outputType, outputParams);
            var instanceWithName = instancesOfType.FirstOrDefault(x => x.Id == newInstance.Id || x.Name == newInstance.Name);
            if (instanceWithName == null)
                return newInstance;

            newInstance.Dispose();

            return instanceWithName;
        }

        private IAudioInput GetInputInstanceWithParams(Type inputType, List<string> inputParams)
        {
            var instancesOfType = AudioInputs.Where(t => t.GetType() == inputType).ToList();
            var newInstance = (IAudioInput)CreateInstanceWithParams(inputType, inputParams);
            var instanceWithName = instancesOfType.FirstOrDefault(x => x.Id == newInstance.Id || x.Name == newInstance.Name);
            if (instanceWithName == null)
                return newInstance;

            newInstance.Dispose();

            return instanceWithName;
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
          AudioInputs.Add(input);
          AudioOutputs.Add(output);

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
            Console.WriteLine($"Unmapped {audioStream.Input.Name} -> {audioStream.Output.Name}");
            audioStream.Dispose();
            AudioStreams.RemoveAt(streamIndex);

            if (AudioStreams.Count(x => x.Input == audioStream.Input) < 2 && !audioStream.Input.GetType().IsAssignableTo(typeof(AudioBus)))
                audioStream.Input.Dispose();
            if (AudioStreams.Count(x => x.Output == audioStream.Output) < 2 && !audioStream.Input.GetType().IsAssignableTo(typeof(AudioBus)))
                audioStream.Output.Dispose();


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
