# RimLua

Lua scripting language implementation for RimWorld modding. Why? For God sake.

## Tutorials
### Installation
1. Launch RimWorld with RimLua enabled
2. Observe RimLua-addons folder in Documents directory (idk if it compatible with Linux, sorry)
3. Create "core" folder in "RimLua-addons" director
4. Copy https://github.com/RimLua/RimLua-core content into it
5. Yay! You can paste addons in RimLua-addons directory, and they will be work(i hope)
### Addon creation
1. Create addon folder in RimLua-addons
2. Create file with random name(preferably main.lua for main file) and lua extensions
3. Write something in this file, for example
```
hook.Add("Initialize", "my_addon.InitializeHook", function()
    print("Hello!")
end)
```
4. Launch Rimworld and see result in console: "[your addon folder name] Hello!"

## Documentation
Will be when the normal functionality will be added 