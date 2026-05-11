using AtomUI.Controls;
using AtomUI.Controls.AsyncLoad;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public partial class Select
{
    private readonly AsyncSearchLoadCoordinator<object?, SelectOptionsLoadResult> _asyncLoadCoordinator = new();
    private bool _asyncOptionsLoaded;

    private void LoadOptionsAsync()
    {
        if (TryLoadOptionsAsync(OptionsAsyncLoadContext))
        {
            return;
        }
        var loadingEventArgs = new SelectOptionsLoadingEventArgs(OptionsAsyncLoadContext);
        NotifyOptionsLoading(loadingEventArgs);
        if (!loadingEventArgs.Cancel)
        {
            OptionsLoadComplete(OptionsAsyncLoadContext);
        }
    }

    protected virtual void NotifyOptionsLoading(SelectOptionsLoadingEventArgs e)
    {
        IsLoading = true;
        OptionsLoading?.Invoke(this, e);
    }

    protected virtual void NotifyOptionsLoaded(SelectOptionsLoadedEventArgs e)
    {
        IsLoading = false;
        OptionsLoaded?.Invoke(this, e);
    }

    private bool TryLoadOptionsAsync(object? context)
    {
        if (OptionsLoader == null)
        {
            return false;
        }

        _asyncLoadCoordinator.Timeout = AsyncLoadTimeout;
        _ = LoadOptionAsync(context);
        return true;
    }

    private async Task LoadOptionAsync(object? context)
    {
        var loader = OptionsLoader;
        if (loader == null)
        {
            return;
        }

        IsLoading = true;

        var outcome = await _asyncLoadCoordinator.LoadAsync(
            context,
            (ctx, token) => loader.LoadAsync(ctx, token));

        if (outcome.IsSkipped)
        {
            return;
        }

        if (outcome.IsSuccess && outcome.Result != null)
        {
            var result = outcome.Result;
            await Dispatcher.InvokeAsync(() =>
            {
                SetCurrentValue(OptionsSourceProperty, result.Data);
                OptionsLoadComplete(context, result);
                _asyncOptionsLoaded = true;
                IsLoading = false;
            });
            return;
        }

        var statusCode = outcome.Status switch
        {
            AsyncLoadStatus.TimedOut  => RpcStatusCode.Timeout,
            AsyncLoadStatus.Cancelled => RpcStatusCode.Cancelled,
            _                         => RpcStatusCode.Unknown
        };

        IsLoading = false;
        OptionsLoaded?.Invoke(this, new SelectOptionsLoadedEventArgs(context, new SelectOptionsLoadResult()
        {
            UserFriendlyMessage = outcome.Error?.Message,
            StatusCode          = statusCode
        }));
    }

    private void OptionsLoadComplete(object? context, SelectOptionsLoadResult? loadResult = null)
    {
        // Fire the Populated event containing the read-only view data.
        var optionsLoaded = new SelectOptionsLoadedEventArgs(context, loadResult);
        NotifyOptionsLoaded(optionsLoaded);

        bool isDropDownOpen = loadResult?.Data?.Any() == true;
        if (isDropDownOpen != IsDropDownOpen)
        {
            IgnorePropertyChange = true;
            SetCurrentValue(IsDropDownOpenProperty, isDropDownOpen);
        }
        if (IsDropDownOpen)
        {
            OpeningDropDown(false);
        }
        else
        {
            ClosingDropDown(true);
        }
    }
}
