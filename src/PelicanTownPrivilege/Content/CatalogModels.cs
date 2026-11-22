using System.Collections.Generic;

namespace PelicanTownPrivilege.Content
{
    /// <summary>The shape of assets/catalog.json. Edit that file to change what each shop sells.</summary>
    public sealed class CatalogFile
    {
        public string Description { get; set; }
        public List<CatalogEntry> Entries { get; set; } = new List<CatalogEntry>();
    }

    /// <summary>One row added to a shop.</summary>
    public sealed class CatalogEntry
    {
        /// <summary>Data/Shops key. SeedShop is Pierre's General Store; Blacksmith is Clint.</summary>
        public string Shop { get; set; }

        /// <summary>Label used in logs and documentation.</summary>
        public string Section { get; set; }

        /// <summary>Name of a true/false setting in config.json. The row is added only while it is true.</summary>
        public string Toggle { get; set; }

        /// <summary>Internal game name. Used to check the ItemId, or to find the item when no ItemId is given.</summary>
        public string Name { get; set; }

        /// <summary>Qualified item ID such as (O)107, (BC)645, (W)4 or (T)IridiumAxe.</summary>
        public string ItemId { get; set; }

        /// <summary>Optional name shown in the shop instead of the item's own name.</summary>
        public string DisplayName { get; set; }

        /// <summary>Optional game state query. The row is offered only while it is true.</summary>
        public string Condition { get; set; }

        /// <summary>Optional mod-owned tags placed on the item when it is bought. Keys start with PelicanTownPrivilege/.</summary>
        public Dictionary<string, string> ModData { get; set; }
    }
}
