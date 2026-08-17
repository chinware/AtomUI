extern alias LinkedPublish;

using LinkedEdge = LinkedPublish::AtomUI.Generator.LinkedRegistration.Manifest.LinkedUnitEdgeManifestRecord;
using LinkedEdgeKind = LinkedPublish::AtomUI.Generator.LinkedRegistration.Manifest.LinkedUnitEdgeEvidenceKind;
using LinkedPlanner = LinkedPublish::AtomUI.Generator.LinkedRegistration.RegistrationUnitGraphPlanner;
using LinkedUnit = LinkedPublish::AtomUI.Generator.LinkedRegistration.Manifest.LinkedUnitManifestRecord;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class RegistrationUnitGraphPlannerTests
{
    [Fact]
    public void Long_dependency_chain_is_planned_without_recursive_stack_growth()
    {
        const int unitCount = 10_000;
        var units = Enumerable.Range(0, unitCount)
            .Select(index => new LinkedUnit(
                "Acme.Controls",
                "Unit" + index,
                "GeneratedUnit" + index,
                "Add",
                index))
            .ToArray();
        var edges = Enumerable.Range(1, unitCount - 1)
            .Select(index => new LinkedEdge(
                "Acme.Controls",
                "Unit" + index,
                "Unit" + (index - 1),
                LinkedEdgeKind.CSharpCall))
            .ToArray();

        var result = LinkedPlanner.Plan(units, edges, ["Unit" + (unitCount - 1)]);

        result.Length.ShouldBe(unitCount);
        result[0].UnitId.ShouldBe("Unit0");
        result[^1].UnitId.ShouldBe("Unit9999");
    }

    [Fact]
    public void Cyclic_units_are_collapsed_and_dependencies_still_precede_dependents()
    {
        var units = new[]
        {
            new LinkedUnit("Acme.Controls", "A", "GeneratedA", "Add", 10),
            new LinkedUnit("Acme.Controls", "B", "GeneratedB", "Add", 20),
            new LinkedUnit("Acme.Controls", "C", "GeneratedC", "Add", 30),
            new LinkedUnit("Acme.Controls", "D", "GeneratedD", "Add", 40)
        };
        var edges = new[]
        {
            new LinkedEdge("Acme.Controls", "A", "B", LinkedEdgeKind.CSharpCall),
            new LinkedEdge("Acme.Controls", "B", "A", LinkedEdgeKind.CSharpCall),
            new LinkedEdge("Acme.Controls", "C", "B", LinkedEdgeKind.CSharpCall),
            new LinkedEdge("Acme.Controls", "D", "C", LinkedEdgeKind.CSharpCall)
        };

        var result = LinkedPlanner.Plan(units, edges, ["D"]);

        result.Select(static unit => unit.UnitId).ShouldBe(["A", "B", "C", "D"]);
    }
}
