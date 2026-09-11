using Avalonia;

namespace AtomUI;

internal interface IAtomUIOwnedService : IDisposable
{
    void Attach(Application application);

    void Detach(Application application);
}

internal sealed record AtomUIOwnedServiceRegistration(
    string Id,
    Func<Application, IAtomUIOwnedService> Factory);
