using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class QRCode : AbstractQRCode
{
    private const string ImageFrameName = "ImageFrame";
    private const string LoadingLayoutName = "LoadingLayout";
    private const string ExpiredLayoutName = "ExpiredLayout";
    private const string ScannedLayoutName = "ScannedLayout";
    private const string RefreshButtonName = "PART_RefreshButton";

    private Panel? _imageHost;
    private Panel? _statusHost;
    private Border? _iconFrame;
    private Image? _iconImage;
    private Control? _statusLayer;
    private QRCodeStatus? _statusLayerStatus;
    private Button? _statusRefreshButton;
    private List<IDisposable>? _statusBindings;

    public QRCode()
    {
        this.RegisterTokenResourceScope(QRCodeToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseIconLayer();
        ReleaseStatusLayer();
        base.OnApplyTemplate(e);
        _imageHost  = e.NameScope.Find<Panel>("PART_ImageHost");
        _statusHost = e.NameScope.Find<Panel>("PART_StatusHost");
        UpdateIconLayer();
        UpdateStatusLayer(force: true);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateIconLayer();
        UpdateStatusLayer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseStatusLayer();
        ReleaseIconLayer();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty ||
            change.Property == IconSizeProperty ||
            change.Property == IconBgColorProperty)
        {
            UpdateIconLayer();
        }
        if (change.Property == StatusProperty)
        {
            UpdateStatusLayer();
        }
        else if (change.Property == LoadingContentProperty ||
                 change.Property == LoadingContentTemplateProperty ||
                 change.Property == ExpiredContentProperty ||
                 change.Property == ExpiredContentTemplateProperty ||
                 change.Property == ScannedContentProperty ||
                 change.Property == ScannedContentTemplateProperty)
        {
            UpdateStatusLayer(force: true);
        }
    }

    private void UpdateIconLayer()
    {
        if (Icon is null || _imageHost is null)
        {
            ReleaseIconLayer();
            return;
        }

        if (_iconFrame is null)
        {
            _iconImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            _iconFrame = new Border
            {
                Name                = ImageFrameName,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Child               = _iconImage
            };
            _iconFrame.SetTemplatedParentRecursive(this);
            _imageHost.Children.Add(_iconFrame);
        }

        _iconFrame.SetCurrentValue(WidthProperty, (double)IconSize);
        _iconFrame.SetCurrentValue(HeightProperty, (double)IconSize);
        _iconFrame.SetCurrentValue(BackgroundProperty, IconBgColor);
        _iconImage?.SetCurrentValue(Image.SourceProperty, Icon);
    }

    private void ReleaseIconLayer()
    {
        if (_iconFrame is null)
        {
            return;
        }

        if (_iconFrame.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_iconFrame);
        }
        else
        {
            _imageHost?.Children.Remove(_iconFrame);
        }

        _iconImage?.SetCurrentValue(Image.SourceProperty, null);
        _iconFrame.Child = null;
        _iconFrame.SetTemplatedParentRecursive(null);
        _iconImage = null;
        _iconFrame = null;
    }

    private void UpdateStatusLayer(bool force = false)
    {
        if (Status == QRCodeStatus.Active || _statusHost is null)
        {
            ReleaseStatusLayer();
            return;
        }

        if (!force && _statusLayer is not null && _statusLayerStatus == Status)
        {
            return;
        }

        ReleaseStatusLayer();
        _statusLayer       = CreateStatusLayer(Status);
        _statusLayerStatus = Status;
        _statusHost.Children.Add(_statusLayer);
    }

    private Control CreateStatusLayer(QRCodeStatus status)
    {
        return status switch
        {
            QRCodeStatus.Loading => LoadingContent is null
                ? CreateDefaultLoadingLayer()
                : CreateContentStatusLayer(LoadingLayoutName, LoadingContent, LoadingContentTemplate),
            QRCodeStatus.Expired => ExpiredContent is null
                ? CreateDefaultExpiredLayer()
                : CreateContentStatusLayer(ExpiredLayoutName, ExpiredContent, ExpiredContentTemplate),
            QRCodeStatus.Scanned => ScannedContent is null
                ? CreateDefaultScannedLayer()
                : CreateContentStatusLayer(ScannedLayoutName, ScannedContent, ScannedContentTemplate),
            _ => CreateDefaultScannedLayer()
        };
    }

    private Panel CreateDefaultLoadingLayer()
    {
        var panel = new Panel
        {
            Name = LoadingLayoutName
        };
        panel.Children.Add(new Spin
        {
            Name                = "PART_LoadingSpin",
            IsSpinning          = true,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        });
        panel.SetTemplatedParentRecursive(this);
        return panel;
    }

    private Panel CreateDefaultExpiredLayer()
    {
        var panel = new Panel
        {
            Name = ExpiredLayoutName
        };
        var layout = new StackPanel
        {
            Orientation         = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        var expiredText = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };
        AddStatusBinding(LanguageResourceBinder.CreateBinding(
            expiredText,
            TextBlock.TextProperty,
            QRCodeLangResourceKind.Expired));

        _statusRefreshButton = new Button
        {
            Name                = RefreshButtonName,
            HorizontalAlignment = HorizontalAlignment.Center,
            Icon                = new ReloadOutlined(),
            ButtonType          = ButtonType.Link
        };
        AddStatusBinding(LanguageResourceBinder.CreateBinding(
            _statusRefreshButton,
            ContentControl.ContentProperty,
            QRCodeLangResourceKind.Refresh));
        _statusRefreshButton.Click += HandleStatusRefreshButtonClicked;

        layout.Children.Add(expiredText);
        layout.Children.Add(_statusRefreshButton);
        panel.Children.Add(layout);
        panel.SetTemplatedParentRecursive(this);
        return panel;
    }

    private Panel CreateDefaultScannedLayer()
    {
        var panel = new Panel
        {
            Name = ScannedLayoutName
        };
        var scannedText = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        AddStatusBinding(LanguageResourceBinder.CreateBinding(
            scannedText,
            TextBlock.TextProperty,
            QRCodeLangResourceKind.Scanned));
        panel.Children.Add(scannedText);
        panel.SetTemplatedParentRecursive(this);
        return panel;
    }

    private ContentPresenter CreateContentStatusLayer(string name, object? content, IDataTemplate? contentTemplate)
    {
        var presenter = new ContentPresenter
        {
            Name                = name,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            Content             = content,
            ContentTemplate     = contentTemplate
        };
        presenter.SetTemplatedParent(this);
        return presenter;
    }

    private void ReleaseStatusLayer()
    {
        if (_statusLayer is null)
        {
            return;
        }

        if (_statusRefreshButton is not null)
        {
            _statusRefreshButton.Click -= HandleStatusRefreshButtonClicked;
            _statusRefreshButton.SetCurrentValue(Button.IconProperty, null);
            _statusRefreshButton = null;
        }

        if (_statusBindings is not null)
        {
            foreach (var binding in _statusBindings)
            {
                binding.Dispose();
            }
            _statusBindings = null;
        }

        if (_statusLayer is ContentPresenter presenter)
        {
            presenter.Content         = null;
            presenter.ContentTemplate = null;
        }

        if (_statusLayer.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_statusLayer);
        }
        else
        {
            _statusHost?.Children.Remove(_statusLayer);
        }

        if (_statusLayer is ContentPresenter)
        {
            _statusLayer.SetTemplatedParent(null);
        }
        else
        {
            _statusLayer.SetTemplatedParentRecursive(null);
        }
        _statusLayer       = null;
        _statusLayerStatus = null;
    }

    private void AddStatusBinding(IDisposable binding)
    {
        _statusBindings ??= new List<IDisposable>();
        _statusBindings.Add(binding);
    }

    private void HandleStatusRefreshButtonClicked(object? sender, RoutedEventArgs e)
    {
        RaiseRefreshRequested();
    }
}
