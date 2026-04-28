# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.8] - 2026-04-28
### Added
- New `PropertyUtility` class in `Deepwave.Core.Editor.Utilities` for shared PropertyDrawer logic.

### Changed
- Refactored `GetDynamicMax` logic into a public static utility method in `PropertyUtility`.
- Updated `DynamicValueDrawer`, `Vector2RangeDrawer`, and `DynamicRangeDrawer` to use the centralized `GetDynamicMax` implementation.
- Standardized array-based dynamic limits across all core data drawers.
- Refactored `DynamicFloat` and `DynamicInt` to use private serialized fields with public properties for better encapsulation.
- Moved `Vector2Range`, `Vector2IntRange`, `DynamicFloat`, and `DynamicInt` to individual files in `Runtime/Data`.
- Relocated and refactored `DynamicValueDrawer`, `Vector2RangeDrawer`, and `CustomBehaviourInspector` to their respective industrial-standard folders.
- Applied rigid member ordering and standardized header aesthetics across attributes.
- Converted `MathUtils` to a static class with industrial-grade documentation and ordering.
- Refactored `Singleton` and `SingletonBehaviour` patterns to align with company-wide thread-safety and lifecycle standards.

