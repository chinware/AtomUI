using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Cascader;

public partial class CascaderShowCase : GalleryReactiveUserControl<CascaderViewModel>
{
    public const string LanguageId = nameof(CascaderShowCase);

    private const string BasicScenario        = "Basic";
    private const string MultipleScenario     = "Multiple";
    private const string AdvancedScenario     = "Advanced";
    private const string CascaderViewScenario = "CascaderView";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public CascaderShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is CascaderViewModel viewModel)
            {
                RefreshCascaderData(viewModel);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshCascaderData(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    disposables.Add(Disposable.Create(() => themeManager.LanguageVariantChanged -= handler));
                }
            }
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario        => new CascaderBasicShowCase(),
            MultipleScenario     => new CascaderMultipleShowCase(),
            AdvancedScenario     => new CascaderAdvancedShowCase(),
            CascaderViewScenario => new CascaderViewShowCase(),
            _                    => throw new InvalidOperationException($"Unknown Cascader scenario: {scenario}")
        };
    }

    private void RefreshCascaderData(CascaderViewModel viewModel)
    {
        InitBasicCascaderData(viewModel);
        InitDefaultValueCascaderData(viewModel);
        InitHoverCascaderData(viewModel);
        InitDisabledCascaderData(viewModel);
        InitSelectParentCascaderData(viewModel);
        InitMultiSelectCascaderData(viewModel);
        InitCheckStrategyCascaderData(viewModel);
        InitPrefixAndSuffixCascaderData(viewModel);
        InitPlacementCascaderData(viewModel);
        InitSearchCascaderData(viewModel);
        InitSizeCascaderData(viewModel);
        InitCascaderViewData(viewModel);
        InitCascaderViewAsyncLoadData(viewModel);
        InitCascaderViewSearchData(viewModel);
        InitCascaderViewDefaultExpandData(viewModel);
    }

    private static string Lang(CascaderShowCaseLangResourceKind resourceKind, string fallback)
    {
        return CascaderShowCaseLanguage.Get(resourceKind, fallback);
    }

    private void InitBasicCascaderData(CascaderViewModel viewModel)
    {
        viewModel.BasicCascaderViewNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitDefaultValueCascaderData(CascaderViewModel viewModel)
    {
        viewModel.DefaultSelectOptionPath = new TreeNodePath(["zhejiang", "hangzhou", "xihu"]);
    }

    private void InitHoverCascaderData(CascaderViewModel viewModel)
    {
        viewModel.HoverCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitDisabledCascaderData(CascaderViewModel viewModel)
    {
        viewModel.DisabledCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                IsEnabled = false,
                                Header    = Lang(CascaderShowCaseLangResourceKind.P2HeaderHefangJie, "Hefang jie"),
                                ItemKey   = "Hefang jie",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header    = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey   = "jiangsu",
                IsEnabled = false,
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitSelectParentCascaderData(CascaderViewModel viewModel)
    {
        viewModel.SelectParentCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitMultiSelectCascaderData(CascaderViewModel viewModel)
    {
        viewModel.MultipleSelectCascaderNodes = GenerateMultiSelectCascaderNodes();
    }

    private void InitCheckStrategyCascaderData(CascaderViewModel viewModel)
    {
        viewModel.CheckStrategyShowAllCascaderNodes    = GenerateCheckStrategyCascaderNodes();
        viewModel.CheckStrategyShowParentCascaderNodes = GenerateCheckStrategyCascaderNodes();
    }

    private List<ICascaderOption> GenerateCheckStrategyCascaderNodes()
    {
        return [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),  ItemKey = "xihu" },
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"), ItemKey = "lingyinshi" }
                        ]
                    },
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNingbo, "Ningbo"),
                        ItemKey = "ningbo",
                        Children = [
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderTianyiPavilion, "Tianyi Pavilion"), ItemKey = "tianyige" },
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderMoonLake, "Moon Lake"),       ItemKey = "yuehu" }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),         ItemKey = "zhonghuamen" },
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderSunYatSenMausoleum, "Sun Yat-sen Mausoleum"), ItemKey = "zhongshanling" }
                        ]
                    },
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderSuzhou, "Suzhou"),
                        ItemKey = "suzhou",
                        Children = [
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderHumbleAdministratorsGarden, "Humble Administrator's Garden"), ItemKey = "zhuozhengyuan" },
                            new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingeringGarden, "Lingering Garden"),              ItemKey = "liuyuan" }
                        ]
                    }
                ]
            }
        ];
    }

    private List<ICascaderOption> GenerateMultiSelectCascaderNodes()
    {
        var lightNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLight, "Light"),
            Value  = "light",
            Children = [
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderBamboo, "Bamboo"), Value = "bamboo" },
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLittle, "Little"), Value = "little" }
            ]
        };
        var boyNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderBambooBoy, "Bamboo Boy"),
            Value  = "bambooBoy",
            Children = [
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLittle, "Little"), Value = "little" },
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderTadpole, "Tadpole"), Value = "tadpole" }
            ]
        };
        var mergeNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLittleTadpole, "Little Tadpole"),
            Value  = "littleTadpole",
        };

        var zhangSanNode = new CascaderOption()
        {
            Header   = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhangSan, "Zhang San"),
            Value    = "zhangsan",
            Children = [lightNode, boyNode, mergeNode]
        };

        var greenNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderGreen, "Green"),
            Value  = "green",
            Children = [
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderWildWolf, "Wild Wolf"), Value = "wildWolf" },
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderGrayWolf, "Gray Wolf"), Value = "grayWolf" }
            ]
        };

        var yellowNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderYellow, "Yellow"),
            Value  = "yellow",
            Children = [
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiaoTailang, "Jiao Tailang"), Value = "jiaoTailang" },
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderBanban, "Banban"), Value = "banban" }
            ]
        };

        var bigNode = new CascaderOption()
        {
            Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderBig, "Big"),
            Value  = "big",
            Children = [
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderFeifei, "Feifei"), Value = "feifei" },
                new CascaderOption() { Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderXiaoXingxing, "Xiao Xingxing"), Value = "xiaoxingxing" }
            ]
        };

        var xiaoHuihuiNode = new CascaderOption()
        {
            Header   = Lang(CascaderShowCaseLangResourceKind.P2HeaderXiaoHuihui, "Xiao Huihui"),
            Value    = "xiaoHuihui",
            Children = [greenNode, yellowNode, bigNode]
        };
        return [zhangSanNode, xiaoHuihuiNode];
    }

    private void InitPrefixAndSuffixCascaderData(CascaderViewModel viewModel)
    {
        viewModel.PrefixAndSuffixCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitPlacementCascaderData(CascaderViewModel viewModel)
    {
        viewModel.Placement              = SelectPopupPlacement.TopEdgeAlignedLeft;
        viewModel.PlacementCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitSearchCascaderData(CascaderViewModel viewModel)
    {
        viewModel.SearchCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = Lang(CascaderShowCaseLangResourceKind.P2HeaderXisha, "Xisha"),
                                ItemKey   = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitSizeCascaderData(CascaderViewModel viewModel)
    {
        viewModel.SizeCascaderNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitCascaderViewData(CascaderViewModel viewModel)
    {
        viewModel.BasicCascaderViewNodes = [
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                Value  = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                Value  = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
        viewModel.BasicCheckableCascaderViewNodes = [
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                Value  = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                Value  = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitCascaderViewAsyncLoadData(CascaderViewModel viewModel)
    {
        viewModel.AsyncLoadCascaderViewNodes = [
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                Value  = "zhejiang",
                IsLeaf = false,
            },
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                Value  = "jiangsu",
                IsLeaf = false,
            }
        ];
        viewModel.AsyncCascaderNodeLoader = new CascaderItemDataLoader();
    }

    private void InitCascaderViewSearchData(CascaderViewModel viewModel)
    {
        viewModel.SearchCascaderViewNodes = [
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                Value  = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = Lang(CascaderShowCaseLangResourceKind.P2HeaderXisha, "Xisha"),
                                Value     = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                Value  = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitCascaderViewDefaultExpandData(CascaderViewModel viewModel)
    {
        viewModel.DefaultExpandPath = new TreeNodePath(["zhejiang", "hangzhou", "lingyinshi"]);
        viewModel.DefaultExpandCascaderViewNodes = [
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang"),
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou"),
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderWestLake, "West Lake"),
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin shi"),
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = Lang(CascaderShowCaseLangResourceKind.P2HeaderXisha, "Xisha"),
                                ItemKey   = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu"),
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing"),
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = Lang(CascaderShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men"),
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

}

internal static class CascaderShowCaseLanguage
{
    public static string Get(CascaderShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    public static string Format(CascaderShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(resourceKind, fallback), args);
    }

    public static string FormatDynamicOption(ICascaderOption targetCascaderItem, int index)
    {
        var parentHeader = targetCascaderItem.Header?.ToString()
                           ?? targetCascaderItem.Value?.ToString()
                           ?? string.Empty;
        return Format(CascaderShowCaseLangResourceKind.P2HeaderDynamicOptionFormat,
            "{0} Dynamic {1}",
            parentHeader,
            index);
    }
}
