## Installation
- Download latest `ItemRemover.zip` archive from [release page](https://github.com/dm1tz/ItemRemover/releases/latest).
- Extract archive contents into ASF `plugins` directory.

## Commands

Command | Alias | Access | Description
--- | --- | --- | ---
`removeinventory [Bots] <AppID> <ContextID>` | `rmi` | `Master` | Removes all items from inventory of given bot instances for specified `AppID`.
`removeinventory& [Bots] <AppID> <ContextID> <Rarities>` | `rmi&` | `Master` | Removes all items of specified rarities from inventory of given bot instances for specified `AppID`.
`removeitem [Bots] <AppID> <ContextID> <ItemIDs>` | `rmit` | `Master` | Removes all items of specified IDs from inventory of given bot instances for specified `AppID`.
`removeitem* [Bots] <AppID> <ContextID> <ItemNames>` | `rmit*` | `Master` | Removes all items of specified names from inventory of given bot instances for specified `AppID`. *Note*: item's name may be localized.
`irversion` | `irv` | `FamilySharing` | Prints plugin version.
