using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomCarousel = AtomUI.Desktop.Controls.Carousel;
using AtomCarouselPage = AtomUI.Desktop.Controls.CarouselPage;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly IBrush CarouselBackground = Avalonia.Media.Brush.Parse("#364d79");

    private static IReadOnlyList<PerfScenario> CreateCarouselScenarios()
    {
        return
        [
            new PerfScenario("Carousel.Basic4", _ => CreateCarousel(selectedIndex: 2)),
            new PerfScenario("Carousel.NoPagination4", _ => CreateCarousel(isShowPagination: false)),
            new PerfScenario("Carousel.NavButtons4", _ => CreateCarousel(isShowNavButtons: true)),
            new PerfScenario("Carousel.NavButtonsFiniteLeft4", _ => CreateCarousel(
                isShowNavButtons: true,
                isInfinite: false,
                paginationPosition: CarouselPaginationPosition.Left)),
            new PerfScenario("Carousel.AutoPlayFinite4", _ => CreateCarousel(
                isAutoPlay: true,
                isInfinite: false)),
            new PerfScenario("Carousel.Fade4", _ => CreateCarousel(
                transitionEffect: CarouselTransitionEffect.Fade,
                useColoredPages: true)),
            new PerfScenario("Carousel.Progress4", _ => CreateCarousel(
                isAutoPlay: true,
                isShowTransitionProgress: true)),
            new PerfScenario("Carousel.GalleryShape.Batch7", _ => CreateCarouselGalleryShape())
        ];
    }

    private static AtomCarousel CreateCarousel(
        int selectedIndex = 0,
        bool isShowPagination = true,
        bool isShowNavButtons = false,
        bool isAutoPlay = false,
        bool isInfinite = true,
        bool isShowTransitionProgress = false,
        CarouselPaginationPosition paginationPosition = CarouselPaginationPosition.Bottom,
        CarouselTransitionEffect transitionEffect = CarouselTransitionEffect.Scroll,
        bool useColoredPages = false)
    {
        var carousel = new AtomCarousel
        {
            Height                   = 160,
            Background               = CarouselBackground,
            SelectedIndex            = selectedIndex,
            IsShowPagination         = isShowPagination,
            IsShowNavButtons         = isShowNavButtons,
            IsAutoPlay               = isAutoPlay,
            IsInfinite               = isInfinite,
            IsShowTransitionProgress = isShowTransitionProgress,
            PaginationPosition       = paginationPosition,
            TransitionEffect         = transitionEffect,
            HorizontalAlignment      = HorizontalAlignment.Stretch
        };

        for (var i = 0; i < 4; i++)
        {
            var page = new AtomCarouselPage
            {
                Content = (i + 1).ToString()
            };
            if (useColoredPages)
            {
                page.Background = Avalonia.Media.Brush.Parse(i switch
                {
                    0 => "#B3001B",
                    1 => "#255C99",
                    2 => "#262626",
                    _ => "#CCAD8F"
                });
            }
            carousel.Items.Add(page);
        }

        return carousel;
    }

    private static Control CreateCarouselGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 20,
            Width   = 980
        };

        root.Children.Add(CreateCarousel(selectedIndex: 2));
        root.Children.Add(CreatePaginationPositionExample());
        root.Children.Add(CreateCarousel(isAutoPlay: true, isInfinite: false));
        root.Children.Add(CreateCarousel(
            transitionEffect: CarouselTransitionEffect.Fade,
            useColoredPages: true));
        root.Children.Add(CreateCarouselNavButtonsExample());
        root.Children.Add(CreateProgressExample());

        return root;
    }

    private static Control CreatePaginationPositionExample()
    {
        var positionOptions = new OptionButtonGroup
        {
            ButtonStyle = OptionButtonStyle.Outline
        };
        positionOptions.Items.Add(new OptionButton { Content = "Top" });
        positionOptions.Items.Add(new OptionButton { Content = "Bottom", IsChecked = true });
        positionOptions.Items.Add(new OptionButton { Content = "Left" });
        positionOptions.Items.Add(new OptionButton { Content = "Right" });

        return new StackPanel
        {
            Spacing = 20,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 5,
                    Children =
                    {
                        new AtomTextBlock
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Text              = "Pagination Position:"
                        },
                        positionOptions
                    }
                },
                CreateCarousel()
            }
        };
    }

    private static Control CreateCarouselNavButtonsExample()
    {
        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                CreateCarousel(isShowNavButtons: true),
                CreateCarousel(
                    isShowNavButtons: true,
                    isInfinite: false,
                    paginationPosition: CarouselPaginationPosition.Left)
            }
        };
    }

    private static Control CreateProgressExample()
    {
        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                CreateCarousel(
                    isAutoPlay: true,
                    isShowTransitionProgress: true)
            }
        };
    }
}
