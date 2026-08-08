# Localization Generator-First Build Design

## Goal

Move as much localization semantics as possible from `build/AtomUI.Localization.props` and
`build/AtomUI.Localization.targets` into `AtomUI.Generator`, while keeping the MSBuild files
small and preserving the existing conceptual workflow: local XLIFF inputs are compiled into
static runtime data, language-pack projects produce declarative package assets, project
references can provide source contracts, and template export remains an MSBuild command.

This is an intentional build-contract redesign. Existing MSBuild properties and metadata may
be removed or renamed; source compatibility with the current targets is not a requirement.

## Non-goals

- Do not make runtime localization parse XLIFF or inspect assemblies dynamically.
- Do not make the source generator write arbitrary files into `obj`, `bin`, or the NuGet package.
- Do not replace MSBuild's project-to-project target protocol with reflection, filesystem probing,
  or package-directory scanning.
- Do not change the generated runtime registration model beyond what is needed to move semantic
  validation into the generator.

## Responsibilities

### `AtomUI.Generator`

The generator becomes the single semantic authority for XLIFF inputs consumed by a compiling
project. It owns:

- XLIFF 2.1 parsing and source locations for diagnostics.
- Language-tag, translation-state, placeholder, source-text, and complete-bundle validation.
- Catalog/module resolution against current and referenced Roslyn symbols.
- `Verified` and `Deferred` contract handling, including deferred activation when a referenced
  module becomes available.
- Source-fingerprint and contract-version validation.
- Duplicate translation-source and override-unit conflict detection.
- Deterministic ordering of files, catalogs, bundles, and generated source.
- Generation of catalog descriptors, translation bundles, module registration, and application
  bootstrap.

The semantic pipeline should be factored so parsing/normalization and validation are reusable by
the existing incremental generator steps. The generator must remain AOT-friendly and must not
use reflection, runtime XML parsing, path globbing, or package probing.

### `AtomUI.Build.Tasks`

Build Tasks retain only operations that require MSBuild or filesystem side effects:

- `PrepareLanguagePackageTask`: validate/package static language-pack contents, resolve an
  authoritative source contract when available, compute package paths/fingerprints, and write the
  manifest input.
- `GenerateLanguagePackagePropsTask`: write the package's declarative `buildTransitive` props.
- `ExportLanguageTemplatesTask`: merge an authoritative `en-US` source into a target-language
  template.
- `CollectLanguageCatalogsTask` may be folded into the export path if no independent consumer
  remains.

`ValidateLanguageFilesTask` is removed from ordinary module/application compilation. Static
language-pack projects still invoke validation through `PrepareLanguagePackageTask`, because those
projects intentionally do not run the localization generator as a runtime compilation step.

The XLIFF parser, normalized document model, fingerprint code, and package writers remain in
`AtomUI.Localization.Build.Shared` and are used by both the generator and Build Tasks.

### Third-party language-pack projects

Third-party authors are a first-class build path, not an afterthought to the official module
packages. A third-party static language-pack project has no runtime assembly and does not need to
compile a Catalog. It sets an explicit package ID, target language, and module ID, then relies on
the pack targets to validate and package its XLIFF files.

There are two supported authoring modes:

- **Verified authoring**: the project adds the target component package as an authoring-only
  `PackageReference` (`PrivateAssets=all`). The Build Tasks read that package's authoritative
  `en-US` XLIFF and Catalog metadata, bind every target file to exactly one Catalog, copy the real
  ContractVersion, and emit `Verified` package metadata.
- **Deferred authoring**: the author cannot obtain the authoritative source contract. The Build
  Tasks still validate XLIFF structure, target language, translation states, package contents, and
  normalized paths, then emit `Deferred` metadata without inventing a ContractVersion and report a
  warning. The package is usable, but contract validation is deferred to the consuming application.

The consuming application's Generator is authoritative for both modes:

1. It reads the package's declarative `buildTransitive` inputs.
2. It resolves the target `file id` to a Catalog enum and module identity from the current or
   referenced component assemblies.
3. If the module is present, it activates the bundle and performs full source, unit, placeholder,
   state, ContractVersion, and fingerprint validation. A deferred input is not treated as weaker
   validation once activated.
4. If the module is not present, a static input remains dormant and produces no bundle or error.

This means a third-party package can be published before it can reference every optional AtomUI
component, while an application that actually installs the component still receives strict compile
diagnostics. A package must target exactly one `AtomUILanguageModuleId`; multi-module translations
are published as separate packages or an explicit aggregate meta-package. Aggregate packages carry
only dependencies and do not add XLIFF, buildTransitive localization items, analyzers, or runtime
assets.

### `AtomUI.Localization.props`

The props file contains only stable defaults and item definitions:

- module identity fallback (`PackageId`, then `AssemblyName`);
- default contract version;
- default metadata for module-built-in and application-override items.

It does not define validation policy, target ordering, task paths, or package behavior.

### `AtomUI.Localization.targets`

The targets file is reduced to three concerns.

1. **Input projection**
   - Discover local XLIFF files and override files with one normalized item pipeline.
   - Exclude output/intermediate/generated directories.
   - Project the resulting items, plus resolved project-reference assets, into `AdditionalFiles`
     with the minimal metadata the generator cannot infer from symbols.
   - Expose only the required compiler-visible item metadata and project properties.

2. **Project-reference bridge**
   - Keep a small provider target that returns an authoritative `en-US` source asset for a module.
   - Keep a small consumer target that invokes the provider for explicit language-pack project
     references and adds the returned assets to `AdditionalFiles` before editorconfig generation
     and compilation.
   - This protocol remains MSBuild-only because a source generator cannot invoke another project
     target during the current evaluation.

3. **Pack/export hooks**
   - Keep pack preparation targets that call the three side-effecting Build Tasks.
   - Keep `AtomUIExportLanguageTemplates` as the public MSBuild command.
   - Remove compile-only validation targets, duplicated metadata-forwarding blocks, and configurable
     minimum-state properties. Minimum state is fixed in the semantic pipeline (`translated` for
     normal compile inputs) and package preparation (`final` for static language packages).

Where possible, item metadata is declared once in an `ItemDefinitionGroup` and forwarded through a
single combined `AdditionalFiles` item instead of separate blocks for module and override inputs.

## Input and diagnostic flow

```text
MSBuild glob/project-reference bridge
  -> AdditionalFiles + minimal source metadata
  -> incremental generator parses and normalizes XLIFF
  -> symbol resolver classifies active/dormant/deferred inputs
  -> semantic validator reports diagnostics
  -> valid active catalogs produce deterministic generated sources
```

The generator must report diagnostics even when no source is emitted for a catalog. A dormant
static input is the sole intentional exception: it is ignored until its module is present in the
compilation. Once active, deferred inputs receive the same strict contract and bundle validation
as verified inputs.

## Proposed internal structure

Add a focused semantic layer under `src/AtomUI.Generator/Localization`:

```text
Localization/
  Semantic/
    LocalizationInputNormalizer.cs
    LocalizationSemanticValidator.cs
    LocalizationSourceIndex.cs
    LocalizationDiagnosticFactory.cs
```

The exact file split may follow existing generator conventions, but responsibilities must remain
separate:

- normalization converts analyzer options and parsed XLIFF into immutable input records;
- the source index resolves owned and referenced Catalog symbols;
- validation produces diagnostics without writing sources;
- existing compiler/writers consume the validated immutable model.

No new public runtime API is required. Any new marker or helper used only by generated source stays
`internal` or generator-only.

## Error handling

- Invalid XLIFF or metadata produces source-generator diagnostics with file locations.
- A static package with no source contract is allowed only when package preparation marks it
  `Deferred`; the consuming compilation decides whether it is dormant or active.
- A partially resolvable module is an error; inputs must not silently downgrade from verified to
  deferred on a per-file basis.
- No diagnostic is swallowed to permit partial generated output. Generated source is emitted only
  for catalogs whose complete semantic compilation succeeded.

## Testing strategy

### Generator tests

Expand `tests/AtomUI.Generator.Tests/Localization` to cover:

- all XLIFF structural and translation diagnostics previously covered by `ValidateLanguageFilesTask`;
- module/source identity defaults and explicit metadata overrides;
- duplicate bundles and duplicate override units;
- verified/deferred contract-version and fingerprint paths;
- dormant static inputs and deferred activation through referenced symbols;
- deterministic output and incremental cache behavior.

### Build Task tests

Keep focused tests for:

- package content safety and normalized package paths;
- verified/deferred package preparation and manifest output;
- generated package props;
- template export and project-reference asset returns.

### Integration tests

Update the end-to-end language-pack fixtures to prove that:

- ordinary module/application builds rely on generator diagnostics;
- static language-pack projects still fail before packing on invalid content;
- consuming applications compile active bundles and ignore dormant optional bundles;
- third-party verified packages bind the referenced module contract;
- third-party deferred packages publish without a fabricated ContractVersion, warn once during pack,
  activate and validate when the module is installed, and remain dormant when it is absent;
- aggregate meta-packages do not leak localization inputs or runtime assets;
- generated package assets remain deterministic.

The final validation set is:

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --no-restore
dotnet test tests/AtomUI.Build.Tasks.Tests/AtomUI.Build.Tasks.Tests.csproj --no-restore
dotnet test tests/AtomUI.Localization.IntegrationTests/AtomUI.Localization.IntegrationTests.csproj --no-restore
git diff --check
```

## Acceptance criteria

- `build/AtomUI.Localization.props` contains only defaults/item definitions.
- `build/AtomUI.Localization.targets` contains no ordinary compile validation target and has one
  combined localization input projection path.
- The generator is the only semantic validator for compiling projects.
- Static language-pack pack/export behavior still has explicit Build Task coverage.
- Third-party Verified and Deferred package flows are both covered end to end.
- No runtime reflection, runtime XLIFF parsing, or generated-source nondeterminism is introduced.
- All focused tests and `git diff --check` pass.
