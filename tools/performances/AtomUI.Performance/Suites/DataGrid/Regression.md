# DataGrid Query + Range Source Regression Matrix

This matrix is the acceptance contract for redefining `ItemsSource` as
`IDataGridSource?` and replacing `DataGridCollectionView` with the immutable
`DataGridQuery` + range-based source architecture. A checked item must have repeatable automated
or recorded visual evidence; implementation completion alone is not evidence.

## Data and Range Contract

- [ ] Empty source commits zero entries and a stable zero extent without requesting an invalid range.
- [ ] One-entry source realizes one data row and does not prefetch beyond the declared total.
- [ ] A short final block accepts fewer entries than requested only when it ends at the declared total.
- [ ] A malformed start, oversized block, invalid count, duplicate key, stale revision, stale generation, old source, or old snapshot is rejected before presentation mutation.
- [ ] Initial load issues exactly one bootstrap request; post-bootstrap concurrency never exceeds two.
- [ ] Visible requests outrank prefetch; the previous committed viewport remains rendered while replacement data is pending.
- [ ] A new viewport retains leases for shared blocks before canceling obsolete blocks, so adjacent targets neither duplicate nor abort useful work.
- [ ] Superseding a viewport removes orphaned semaphore waiters, cancels active orphaned Source work, and prevents ignored-cancellation results from entering cache or presentation state.
- [ ] One snapshot-expired response restarts once with a new generation; a second expiry becomes a visible load error without retry churn.
- [ ] Refresh either atomically replaces the committed snapshot or preserves the complete previous snapshot on failure.
- [ ] Query, page request, group expansion, source replacement, invalidation, detach, and reattach each cancel obsolete work and commit no stale result.
- [ ] Continuous mode rejects totals outside the validated `int` presentation domain instead of truncating or overflowing.

## Virtualization and Geometry

- [ ] Fixed-height rows map offset to display index in O(1) and preserve top-row anchoring after a commit.
- [ ] Auto-height rows update the bounded sparse height index in O(log M) and preserve the first visible key plus intra-row offset.
- [ ] Group headers and expanded row details participate in offset mapping without creating placeholder rows.
- [ ] Realized rows stay bounded by the visible viewport plus at most one edit pin and one drag pin.
- [ ] Prefetch stays bounded to one viewport before and one viewport after the desired viewport.
- [ ] Measure, arrange, container prepare, and container recycle perform no source fetch, synchronous wait, or range-buffer allocation.
- [ ] Ten thousand alternating forward/backward scroll cycles keep cache blocks, pins, realized rows, subscriptions, and memory bounded.
- [ ] Thumb targets dispatched across separate UI turns commit only the final viewport; intermediate intents create no unbounded request or continuation backlog.
- [ ] Both scrollbars remain visible and correct under the same width/height constraints as the baseline.
- [ ] Left- and right-frozen columns retain alignment, clipping, hit testing, and horizontal virtualization.
- [ ] Nested scrolling consumes wheel input inside the inner range and chains to the outer scroller only at the inner boundary.

## Rows, Details, Grouping, and Paging

- [ ] RowDetails expand, collapse, recycle, and re-expand with the correct stable row key and measured height.
- [ ] Grouped ranges render headers in source order; collapsed groups expose no hidden member slots.
- [ ] Expanding or collapsing an already matching group state is a no-request no-op.
- [ ] Top and bottom pagination show the same page, total, visibility, and navigation state.
- [ ] Page changes use a new generation, cancel old range work, and never briefly display rows from the previous page.

## Query and Interaction

- [ ] Normal-click sorting follows replace/cycle semantics and projects direction plus priority from committed `Query.Sorts`.
- [ ] Shift-click sorting follows append/in-place-cycle/remove semantics and compresses remaining priorities.
- [ ] Filtering commits one immutable query, restores checked state from that query, and never retains a second filter owner.
- [ ] Query validation rejects unknown fields/operators and unsupported directions/kinds before calling the source.
- [ ] A blocking edit commit failure leaves query, event count, revision, request count, and visuals unchanged.
- [ ] Successful edit commit and cancel retain the correct row key across block eviction and re-realization.
- [ ] Reorder accept applies one validated source transaction; reject rolls back without partial visual movement.

## Selection and Current State

- [ ] Single selection follows stable keys across sort, filter, refresh, paging, eviction, and re-realization.
- [ ] Extended selection supports loaded keys, all-matching mode, exclusion keys, and unresolved index intervals without fabricating unloaded items.
- [ ] Current row/cell restoration uses stable key plus field id and never aliases a recycled container.
- [ ] Selection/current/edit pins are acquired once and released on completion, replacement, rollback, detach, and source change.

## Lifecycle and Resources

- [ ] Source replacement unsubscribes the previous `Invalidated` event exactly once and never disposes the external source.
- [ ] Detach cancels generation/request tokens, removes source/template/pagination subscriptions, releases cache pins, and prevents continuations from retaining the grid.
- [ ] Reattach establishes one subscription set and one active generation without duplicate requests.
- [ ] Template reapply detaches old parts before attaching new parts and keeps the committed presentation consistent.
- [ ] Repeated attach/detach, source replacement, failed requests, and cancellation leave no growing CTS, continuation, pooled buffer, pin, or event-handler population.

## Visual Contract

- [ ] Light and Dark themes preserve baseline header, row, cell, border, scrollbar, empty, loading, and error visuals.
- [ ] Every `SizeType` preserves its baseline row/header dimensions, padding, typography, and focus geometry.
- [ ] Empty, initial loading, refreshing, error, and rollback states use existing static template nodes with no dynamic visual-tree creation; initial loading uses the existing Spin, while refreshing keeps committed content fully opaque and does not start the automatic Spin.
- [ ] Selected-row background retains priority over sorted-column cell background, including hover and current-cell states.
- [ ] Hover and pointer behavior works across the full left/center/right row width with frozen columns and both scrollbars present.
- [ ] Gallery screenshots show no clipping, overlap, layout jump, stale row, blank committed viewport, or scrollbar regression.

## Performance Gates

- [ ] Run the dedicated DataGrid benchmark with the same Release/net10.0 parameters before and after the refactor.
- [ ] Repeated mean, median, P95 time and allocated bytes have no unexplained regression beyond the recorded baseline noise.
- [ ] At least ten independent cold `DataGrid.GalleryShape` samples are retained and compared separately from repeated in-process samples.
- [ ] Range cache memory is bounded by configured blocks plus explicitly pinned blocks, independent of source total count.
- [ ] Request count and allocation growth remain bounded during 10,000 bidirectional scroll cycles.
- [ ] Twenty rapid-thumb samples with the fixed-latency cooperative Source retain mean, median, and P95 final-viewport latency near one current visible fetch wave.
