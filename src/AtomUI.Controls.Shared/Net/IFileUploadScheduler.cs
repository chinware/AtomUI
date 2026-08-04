namespace AtomUI.Controls;

internal interface IFileUploadScheduler
{
    IFileUploadTransport? Transport { get; }
    void EnqueueTask(FileUploadTask task);
    Task CancelUploadAsync(FileUploadTask task);
    Task CancelAllAsync(CancellationToken cancellationToken = default); 
    void DisableSchedule();
    void EnableSchedule();
    bool IsScheduleEnabled();
    Task SetMaxConcurrentTasksAsync(int taskCount, CancellationToken cancellationToken = default);
    Task SetTransportAsync(IFileUploadTransport? transport, CancellationToken cancellationToken = default);
}
