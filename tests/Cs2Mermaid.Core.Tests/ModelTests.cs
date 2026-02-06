using FluentAssertions;
using Xunit;

namespace Cs2Mermaid.Core.Tests;

public class ModelTests
{
    [Fact]
    public void TypeIR_ShouldCreateCorrectly()
    {
        // Arrange & Act
        var typeIR = new TypeIR(
            "T:MyNamespace.MyClass",
            "MyClass",
            "MyNamespace",
            TypeKind.Class,
            "public",
            false,
            false,
            false
        );

        // Assert
        typeIR.DocId.Should().Be("T:MyNamespace.MyClass");
        typeIR.Name.Should().Be("MyClass");
        typeIR.Namespace.Should().Be("MyNamespace");
        typeIR.Kind.Should().Be(TypeKind.Class);
        typeIR.Accessibility.Should().Be("public");
        typeIR.IsAbstract.Should().BeFalse();
        typeIR.IsSealed.Should().BeFalse();
        typeIR.IsStatic.Should().BeFalse();
    }

    [Fact]
    public void TypeIR_AbstractClass_ShouldHaveAbstractFlag()
    {
        // Arrange & Act
        var typeIR = new TypeIR(
            "T:MyNamespace.AbstractClass",
            "AbstractClass",
            "MyNamespace",
            TypeKind.Class,
            "public",
            true,
            false,
            false
        );

        // Assert
        typeIR.IsAbstract.Should().BeTrue();
        typeIR.IsSealed.Should().BeFalse();
        typeIR.IsStatic.Should().BeFalse();
    }

    [Fact]
    public void TypeIR_SealedClass_ShouldHaveSealedFlag()
    {
        // Arrange & Act
        var typeIR = new TypeIR(
            "T:MyNamespace.SealedClass",
            "SealedClass",
            "MyNamespace",
            TypeKind.Class,
            "public",
            false,
            true,
            false
        );

        // Assert
        typeIR.IsSealed.Should().BeTrue();
        typeIR.IsAbstract.Should().BeFalse();
    }

    [Fact]
    public void TypeIR_StaticClass_ShouldHaveStaticFlag()
    {
        // Arrange & Act
        var typeIR = new TypeIR(
            "T:MyNamespace.StaticClass",
            "StaticClass",
            "MyNamespace",
            TypeKind.Class,
            "public",
            false,
            false,
            true
        );

        // Assert
        typeIR.IsStatic.Should().BeTrue();
    }

    [Theory]
    [InlineData(TypeKind.Class)]
    [InlineData(TypeKind.Interface)]
    [InlineData(TypeKind.Struct)]
    [InlineData(TypeKind.Enum)]
    [InlineData(TypeKind.Delegate)]
    [InlineData(TypeKind.RecordClass)]
    [InlineData(TypeKind.RecordStruct)]
    public void TypeIR_ShouldSupportAllTypeKinds(TypeKind kind)
    {
        // Arrange & Act
        var typeIR = new TypeIR(
            "T:MyNamespace.MyType",
            "MyType",
            "MyNamespace",
            kind,
            "public",
            false,
            false,
            false
        );

        // Assert
        typeIR.Kind.Should().Be(kind);
    }

    [Fact]
    public void RelationIR_Inheritance_ShouldCreateCorrectly()
    {
        // Arrange & Act
        var relation = new RelationIR(
            "T:MyNamespace.DerivedClass",
            "T:MyNamespace.BaseClass",
            RelationKind.Inheritance
        );

        // Assert
        relation.FromDocId.Should().Be("T:MyNamespace.DerivedClass");
        relation.ToDocId.Should().Be("T:MyNamespace.BaseClass");
        relation.Kind.Should().Be(RelationKind.Inheritance);
    }

    [Fact]
    public void RelationIR_Realization_ShouldCreateCorrectly()
    {
        // Arrange & Act
        var relation = new RelationIR(
            "T:MyNamespace.MyClass",
            "T:MyNamespace.IMyInterface",
            RelationKind.Realization
        );

        // Assert
        relation.FromDocId.Should().Be("T:MyNamespace.MyClass");
        relation.ToDocId.Should().Be("T:MyNamespace.IMyInterface");
        relation.Kind.Should().Be(RelationKind.Realization);
    }

    [Fact]
    public void ProjectIR_ShouldCreateCorrectly()
    {
        // Arrange
        var types = new List<TypeIR>
        {
            new TypeIR("T:NS.Class1", "Class1", "NS", TypeKind.Class, "public", false, false, false),
            new TypeIR("T:NS.Class2", "Class2", "NS", TypeKind.Class, "public", false, false, false)
        };
        var relations = new List<RelationIR>
        {
            new RelationIR("T:NS.Class1", "T:NS.Class2", RelationKind.Inheritance)
        };

        // Act
        var projectIR = new ProjectIR("TestProject", types, relations);

        // Assert
        projectIR.Name.Should().Be("TestProject");
        projectIR.Types.Should().HaveCount(2);
        projectIR.Relations.Should().HaveCount(1);
    }

    [Fact]
    public void SolutionIR_ShouldCreateCorrectly()
    {
        // Arrange
        var projects = new List<ProjectIR>
        {
            new ProjectIR("Project1", new List<TypeIR>(), new List<RelationIR>()),
            new ProjectIR("Project2", new List<TypeIR>(), new List<RelationIR>())
        };

        // Act
        var solutionIR = new SolutionIR(projects);

        // Assert
        solutionIR.Projects.Should().HaveCount(2);
    }
}
