cd  %~dp0
@set dir="..\output\Nuget"
                 
for %%f in (%dir%\*.nupkg) do (
	dotnet nuget push %%f  --source "D:\nuget.local"
)

pause