import { dotnet } from './_framework/dotnet.js'

try {
    const dotnetRuntime = await dotnet
        .withApplicationArgumentsFromQuery()
        .create();
    const config = dotnetRuntime.getConfig();
    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
} catch (error) {
    console.error('[AtomUIGallery.Browser] startup failed', error);

    const splash = document.querySelector('.avalonia-splash');
    if (splash) {
        const detail = document.createElement('pre');
        detail.className = 'splash-error';
        detail.textContent = error?.stack ?? error?.message ?? String(error);
        splash.appendChild(detail);
    }
}
