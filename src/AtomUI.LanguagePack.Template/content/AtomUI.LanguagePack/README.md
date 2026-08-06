# AtomUI static language pack

This project contains declarative XLIFF translations and produces a NuGet package with no runtime assembly.

## 1. Export Catalog templates

Add the AtomUI or third-party component packages whose Catalogs you translate, then run:

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
