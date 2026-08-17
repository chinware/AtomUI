using System.Collections.Immutable;
using AtomUI.Generator.LinkedRegistration.Manifest;

namespace AtomUI.Generator.LinkedRegistration;

internal static class RegistrationUnitGraphPlanner
{
    internal static ImmutableArray<LinkedUnitManifestRecord> Plan(
        IReadOnlyList<LinkedUnitManifestRecord> packageUnits,
        IReadOnlyList<LinkedUnitEdgeManifestRecord> packageEdges,
        IEnumerable<string> roots)
    {
        var unitsById = packageUnits.ToDictionary(
            static unit => unit.UnitId,
            StringComparer.Ordinal);
        var dependencies = packageEdges.GroupBy(static edge => edge.SourceUnitId, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => (IReadOnlyList<string>)group.Select(static edge => edge.TargetUnitId)
                    .Where(static unitId => unitId.Length != 0)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(static unitId => unitId, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
        var selected = SelectClosure(unitsById, dependencies, roots);
        if (selected.Count == 0)
        {
            return ImmutableArray<LinkedUnitManifestRecord>.Empty;
        }

        var orderedDependencies = selected.ToDictionary(
            unitId => unitId,
            unitId => dependencies.TryGetValue(unitId, out var unitDependencies)
                ? (IReadOnlyList<string>)unitDependencies.Where(selected.Contains).ToArray()
                : [],
            StringComparer.Ordinal);
        var components = FindStronglyConnectedComponents(selected, orderedDependencies);
        var componentByUnit = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var componentIndex = 0; componentIndex < components.Count; componentIndex++)
        {
            foreach (var unitId in components[componentIndex])
            {
                componentByUnit.Add(unitId, componentIndex);
            }
        }

        var componentDependencies = new List<HashSet<int>>(components.Count);
        var componentDependents = new List<HashSet<int>>(components.Count);
        for (var index = 0; index < components.Count; index++)
        {
            componentDependencies.Add([]);
            componentDependents.Add([]);
        }
        foreach (var edge in orderedDependencies)
        {
            var sourceComponent = componentByUnit[edge.Key];
            foreach (var targetUnitId in edge.Value)
            {
                var targetComponent = componentByUnit[targetUnitId];
                if (sourceComponent == targetComponent)
                {
                    continue;
                }
                componentDependencies[sourceComponent].Add(targetComponent);
                componentDependents[targetComponent].Add(sourceComponent);
            }
        }

        var componentOrderKeys = components.Select(component => component.Min(
            unitId => unitsById[unitId].OrderKey)).ToArray();
        var componentNames = components.Select(GetMinUnitId).ToArray();
        var dependencyCount = componentDependencies.Select(static items => items.Count).ToArray();
        var ready = new SortedSet<int>(Comparer<int>.Create((left, right) =>
        {
            if (left == right)
            {
                return 0;
            }
            var order = componentOrderKeys[left].CompareTo(componentOrderKeys[right]);
            return order != 0
                ? order
                : string.CompareOrdinal(componentNames[left], componentNames[right]);
        }));
        for (var componentIndex = 0; componentIndex < dependencyCount.Length; componentIndex++)
        {
            if (dependencyCount[componentIndex] == 0)
            {
                ready.Add(componentIndex);
            }
        }

        var orderedComponents = new List<int>(components.Count);
        while (ready.Count != 0)
        {
            var componentIndex = ready.Min;
            ready.Remove(componentIndex);
            orderedComponents.Add(componentIndex);
            foreach (var dependent in componentDependents[componentIndex]
                         .OrderBy(index => componentOrderKeys[index])
                         .ThenBy(index => componentNames[index], StringComparer.Ordinal))
            {
                dependencyCount[dependent]--;
                if (dependencyCount[dependent] == 0)
                {
                    ready.Add(dependent);
                }
            }
        }

        if (orderedComponents.Count != components.Count)
        {
            throw new InvalidOperationException("Registration Unit component graph did not converge.");
        }

        var result = ImmutableArray.CreateBuilder<LinkedUnitManifestRecord>(selected.Count);
        foreach (var componentIndex in orderedComponents)
        {
            foreach (var unitId in components[componentIndex]
                         .OrderBy(id => unitsById[id].OrderKey)
                         .ThenBy(static id => id, StringComparer.Ordinal))
            {
                result.Add(unitsById[unitId]);
            }
        }
        return result.ToImmutable();
    }

    private static HashSet<string> SelectClosure(
        IReadOnlyDictionary<string, LinkedUnitManifestRecord> unitsById,
        IReadOnlyDictionary<string, IReadOnlyList<string>> dependencies,
        IEnumerable<string> roots)
    {
        var selected = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Stack<string>(roots.Where(unitsById.ContainsKey)
            .Distinct(StringComparer.Ordinal)
            .OrderByDescending(static unitId => unitId, StringComparer.Ordinal));
        while (pending.Count != 0)
        {
            var unitId = pending.Pop();
            if (!selected.Add(unitId) || !dependencies.TryGetValue(unitId, out var unitDependencies))
            {
                continue;
            }
            for (var index = unitDependencies.Count - 1; index >= 0; index--)
            {
                var dependency = unitDependencies[index];
                if (unitsById.ContainsKey(dependency))
                {
                    pending.Push(dependency);
                }
            }
        }
        return selected;
    }

    private static List<List<string>> FindStronglyConnectedComponents(
        ISet<string> selected,
        IReadOnlyDictionary<string, IReadOnlyList<string>> dependencies)
    {
        var reverseDependencies = selected.ToDictionary(
            unitId => unitId,
            static _ => new List<string>(),
            StringComparer.Ordinal);
        foreach (var edge in dependencies)
        {
            foreach (var dependency in edge.Value)
            {
                reverseDependencies[dependency].Add(edge.Key);
            }
        }
        foreach (var reverse in reverseDependencies.Values)
        {
            reverse.Sort(StringComparer.Ordinal);
        }

        var visited = new HashSet<string>(StringComparer.Ordinal);
        var finishOrder = new List<string>(selected.Count);
        foreach (var root in selected.OrderBy(static unitId => unitId, StringComparer.Ordinal))
        {
            if (!visited.Add(root))
            {
                continue;
            }
            var frames = new Stack<TraversalFrame>();
            frames.Push(new TraversalFrame(root, dependencies[root]));
            while (frames.Count != 0)
            {
                var frame = frames.Pop();
                if (frame.NextIndex < frame.Neighbors.Count)
                {
                    var neighbor = frame.Neighbors[frame.NextIndex];
                    frame.NextIndex++;
                    frames.Push(frame);
                    if (visited.Add(neighbor))
                    {
                        frames.Push(new TraversalFrame(neighbor, dependencies[neighbor]));
                    }
                }
                else
                {
                    finishOrder.Add(frame.UnitId);
                }
            }
        }

        visited.Clear();
        var components = new List<List<string>>();
        for (var index = finishOrder.Count - 1; index >= 0; index--)
        {
            var root = finishOrder[index];
            if (!visited.Add(root))
            {
                continue;
            }
            var component = new List<string>();
            var pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var unitId = pending.Pop();
                component.Add(unitId);
                var neighbors = reverseDependencies[unitId];
                for (var neighborIndex = neighbors.Count - 1; neighborIndex >= 0; neighborIndex--)
                {
                    var neighbor = neighbors[neighborIndex];
                    if (visited.Add(neighbor))
                    {
                        pending.Push(neighbor);
                    }
                }
            }
            components.Add(component);
        }
        return components;
    }

    private static string GetMinUnitId(IEnumerable<string> unitIds)
    {
        return unitIds.OrderBy(static unitId => unitId, StringComparer.Ordinal).First();
    }

    private sealed class TraversalFrame
    {
        internal TraversalFrame(string unitId, IReadOnlyList<string> neighbors)
        {
            UnitId = unitId;
            Neighbors = neighbors;
        }

        internal string UnitId { get; }
        internal IReadOnlyList<string> Neighbors { get; }
        internal int NextIndex { get; set; }
    }
}
