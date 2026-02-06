using FluentAssertions;
using Xunit;
using System.Diagnostics;

namespace Cs2Mermaid.Cli.Tests;

public class CliIntegrationTests
{
    private static string GetCliPath()
    {
        // Find the CLI executable - detect configuration from test assembly location
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        
        // Determine build configuration (Debug or Release) from the test binary path
        var configuration = basePath.Contains("Release") ? "Release" : "Debug";
        
        var cliPath = Path.Combine(basePath, "..", "..", "..", "..", "..", "src", "Cs2Mermaid.Cli", "bin", configuration, "net8.0", "Cs2Mermaid.Cli.dll");
        return Path.GetFullPath(cliPath);
    }

    [Fact]
    public async Task Cli_Help_ReturnsZeroExitCode()
    {
        // Arrange
        var cliPath = GetCliPath();
        if (!File.Exists(cliPath))
        {
            // Build the CLI first using the detected configuration
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var configuration = basePath.Contains("Release") ? "Release" : "Debug";
            var buildResult = await RunProcessAsync("dotnet", $"build {Path.Combine(Path.GetDirectoryName(cliPath)!, "..", "..", "..", "..", "..", "Cs2Mermaid.sln")} -c {configuration}");
            buildResult.ExitCode.Should().Be(0, "CLI should build successfully");
        }

        // Act
        var result = await RunProcessAsync("dotnet", $"\"{cliPath}\" --help");

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("cs2mermaid");
    }

    [Fact]
    public async Task Cli_EmitCommand_WithValidProject_GeneratesMermaidFile()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"cs2mermaid_cli_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>";

        var csCode = @"
namespace TestNamespace;

public class TestClass
{
}";

        var projectPath = Path.Combine(tempDir, "TestProject.csproj");
        await File.WriteAllTextAsync(projectPath, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "TestClass.cs"), csCode);

        var outputPath = Path.Combine(tempDir, "output.mmd");
        var cliPath = GetCliPath();

        try
        {
            // Act
            var result = await RunProcessAsync("dotnet", $"\"{cliPath}\" emit \"{projectPath}\" --out \"{outputPath}\"");

            // Assert
            result.ExitCode.Should().Be(0);
            File.Exists(outputPath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("classDiagram");
            content.Should().Contain("TestClass");
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task Cli_EmitCommand_WithInvalidPath_ReturnsNonZeroExitCode()
    {
        // Arrange
        var cliPath = GetCliPath();
        var invalidPath = "/invalid/path/test.csproj";

        // Act
        var result = await RunProcessAsync("dotnet", $"\"{cliPath}\" emit \"{invalidPath}\"");

        // Assert
        result.ExitCode.Should().NotBe(0);
    }

    [Fact]
    public async Task Cli_EmitCommand_WithDirection_GeneratesCorrectDirection()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"cs2mermaid_cli_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>";

        var csCode = "namespace Test; public class C { }";
        var projectPath = Path.Combine(tempDir, "Test.csproj");
        await File.WriteAllTextAsync(projectPath, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "C.cs"), csCode);

        var outputPath = Path.Combine(tempDir, "output.mmd");
        var cliPath = GetCliPath();

        try
        {
            // Act
            var result = await RunProcessAsync("dotnet", $"\"{cliPath}\" emit \"{projectPath}\" --out \"{outputPath}\" --direction TB");

            // Assert
            result.ExitCode.Should().Be(0);
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("direction TB");
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private static async Task<ProcessResult> RunProcessAsync(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        var outputBuilder = new System.Text.StringBuilder();
        var errorBuilder = new System.Text.StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                outputBuilder.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                errorBuilder.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            Output = outputBuilder.ToString(),
            Error = errorBuilder.ToString()
        };
    }

    private class ProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
