@set dir="..\_output\Nuget"
                 
for %%f in (%dir%\*.nupkg) do (
    dotnet nuget push %%f --api-key [key] --source https://api.nuget.org/v3/index.json
)

pause