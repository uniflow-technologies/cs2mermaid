using FluentAssertions;
using Xunit;

namespace Cs2Mermaid.Core.Tests;

public class MermaidEmitterTests
{
    [Fact]
    public async Task EmitAsync_SimpleClass_GeneratesCorrectMermaid()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:MyNamespace.MyClass", "MyClass", "MyNamespace", TypeKind.Class, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            File.Exists(outputPath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("classDiagram");
            content.Should().Contain("direction LR");
            content.Should().Contain("namespace MyNamespace");
            content.Should().Contain("class MyNamespace::MyClass");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_AbstractClass_GeneratesAbstractStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.AbstractClass", "AbstractClass", "NS", TypeKind.Class, "public", true, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "TB", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<Abstract>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_Interface_GeneratesInterfaceStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.IMyInterface", "IMyInterface", "NS", TypeKind.Interface, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<Interface>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_StaticClass_GeneratesStaticStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.StaticClass", "StaticClass", "NS", TypeKind.Class, "public", false, false, true)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<static>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_SealedClass_GeneratesSealedStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.SealedClass", "SealedClass", "NS", TypeKind.Class, "public", false, true, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<sealed>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_Struct_GeneratesStructStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.MyStruct", "MyStruct", "NS", TypeKind.Struct, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<struct>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_Enum_GeneratesEnumStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.MyEnum", "MyEnum", "NS", TypeKind.Enum, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<Enumeration>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_RecordClass_GeneratesRecordStereotype()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.MyRecord", "MyRecord", "NS", TypeKind.RecordClass, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<<record>>");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_Inheritance_GeneratesCorrectRelation()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.BaseClass", "BaseClass", "NS", TypeKind.Class, "public", false, false, false),
            new TypeIR("T:NS.DerivedClass", "DerivedClass", "NS", TypeKind.Class, "public", false, false, false)
        };
        var relations = new List<RelationIR>
        {
            new RelationIR("T:NS.DerivedClass", "T:NS.BaseClass", RelationKind.Inheritance)
        };
        var projectIR = new ProjectIR("TestProject", types, relations);
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("<|--");
            content.Should().Contain("DerivedClass");
            content.Should().Contain("BaseClass");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_Realization_GeneratesCorrectRelation()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.IMyInterface", "IMyInterface", "NS", TypeKind.Interface, "public", false, false, false),
            new TypeIR("T:NS.MyClass", "MyClass", "NS", TypeKind.Class, "public", false, false, false)
        };
        var relations = new List<RelationIR>
        {
            new RelationIR("T:NS.MyClass", "T:NS.IMyInterface", RelationKind.Realization)
        };
        var projectIR = new ProjectIR("TestProject", types, relations);
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("..|>");
            content.Should().Contain("MyClass");
            content.Should().Contain("IMyInterface");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_MultipleNamespaces_GroupsCorrectly()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS1.Class1", "Class1", "NS1", TypeKind.Class, "public", false, false, false),
            new TypeIR("T:NS2.Class2", "Class2", "NS2", TypeKind.Class, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("namespace NS1");
            content.Should().Contain("namespace NS2");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_WriteIfChanged_DoesNotOverwriteIdenticalFile()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.MyClass", "MyClass", "NS", TypeKind.Class, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act - First write
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);
            var firstWriteTime = File.GetLastWriteTimeUtc(outputPath);
            
            // Small delay to ensure time difference if file is rewritten
            await Task.Delay(100);
            
            // Act - Second write (should not change file)
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);
            var secondWriteTime = File.GetLastWriteTimeUtc(outputPath);

            // Assert
            secondWriteTime.Should().Be(firstWriteTime, "file should not be rewritten if content is identical");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task EmitAsync_EmptyNamespace_UsesGlobal()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:MyClass", "MyClass", "", TypeKind.Class, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, "LR", outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain("namespace Global");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Theory]
    [InlineData("TB")]
    [InlineData("BT")]
    [InlineData("LR")]
    [InlineData("RL")]
    public async Task EmitAsync_DifferentDirections_GeneratesCorrectDirective(string direction)
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.MyClass", "MyClass", "NS", TypeKind.Class, "public", false, false, false)
        };
        var projectIR = new ProjectIR("TestProject", types, new List<RelationIR>());
        var solutionIR = new SolutionIR(new List<ProjectIR> { projectIR });
        var outputPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.mmd");

        try
        {
            // Act
            await MermaidEmitter.EmitAsync(solutionIR, direction, outputPath, CancellationToken.None);

            // Assert
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().Contain($"direction {direction}");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }
}
