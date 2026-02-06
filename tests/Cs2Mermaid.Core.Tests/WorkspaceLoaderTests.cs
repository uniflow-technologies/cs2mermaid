using FluentAssertions;
using Xunit;

namespace Cs2Mermaid.Core.Tests;

public class WorkspaceLoaderTests
{
    [Fact]
    public async Task CreateWorkspaceAsync_ShouldReturnWorkspace()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Act
        var workspace = await WorkspaceLoader.CreateWorkspaceAsync(cts.Token);

        // Assert
        workspace.Should().NotBeNull();
        workspace.Dispose();
    }

    [Theory]
    [InlineData("test.sln", true)]
    [InlineData("TEST.SLN", true)]
    [InlineData("test.csproj", false)]
    [InlineData("test.txt", false)]
    public void IsSolution_ShouldDetectSolutionFiles(string path, bool expected)
    {
        // Act
        var result = WorkspaceLoader.IsSolution(path);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("test.csproj", true)]
    [InlineData("TEST.CSPROJ", true)]
    [InlineData("test.sln", false)]
    [InlineData("test.txt", false)]
    public void IsProject_ShouldDetectProjectFiles(string path, bool expected)
    {
        // Act
        var result = WorkspaceLoader.IsProject(path);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void EnsureFullPath_ShouldReturnFullPath()
    {
        // Arrange
        var relativePath = "test.csproj";

        // Act
        var fullPath = WorkspaceLoader.EnsureFullPath(relativePath);

        // Assert
        fullPath.Should().NotBeNullOrEmpty();
        Path.IsPathRooted(fullPath).Should().BeTrue();
    }

    [Fact]
    public void EnsureFullPath_WithFullPath_ShouldReturnSamePath()
    {
        // Arrange
        var fullPath = Path.GetFullPath("test.csproj");

        // Act
        var result = WorkspaceLoader.EnsureFullPath(fullPath);

        // Assert
        result.Should().Be(fullPath);
    }
}
