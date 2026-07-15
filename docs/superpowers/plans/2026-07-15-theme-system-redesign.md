# AtomUI Theme System Redesign Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Design:** [AtomUI 主题系统重构设计](../../modules/core/theme-system-redesign.md)

**Goal:** Rebuild AtomUI theme parsing, compilation, activation, local inheritance, component-token isolation, and event delivery around immutable theme snapshots while preserving existing Token values and Avalonia ControlTheme contracts.

**Architecture:** A pure ThemeDefinitionParser feeds a cached ThemeCatalog; one ThemeCompiler produces logically immutable ThemeSnapshot objects for both global and local themes. ThemeTokenResourceProvider exposes snapshots through Avalonia DynamicResource, component-shared resource keys implement Ant Design component isolation, and ThemeCoordinator serializes atomic global transitions.

**Tech Stack:** C# 12, .NET 8 and .NET 10, Avalonia 12 ResourceProvider, DynamicResource and StyledProperty, XDocument, xUnit v3, Shouldly, Roslyn incremental generators, NativeAOT.

## Global Constraints

- Preserve existing Seed, Map, Alias, ControlToken values and Default, Dark and Compact calculator behavior.
- Preserve existing ControlTheme keys, template parts, public Token names, and AXAML collection syntax.
- ThemeConfigProvider.Inherit defaults to true and follows Ant Design parent merge semantics.
- Component Design Token overrides affect only the target component style computation, never arbitrary Content descendants.
- Do not add string-based runtime Binding, runtime assembly scanning, unannotated reflection, or a new dependency package.
- Treat every published ThemeSnapshot as immutable; failed work must not mutate active state.
- Each ThemeConfigProvider owns exactly one ThemeTokenResourceProvider.
- All global transitions execute on Dispatcher.UIThread, are non-reentrant, and emit events only after commit.
- Keep the legacy ScopeHost and lifecycle events only as migration adapters.
- Do not touch unrelated dirty WindowTitleBar files.

---

## Planned File Structure

### New Core Units

- src/AtomUI.Core/Theme/Definitions/ThemeDefinitionDiagnostic.cs: structured parser and compiler diagnostics.
- src/AtomUI.Core/Theme/Definitions/ThemeDefinitionParseResult.cs: immutable parse result.
- src/AtomUI.Core/Theme/Definitions/ThemeDefinitionParser.cs: pure XML parser.
- src/AtomUI.Core/Theme/Compilation/ThemeCompileRequest.cs: normalized compiler input.
- src/AtomUI.Core/Theme/Compilation/ThemeCompileResult.cs: snapshot or diagnostics.
- src/AtomUI.Core/Theme/Compilation/ThemeSnapshot.cs: effective global Token and resource snapshot.
- src/AtomUI.Core/Theme/Compilation/ComponentThemeSnapshot.cs: effective component shared and own Token snapshot.
- src/AtomUI.Core/Theme/Compilation/ThemeCompiler.cs: sole Seed, Map, Alias and component compiler.
- src/AtomUI.Core/Theme/Catalog/ThemeDescriptor.cs: one parsed theme definition.
- src/AtomUI.Core/Theme/Catalog/ThemeCatalog.cs: discovery, precedence, diagnostics and parse-once behavior.
- src/AtomUI.Core/Theme/Resources/ComponentTokenIdentity.cs: catalog-aware component identity.
- src/AtomUI.Core/Theme/Resources/ComponentSharedTokenResourceKey.cs: component-private shared Token key.
- src/AtomUI.Core/Theme/Resources/ComponentSharedTokenResourceExtension.cs: DynamicResource markup base.
- src/AtomUI.Core/Theme/Resources/ThemeTokenResourceProvider.cs: snapshot-backed ResourceProvider.
- src/AtomUI.Core/Theme/Scope/ThemeScope.cs: scoped snapshot property and lookup.
- src/AtomUI.Core/Theme/Transitions/ThemeRequest.cs: complete desired global theme.
- src/AtomUI.Core/Theme/Transitions/ThemeTransitionEventArgs.cs: transition context.
- src/AtomUI.Core/Theme/Transitions/ThemeCoordinator.cs: prepare and commit state machine.

### New Test Project

- tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj
- tests/AtomUI.Core.Tests/Theme/ThemeDefinitionParserTests.cs
- tests/AtomUI.Core.Tests/Theme/TokenValueConverterTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeCompilerTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeCatalogTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeTokenResourceProviderTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeConfigProviderTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeCoordinatorTests.cs
- tests/AtomUI.Core.Tests/Theme/ThemeSnapshotCacheTests.cs

---

### Task 1: Establish AtomUI.Core Theme Test Ownership

**Files:**
- Create: tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj
- Create: tests/AtomUI.Core.Tests/Theme/DesignTokenBaselineTests.cs
- Modify: src/AtomUI.Core/AtomUI.Core.csproj
- Modify: AtomUI.slnx

**Interfaces:**
- Consumes: existing DesignToken and DefaultThemeVariantCalculator.
- Produces: an AtomUI.Core.Tests assembly with access to AtomUI.Core internals.

- [ ] **Step 1: Add the test project and internals access**

~~~xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <RootNamespace>AtomUI.Core.Tests</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="Shouldly" />
    <PackageReference Include="xunit.v3" />
    <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/AtomUI.Core/AtomUI.Core.csproj" />
  </ItemGroup>
</Project>
~~~

Add InternalsVisibleTo for AtomUI.Core.Tests and add the project to AtomUI.slnx.

- [ ] **Step 2: Lock current default derivation**

~~~csharp
[Fact]
public void DefaultCalculator_Derives_Stable_Primary_And_Control_Size_Tokens()
{
    var token = new DesignToken();
    var calculator = new DefaultThemeVariantCalculator();

    calculator.Calculate(token);
    token.CalculateAliasTokenValues();

    token.ColorPrimary.ShouldBe(Color.Parse("#1677ff"));
    token.ControlHeight.ShouldBe(32);
    token.BorderRadius.ShouldBe(new CornerRadius(6));
}
~~~

- [ ] **Step 3: Run the new project**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore

Expected: PASS with one test and zero failures.

- [ ] **Step 4: Commit**

~~~bash
git add AtomUI.slnx src/AtomUI.Core/AtomUI.Core.csproj tests/AtomUI.Core.Tests
git commit -m "test(Theme): add core theme test project"
~~~

### Task 2: Replace the Cursor-Based XML Reader with a Pure Parser

**Files:**
- Create: src/AtomUI.Core/Theme/Definitions/ThemeDefinitionDiagnostic.cs
- Create: src/AtomUI.Core/Theme/Definitions/ThemeDefinitionParseResult.cs
- Create: src/AtomUI.Core/Theme/Definitions/ThemeDefinitionParser.cs
- Modify: src/AtomUI.Core/Theme/ThemeDefinition.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeDefinitionParserTests.cs

**Interfaces:**
- Consumes: ThemeDefinitionParseRequest containing a Stream, file path, shared Token schema and registered component schema.
- Produces: ThemeDefinitionParseResult ThemeDefinitionParser.Parse(ThemeDefinitionParseRequest request).

- [ ] **Step 1: Write whitespace-independent parser tests**

~~~csharp
[Theory]
[InlineData("<Theme Name='T' IsDefault='true'><Algorithms>Default,Dark</Algorithms><SharedTokens><Token Name='ColorPrimary' Value='#ff0000'/></SharedTokens><ControlTokens/></Theme>")]
[InlineData("""
<Theme Name="T" IsDefault="true">
  <Algorithms>Default, Dark</Algorithms>
  <SharedTokens>
    <Token Name="ColorPrimary">#ff0000</Token>
  </SharedTokens>
  <ControlTokens />
</Theme>
""")]
public void Parse_Is_Independent_Of_Formatting(string xml)
{
    var result = Parse(xml);

    result.Success.ShouldBeTrue();
    result.Definition!.Algorithms.ShouldBe(
        [ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);
    result.Definition.SharedTokens["ColorPrimary"].ShouldBe("#ff0000");
}
~~~

Add focused tests for body-valued IsShared, self-closing containers, duplicate keys, missing root, invalid bool, unknown algorithm, unknown global Token, known component with an unknown property, optional unregistered component warning, and line and column diagnostics.

- [ ] **Step 2: Verify the new tests fail**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeDefinitionParserTests

Expected: FAIL because the parser contracts do not exist.

- [ ] **Step 3: Add immutable diagnostic contracts**

~~~csharp
internal enum ThemeDiagnosticSeverity
{
    Warning,
    Error
}

internal sealed record ThemeDefinitionDiagnostic(
    string Code,
    ThemeDiagnosticSeverity Severity,
    string FilePath,
    int Line,
    int Column,
    string Path,
    string Message);

internal sealed record ThemeDefinitionParseResult(
    ThemeDefinition? Definition,
    IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics)
{
    public bool Success =>
        Definition is not null &&
        Diagnostics.All(x => x.Severity != ThemeDiagnosticSeverity.Error);
}
~~~

Make ThemeDefinition constructor-only and expose ordered algorithms plus copied read-only dictionaries.

- [ ] **Step 4: Implement secured XDocument parsing**

~~~csharp
var settings = new XmlReaderSettings
{
    CloseInput = false,
    DtdProcessing = DtdProcessing.Prohibit,
    XmlResolver = null
};

using var reader = XmlReader.Create(request.Stream, settings, request.FilePath);
var document = XDocument.Load(reader, LoadOptions.SetLineInfo);
~~~

Read attributes before content. Require exactly one Token value source. Convert every structural failure into stable diagnostics ATMTHM001 through ATMTHM010.

- [ ] **Step 5: Run all Core tests**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore

Expected: PASS.

- [ ] **Step 6: Commit without deleting the legacy reader**

~~~bash
git add src/AtomUI.Core/Theme/Definitions src/AtomUI.Core/Theme/ThemeDefinition.cs tests/AtomUI.Core.Tests/Theme
git commit -m "refactor(Theme): add deterministic theme definition parser"
~~~

### Task 3: Make Token Conversion Deterministic

**Files:**
- Modify: src/AtomUI.Core/Theme/TokenSystem/BuiltInTokenValueConverters.cs
- Modify: src/AtomUI.Core/Theme/TokenSystem/AbstractDesignToken.cs
- Test: tests/AtomUI.Core.Tests/Theme/TokenValueConverterTests.cs

**Interfaces:**
- Consumes: string Token values.
- Produces: invariant typed values and cache-consistent LoadConfig behavior.

- [ ] **Step 1: Write culture and cache tests**

~~~csharp
[Theory]
[InlineData("fr-FR")]
[InlineData("zh-CN")]
[InlineData("en-US")]
public void Double_Conversion_Uses_Invariant_Culture(string cultureName)
{
    using var culture = new CultureScope(cultureName);
    new DoubleTokenValueConverter().Convert("1.5").ShouldBe(1.5d);
}

[Fact]
public void LoadConfig_Invalidates_Previously_Read_Token_Value()
{
    var token = new DesignToken();
    token.GetTokenValue(nameof(DesignToken.BorderRadius));

    token.LoadConfig(new Dictionary<string, string>
    {
        [nameof(DesignToken.BorderRadius)] = "12"
    });

    token.GetTokenValue(nameof(DesignToken.BorderRadius))
         .ShouldBe(new CornerRadius(12));
}
~~~

- [ ] **Step 2: Verify failures under fr-FR and cached access**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~TokenValueConverterTests

Expected: the decimal and cache tests fail.

- [ ] **Step 3: Implement invariant conversion and invalidation**

Use NumberStyles.Integer or NumberStyles.Float with CultureInfo.InvariantCulture. Remove each assigned Token name from the access cache before setting the property. Include Token name, target type and raw value in conversion errors.

- [ ] **Step 4: Run and commit**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore

Expected: PASS.

~~~bash
git add src/AtomUI.Core/Theme/TokenSystem tests/AtomUI.Core.Tests/Theme/TokenValueConverterTests.cs
git commit -m "fix(Theme): make token conversion deterministic"
~~~

### Task 4: Introduce ThemeSnapshot and the Single ThemeCompiler

**Files:**
- Create: src/AtomUI.Core/Theme/Resources/ComponentTokenIdentity.cs
- Create: src/AtomUI.Core/Theme/Compilation/ThemeCompileRequest.cs
- Create: src/AtomUI.Core/Theme/Compilation/ThemeCompileResult.cs
- Create: src/AtomUI.Core/Theme/Compilation/ThemeSnapshot.cs
- Create: src/AtomUI.Core/Theme/Compilation/ComponentThemeSnapshot.cs
- Create: src/AtomUI.Core/Theme/Compilation/ThemeCompiler.cs
- Modify: src/AtomUI.Core/Theme/ControlTokenRegistration.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeCompilerTests.cs

**Interfaces:**
- Consumes: ThemeCompileRequest with definition, parent, algorithms, overrides, runtime overrides and registrations.
- Produces: ThemeCompileResult with one complete ThemeSnapshot or diagnostics and exception.

- [ ] **Step 1: Write global and component compiler tests**

~~~csharp
[Fact]
public void Component_Algorithm_False_Does_Not_Derive_Map_Tokens()
{
    var result = Compile(component: Component(
        "Button",
        algorithm: false,
        shared: Token("ColorPrimary", "#00b96b")));

    var button = result.Snapshot!.Components[Identity("Button")];

    button.EffectiveSharedToken.ColorPrimary
          .ShouldBe(Color.Parse("#00b96b"));
    button.EffectiveSharedToken.ColorPrimaryHover
          .ShouldBe(result.Snapshot.SharedToken.ColorPrimaryHover);
}

[Fact]
public void Component_Algorithm_True_Derives_Private_Map_Tokens()
{
    var result = Compile(component: Component(
        "Button",
        algorithm: true,
        shared: Token("ColorPrimary", "#00b96b")));

    var button = result.Snapshot!.Components[Identity("Button")];

    button.EffectiveSharedToken.ColorPrimary
          .ShouldBe(Color.Parse("#00b96b"));
    button.EffectiveSharedToken.ColorPrimaryHover
          .ShouldNotBe(result.Snapshot.SharedToken.ColorPrimaryHover);
}
~~~

Also test ordered Dark and Compact composition, parent immutability, Map and Alias override order, component own Token override, unknown registration diagnostics, and failure without a partial snapshot.

- [ ] **Step 2: Verify missing-type failures**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeCompilerTests

Expected: FAIL.

- [ ] **Step 3: Add compiler contracts**

~~~csharp
internal sealed record ThemeCompileRequest(
    string ThemeId,
    ThemeDefinition Definition,
    ThemeSnapshot? Parent,
    IReadOnlyList<ThemeAlgorithm> Algorithms,
    IReadOnlyDictionary<string, string> SharedOverrides,
    IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> ComponentOverrides,
    IReadOnlyList<ControlTokenRegistration> Registrations,
    IReadOnlyDictionary<string, string> RuntimeOverrides);

internal sealed record ThemeCompileResult(
    ThemeSnapshot? Snapshot,
    IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics,
    Exception? Exception)
{
    public bool Success => Snapshot is not null && Exception is null;
}
~~~

- [ ] **Step 4: Extract one deterministic compile pipeline**

Create a fresh DesignToken and fresh ControlToken graph for every uncached compile. Apply Seed, algorithms, Map, Alias and component calculations in the order fixed by the design. Build resource maps off-side. Publish only when every component succeeds.

Do not change Theme or ThemeConfigProvider yet.

- [ ] **Step 5: Run compiler tests twice**

Run the compiler-filtered test command twice. Expected: both runs PASS with identical results.

- [ ] **Step 6: Commit**

~~~bash
git add src/AtomUI.Core/Theme/Compilation src/AtomUI.Core/Theme/ControlTokenRegistration.cs tests/AtomUI.Core.Tests/Theme/ThemeCompilerTests.cs
git commit -m "refactor(Theme): centralize token compilation in snapshots"
~~~

### Task 5: Parse Definitions Once Through ThemeCatalog

**Files:**
- Create: src/AtomUI.Core/Theme/Catalog/ThemeDescriptor.cs
- Create: src/AtomUI.Core/Theme/Catalog/ThemeCatalog.cs
- Modify: src/AtomUI.Core/Theme/ThemeManager.cs
- Modify: src/AtomUI.Core/Theme/Theme.cs
- Remove: src/AtomUI.Core/Theme/ThemeDefinitionReader.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeCatalogTests.cs

**Interfaces:**
- Consumes: ordered theme sources, parser and registered Token schema.
- Produces: one ThemeDescriptor per theme id and compile requests for variants.

- [ ] **Step 1: Write precedence and parse-once tests**

~~~csharp
[Fact]
public void Catalog_Parses_One_File_Once_For_All_Variants()
{
    var source = new CountingThemeSource("Brand.xml", ValidThemeXml);
    var catalog = CreateCatalog(source);

    catalog.GetDescriptor("Brand").ShouldNotBeNull();
    catalog.CreateCompileRequest(
        "Brand",
        [ThemeAlgorithm.Default]);
    catalog.CreateCompileRequest(
        "Brand",
        [ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);

    source.OpenCount.ShouldBe(1);
}
~~~

Add tests for custom precedence, duplicate id diagnostics, explicit builder default, IsDefault fallback, unavailable invalid custom theme, and fatal invalid built-in default.

- [ ] **Step 2: Verify failure**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeCatalogTests

Expected: FAIL.

- [ ] **Step 3: Implement ThemeDescriptor and ThemeCatalog**

~~~csharp
internal sealed record ThemeDescriptor(
    string Id,
    string DefinitionFilePath,
    bool IsBuiltIn,
    int SourcePriority,
    ThemeDefinition? Definition,
    IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics)
{
    public bool IsAvailable =>
        Definition is not null &&
        Diagnostics.All(x => x.Severity != ThemeDiagnosticSeverity.Error);
}
~~~

Sort paths ordinally. Record duplicates instead of silently continuing.

- [ ] **Step 4: Adapt Theme as a compatibility facade**

Theme receives a descriptor, asks ThemeCompiler for a snapshot through ThemeCatalog, and keeps the current activation facade temporarily. It no longer opens a file or owns Seed, Map, Alias, or component compile loops. Delete ThemeDefinitionReader only after the following search returns no consumers:

Run: rg "ThemeDefinitionReader" src tests

Expected before deletion: only the reader definition remains.

- [ ] **Step 5: Run Core and initial-mode tests**

Run:
- dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
- dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeInitialModeTests

Expected: both PASS.

- [ ] **Step 6: Commit**

~~~bash
git add src/AtomUI.Core/Theme/Catalog src/AtomUI.Core/Theme/Theme.cs src/AtomUI.Core/Theme/ThemeManager.cs src/AtomUI.Core/Theme/ThemeDefinitionReader.cs tests/AtomUI.Core.Tests/Theme/ThemeCatalogTests.cs
git commit -m "refactor(Theme): parse and catalog definitions once"
~~~

### Task 6: Add Snapshot-Backed ResourceProvider and Component Keys

**Files:**
- Modify: src/AtomUI.Core/Theme/Resources/ComponentTokenIdentity.cs
- Create: src/AtomUI.Core/Theme/Resources/ComponentSharedTokenResourceKey.cs
- Create: src/AtomUI.Core/Theme/Resources/ComponentSharedTokenResourceExtension.cs
- Create: src/AtomUI.Core/Theme/Resources/ThemeTokenResourceProvider.cs
- Modify: src/AtomUI.Core/Theme/Compilation/ThemeSnapshot.cs
- Modify: src/AtomUI.Core/Theme/Compilation/ComponentThemeSnapshot.cs
- Modify: src/AtomUI.Core/Theme/Theme.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeTokenResourceProviderTests.cs

**Interfaces:**
- Consumes: ThemeSnapshot.
- Produces: ResourceProvider resolving global, component-private shared, and component own Token keys.

- [ ] **Step 1: Write lookup and notification tests**

~~~csharp
[Fact]
public void ComponentShared_Key_Resolves_Private_Value_Without_Changing_Global()
{
    var snapshot = CompileButtonPrimary("#00b96b");
    var provider = new ThemeTokenResourceProvider(snapshot);

    provider.TryGetResource(
        SharedTokenKind.ColorPrimary,
        null,
        out var global).ShouldBeTrue();

    provider.TryGetResource(
        new ComponentSharedTokenResourceKey(
            null,
            "Button",
            SharedTokenKind.ColorPrimary),
        null,
        out var component).ShouldBeTrue();

    component.ShouldNotBe(global);
}
~~~

Also test unknown ids, registered unconfigured fallback to scope-global values, catalog identity, owner attachment, silent prepare followed by one publish notification, and normal ReplaceSnapshot notification.

- [ ] **Step 2: Verify failure**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeTokenResourceProviderTests

Expected: FAIL.

- [ ] **Step 3: Implement stable identities and cached keys**

~~~csharp
public readonly record struct ComponentSharedTokenResourceKey(
    string? Catalog,
    string ComponentId,
    SharedTokenKind Kind);
~~~

Normalize empty Catalog to null. Cache keys by ComponentTokenIdentity and SharedTokenKind. ComponentTokenIdentity is an internal compiler value; generated public extensions pass catalog and id strings to their public base constructor.

- [ ] **Step 4: Implement ThemeTokenResourceProvider**

Subclass Avalonia ResourceProvider. Resolve prebuilt maps without reflection. Provide PrepareSnapshot for a transaction owner to swap the reference silently and PublishSnapshotChanged to raise exactly one notification after the surrounding scope state is consistent. ReplaceSnapshot combines those two operations for simple callers. Mount one provider in every compatibility Theme resource layer so old ThemeManager activation can resolve component keys before ThemeCoordinator lands.

- [ ] **Step 5: Run provider and compiler tests**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter "FullyQualifiedName~ThemeTokenResourceProviderTests|FullyQualifiedName~ThemeCompilerTests"

Expected: PASS.

- [ ] **Step 6: Commit**

~~~bash
git add src/AtomUI.Core/Theme/Resources src/AtomUI.Core/Theme/Compilation tests/AtomUI.Core.Tests/Theme/ThemeTokenResourceProviderTests.cs
git commit -m "feat(Theme): add snapshot token resource provider"
~~~

### Task 7: Generate Strongly Typed Component-Shared Extensions

**Files:**
- Modify: src/AtomUI.Generator/DesignToken/TokenInfo.cs
- Modify: src/AtomUI.Generator/DesignToken/ControlTokenPropertyWalker.cs
- Modify: src/AtomUI.Generator/DesignToken/ResourceKeyClassWriter.cs
- Modify: src/AtomUI.Generator/DesignToken/ControlTokenTypePoolClassWriter.cs
- Modify: src/AtomUI.Core/Theme/ControlTokenRegistration.cs
- Test: tests/AtomUI.Generator.Tests/TokenResourceKeyGeneratorTests.cs

**Interfaces:**
- Consumes: ControlDesignToken class with public const string ID.
- Produces: registration identity and ControlNameSharedTokenResourceExtension.

- [ ] **Step 1: Add generator output tests**

Input:

~~~csharp
[ControlDesignToken]
internal sealed class ButtonToken : AbstractControlDesignToken
{
    public const string ID = "Button";
    public ButtonToken() : base(ID) { }
    public double Height { get; set; }
}
~~~

Expected generated API:

~~~csharp
public sealed class ButtonTokenSharedTokenResourceExtension
    : ComponentSharedTokenResourceExtension
{
    public ButtonTokenSharedTokenResourceExtension(SharedTokenKind kind)
        : base(null, "Button", kind)
    {
    }
}
~~~

Also assert a generator diagnostic when ID is missing or non-constant.

- [ ] **Step 2: Verify generator failure**

Run: dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~TokenResourceKeyGeneratorTests

Expected: FAIL.

- [ ] **Step 3: Extend generator metadata**

Add ControlId, ResourceCatalog and generated TokenKind type to ControlTokenInfo. Emit ControlTokenRegistration with identity metadata rather than creating a Token instance to discover its id.

- [ ] **Step 4: Emit component-shared markup extensions**

Keep existing component own Token extensions. Emit one additional SharedTokenKind extension per registered ControlToken.

- [ ] **Step 5: Verify generator and consumers**

Run:
- dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore
- dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj --framework net10.0 --no-restore

Expected: PASS and successful build.

- [ ] **Step 6: Commit**

~~~bash
git add src/AtomUI.Generator src/AtomUI.Core/Theme/ControlTokenRegistration.cs tests/AtomUI.Generator.Tests
git commit -m "feat(Generator): emit component shared token resources"
~~~

### Task 8: Rebuild ThemeConfigProvider as an Inheriting Snapshot Scope

**Files:**
- Create: src/AtomUI.Core/Theme/Scope/ThemeScope.cs
- Modify: src/AtomUI.Core/Theme/ThemeConfigProvider.cs
- Modify: src/AtomUI.Core/Theme/IThemeConfigProvider.cs
- Modify: src/AtomUI.Core/Theme/TokenSetter.cs
- Modify: src/AtomUI.Core/Theme/ControlTokenInfoSetter.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeConfigProviderTests.cs

**Interfaces:**
- Consumes: inherited parent Snapshot, observable local config and ThemeCompiler.
- Produces: one child Snapshot, one ResourceProvider, and inherited context on Content.

- [ ] **Step 1: Write nested inheritance and update tests**

~~~csharp
[Fact]
public void Child_Provider_Inherits_Unchanged_Parent_Tokens()
{
    var parent = Provider(
        Token("ColorPrimary", "#ff0000"),
        Token("BorderRadius", "12"));
    var child = Provider(
        Token("ColorPrimary", "#00b96b"));

    parent.Content = child;
    child.Content = new Border();
    Attach(parent);

    child.SharedToken.ColorPrimary
         .ShouldBe(Color.Parse("#00b96b"));
    child.SharedToken.BorderRadius
         .ShouldBe(new CornerRadius(12));
}
~~~

Add tests for Inherit false, parent updates, list add and remove, TokenSetter Value changes, algorithm changes, failed compile rollback, one merged provider after 20 updates, and Content replacement cleanup.

- [ ] **Step 2: Verify current failures**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeConfigProviderTests

Expected: failures for inheritance, change tracking, resource count and rollback.

- [ ] **Step 3: Define ThemeScope and observable config**

~~~csharp
internal static class ThemeScope
{
    public static readonly AttachedProperty<ThemeSnapshot?> SnapshotProperty =
        AvaloniaProperty.RegisterAttached<
            ThemeConfigProvider,
            StyledElement,
            ThemeSnapshot?>(
                "Snapshot",
                inherits: true);
}
~~~

Convert TokenSetter and ControlTokenInfoSetter to AvaloniaObject properties. Use AvaloniaList collections and add InheritProperty with default true while preserving AXAML content syntax.

- [ ] **Step 4: Replace CalculateTokenResources with compile and commit**

The provider inherits the parent Snapshot on itself and compiles a child Snapshot off-side. On success:

1. replace the single ThemeTokenResourceProvider Snapshot without notification;
2. set the same Snapshot on Content through ThemeScope;
3. raise one ResourcesChanged notification;
4. publish compatibility SharedToken and ControlTokens views.

On failure, retain every previously published object and resource and raise ThemeScopeCompileFailed with diagnostics.

- [ ] **Step 5: Coalesce runtime changes**

Queue at most one Dispatcher.UIThread recompile per event-loop turn. Detach collection, item and parent handlers symmetrically on Content replacement and logical detach.

- [ ] **Step 6: Verify compatibility**

Run:
- dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
- dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~Button_Primary_Variant_Background_Follows_Scoped_ColorPrimary

Expected: PASS.

- [ ] **Step 7: Commit**

~~~bash
git add src/AtomUI.Core/Theme/Scope src/AtomUI.Core/Theme/ThemeConfigProvider.cs src/AtomUI.Core/Theme/IThemeConfigProvider.cs src/AtomUI.Core/Theme/TokenSetter.cs src/AtomUI.Core/Theme/ControlTokenInfoSetter.cs tests/AtomUI.Core.Tests/Theme/ThemeConfigProviderTests.cs
git commit -m "refactor(Theme): make config provider inherit snapshots"
~~~

### Task 9: Prove Component Isolation with Button

**Files:**
- Modify: src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml
- Modify: src/AtomUI.Desktop.Controls/Buttons/Button.cs
- Modify: src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs
- Test: tests/AtomUI.Desktop.Controls.Tests/Buttons/ButtonThemeScopeTests.cs

**Interfaces:**
- Consumes: ButtonTokenSharedTokenResourceExtension and scoped Snapshot.
- Produces: first component with Ant Design isolation and no instance scope host.

- [ ] **Step 1: Write component isolation tests**

~~~csharp
[Fact]
public void Button_Component_ColorPrimary_Does_Not_Leak_To_Content()
{
    var content = new Border();
    content[!Border.BackgroundProperty] =
        new DynamicResourceExtension(SharedTokenKind.ColorPrimary);

    var button = new Button { Content = content };
    var provider = Provider(
        component: Component(
            "Button",
            shared: Token("ColorPrimary", "#00b96b")),
        content: button);

    ShowInWindow(provider, () =>
    {
        BrushColor(button.Background)
            .ShouldBe(Color.Parse("#00b96b"));
        BrushColor(content.Background)
            .ShouldBe(GlobalColorPrimary);
    });
}
~~~

Add tests for a nested Input using Input config, runtime Button component updates without reattach, and global theme switch refresh.

- [ ] **Step 2: Verify leakage or staleness**

Run: dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ButtonThemeScopeTests

Expected: current Host behavior fails at least one assertion.

- [ ] **Step 3: Migrate ButtonTheme references**

Replace component-semantic SharedTokenResource usages with the generated Button component-shared extension. Inventory every SharedTokenResourceValue separately: use a component-aware DynamicResource when the target is a StyledElement; for non-Visual transition or animation objects, project the Token onto an owning control property with an explicit lifecycle instead of attaching DynamicResource to the non-Visual object. Leave genuinely application-global non-design resources unchanged.

- [ ] **Step 4: Remove Button instance scope registration**

Remove RegisterTokenResourceScope from Button and remove ButtonToken.ScopeProvider. Keep shared Host infrastructure for unconverted controls.

- [ ] **Step 5: Run all Button and scope tests**

Run: dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter "FullyQualifiedName~Button|FullyQualifiedName~ThemeConfigProvider"

Expected: PASS.

- [ ] **Step 6: Commit**

~~~bash
git add src/AtomUI.Desktop.Controls/Buttons tests/AtomUI.Desktop.Controls.Tests/Buttons/ButtonThemeScopeTests.cs
git commit -m "refactor(Button): isolate component theme tokens"
~~~

### Task 10: Replace Global Activation with ThemeCoordinator

**Files:**
- Create: src/AtomUI.Core/Theme/Transitions/ThemeRequest.cs
- Create: src/AtomUI.Core/Theme/Transitions/ThemeTransitionEventArgs.cs
- Create: src/AtomUI.Core/Theme/Transitions/ThemeCoordinator.cs
- Modify: src/AtomUI.Core/Theme/ThemeManager.cs
- Modify: src/AtomUI.Core/ApplicationExtensions.cs
- Modify: src/AtomUI.Controls.Shared/ApplicationExtensions.cs
- Modify: src/AtomUI.Desktop.Controls/Window/MediaBreakPointThemeBootstrapper.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeCoordinatorTests.cs

**Interfaces:**
- Consumes: ThemeRequest, Catalog, Compiler or cache, Application and the global resource layer.
- Produces: serialized atomic transitions and committed events.

- [ ] **Step 1: Write transition tests**

~~~csharp
[Fact]
public void Dark_Compact_Startup_Commits_Exactly_Once()
{
    var coordinator = CreateCoordinator();
    var changed = new List<ThemeTransitionEventArgs>();
    coordinator.ThemeChanged += (_, args) => changed.Add(args);

    coordinator.Request(new ThemeRequest(
        "DaybreakBlue",
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ],
        ThemeTransitionReason.Startup));

    changed.Count.ShouldBe(1);
    changed[0].NewSnapshot.Algorithms.ShouldBe(
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ]);
}
~~~

Add tests for identical no-op requests, queued requests from Changed subscribers, parser and compiler failure rollback, observer exception isolation, and UI-thread verification.

- [ ] **Step 2: Verify failure**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeCoordinatorTests

Expected: FAIL.

- [ ] **Step 3: Implement one request model and state machine**

~~~csharp
internal sealed record ThemeRequest(
    string ThemeId,
    IReadOnlyList<ThemeAlgorithm> Algorithms,
    ThemeTransitionReason Reason,
    IReadOnlyDictionary<string, string>? RuntimeOverrides = null);
~~~

Use Idle, Preparing and Committing states with a FIFO queue. Normalize and compare requests before prepare. Build the target Snapshot before touching Application or manager properties.

- [ ] **Step 4: Implement guarded commit order**

Commit in this order:

1. prepare or replace the target global ResourceProvider;
2. assign active Snapshot and derived manager state under a guard;
3. set Application.RequestedThemeVariant;
4. acknowledge the synchronous ActualThemeVariant callback without creating a request;
5. raise Changed after all state and resources agree;
6. drain the queued request.

Observer failures are logged separately and never converted to ThemeChangeFailed.

- [ ] **Step 5: Route public extensions through complete requests**

SetDarkThemeMode and SetCompactThemeMode create requests using both desired flags. Move default font into compile overrides. Migrate MediaBreakPoint bootstrap to observe committed Snapshots.

- [ ] **Step 6: Remove recursive and fake lifecycle paths**

Remove the second ConfigureThemeVariant call, bool-property command recursion, same-theme activation events, fake unload events, and ThemeLoaded mutation hooks after consumers migrate.

- [ ] **Step 7: Verify transition behavior**

Run:
- dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
- dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeInitialModeTests
- dotnet test tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~GalleryCodeViewer_Updates_TextMate_Theme

Expected: PASS and one commit per request.

- [ ] **Step 8: Commit**

~~~bash
git add src/AtomUI.Core/Theme/Transitions src/AtomUI.Core/Theme/ThemeManager.cs src/AtomUI.Core/ApplicationExtensions.cs src/AtomUI.Controls.Shared/ApplicationExtensions.cs src/AtomUI.Desktop.Controls/Window/MediaBreakPointThemeBootstrapper.cs tests/AtomUI.Core.Tests/Theme/ThemeCoordinatorTests.cs
git commit -m "refactor(Theme): serialize atomic theme transitions"
~~~

### Task 11: Migrate Remaining Components and Delete ScopeHost

**Files:**
- Modify: AXAML inventory produced by the SharedTokenResource search under src/AtomUI.Controls and all desktop control packages.
- Modify: C# inventory produced by the RegisterTokenResourceScope search under src.
- Modify: ControlToken inventory produced by the ScopeProvider search under src.
- Remove: src/AtomUI.Core/Theme/ControlTokenResourcesHost.cs
- Remove: src/AtomUI.Core/Theme/IControlTokenResourceScopeProvider.cs
- Modify: src/AtomUI.Core/Data/TokenFinderUtils.cs
- Modify: src/AtomUI.Core/Data/TokenResourceUtils.cs
- Test: tests/AtomUI.Desktop.Controls.Tests/Theme/ComponentThemeIsolationTests.cs
- Test: existing ThemeContract tests under tests/AtomUI.Desktop.Controls.Tests

**Interfaces:**
- Consumes: generated component-shared extensions and ThemeScope.
- Produces: repository-wide component isolation with zero instance Token dictionaries.

- [ ] **Step 1: Capture and classify a checked inventory**

Run:

~~~bash
rg -l "RegisterTokenResourceScope" src --glob "*.cs" | sort
rg -l "SharedTokenResource" src --glob "*.axaml" | sort
rg -l "SharedTokenResourceValue" src --glob "*.axaml" | sort
rg -l "ScopeProvider" src --glob "*Token.cs" | sort
~~~

Classify every result as component-style consumption, global-style consumption, static non-Visual consumption, or obsolete registration. Any non-Visual DynamicResource replacement must have a scoped owner and verified release path; do not use a blind repository-wide replacement.

- [ ] **Step 2: Migrate small packages first**

Migrate src/AtomUI.Controls, DataGrid, ColorPicker and Extras. For each identity, replace component-style shared references, remove constructor registration and remove ScopeProvider.

Run:

~~~bash
dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeContract
~~~

Expected: PASS.

- [ ] **Step 3: Migrate the main desktop package by domain**

Use separate reviewed commits for General and Layout, Navigation, DataEntry, DataDisplay, Feedback, and Window and Overlay. Each commit removes every Host registration and ScopeProvider for its domain and runs its ThemeContract and Token tests.

- [ ] **Step 4: Add cross-component isolation tests**

Cover Button containing Icon, LineEdit containing AddOnDecoratedBox, Select popup content, DatePicker popup content, Modal content and DataGrid cell content. Parent component overrides must not alter child component Token; scope-global overrides must affect both.

- [ ] **Step 5: Delete legacy infrastructure only after zero references**

Run:

~~~bash
rg "RegisterTokenResourceScope|ControlTokenResourceScopeHost|ScopeProvider" src tests
~~~

Expected: no matches outside migration documentation.

Delete the Host, interface, attached properties and dead catalog parameter.

- [ ] **Step 6: Run control suites**

Run:
- dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --framework net10.0 --no-restore
- dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
- dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --framework net10.0 --no-restore

Expected: PASS.

- [ ] **Step 7: Commit final removal**

~~~bash
git add src tests/AtomUI.Desktop.Controls.Tests/Theme
git commit -m "refactor(Theme)!: remove per-control token resource scopes"
~~~

### Task 12: Add Snapshot Caching and Remove Mutable Compile Paths

**Files:**
- Create: src/AtomUI.Core/Theme/Compilation/ThemeSnapshotCache.cs
- Modify: src/AtomUI.Core/Theme/Compilation/ThemeCompileRequest.cs
- Modify: src/AtomUI.Core/Theme/ThemeManager.cs
- Modify: src/AtomUI.Core/Theme/ThemeConfigProvider.cs
- Modify: src/AtomUI.Core/Theme/Theme.cs
- Test: tests/AtomUI.Core.Tests/Theme/ThemeSnapshotCacheTests.cs

**Interfaces:**
- Consumes: normalized compile request identity.
- Produces: bounded shared Snapshot cache without sharing owner-bound ResourceProvider instances.

- [ ] **Step 1: Write identity and eviction tests**

Assert equal definition revision, parent version, ordered algorithms, overrides and registration version return the same Snapshot reference. Algorithm order or any override difference returns a different Snapshot. Provider instances remain distinct. Active Snapshots cannot be evicted.

- [ ] **Step 2: Verify failure**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~ThemeSnapshotCacheTests

Expected: FAIL.

- [ ] **Step 3: Implement a bounded normalized cache**

Use a value key containing definition revision, parent Snapshot version, ordered algorithm ids, sorted override pairs and registration version. Limit inactive entries to 32 and pin active Snapshot references. Do not add lazy component compilation in this task.

- [ ] **Step 4: Remove duplicate mutable loops**

Verify Theme and ThemeConfigProvider contain no Seed, Map or Alias compile loops, Activator-based metadata lookup, or direct token ResourceDictionary construction.

Run:

~~~bash
rg "TokenConfigBuckets|CalculateAliasTokenValues|Activator.CreateInstance" src/AtomUI.Core/Theme/Theme.cs src/AtomUI.Core/Theme/ThemeConfigProvider.cs
~~~

Expected: no matches.

- [ ] **Step 5: Add allocation and count assertions**

Twenty identical ThemeConfigProviders must share one Snapshot, own twenty ResourceProviders, and retain one provider layer each. One hundred identical global requests must compile once and emit one Changed event.

- [ ] **Step 6: Run and commit**

Run: dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore

Expected: PASS.

~~~bash
git add src/AtomUI.Core/Theme tests/AtomUI.Core.Tests/Theme/ThemeSnapshotCacheTests.cs
git commit -m "perf(Theme): cache compiled theme snapshots"
~~~

### Task 13: Documentation, Lifecycle and Release Validation

**Files:**
- Modify: docs/architecture/startup-and-registration.md
- Modify: docs/modules/core/overview.md
- Create: docs/modules/core/theme-system.md
- Modify: CustomizeTheme localization files under controlgallery/AtomUIGallery/ShowCases/General/CustomizeTheme/Localization
- Modify: tests/AtomUIGallery.Tests/ShowCases/CustomizeThemeShowCasePageTests.cs

**Interfaces:**
- Consumes: completed snapshot architecture.
- Produces: maintained docs, Gallery contracts and release evidence.

- [ ] **Step 1: Document final runtime flow**

Document startup request, prepare and commit order, Snapshot contents, nested merge, component-private keys, event order, failure behavior and extension points.

- [ ] **Step 2: Update Gallery examples**

Add examples for nested inheritance, Inherit false, runtime Setter changes, component algorithm true and false, and isolation from Content.

- [ ] **Step 3: Run lifecycle checks**

Verify old ShowCases, ThemeConfigProviders, ResourceProviders and DynamicResourceExpression instances do not grow monotonically after Gallery navigation. Verify Popup and Flyout content follow the owning scope.

- [ ] **Step 4: Run target test projects**

~~~bash
dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
~~~

Expected: all commands exit 0.

- [ ] **Step 5: Build both release targets**

Run: dotnet build AtomUI.slnx --configuration Release --no-restore

Expected: net8.0 and net10.0 builds succeed without new warnings.

- [ ] **Step 6: Publish NativeAOT Gallery**

~~~bash
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 -publishRootPath /tmp/atomui-gallery-theme-aot -runtime osx-arm64 -buildType Release -publishAot true
~~~

Expected: publish exits 0. Launch the result and verify global Dark and Compact, nested scopes, component isolation, Popup content and runtime updates.

- [ ] **Step 7: Run hygiene checks**

~~~bash
rg "ThemeDefinitionReader|ControlTokenResourcesScopeHostExtensions|RegisterTokenResourceScope|ThemeAboutToUnload|ThemeUnloaded" src tests
git diff --check
~~~

Expected: no production legacy references and a clean diff check.

- [ ] **Step 8: Commit docs and validation updates**

~~~bash
git add docs controlgallery/AtomUIGallery/ShowCases/General/CustomizeTheme tests/AtomUIGallery.Tests/ShowCases/CustomizeThemeShowCasePageTests.cs
git commit -m "docs(Theme): document snapshot theme architecture"
~~~

---

## Review Gates

1. **Foundation gate after Task 5:** Parser, Compiler and Catalog are pure and tested. Do not migrate behavior if old and new default Token values differ.
2. **Architecture gate after Task 9:** Button proves component isolation, runtime refresh and zero per-instance Token dictionaries.
3. **Transition gate after Task 10:** Dark and Compact, no-op, failure rollback and reentry tests pass before broad migration.
4. **Removal gate during Task 11:** Delete ScopeHost only after repository searches show zero consumers.
5. **Release gate after Task 13:** Target tests, dual-target Release build, NativeAOT publish, lifecycle checks and hygiene all pass.

## Explicit Non-Goals

- Changing Ant Design Token values or palette algorithms.
- Replacing Avalonia ControlTheme and AXAML with generated runtime styles.
- Adding third-party serialization, dependency injection or caching packages.
- Adding theme-file watching or live hot reload.
- Adding lazy per-component compilation before profiling proves it is needed.
- Preserving accidental component override leakage into arbitrary Content descendants.
