using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartTargetResolution
{
    public SemanticPartTargetResolution(IReadOnlyList<Visual> targets, int totalMatchCount)
    {
        Targets         = targets;
        TotalMatchCount = totalMatchCount;
    }

    public IReadOnlyList<Visual> Targets { get; }

    public int TotalMatchCount { get; }

    public bool IsTruncated => Targets.Count < TotalMatchCount;
}

internal static class SemanticPartTargetResolver
{
    public const int DefaultTargetBudget = 32;

    public static SemanticPartTargetResolution Resolve(
        Control owner,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        IEnumerable<Visual>? additionalRoots = null,
        int targetBudget = DefaultTargetBudget)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(part);
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetBudget);

        var targets = new List<Visual>(Math.Min(targetBudget, 4));
        var matches = new HashSet<Visual>();
        var totalMatchCount = 0;

        void AddMatch(Visual candidate)
        {
            if (!matches.Add(candidate) || !IsEligible(candidate, part))
            {
                return;
            }

            totalMatchCount++;
            if (targets.Count < targetBudget)
            {
                targets.Add(candidate);
            }
        }

        if (string.Equals(part.Path, "root", StringComparison.Ordinal))
        {
            AddMatch(owner);
            return new SemanticPartTargetResolution(targets, totalMatchCount);
        }

        // 模板节点被嵌套控件的内容属性收养时（decorated box 的左右 addon 等），
        // Avalonia 会清空其 TemplatedParent，因此不能按 TemplatedParent 过滤；
        // 统一沿 owner 的视觉子树收集，并以嵌套语义 owner 作为遍历边界。
        if (part.RuntimeCreated && HasMarker(owner, part))
        {
            AddMatch(owner);
        }

        TraverseOwnerScope(owner, owner, part, registry, AddMatch);

        if (part.CrossVisualRoot && additionalRoots is not null)
        {
            foreach (var root in additionalRoots)
            {
                if (root is null)
                {
                    continue;
                }

                TraverseAdditionalRoot(root, owner, part, registry, AddMatch);
            }
        }

        return new SemanticPartTargetResolution(targets, totalMatchCount);
    }

    private static void TraverseOwnerScope(
        Visual scope,
        Control owner,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        Action<Visual> addMatch)
    {
        foreach (var child in scope.GetVisualChildren())
        {
            if (HasMarker(child, part))
            {
                addMatch(child);
            }

            if (IsNestedSemanticOwner(child, owner, registry))
            {
                continue;
            }

            TraverseOwnerScope(child, owner, part, registry, addMatch);
        }
    }

    private static void TraverseAdditionalRoot(
        Visual root,
        Control owner,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        Action<Visual> addMatch)
    {
        if (IsNestedSemanticOwner(root, owner, registry))
        {
            return;
        }

        if (HasMarker(root, part))
        {
            addMatch(root);
        }

        TraverseOwnerScope(root, owner, part, registry, addMatch);
    }

    private static bool IsNestedSemanticOwner(
        Visual candidate,
        Control owner,
        SemanticPartRegistry registry)
    {
        return candidate is Control control &&
               !ReferenceEquals(control, owner) &&
               !IsOwnerTemplateDescendant(control, owner) &&
               registry.TryGetControl(control.GetType(), out _);
    }

    private static bool IsOwnerTemplateDescendant(Control candidate, Control owner)
    {
        for (var current = candidate as TemplatedControl;
             current is not null;
             current = current.TemplatedParent as TemplatedControl)
        {
            if (ReferenceEquals(current, owner))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasMarker(Visual candidate, SemanticPartDescriptor part)
    {
        return part.SelectorClass is { } selectorClass &&
               part.ContractType.IsInstanceOfType(candidate) &&
               candidate.Classes.Contains(selectorClass);
    }

    private static bool IsEligible(Visual candidate, SemanticPartDescriptor part)
    {
        if (!part.ContractType.IsInstanceOfType(candidate) ||
            !candidate.IsAttachedToVisualTree() ||
            !candidate.IsEffectivelyVisible ||
            candidate.Bounds.Width <= 0 ||
            candidate.Bounds.Height <= 0)
        {
            return false;
        }

        return candidate.GetSelfAndVisualAncestors().All(static ancestor => ancestor.Opacity > 0);
    }
}
