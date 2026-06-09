import { dotnet } from './_framework/dotnet.js'

const splashProgress = document.querySelector('.splash-progress');
const splashProgressValue = document.querySelector('.splash-progress-value');
const splashProgressStatus = document.querySelector('.splash-progress-status');
const splashProgressPercent = document.querySelector('.splash-progress-percent');

function setSplashProgress(value, status) {
    const percent = Math.max(0, Math.min(100, Math.round(value)));
    if (splashProgress) {
        splashProgress.setAttribute('aria-valuenow', `${percent}`);
    }
    if (splashProgressValue) {
        splashProgressValue.style.width = `${percent}%`;
    }
    if (splashProgressPercent) {
        splashProgressPercent.textContent = `${percent}%`;
    }
    if (splashProgressStatus && status) {
        splashProgressStatus.textContent = status;
    }
}

try {
    setSplashProgress(0, '准备加载...');
    const dotnetRuntime = await dotnet
        .withApplicationArgumentsFromQuery()
        .withModuleConfig({
            onDownloadResourceProgress: (loaded, total) => {
                if (total > 0) {
                    setSplashProgress((loaded / total) * 90, `加载资源 ${loaded}/${total}`);
                }
            }
        })
        .create();
    const config = dotnetRuntime.getConfig();
    setSplashProgress(95, '启动 Gallery...');
    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
    setSplashProgress(100, '加载完成');
} catch (error) {
    console.error('[AtomUIGallery.Browser] startup failed', error);
    setSplashProgress(100, '启动失败');

    const splash = document.querySelector('.avalonia-splash');
    if (splash) {
        const detail = document.createElement('pre');
        detail.className = 'splash-error';
        detail.textContent = error?.stack ?? error?.message ?? String(error);
        splash.appendChild(detail);
    }
}
