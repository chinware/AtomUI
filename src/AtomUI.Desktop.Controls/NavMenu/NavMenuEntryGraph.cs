using System.Collections;

namespace AtomUI.Desktop.Controls;

internal static class NavMenuEntryGraph
{
    public static void ValidateInsertion(INavMenuEntry owner, INavMenuEntry candidate)
    {
        var visited = new HashSet<INavMenuEntry>(ReferenceEqualityComparer.Instance);
        var active  = new HashSet<INavMenuEntry>(ReferenceEqualityComparer.Instance);
        var pending = new Stack<(INavMenuEntry Entry, bool Exit)>();
        pending.Push((candidate, false));

        while (pending.Count > 0)
        {
            var (entry, exit) = pending.Pop();
            if (exit)
            {
                active.Remove(entry);
                visited.Add(entry);
                continue;
            }

            if (ReferenceEquals(entry, owner) || !active.Add(entry))
            {
                throw new InvalidOperationException(
                    $"Adding {candidate.GetType().Name} to {owner.GetType().Name} would create a NavMenu entry cycle.");
            }

            if (visited.Contains(entry))
            {
                active.Remove(entry);
                continue;
            }

            pending.Push((entry, true));
            foreach (var child in EnumerateOwnedEntries(entry))
            {
                pending.Push((child, false));
            }
        }
    }

    public static bool ContainsDirectNode(IEnumerable entries, INavMenuNode target)
    {
        foreach (var entry in entries)
        {
            switch (entry)
            {
                case INavMenuNode node when ReferenceEquals(node, target):
                    return true;

                case NavMenuGroup group when ContainsDirectNode(group.Entries, target):
                    return true;
            }
        }

        return false;
    }

    private static IEnumerable<INavMenuEntry> EnumerateOwnedEntries(INavMenuEntry entry)
    {
        return entry switch
        {
            INavMenuNode node => node.Entries,
            NavMenuGroup group => group.Entries,
            _ => []
        };
    }
}
