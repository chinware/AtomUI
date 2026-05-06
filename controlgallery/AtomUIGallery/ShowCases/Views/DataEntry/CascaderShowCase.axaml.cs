using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CascaderShowCase : ReactiveUserControl<CascaderViewModel>
{
    public CascaderShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is CascaderViewModel viewModel)
            {
                InitBasicCascaderData(viewModel);
                InitDefaultValueCascaderData(viewModel);
                InitHoverCascaderData(viewModel);
                InitDisabledCascaderData(viewModel);
                InitSelectParentCascaderData(viewModel);
                InitMultiSelectCascaderData(viewModel);
                InitPrefixAndSuffixCascaderData(viewModel);
                InitPlacementCascaderData(viewModel);
                InitSearchCascaderData(viewModel);
                InitSizeCascaderData(viewModel);
                InitCascaderViewData(viewModel);
                InitCascaderViewAsyncLoadData(viewModel);
                InitCascaderViewSearchData(viewModel);
                InitCascaderViewDefaultExpandData(viewModel);

                BasicCascader.OptionsSource = viewModel.BasicCascaderViewNodes;
                DefaultValueCascader.OptionsSource = viewModel.BasicCascaderViewNodes;
                DefaultValueCascader.DefaultSelectOptionPath = viewModel.DefaultSelectOptionPath;
                HoverCascader.OptionsSource = viewModel.HoverCascaderNodes;
                DisabledCascader.OptionsSource = viewModel.DisabledCascaderNodes;
                SelectParentCascader.OptionsSource = viewModel.SelectParentCascaderNodes;
                MultiSelectCascader.OptionsSource = viewModel.MultipleSelectCascaderNodes;
                CheckStrategyAllCascader.OptionsSource = viewModel.CheckStrategyShowAllCascaderNodes;
                CheckStrategyShowParentCascader.OptionsSource = viewModel.CheckStrategyShowParentCascaderNodes;
                SearchCascader.OptionsSource = viewModel.SearchCascaderNodes;
                AsyncLoadCascader.OptionsSource = viewModel.AsyncLoadCascaderViewNodes;
                AsyncLoadCascader.DataLoader = viewModel.AsyncCascaderNodeLoader;
                PrefixAndSuffixCascader1.OptionsSource = viewModel.PrefixAndSuffixCascaderNodes;
                PrefixAndSuffixCascader2.OptionsSource = viewModel.PrefixAndSuffixCascaderNodes;
                PrefixAndSuffixCascader3.OptionsSource = viewModel.PrefixAndSuffixCascaderNodes;
                PlacementCascader.OptionsSource = viewModel.PlacementCascaderNodes;
                SizeCascader1.OptionsSource = viewModel.SizeCascaderNodes;
                SizeCascader2.OptionsSource = viewModel.SizeCascaderNodes;
                SizeCascader3.OptionsSource = viewModel.SizeCascaderNodes;
                BasicCascaderView.OptionsSource = viewModel.BasicCascaderViewNodes;
                BasicCheckableCascaderView.OptionsSource = viewModel.BasicCheckableCascaderViewNodes;
                AsyncLoadCascaderView.OptionsSource = viewModel.AsyncLoadCascaderViewNodes;
                AsyncLoadCascaderView.DataLoader = viewModel.AsyncCascaderNodeLoader;
                SearchCascaderViewItemsSource.OptionsSource = viewModel.SearchCascaderViewNodes;
                DefaultExpandCascaderView.DefaultExpandedPath = viewModel.DefaultExpandPath;
                DefaultExpandCascaderView.OptionsSource = viewModel.DefaultExpandCascaderViewNodes;
            }
        });
        InitializeComponent();
    }

    private void InitBasicCascaderData(CascaderViewModel viewModel)
    {
        viewModel.BasicCascaderViewNodes = [
            new CascaderOption()
            {
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                IsEnabled = false,
                                Header    = "Hefang jie",
                                ItemKey   = "Hefang jie",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header    = "Jiangsu",
                ItemKey   = "jiangsu",
                IsEnabled = false,
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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

    private List<ICascaderOption> GenerateMultiSelectCascaderNodes()
    {
        var lightNode = new CascaderOption()
        {
            Header = "Light",
            Value  = "light",
            Children = [
                new CascaderOption() { Header = "Bamboo", Value = "bamboo" },
                new CascaderOption() { Header = "Little", Value = "little" }
            ]
        };
        var boyNode = new CascaderOption()
        {
            Header = "Bamboo Boy",
            Value  = "bambooBoy",
            Children = [
                new CascaderOption() { Header = "Little", Value = "little" },
                new CascaderOption() { Header = "Tadpole", Value = "tadpole" }
            ]
        };
        var mergeNode = new CascaderOption()
        {
            Header = "Little Tadpole",
            Value  = "littleTadpole",
        };

        var zhangSanNode = new CascaderOption()
        {
            Header   = "Zhang San",
            Value    = "zhangsan",
            Children = [lightNode, boyNode, mergeNode]
        };

        var greenNode = new CascaderOption()
        {
            Header = "Green",
            Value  = "green",
            Children = [
                new CascaderOption() { Header = "Wild Wolf", Value = "wildWolf" },
                new CascaderOption() { Header = "Gray Wolf", Value = "grayWolf" }
            ]
        };

        var yellowNode = new CascaderOption()
        {
            Header = "Yellow",
            Value  = "yellow",
            Children = [
                new CascaderOption() { Header = "Jiao Tailang", Value = "jiaoTailang" },
                new CascaderOption() { Header = "Banban", Value = "banban" }
            ]
        };

        var bigNode = new CascaderOption()
        {
            Header = "Big",
            Value  = "big",
            Children = [
                new CascaderOption() { Header = "Feifei", Value = "feifei" },
                new CascaderOption() { Header = "Xiao Xingxing", Value = "xiaoxingxing" }
            ]
        };

        var xiaoHuihuiNode = new CascaderOption()
        {
            Header   = "Xiao Huihui",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = "XiSha",
                                ItemKey   = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
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
                Header = "Zhejiang",
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Hangzhou",
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "West Lake",
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = "Lingyin shi",
                                Value  = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = "Jiangsu",
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Nanjing",
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "Zhong Hua Men",
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
                Header = "Zhejiang",
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Hangzhou",
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "West Lake",
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = "Lingyin shi",
                                Value  = "lingyinshi",
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = "Jiangsu",
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Nanjing",
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "Zhong Hua Men",
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
                Header = "Zhejiang",
                Value  = "zhejiang",
                IsLeaf = false,
            },
            new CascaderOption()
            {
                Header = "Jiangsu",
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
                Header = "Zhejiang",
                Value  = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Hangzhou",
                        Value  = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "West Lake",
                                Value  = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header = "Lingyin shi",
                                Value  = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = "XiSha",
                                Value     = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header = "Jiangsu",
                Value  = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header = "Nanjing",
                        Value  = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header = "Zhong Hua Men",
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
                Header  = "Zhejiang",
                ItemKey = "zhejiang",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Hangzhou",
                        ItemKey = "hangzhou",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "West Lake",
                                ItemKey = "xihu",
                            },
                            new CascaderOption()
                            {
                                Header  = "Lingyin shi",
                                ItemKey = "lingyinshi",
                            },
                            new CascaderOption()
                            {
                                Header    = "XiSha",
                                ItemKey   = "xisha",
                                IsEnabled = false,
                            }
                        ]
                    }
                ]
            },
            new CascaderOption()
            {
                Header  = "Jiangsu",
                ItemKey = "jiangsu",
                Children = [
                    new CascaderOption()
                    {
                        Header  = "Nanjing",
                        ItemKey = "nanjing",
                        Children = [
                            new CascaderOption()
                            {
                                Header  = "Zhong Hua Men",
                                ItemKey = "zhonghuamen",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void HandleFilterCascaderViewClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchCascaderView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandleFilterCascaderViewItemsSourceClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchCascaderViewItemsSource.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is not CascaderViewModel viewModel)
        {
            return;
        }

        viewModel.Placement = args.Index switch
        {
            0 => SelectPopupPlacement.TopEdgeAlignedLeft,
            1 => SelectPopupPlacement.TopEdgeAlignedRight,
            2 => SelectPopupPlacement.BottomEdgeAlignedLeft,
            _ => SelectPopupPlacement.BottomEdgeAlignedRight
        };
    }
}
