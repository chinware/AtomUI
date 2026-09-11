namespace AtomUI.Controls;

internal static class ImageLoadEventDispatcher
{
    internal static void Dispatch<TEventArgs>(
        EventHandler<TEventArgs>? handlers,
        object sender,
        TEventArgs args)
        where TEventArgs : EventArgs
    {
        if (handlers is null)
        {
            return;
        }

        foreach (var invocation in handlers.GetInvocationList())
        {
            if (invocation is not EventHandler<TEventArgs> handler)
            {
                continue;
            }
            try
            {
                handler(sender, args);
            }
            catch (Exception exception) when (IsNonFatal(exception))
            {
                // Image observers must not corrupt the loading state machine.
            }
        }
    }

    internal static bool IsNonFatal(Exception exception)
    {
        if (exception is AggregateException aggregate)
        {
            return aggregate.Flatten().InnerExceptions.All(IsNonFatal);
        }
        return exception is not OutOfMemoryException and
               not StackOverflowException and
               not AccessViolationException;
    }
}
