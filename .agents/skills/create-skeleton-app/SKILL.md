---
name: create-skeleton-app
description: Use when creating or updating a minimal AtomUI/Avalonia skeleton application, sample app, or reproduction project that demonstrates AtomUI setup, controls, themes, and basic runtime wiring.
---

# Create Skeleton App

Use this skill when the user asks for a minimal runnable AtomUI or Avalonia sample application.

## Workflow

1. Confirm the target location, framework, and purpose from the request or nearby project structure.
2. Reuse existing repository conventions for project files, package versions, namespaces, theme setup, and control imports.
3. Keep the skeleton small but runnable:
   - Application entry point
   - Main window or view
   - Theme/resource registration
   - One or two representative AtomUI controls
4. Avoid introducing unrelated architecture, services, or styling.
5. Run the smallest useful build command for the created project and report the result.

## Notes

Prefer existing templates and examples already present in the repository over inventing new structure.
