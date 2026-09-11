using System.Collections;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks.Tests;

internal sealed class RecordingBuildEngine : IBuildEngine
{
    internal List<BuildErrorEventArgs> Errors { get; } = [];

    internal List<BuildWarningEventArgs> Warnings { get; } = [];

    public bool ContinueOnError => false;

    public int LineNumberOfTaskNode => 0;

    public int ColumnNumberOfTaskNode => 0;

    public string ProjectFileOfTaskNode => "AtomUI.Build.Tasks.Tests.proj";

    public void LogErrorEvent(BuildErrorEventArgs e) => Errors.Add(e);

    public void LogWarningEvent(BuildWarningEventArgs e) => Warnings.Add(e);

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
    }

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
    }

    public bool BuildProjectFile(
        string projectFileName,
        string[] targetNames,
        IDictionary globalProperties,
        IDictionary targetOutputs) => false;
}

internal sealed class TestTaskItem : ITaskItem
{
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    internal TestTaskItem(string itemSpec, params (string Name, string Value)[] metadata)
    {
        ItemSpec = itemSpec;
        foreach (var (name, value) in metadata)
        {
            _metadata.Add(name, value);
        }
    }

    public string ItemSpec { get; set; }

    public int MetadataCount => _metadata.Count;

    public ICollection MetadataNames => _metadata.Keys.ToArray();

    public string GetMetadata(string metadataName)
    {
        return _metadata.TryGetValue(metadataName, out var value) ? value : string.Empty;
    }

    public void SetMetadata(string metadataName, string metadataValue)
    {
        _metadata[metadataName] = metadataValue;
    }

    public void RemoveMetadata(string metadataName)
    {
        _metadata.Remove(metadataName);
    }

    public void CopyMetadataTo(ITaskItem destinationItem)
    {
        foreach (var (name, value) in _metadata)
        {
            destinationItem.SetMetadata(name, value);
        }
    }

    public IDictionary CloneCustomMetadata()
    {
        return new Dictionary<string, string>(_metadata, StringComparer.OrdinalIgnoreCase);
    }
}
