# AtomUI static language pack

This project contains declarative XLIFF translations and produces a NuGet package with no runtime assembly.

## 1. Export Catalog templates

Adding the AtomUI or third-party component package is optional. Without it, build and pack use deferred
contract validation and emit `ATOMUILOC010`. To enable template export and verified validation, add an
authoring-only reference:

```xml
<PackageReference Include="__ATOMUI_LANGUAGE_MODULE_ID__"
                  Version="__ATOMUI_VERSION__"
                  PrivateAssets="all" />
```

Then run:

```bash
dotnet msbuild -t:AtomUIExportLanguageTemplates -p:AtomUITargetLanguage=__ATOMUI_LANGUAGE_TAG__
```

The target exports or merges `Localization/**/__ATOMUI_LANGUAGE_TAG__.xlf` files from the available `en-US` Catalogs.
Translate the generated `<target>` values and keep the XLIFF `file`/`unit` identities unchanged.

## 2. Validate and pack

```bash
dotnet build
dotnet pack
```

The package contains only XLIFF content, a generated manifest, and a transitive props file. The consuming application
still declares its supported languages through `builder.UseLanguages(...)`; no runtime language-pack loading API is
required.
