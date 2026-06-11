import { dotnet } from './_framework/dotnet.js';

try {
    const dotnetRuntime = await dotnet.create();
    const config = dotnetRuntime.getConfig();
    await dotnetRuntime.runMain(config.mainAssemblyName, []);
} catch (error) {
    console.error('[AvaloniaBrowserAotSmoke] startup failed', error);
    const host = document.querySelector('#out');
    if (host) {
        const detail = document.createElement('pre');
        detail.textContent = error?.stack ?? error?.message ?? String(error);
        host.replaceChildren(detail);
    }
}
