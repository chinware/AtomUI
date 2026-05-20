using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using AvaTextBlock = Avalonia.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSkeletonScenarios()
    {
        return
        [
            new PerfScenario("Skeleton.Basic.Loading", _ => CreateBasicSkeleton()),
            new PerfScenario("Skeleton.Avatar.Paragraph4", _ => CreateAvatarParagraphSkeleton()),
            new PerfScenario("Skeleton.Active.Basic", _ => CreateActiveSkeleton()),
            new PerfScenario("Skeleton.Content.NotLoading", _ => CreateSkeletonWithContent()),
            new PerfScenario("Skeleton.Paragraph.Rows4", _ => CreateParagraphSkeleton()),
            new PerfScenario("Skeleton.Elements", _ => CreateSkeletonElements())
        ];
    }

    private static Skeleton CreateBasicSkeleton()
    {
        return new Skeleton
        {
            IsLoading = true,
            Width     = 480
        };
    }

    private static Skeleton CreateAvatarParagraphSkeleton()
    {
        return new Skeleton
        {
            IsLoading     = true,
            IsShowAvatar  = true,
            ParagraphRows = 4,
            Width         = 480
        };
    }

    private static Skeleton CreateActiveSkeleton()
    {
        return new Skeleton
        {
            IsLoading = true,
            IsActive  = true,
            Width     = 480
        };
    }

    private static Skeleton CreateSkeletonWithContent()
    {
        return new Skeleton
        {
            IsLoading = false,
            Width     = 480,
            Content   = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new AvaTextBlock { Text = "Ant Design, a design language" },
                    new AvaTextBlock { Text = "High quality design resources for product prototypes." }
                }
            }
        };
    }

    private static SkeletonParagraph CreateParagraphSkeleton()
    {
        return new SkeletonParagraph
        {
            Rows  = 4,
            Width = 480
        };
    }

    private static Control CreateSkeletonElements()
    {
        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new SkeletonButton(),
                new SkeletonAvatar(),
                new SkeletonInput(),
                new SkeletonImage(),
                new SkeletonNode { Width = 160 }
            }
        };
    }
}
