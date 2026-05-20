using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateCascaderScenarios()
    {
        return
        [
            new PerfScenario("Cascader.Default.Closed", _ => new Cascader
            {
                OptionsSource = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.AllowClear.Closed", _ => new Cascader
            {
                IsAllowClear  = true,
                OptionsSource = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.DefaultPath.Closed", _ => new Cascader
            {
                DefaultSelectOptionPath = new TreeNodePath(["zhejiang", "hangzhou", "xihu"]),
                OptionsSource           = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.Single.Filter.Closed", _ => new Cascader
            {
                IsFilterEnabled = true,
                OptionsSource   = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.Single.Filter.PopupMaterialized", _ => MaterializePopupAfterLoaded(new Cascader
            {
                IsFilterEnabled = true,
                OptionsSource   = CreateCascaderOptions()
            })),
            new PerfScenario("Cascader.Multiple.Empty", _ => new Cascader
            {
                IsMultiple    = true,
                OptionsSource = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.Multiple.Selected", _ =>
            {
                var options = CreateCascaderOptions();
                return new Cascader
                {
                    IsMultiple      = true,
                    ShowCheckedStrategy = TreeSelectCheckedStrategy.ShowParent,
                    OptionsSource   = options,
                    SelectedOptions = [options[0]]
                };
            }),
            new PerfScenario("Cascader.Multiple.ResponsiveTags.Empty", _ => new Cascader
            {
                IsMultiple            = true,
                IsResponsiveTagMode   = true,
                ShowCheckedStrategy   = TreeSelectCheckedStrategy.All,
                OptionsSource         = CreateCascaderOptions()
            }),
            new PerfScenario("Cascader.AsyncLoader.Closed", _ => new Cascader
            {
                OptionsSource = CreateAsyncCascaderOptions(),
                DataLoader    = new CascaderNoopDataLoader()
            }),
            new PerfScenario("Cascader.Default.PopupMaterialized", _ => MaterializePopupAfterLoaded(new Cascader
            {
                OptionsSource = CreateCascaderOptions()
            })),
            new PerfScenario("CascaderView.Default.Direct", _ => new CascaderView
            {
                OptionsSource = CreateCascaderOptions()
            }),
            new PerfScenario("CascaderView.Checkable.Direct", _ => new CascaderView
            {
                IsCheckable   = true,
                OptionsSource = CreateCascaderOptions()
            }),
            new PerfScenario("CascaderView.Filter.Active", _ => new CascaderView
            {
                OptionsSource = CreateCascaderOptions(),
                FilterValue   = "lake"
            }),
            new PerfScenario("CascaderView.DefaultExpanded.Direct", _ => new CascaderView
            {
                DefaultExpandedPath = new TreeNodePath(["zhejiang", "hangzhou", "xihu"]),
                OptionsSource       = CreateCascaderOptions()
            }),
            new PerfScenario("CascaderView.AsyncLoader.Direct", _ => new CascaderView
            {
                OptionsSource = CreateAsyncCascaderOptions(),
                DataLoader    = new CascaderNoopDataLoader()
            }),
            new PerfScenario("CascaderView.Empty.Direct", _ => new CascaderView())
        ];
    }

    private static List<ICascaderOption> CreateCascaderOptions()
    {
        return
        [
            new CascaderOption
            {
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Value   = "zhejiang",
                Children =
                [
                    new CascaderOption
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Value   = "hangzhou",
                        Children =
                        [
                            new CascaderOption { Header = "West Lake", ItemKey = "xihu", Value = "xihu" },
                            new CascaderOption { Header = "Lingyin", ItemKey = "lingyin", Value = "lingyin" }
                        ]
                    }
                ]
            },
            new CascaderOption
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Value   = "jiangsu",
                Children =
                [
                    new CascaderOption
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Value   = "nanjing",
                        Children =
                        [
                            new CascaderOption
                            {
                                Header  = "Zhong Hua Men",
                                ItemKey = "zhonghuamen",
                                Value   = "zhonghuamen"
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private static List<ICascaderOption> CreateAsyncCascaderOptions()
    {
        return
        [
            new CascaderOption
            {
                Header = "Zhejiang",
                Value  = "zhejiang",
                IsLeaf = false
            },
            new CascaderOption
            {
                Header = "Jiangsu",
                Value  = "jiangsu",
                IsLeaf = false
            }
        ];
    }

    private sealed class CascaderNoopDataLoader : ICascaderItemDataLoader
    {
        public Task<CascaderItemLoadResult> LoadAsync(ICascaderOption targetCascaderItem, CancellationToken token)
        {
            return Task.FromResult(new CascaderItemLoadResult
            {
                IsSuccess = true,
                Data      = []
            });
        }
    }
}
