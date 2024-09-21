using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace SEM.Prototype.Services.OnlineIDE
{
    public class CodeExecutionService
    {
        private readonly DockerClient _dockerClient;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30); // Adjust this value as needed



        public CodeExecutionService()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        public async Task<string> ExecuteCodeAsync(string code, string language)
        {
            if (language.ToLower() != "python")
            {
                return "Only Python is supported at the moment.";
            }

            try
            {
                using var cts = new CancellationTokenSource(_timeout);

                var imageName = "python:3.9-slim";
                await PullImageAsync(imageName, cts.Token);

                var tempDir = Path.GetTempPath();
                var codePath = Path.Combine(tempDir, "code.py");
                File.WriteAllText(codePath, code);

                var containerId = await CreateAndStartContainerAsync(imageName, tempDir, cts.Token);

                var output = await GetContainerLogsAsync(containerId, cts.Token);

                await CleanupContainerAsync(containerId, cts.Token);

                File.Delete(codePath);

                return output;
            }
            catch (OperationCanceledException)
            {
                return "The operation timed out. Please try again or simplify your code.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        private async Task PullImageAsync(string imageName, CancellationToken cancellationToken)
        {
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = imageName, Tag = "latest" },
                null,
                new Progress<JSONMessage>(),
                cancellationToken
            );
        }

        private async Task<string> CreateAndStartContainerAsync(string imageName, string tempDir, CancellationToken cancellationToken)
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters
                {
                    Image = imageName,
                    Cmd = new[] { "python", "/code/code.py" },
                    HostConfig = new HostConfig
                    {
                        Binds = new[] { $"{tempDir}:/code" },
                        Memory = 104857600, // 100MB
                        MemorySwap = 104857600, // 100MB
                        NanoCPUs = 1000000000 // 1 CPU
                    }
                },
                cancellationToken
            );

            await _dockerClient.Containers.StartContainerAsync(
                response.ID,
                new ContainerStartParameters(),
                cancellationToken
            );

            return response.ID;
        }

        private async Task<string> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken)
        {
            var logStream = await _dockerClient.Containers.GetContainerLogsAsync(
                containerId,
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Follow = true },
                cancellationToken
            );

            using var reader = new StreamReader(logStream);
            return await reader.ReadToEndAsync();
        }

        private async Task CleanupContainerAsync(string containerId, CancellationToken cancellationToken)
        {
            await _dockerClient.Containers.StopContainerAsync(
                containerId,
                new ContainerStopParameters(),
                cancellationToken
            );

            await _dockerClient.Containers.RemoveContainerAsync(
                containerId,
                new ContainerRemoveParameters(),
                cancellationToken
            );
        }
    }
}