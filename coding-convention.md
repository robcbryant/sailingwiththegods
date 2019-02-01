# Editor Settings (see .editorconfig)

## Indentation:
  * Real Tabs, not spaces
  * Tab width: 4
  * unix line endings
  * UTF-8, no BOM

## Blocks:
  * Braces on same line for methods, control blocks
  * Braces on a new line for classes, structs, enums, and everything else

# Style Conventions

## Class Names
  * Begin with Capital letter
  * For MonoBehaviours, must match the filename
  * Exception for legacy classes. Should aim to clean these up later

## Readability/Maintainability

  * Delete unused code. It obscures the functionality
  * Always have no warnings in Unity, so that when new warnings pop up, we'll notice.
  * Scoping, encapsulation, separation of concerns
    * Hide as much as possible from other systems within a system, to   prevent bugs related to modifying data you're not supposed to.
    * Code with minimal dependencies on other code should be separated into separate files and separate systems.
  * Cyclomatic complexity
    * Avoid excessive nesting of if blocks

## Inspector Guidelines

Only show things in the inspector that should actually be set in the inspector. Otherwise it's unclear how things are expected to be hooked up.

* Use `internal` (default for no qualifier) for things not referenced in any other scripts
* Use properties or `[HideInInspector]` for things that are accessed in other scripts but aren't meant to be set in the inspector. Prefer properties when possible.
* Use `[ReadOnly]` for things that are shown in the inspector but are just there for debug and aren't accessed in other scripts
* Use [SerializeField] if not accessed publicly, exception for legacy code already using public. Convert when convenient.
