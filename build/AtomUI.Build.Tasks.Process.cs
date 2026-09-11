#nullable disable
// The adapter is compiled by the SDK; the value protocol is shared with the worker.
// Only this source adapter enters the persistent build node. The real task DLL
// and its dependencies are loaded by a single-use dotnet process.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using AtomUI.Build.Tasks.Isolation;
using Microsoft.Build.Framework;
#if !ATOMUI_BUILD_TASK_HOST
using Microsoft.Build.Utilities;
#endif

namespace AtomUI.Build.Tasks.Isolation
{
    // The protocol contains values only. No task-defined types or MSBuild runtime
    // objects cross the process boundary, including item implementations.
    [DataContract]
    public sealed class TaskRequest
    {
        [DataMember] public string TaskName { get; set; }
        [DataMember] public string ProjectFile;
        [DataMember] public int Line;
        [DataMember] public int Column;
        [DataMember] public Dictionary<string, string> Properties = new Dictionary<string, string>();
        [DataMember] public Dictionary<string, TaskWireItem[]> Items = new Dictionary<string, TaskWireItem[]>();
    }

    [DataContract]
    public sealed class TaskResponse
    {
        [DataMember] public bool Success;
        [DataMember] public Dictionary<string, string> Properties = new Dictionary<string, string>();
        [DataMember] public Dictionary<string, TaskWireItem[]> Items = new Dictionary<string, TaskWireItem[]>();
        [DataMember] public List<TaskDiagnostic> Diagnostics = new List<TaskDiagnostic>();
    }

    [DataContract]
    public sealed class TaskWireItem : ITaskItem
    {
        [DataMember] public string ItemSpec { get; set; }
        [DataMember] public string FullPath;
        [DataMember] public Dictionary<string, string> Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public int MetadataCount { get { return Metadata.Count; } }
        public ICollection MetadataNames { get { return Metadata.Keys.ToArray(); } }
        public string GetMetadata(string name)
        {
            string value;
            if (Metadata.TryGetValue(name, out value))
                return value;
            return name.Equals("FullPath", StringComparison.OrdinalIgnoreCase) ? FullPath ?? string.Empty : string.Empty;
        }
        public void SetMetadata(string name, string value) { Metadata[name] = value; }
        public void RemoveMetadata(string name) { Metadata.Remove(name); }
        public IDictionary CloneCustomMetadata() { return new Dictionary<string, string>(Metadata, StringComparer.OrdinalIgnoreCase); }
        public void CopyMetadataTo(ITaskItem destination)
        {
            foreach (var pair in Metadata)
                if (string.IsNullOrEmpty(destination.GetMetadata(pair.Key)))
                {
                    var literalDestination = destination as ITaskItem2;
                    if (literalDestination != null)
                        literalDestination.SetMetadataValueLiteral(pair.Key, pair.Value);
                    else
                        destination.SetMetadata(pair.Key, pair.Value);
                }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Metadata = new Dictionary<string, string>(Metadata, StringComparer.OrdinalIgnoreCase);
        }

        public static TaskWireItem FromItem(ITaskItem item)
        {
            var result = new TaskWireItem { ItemSpec = item.ItemSpec, FullPath = item.GetMetadata("FullPath") };
            foreach (DictionaryEntry entry in item.CloneCustomMetadata())
                result.Metadata.Add((string)entry.Key, (string)entry.Value);
            return result;
        }
    }

    [DataContract]
    public sealed class TaskDiagnostic
    {
        [DataMember] public string Kind;
        [DataMember] public string Message;
        [DataMember] public string Subcategory;
        [DataMember] public string Code;
        [DataMember] public string HelpKeyword;
        [DataMember] public string File;
        [DataMember] public int Line;
        [DataMember] public int Column;
        [DataMember] public int EndLine;
        [DataMember] public int EndColumn;
        [DataMember] public int Importance;
    }

    public static class TaskWire
    {
        public static void Write<T>(string path, T value)
        {
            using (var stream = File.Create(path))
                Serializer<T>().WriteObject(stream, value);
        }

        public static T Read<T>(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return (T)Serializer<T>().ReadObject(stream);
        }

        public static void WriteResponse(TaskResponse response)
        {
            using (var stream = Console.OpenStandardOutput())
                Serializer<TaskResponse>().WriteObject(stream, response);
        }

        private static DataContractJsonSerializer Serializer<T>()
        {
            return new DataContractJsonSerializer(typeof(T),
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
        }
    }
}


#if !ATOMUI_BUILD_TASK_HOST
namespace AtomUI.Build.Tasks
{
    public abstract class IsolatedBuildTask : Task, ICancelableTask
    {
        [Required] public string TaskAssembly { get; set; }
        public string DotNetPath { get; set; }

        private readonly object processGate = new object();
        private Process process;
        private bool cancelled;

        public void Cancel()
        {
            lock (processGate)
            {
                cancelled = true;
                if (process != null && !process.HasExited)
                    process.Kill();
            }
        }

        public override bool Execute()
        {
            string directory = Path.Combine(Path.GetTempPath(), "atomui-build-task-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                return ExecuteProcess(Path.Combine(directory, "request.json"));
            }
            catch (Exception error)
            {
                Log.LogErrorFromException(error, true);
                return false;
            }
            finally
            {
                lock (processGate)
                {
                    if (process != null)
                    {
                        // A failed read or cancellation must not let a worker outlive
                        // the invocation and keep the toolset locked in the background.
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                        process.Dispose();
                        process = null;
                    }
                }
                Directory.Delete(directory, true);
            }
        }

        private bool ExecuteProcess(string projectFile)
        {
            var parameters = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.DeclaringType == GetType()).ToArray();
            var outputs = parameters.Where(p => p.IsDefined(typeof(OutputAttribute), true)).ToArray();
            var request = new TaskRequest
            {
                TaskName = GetType().Name, ProjectFile = BuildEngine.ProjectFileOfTaskNode,
                Line = BuildEngine.LineNumberOfTaskNode, Column = BuildEngine.ColumnNumberOfTaskNode
            };
            foreach (var parameter in parameters.Except(outputs))
            {
                object value = parameter.GetValue(this, null);
                if (value == null)
                    continue;
                if (parameter.PropertyType == typeof(ITaskItem[]))
                    request.Items.Add(parameter.Name, ((ITaskItem[])value).Select(TaskWireItem.FromItem).ToArray());
                else
                    request.Properties.Add(parameter.Name, Convert.ToString(value, CultureInfo.InvariantCulture));
            }
            TaskWire.Write(projectFile, request);
            string arguments = Quote(Path.GetFullPath(TaskAssembly)) + " " + Quote(projectFile);
            var start = new ProcessStartInfo
            {
                FileName = string.IsNullOrWhiteSpace(DotNetPath) ? "dotnet" : DotNetPath,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            lock (processGate)
            {
                if (cancelled)
                    return false;
                process = Process.Start(start);
            }
            var standardOutput = process.StandardOutput.ReadToEndAsync();
            var standardError = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            string outputText = standardOutput.GetAwaiter().GetResult();
            string errorText = standardError.GetAwaiter().GetResult();
            foreach (string line in errorText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                Log.LogMessageFromText(line, MessageImportance.High);
            if (process.ExitCode != 0)
            {
                Log.LogError("AtomUI build task {0} exited with code {1}. {2}", GetType().Name, process.ExitCode, outputText);
                return false;
            }

            TaskResponse result = TaskWire.Read<TaskResponse>(outputText);
            foreach (TaskDiagnostic diagnostic in result.Diagnostics)
            {
                if (diagnostic.Kind == "error")
                    Log.LogError(diagnostic.Subcategory, diagnostic.Code, diagnostic.HelpKeyword, diagnostic.File,
                        diagnostic.Line, diagnostic.Column, diagnostic.EndLine, diagnostic.EndColumn, diagnostic.Message);
                else if (diagnostic.Kind == "warning")
                    Log.LogWarning(diagnostic.Subcategory, diagnostic.Code, diagnostic.HelpKeyword, diagnostic.File,
                        diagnostic.Line, diagnostic.Column, diagnostic.EndLine, diagnostic.EndColumn, diagnostic.Message);
                else
                    Log.LogMessage((MessageImportance)diagnostic.Importance, "{0}", diagnostic.Message);
            }
            if (!result.Success)
                return false;
            foreach (var output in outputs)
            {
                if (output.PropertyType == typeof(ITaskItem[]))
                {
                    // Keep the original task's literal ITaskItem semantics, including
                    // custom metadata named Identity. TaskItem would reinterpret both.
                    output.SetValue(this, result.Items[output.Name].Cast<ITaskItem>().ToArray(), null);
                }
                else
                    output.SetValue(this, Convert.ChangeType(result.Properties[output.Name], output.PropertyType,
                        CultureInfo.InvariantCulture), null);
            }
            return !Log.HasLoggedErrors;
        }

        private static string Quote(string path)
        {
            if (path.IndexOf('"') >= 0)
                throw new ArgumentException("A process path cannot contain a quote.", "path");
            return "\"" + path + "\"";
        }

    }
}

namespace AtomUI.Build.Tasks
{
    public sealed class GenerateThemeAssetWrappersTask : IsolatedBuildTask
    {
        [Required] public ITaskItem[] ThemeAssets { get; set; } = new ITaskItem[0];
        [Required] public string OutputDirectory { get; set; } = string.Empty;
        [Required] public string AssemblyName { get; set; } = string.Empty;
        [Required] public string GeneratedCodePath { get; set; } = string.Empty;
        [Output] public ITaskItem[] GeneratedAssets { get; set; } = new ITaskItem[0];
    }
    public sealed class CollectAxamlUsageTask : IsolatedBuildTask
    {
        [Required] public ITaskItem[] AxamlFiles { get; set; } = new ITaskItem[0];
        [Required] public string ProjectDirectory { get; set; } = string.Empty;
        [Required] public string OutputPath { get; set; } = string.Empty;
        public string ProjectPackageId { get; set; } = string.Empty;
        public ITaskItem[] UnitRoots { get; set; } = new ITaskItem[0];
        public ITaskItem[] PackageRoots { get; set; } = new ITaskItem[0];
        [Output] public ITaskItem[] UsageCandidates { get; set; } = new ITaskItem[0];
        [Output] public ITaskItem[] Uncertainties { get; set; } = new ITaskItem[0];
    }
    public sealed class ValidateAssemblyMetadataMarkerTask : IsolatedBuildTask
    {
        [Required] public string AssemblyPath { get; set; } = string.Empty;
        [Required] public string MarkerKey { get; set; } = string.Empty;
        [Output] public int MarkerCount { get; set; }
    }
    public sealed class GenerateLinkedRegistrationSidecarTask : IsolatedBuildTask
    {
        [Required] public string AssemblyPath { get; set; } = string.Empty;
        [Required] public string OutputPath { get; set; } = string.Empty;
        public string TargetFramework { get; set; } = string.Empty;
        public bool ExtractedFallback { get; set; }
        public bool ExtractConsumerUsage { get; set; }
        public ITaskItem[] CatalogSidecars { get; set; } = new ITaskItem[0];
        [Output] public string SidecarPath { get; set; } = string.Empty;
        [Output] public bool WroteFile { get; set; }
    }
    public sealed class ResolveLinkedRegistrationSidecarCandidatesTask : IsolatedBuildTask
    {
        public ITaskItem[] SidecarCandidates { get; set; } = Array.Empty<ITaskItem>();
        [Required] public ITaskItem[] ReferencePaths { get; set; } = Array.Empty<ITaskItem>();
        [Output] public ITaskItem[] CanonicalSidecars { get; set; } = Array.Empty<ITaskItem>();
        [Output] public ITaskItem[] ExtractableReferences { get; set; } = Array.Empty<ITaskItem>();
    }
    public sealed class DiscoverLinkedRegistrationConsumerReferencesTask : IsolatedBuildTask
    {
        [Required] public ITaskItem[] ReferencePaths { get; set; } = new ITaskItem[0];
        [Required] public ITaskItem[] CatalogSidecars { get; set; } = new ITaskItem[0];
        [Output] public ITaskItem[] ConsumerReferences { get; set; } = new ITaskItem[0];
    }
    public sealed class ExportLanguageTemplatesTask : IsolatedBuildTask
    {
        [Required] public ITaskItem[] SourceFiles { get; set; } = Array.Empty<ITaskItem>();
        [Required] public string TargetLanguage { get; set; } = string.Empty;
        public string OutputRootDirectory { get; set; }
        [Output] public ITaskItem[] ExportedFiles { get; set; } = Array.Empty<ITaskItem>();
    }
    public sealed class PrepareLanguagePackageAssetsTask : IsolatedBuildTask
    {
        [Required] public string PackageId { get; set; } = string.Empty;
        [Required] public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();
        public ITaskItem[] SourceLanguageFiles { get; set; } = Array.Empty<ITaskItem>();
        public string ExpectedLanguage { get; set; }
        public string MinimumTargetState { get; set; } = "final";
        public bool RequireVerifiedContract { get; set; }
        [Output] public ITaskItem[] PreparedLanguageFiles { get; set; } = Array.Empty<ITaskItem>();
    }
    public sealed class PrepareLanguagePackageTask : IsolatedBuildTask
    {
        [Required] public string PackageId { get; set; } = string.Empty;
        [Required] public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();
        public ITaskItem[] SourceLanguageFiles { get; set; } = Array.Empty<ITaskItem>();
        public ITaskItem[] PackageFiles { get; set; } = Array.Empty<ITaskItem>();
        [Required] public string OutputManifestPath { get; set; } = string.Empty;
        public string ExpectedLanguage { get; set; }
        public string MinimumTargetState { get; set; } = "final";
        public bool RequireVerifiedContract { get; set; }
        [Output] public ITaskItem[] PreparedLanguageFiles { get; set; } = Array.Empty<ITaskItem>();
    }
    public sealed class GenerateLanguagePackagePropsTask : IsolatedBuildTask
    {
        [Required] public string PackageId { get; set; } = string.Empty;
        [Required] public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();
        [Required] public string OutputPath { get; set; } = string.Empty;
        public bool RequireTargetLanguage { get; set; } = true;
        public string SourceKind { get; set; } = "StaticLanguagePack";
    }
}

#endif
