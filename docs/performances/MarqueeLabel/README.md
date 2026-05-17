# MarqueeLabel Performance Notes

## Scope

MarqueeLabel is currently a small control, but it is paid through `Alert` because the Alert template used to materialize a hidden `MarqueeLabel` for every Alert instance. In the real `AlertShowCase`, only the loop-banner example enables marquee behavior, so the default path should not create the marquee visual or its token bindings.

## Optimization Plan

### Phase 0: Baseline and Observability

- Add `MarqueeLabel` scenarios to `tools/performances/AtomUI.Performance`.
- Add `AlertShowCase` to `tools/performances/AtomUI.GalleryPerformance`.
- Track `Alert` and `MarqueeLabel` visual counts in Gallery route stats.
- Add lifecycle verification for Alert marquee enable, disable, message sync, removed-child binding cleanup, detach cleanup, and style parity.

### Phase 1: Alert Lazy Marquee Materialization

- Remove the static `MarqueeLabel` from `AlertTheme.axaml`.
- Keep the template `MessageLabel` behavior and hide it through the existing selector when marquee is enabled.
- Create the `MarqueeLabel` only when `IsMessageMarqueeEnabled=True`.
- Bind `MarqueeLabel.Text` to `Alert.Message` with an explicitly stored disposable binding. This is not a `[!]` case because the lazily-created child can be removed while the `Alert` owner remains alive.
- Remove the created `MarqueeLabel` and dispose its binding when marquee is disabled, the template is reapplied, or the Alert detaches from the visual tree.

### Phase 2: MarqueeLabel Hot Path Cleanup Evaluation

- Evaluated direct `AbstractMarqueeLabel` animation-path cleanup.
- The direct `MarqueeLabel.LongText` scenario did not show a stable improvement, so this phase was not retained.
- Final code keeps the existing MarqueeLabel animation implementation and limits the shipped optimization to the Alert default hidden-cost problem.

### Phase 3: Verification

- Build the owning projects.
- Run `--suite marqueelabel` for control-level materialization cost.
- Run `--verify-marqueelabel-states` for lifecycle and style behavior.
- Run real `AlertShowCase` navigation measurement with multiple cold samples.
- Run `git diff --check`.

## Final Results

### Control-Level

Command:

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --configuration Debug --framework net10.0 -- --suite marqueelabel --count 300
```

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Alert.Default ms/item | 1.991ms | 1.313ms | 34.05% faster |
| Alert.Default KB/item | 250.4KB | 233.7KB | 6.67% less |
| Alert.Default visuals/root | 16 | 15 | 1 fewer |
| Alert.Description KB/item | 277.9KB | 262.3KB | 5.61% less |
| Alert.Description visuals/root | 18 | 17 | 1 fewer |
| Alert.GalleryShape ms/item | 31.937ms | 27.235ms | 14.72% faster |
| Alert.GalleryShape KB/item | 8402.2KB | 8029.8KB | 4.43% less |
| Alert.GalleryShape visuals/root | 505 | 481 | 24 fewer |

Structural result: default/non-marquee Alert no longer creates a hidden `MarqueeLabel`. In the Gallery-shaped Alert batch, this removes 24 unused MarqueeLabel visuals. Small individual Alert timing rows can fluctuate, so the retained control-level claim is based on structural counts, allocation, and the Gallery-shaped batch.

### Real Gallery AlertShowCase

Cold command used 10 independent process samples:

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --configuration Debug --framework net10.0 -- --showcase alert --cold-iterations 10 --warmup 0 --iterations 1 --timeout-ms 10000
```

Repeated command used 10 warmups and 100 measured in-process navigations:

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --configuration Debug --framework net10.0 -- --showcase alert --cold-iterations 1 --warmup 10 --iterations 100 --timeout-ms 10000
```

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 244.75ms | 243.11ms | 0.67% faster |
| Cold first navigation median | 238.69ms | 242.26ms | 1.50% slower |
| Cold first navigation P95 | 276.50ms | 251.38ms | 9.08% faster |
| Cold first navigation alloc mean | 11997.57KB | 11593.07KB | 3.37% less |
| Repeated navigation mean | 65.42ms | 54.67ms | 16.43% faster |
| Repeated navigation median | 62.20ms | 50.11ms | 19.44% faster |
| Repeated navigation P95 | 77.43ms | 76.56ms | 1.12% faster |
| Repeated navigation alloc mean | 10375.75KB | 9985.87KB | 3.76% less |
| Runtime visuals | 585 | 561 | 24 fewer |
| Runtime MarqueeLabel | 25 | 1 | 24 fewer |

The result matches the optimization goal: unused marquee behavior no longer carries default Alert cost. Repeated navigation is clearly faster, while cold first navigation is mostly flat and still dominated by process startup, route setup, icons, labels, and button examples.
