import { dotnet } from './_framework/dotnet.js';

const status = document.querySelector('#status');

try {
    const dotnetRuntime = await dotnet.create();
    const config = dotnetRuntime.getConfig();
    await dotnetRuntime.runMain(config.mainAssemblyName, []);

    status.dataset.status = 'success';
    status.replaceChildren();

    const title = document.createElement('h1');
    title.textContent = '.NET Browser AOT smoke';

    const detail = document.createElement('p');
    detail.textContent = 'Managed Main completed successfully.';

    status.append(title, detail);
} catch (error) {
    console.error('[DotNetBrowserAotSmoke] startup failed', error);
    status.dataset.status = 'failed';
    status.replaceChildren();

    const title = document.createElement('h1');
    title.textContent = '.NET Browser AOT smoke failed';

    const detail = document.createElement('pre');
    detail.textContent = error?.stack ?? error?.message ?? String(error);

    status.append(title, detail);
}
