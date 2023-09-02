namespace AGBrand.Packages.Models
{
    using System.Collections.Generic;

    public sealed class IdUpdater<T>
    {
        public List<T> ExistingIds { get; set; }
        public List<T> IdsToAdd { get; set; }

        public List<T> IdsToRemove { get; set; }
        public List<T> NewIds { get; set; }
    }
}
