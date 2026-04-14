# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-04-14
### Added
- New Engineering & Coding Standards 2.0 (`GEMINI.md`).
- Industrial Standard UPM structure for `Runtime` and `Editor`.
- `Runtime/Data/` folder for modular data structures.
- `Editor/Drawers/` and `Editor/Inspectors/` folders for specialized Editor tooling.
- Full XML Documentation in English for all public APIs.

### Changed
- Refactored `DynamicFloat` and `DynamicInt` to use private serialized fields with public properties for better encapsulation.
- Moved `Vector2Range`, `Vector2IntRange`, `DynamicFloat`, and `DynamicInt` to individual files in `Runtime/Data`.
- Relocated and refactored `DynamicValueDrawer`, `Vector2RangeDrawer`, and `CustomBehaviourInspector` to their respective industrial-standard folders.
- Applied rigid member ordering and standardized header aesthetics across attributes.
- Converted `MathUtils` to a static class with industrial-grade documentation and ordering.
- Refactored `Singleton` and `SingletonBehaviour` patterns to align with company-wide thread-safety and lifecycle standards.
