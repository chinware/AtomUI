param (
    [string]$localSourcesDir,
    [string]$buildType = "Release"
)

dotnet restore --verbosity detailed ../AtomUI.sln
dotnet build --property:Configuration=Release --property:IsNightlyBuild=true ../AtomUI.sln
dotnet pack --no-build --no-restore --output $localSourcesDir --configuration Release /p:IsNightlyBuild=true ../packages/AtomUI/AtomUI.csproj
