using FluentAssertions;
using Xunit;

namespace Cs2Mermaid.Core.Tests;

public class ExtractorTests
{
    private static async Task<string> CreateTestProjectAsync(string code, string projectName = "TestProject")
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"cs2mermaid_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>";

        var projectPath = Path.Combine(tempDir, $"{projectName}.csproj");
        await File.WriteAllTextAsync(projectPath, csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "TestClass.cs"), code);

        return projectPath;
    }

    [Fact]
    public async Task LoadAndExtractAsync_SimpleClass_ExtractsCorrectly()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public class SimpleClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Projects.Should().HaveCount(1);
            var project = result.Projects[0];
            project.Types.Should().Contain(t => t.Name == "SimpleClass" && t.Namespace == "TestNamespace");
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_Interface_ExtractsAsInterface()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public interface IMyInterface
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "IMyInterface" && t.Kind == TypeKind.Interface);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_AbstractClass_ExtractsAsAbstract()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public abstract class AbstractClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "AbstractClass" && t.IsAbstract);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_SealedClass_ExtractsAsSealed()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public sealed class SealedClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "SealedClass" && t.IsSealed);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_StaticClass_ExtractsAsStatic()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public static class StaticClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "StaticClass" && t.IsStatic);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_Inheritance_ExtractsRelation()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public class BaseClass
{
}

public class DerivedClass : BaseClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var relations = result.Projects[0].Relations;
            relations.Should().Contain(r => 
                r.Kind == RelationKind.Inheritance && 
                r.FromDocId.Contains("DerivedClass") && 
                r.ToDocId.Contains("BaseClass"));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_InterfaceImplementation_ExtractsRealization()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public interface IMyInterface
{
}

public class MyClass : IMyInterface
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var relations = result.Projects[0].Relations;
            relations.Should().Contain(r => 
                r.Kind == RelationKind.Realization && 
                r.FromDocId.Contains("MyClass") && 
                r.ToDocId.Contains("IMyInterface"));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_MinAccessPublic_FiltersInternalTypes()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public class PublicClass
{
}

internal class InternalClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "PublicClass");
            types.Should().NotContain(t => t.Name == "InternalClass");
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_MinAccessInternal_IncludesInternalTypes()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public class PublicClass
{
}

internal class InternalClass
{
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "internal", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "PublicClass");
            types.Should().Contain(t => t.Name == "InternalClass");
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_InvalidPath_ThrowsArgumentException()
    {
        // Arrange
        var invalidPath = "/invalid/path/test.txt";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            Extractor.LoadAndExtractAsync(invalidPath, "public", CancellationToken.None));
    }

    [Fact]
    public async Task LoadAndExtractAsync_Enum_ExtractsAsEnum()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public enum MyEnum
{
    Value1,
    Value2
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "MyEnum" && t.Kind == TypeKind.Enum);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_RecordClass_ExtractsAsRecord()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public record MyRecord(string Name, int Age);
";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "MyRecord" && t.Kind == TypeKind.RecordClass);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }

    [Fact]
    public async Task LoadAndExtractAsync_Struct_ExtractsAsStruct()
    {
        // Arrange
        var code = @"
namespace TestNamespace;

public struct MyStruct
{
    public int Value;
}";
        var projectPath = await CreateTestProjectAsync(code);

        try
        {
            // Act
            var result = await Extractor.LoadAndExtractAsync(projectPath, "public", CancellationToken.None);

            // Assert
            var types = result.Projects[0].Types;
            types.Should().Contain(t => t.Name == "MyStruct" && t.Kind == TypeKind.Struct);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(projectPath)!, true);
        }
    }
}
