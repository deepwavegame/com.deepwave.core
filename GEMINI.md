# Deepwave Core — Engineering & Coding Standards

> **Scope:** Entire `com.deepwave.core` package
> **Version:** 2.0 — UPM Industrial Standard
> **Enforced by:** Code review, PR checklist, and Assembly Definitions (`.asmdef`)

---

## 1. Package Structure (UPM Standard)

The directory structure strictly follows the Unity Package Manager (UPM) conventions. This ensures compatibility, clean user projects, and modular distribution for shared utilities.

```text
com.deepwave.core/
├── package.json               # Package manifest (name, version, dependencies)
├── README.md                  # Quick start guide and overview
├── CHANGELOG.md               # Version history (Keep a Changelog format)
├── LICENSE.md                 # Licensing information
├── Runtime/                   # Core runtime scripts (Shared across projects)
│   ├── Deepwave.Core.Runtime.asmdef
│   ├── Abstractions/          # Shared interfaces (ISingleton, IInterpolatable)
│   ├── Attributes/            # Custom Inspector attributes (InspectorHeader, DynamicRange)
│   ├── Data/                  # Dynamic types and structs (DynamicValue, Vector2Range)
│   ├── Singleton/             # Singleton design patterns (Singleton, SingletonBehaviour)
│   └── Utilities/             # High-performance math and system helpers (MathUtils, GuidUtility)
├── Editor/                    # Editor-only scripts (Custom drawers for core features)
│   ├── Deepwave.Core.Editor.asmdef  # References Runtime.asmdef
│   ├── Drawers/               # PropertyDrawers for Attributes and Data types
│   └── Inspectors/            # Custom Inspectors for Core components
├── Tests/                     # Industrial-grade automated testing for math & patterns
│   ├── Runtime/               # PlayMode tests (e.g., Singleton persistence)
│   │   └── Deepwave.Core.Tests.Runtime.asmdef
│   └── Editor/                # EditMode tests (e.g., MathUtils accuracy)
│       └── Deepwave.Core.Tests.Editor.asmdef
├── Samples~/                  # Usage examples for each core module
│   ├── AttributeDemos/        # Demonstrating Inspector headers and ranges
│   └── MathInterpolation/     # Demonstrating framerate-independent lerps
└── Documentation~/            # Docusaurus / DocFX source files
```

**Key Structural Rules:**
- **Hidden Folders (`~`):** `Samples~` and `Documentation~` MUST use the tilde suffix to keep the user's `Assets` folder clean.
- **Assembly Definitions:** No script can exist outside the influence of an `.asmdef` file. Core must be highly modular.
- **Dependency Inversion:** `Runtime` can NEVER reference `Editor`. 

---

## 2. Architecture & SOLID Principles

### 2.1 Pattern Implementation (Thread Safety & Reliability)
Core logic must be robust, as it serves as the foundation for other packages.

- **Rule:** Singletons must be thread-safe (using `Lazy<T>`) and have private constructors to prevent bypass.
- **Rule:** `MathUtils` functions must be pure and deterministic where possible.
- **Rule:** Use `OnValidate` (Editor only) to sync Inspector changes with Runtime data structures (like Dynamic types).

```csharp
// Example of Industrial Standard Core Utility
public struct DynamicValue
{
    [SerializeField] private float _value;
    [SerializeField] private Vector2Range _range;
    [SerializeField] private bool _randomize;

    /// <summary>Returns either the fixed value or a random value within the specified range.</summary>
    public float Evaluate()
    {
        return _randomize ? Random.Range(_range.min, _range.max) : _value;
    }
}
```

### 2.2 Decoupling & Interfaces
- Define core behaviors in `Abstractions/` using interfaces.
- **Stateless Utilities:** Prefer static classes for utilities like `MathUtils` to avoid allocation and state management overhead.

---

## 3. Naming Conventions

| Item | Convention | Example |
|---|---|---|
| Interface | `I` + PascalCase | `IInterpolatable`, `IPattern` |
| Abstract class | PascalCase (no prefix/suffix needed) | `PatternBase`, `AttributeBase` |
| Struct (Data) | PascalCase, assign `readonly` if possible | `readonly struct MathResult` |
| `private` field | `_camelCase` | `_singletonInstance` |
| `private const` | `PascalCase` | `DefaultPrecision`, `MaxBufferSize` |
| Boolean | `Is`, `Has`, `Can`, `Should` | `IsInitialized`, `HasValue` |

---

## 4. Code Structure Inside a File

Every `.cs` file must follow this **exact member ordering**. 
**`#region` is strictly forbidden**.

```csharp
using System;
using UnityEngine;
using Deepwave.Core.Abstractions;

namespace Deepwave.Core.Utilities
{
    /// <summary>
    /// Provides high-performance, framerate-independent mathematical functions.
    /// </summary>
    public static class MathUtils
    {
        // ── Constants & Static ────────────────────────────────────────────
        public const float Epsilon = 1e-6f;

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>
        /// Framerate-independent interpolation using exponential decay.
        /// </summary>
        public static float FloatInterp(float current, float target, float speed, float dt) 
        {
            return Mathf.Lerp(current, target, 1f - Mathf.Exp(-speed * dt));
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private static float SafeDivide(float a, float b) { ... }
    }
}
```

---

## 5. Performance & Zero-GC Strict Rules

Deepwave Core is the engine for all company tools. It must be invisible in terms of overhead.

1. **Zero GC in Hot Paths:** Absolutely NO `new` allocations, NO `LINQ`, and NO `foreach` in core math calculations and frequent utilities.
2. **Burst Compatibility:** Math utilities should be compatible with the Burst compiler where possible (explicit use of `math` from `Unity.Mathematics` vs `Mathf`).
3. **Data-Oriented Math:** Favor structs and simple value types over objects.
4. **Memory Management:** For any pooling implemented in Core, ensure rigid lifecycle management and explicit cleanup.

---

## 6. Pre-Commit Checklist (Industrial Standard)

- [ ] All code resides within the `Deepwave.Core` namespace boundary.
- [ ] No `#region` blocks are used.
- [ ] Editor-specific code (Drawers/Inspectors) is isolated in the `Editor` folder.
- [ ] Zero `new` allocations or LINQ statements exist in performance-critical utilities.
- [ ] Mathematical functions are verified for accuracy and edge cases (NaN, Inf).
- [ ] Public API documentation (XML) is clear and explains the "Why" and "How".
- [ ] Structure follows the UPM industrial standard strictly.
