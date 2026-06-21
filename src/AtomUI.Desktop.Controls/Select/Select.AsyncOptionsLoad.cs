using AtomUI.Controls;
using AtomUI.Controls.AsyncLoad;

namespace AtomUI.Desktop.Controls;

public partial class Select
{
    private readonly AsyncSearchLoadCoordinator<object?, SelectOptionsLoadResult> _asyncLoadCoordinator = new();
    private long _optionsLoadRequestId;
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

    private bool TryLoadOptionsAsync(object? context)
    {
        var loader = OptionsLoader;
        if (loader == null)
        {
            return false;
        }

        var requestId = Interlocked.Increment(ref _optionsLoadRequestId);
        _asyncLoadCoordinator.Timeout = AsyncLoadTimeout;
        _ = LoadOptionAsync(context, loader, requestId);
        return true;
    }

    private async Task LoadOptionAsync(object? context, ISelectOptionsAsyncLoader loader, long requestId)
    {
        IsLoading = true;

        var outcome = await _asyncLoadCoordinator.LoadAsync(
            context,
            (ctx, token) => loader.LoadAsync(ctx, token));

        if (outcome.IsSkipped)
        {
            return;
        }

        await Dispatcher.InvokeAsync(() => CompleteAsyncOptionsLoad(context, loader, requestId, outcome));
    }

    private void CompleteAsyncOptionsLoad(
        object? context,
        ISelectOptionsAsyncLoader loader,
        long requestId,
        AsyncLoadOutcome<SelectOptionsLoadResult> outcome)
    {
        if (Interlocked.Read(ref _optionsLoadRequestId) != requestId ||
            !ReferenceEquals(OptionsLoader, loader))
        {
            return;
        }

        if (outcome.IsSuccess && outcome.Result != null)
        {
            var result = outcome.Result;
            SetCurrentValue(OptionsSourceProperty, result.Data);
            OptionsLoadComplete(context, result);
            _asyncOptionsLoaded = true;
            IsLoading = false;
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

    private void CancelPendingOptionsLoad()
    {
        Interlocked.Increment(ref _optionsLoadRequestId);
        _asyncLoadCoordinator.Cancel();
        IsLoading = false;
    }

    private void OptionsLoadComplete(object? context, SelectOptionsLoadResult? loadResult = null)
    {
        // Fire the Populated event containing the read-only view data.
        var optionsLoaded = new SelectOptionsLoadedEventArgs(context, loadResult);
        NotifyOptionsLoaded(optionsLoaded);

        bool isDropDownOpen = loadResult?.Data?.Count > 0;
        if (isDropDownOpen != IsDropDownOpen)
        {
            SetDropDownOpenWithoutPropertyHandling(isDropDownOpen);
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
