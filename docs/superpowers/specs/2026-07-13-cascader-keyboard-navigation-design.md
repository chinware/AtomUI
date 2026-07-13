# Cascader Keyboard Navigation Design

## Goal

Resolve issue #379 points 2 and 3 without changing public API: searchable Cascader supports keyboard navigation of search results and Enter confirmation, while the normal Cascader popup supports tree-shaped keyboard browsing.

## Scope

This change covers two interaction modes:

1. Search mode: when the popup is open and `Cascader.FilterValue` produces `CascaderView.FilteredPathInfos`, `Up` and `Down` move an internal search candidate, and `Enter` confirms the current candidate path.
2. Tree mode: when the popup is open and not filtering, `Up` and `Down` move through visible Cascader items, `Right` expands or enters the next level, `Left` collapses or returns to the parent level, and `Enter` selects the current candidate when it is selectable.

The change does not add public/protected API, public Avalonia properties, public events, template part names, token names, or documented behavior switches. New state is internal to Cascader implementation classes.

## Search Mode

Search result navigation follows the Select candidate model rather than directly changing `SelectedIndex`.

- `CascaderViewFilterList` owns an internal candidate index/item.
- `Up` and `Down` update the candidate and scroll it into view.
- Moving the candidate must not update `SelectedOption` or close the popup.
- `Enter` confirms the candidate through `Cascader`, using the same selection path as pointer selection.
- If there is no candidate but at least one enabled result exists, `Enter` confirms the first enabled result.
- If there are no enabled results, `Enter` leaves selection and popup state unchanged.

`CascaderView` may expose internal helper methods for candidate movement and retrieval, but it must not treat Enter as a standalone search trigger. `Cascader` interprets Enter because it owns the input box and full-control selection semantics.

## Tree Mode

Tree mode uses TreeView-style semantics adapted to Cascader's multi-column level lists.

- The current keyboard candidate is a visible `CascaderViewItem`.
- `Down` moves to the next visible item. It starts at the first enabled root item when no candidate exists.
- `Up` moves to the previous visible item. It starts at the last enabled visible item when no candidate exists.
- `Right` expands the candidate if it has children and is not expanded. If already expanded, it moves to the first enabled child in the next level.
- `Left` collapses the candidate if it is expanded. If not expanded, it moves to the candidate's parent item in the previous level.
- `Enter` selects the candidate when it is enabled, not loading, and either a leaf or parent selection is allowed. Otherwise it expands the candidate when possible.

The tree candidate is internal highlight state only. It must not change selected option until Enter is pressed.

## State And Lifecycle

Candidate state is cleared when filtering state changes, options source changes, template reapplies, or the view leaves the visual tree. Candidate state must not survive container recycling in a way that highlights the wrong item.

## Tests

Add focused tests under `tests/AtomUI.Desktop.Controls.Tests/Cascader/CascaderSearchSelectionTests.cs` for:

- Search `Down` moves the candidate without selecting.
- Search `Enter` confirms the candidate and updates `Cascader.SelectedOption`.
- Search `Enter` defaults to the first enabled result when no candidate exists.
- Search `Enter` with no result does not update selection or close the popup.
- Tree `Down`/`Right`/`Enter` can navigate root to child and select a leaf.
- Tree `Left` returns from child level to parent.

Run the targeted Cascader tests, then `git diff --check`.
