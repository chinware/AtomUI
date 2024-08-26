cd  %~dp0
@set dir="..\_output\Nuget"
                 
for %%f in (%dir%\*.nupkg) do (
	dotnet nuget push %%f  --source "D:\nuget.local"
)

pause