using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using PelicanTownPrivilege.Services;

namespace PelicanTownPrivilege.Content
{
    /// <summary>An item that passed validation and can be added to a shop.</summary>
    public sealed class ResolvedItem
    {
        public string Label { get; set; }
        public string QualifiedId { get; set; }
        public string GameName { get; set; }
    }

    /// <summary>
    /// Checks catalog item IDs against the installed game's data. If an ID is missing, or points at a different
    /// item, the item is looked up by name. Anything that still cannot be found is skipped and logged.
    /// </summary>
    public sealed class ItemResolver
    {
        private readonly IGameContentHelper content;
        private readonly Diagnostics diagnostics;

        public ItemResolver(IGameContentHelper content, Diagnostics diagnostics)
        {
            this.content = content;
            this.diagnostics = diagnostics;
        }

        /// <summary>Loads the item tables once. Reuse the session for every row in the same asset edit.</summary>
        public ResolutionSession StartSession()
        {
            return new ResolutionSession(
                content.Load<Dictionary<string, ObjectData>>("Data/Objects"),
                content.Load<Dictionary<string, BigCraftableData>>("Data/BigCraftables"),
                content.Load<Dictionary<string, WeaponData>>("Data/Weapons"),
                content.Load<Dictionary<string, ToolData>>("Data/Tools"),
                diagnostics);
        }
    }

    /// <summary>Resolves catalog rows against one snapshot of the game's item tables.</summary>
    public sealed class ResolutionSession
    {
        private readonly Dictionary<string, ObjectData> objects;
        private readonly Dictionary<string, BigCraftableData> bigCraftables;
        private readonly Dictionary<string, WeaponData> weapons;
        private readonly Dictionary<string, ToolData> tools;
        private readonly Diagnostics diagnostics;

        public ResolutionSession(
            Dictionary<string, ObjectData> objects,
            Dictionary<string, BigCraftableData> bigCraftables,
            Dictionary<string, WeaponData> weapons,
            Dictionary<string, ToolData> tools,
            Diagnostics diagnostics)
        {
            this.objects = objects ?? new Dictionary<string, ObjectData>();
            this.bigCraftables = bigCraftables ?? new Dictionary<string, BigCraftableData>();
            this.weapons = weapons ?? new Dictionary<string, WeaponData>();
            this.tools = tools ?? new Dictionary<string, ToolData>();
            this.diagnostics = diagnostics;
        }

        public ResolvedItem Resolve(CatalogEntry entry)
        {
            string label = $"{entry.Section ?? "privilege"} / {entry.DisplayName ?? entry.Name ?? entry.ItemId}";
            string requestedId = (entry.ItemId ?? string.Empty).Trim();
            string name = (entry.Name ?? string.Empty).Trim();

            if (requestedId.Length > 0)
            {
                string gameName = LookupName(requestedId);
                if (gameName != null)
                {
                    if (name.Length == 0 || NamesMatch(gameName, name))
                        return Accept(label, requestedId, gameName);

                    string byName = FindIdByName(name);
                    if (byName != null)
                    {
                        diagnostics.Warn($"{label}: ItemId {requestedId} is '{gameName}', not '{name}'. Using {byName} instead.");
                        return Accept(label, byName, name);
                    }

                    diagnostics.Warn($"{label}: ItemId {requestedId} is '{gameName}', not '{name}', and no item named '{name}' exists. Using the ItemId as written.");
                    return Accept(label, requestedId, gameName);
                }

                diagnostics.Warn($"{label}: ItemId {requestedId} does not exist in this game version.");
            }

            if (name.Length > 0)
            {
                string byName = FindIdByName(name);
                if (byName != null)
                {
                    diagnostics.Info($"{label}: found by name as {byName}.");
                    return Accept(label, byName, name);
                }
            }

            diagnostics.Skip(label, "no matching item in the installed game data");
            return null;
        }

        private static ResolvedItem Accept(string label, string qualifiedId, string gameName)
        {
            return new ResolvedItem { Label = label, QualifiedId = qualifiedId, GameName = gameName };
        }

        /// <summary>Returns the game's name for a qualified ID, or null if the ID does not exist.</summary>
        private string LookupName(string qualifiedId)
        {
            if (!TrySplit(qualifiedId, out string prefix, out string key))
                return null;

            switch (prefix)
            {
                case "O":
                    return objects.TryGetValue(key, out ObjectData obj) && obj != null ? obj.Name ?? string.Empty : null;
                case "BC":
                    return bigCraftables.TryGetValue(key, out BigCraftableData bigCraftable) && bigCraftable != null ? bigCraftable.Name ?? string.Empty : null;
                case "W":
                    return weapons.TryGetValue(key, out WeaponData weapon) && weapon != null ? weapon.Name ?? string.Empty : null;
                case "T":
                    return tools.TryGetValue(key, out ToolData tool) && tool != null ? tool.Name ?? string.Empty : null;
                default:
                    return null;
            }
        }

        /// <summary>Finds a qualified ID by the game's internal name. Objects are searched first.</summary>
        private string FindIdByName(string name)
        {
            foreach (KeyValuePair<string, ObjectData> pair in objects)
                if (pair.Value != null && NamesMatch(pair.Value.Name, name)) return "(O)" + pair.Key;

            foreach (KeyValuePair<string, BigCraftableData> pair in bigCraftables)
                if (pair.Value != null && NamesMatch(pair.Value.Name, name)) return "(BC)" + pair.Key;

            foreach (KeyValuePair<string, WeaponData> pair in weapons)
                if (pair.Value != null && NamesMatch(pair.Value.Name, name)) return "(W)" + pair.Key;

            foreach (KeyValuePair<string, ToolData> pair in tools)
                if (pair.Value != null && NamesMatch(pair.Value.Name, name)) return "(T)" + pair.Key;

            return null;
        }

        private static bool TrySplit(string qualifiedId, out string prefix, out string key)
        {
            prefix = null;
            key = null;
            if (!qualifiedId.StartsWith("(", StringComparison.Ordinal)) return false;

            int close = qualifiedId.IndexOf(')');
            if (close <= 1 || close == qualifiedId.Length - 1) return false;

            prefix = qualifiedId.Substring(1, close - 1);
            key = qualifiedId.Substring(close + 1);
            return true;
        }

        private static bool NamesMatch(string gameName, string wanted)
        {
            return gameName != null && string.Equals(gameName.Trim(), wanted.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
