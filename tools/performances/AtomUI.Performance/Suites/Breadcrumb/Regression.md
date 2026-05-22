# Breadcrumb Regression Matrix

## Functional matrix
- [ ] Direct `BreadcrumbItem` containers keep exactly one `IsLast=True` item as items are added and removed.
- [ ] Parent `Separator` applies to direct items that do not define their own separator.
- [ ] Explicit direct item `Separator` wins over parent `Separator` changes.
- [ ] `BreadcrumbItemData.Separator` wins over parent `Separator` changes for generated containers.
- [ ] Generated containers without data separators follow parent `Separator` changes.
- [ ] `NavigateContext` and `NavigateUri` both toggle `IsNavigateResponsive`.
- [ ] ItemTemplate-generated breadcrumbs keep the template `Content` and `Separator` presenters.

## Gallery ShowCase scripts
- [ ] `BreadcrumbShowCase`: navigate to page -> six ShowCaseItem blocks render -> icon, custom separator, per-item separator, and templated examples are visible.
- [ ] `BreadcrumbShowCase`: activate the "With Params" breadcrumb item -> navigate message is displayed with the item context.

## Lifecycle matrix
- [ ] Mount -> unmount -> re-mount direct items without stale `IsLast` state.
- [ ] Parent `Separator` toggle `/` -> `>` after realization.
- [ ] ItemsSource-generated containers materialize from `BreadcrumbItemData` and keep data separator precedence.
