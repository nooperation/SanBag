using System;

namespace SanBag.Models
{
    public class AtlasResponse
    {
        public Meta Meta { get; set; }
        public Datum[] Data { get; set; }
    }

    public class Meta
    {
        public int Count { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }

    public class Datum
    {
        public override string ToString()
        {
            return Attributes.Name;
        }
 
        public string Type { get; set; }
        public string Id { get; set; }
        public Attributes Attributes { get; set; }
    }

    public class Attributes
    {
        public int AccessControls { get; set; }
        public string CompatVersion { get; set; }
        public DateTime CreationTime { get; set; }
        public int Curation { get; set; }
        public string Description { get; set; }
        public int HeadCount { get; set; }
        public string Handle { get; set; }
        public DateTime ModificationTime { get; set; }
        public string Name { get; set; }
        public string PersonaName { get; set; }
        public string PersonaHandle { get; set; }
        public string PersonaId { get; set; }
        public string SceneAssetId { get; set; }
        public string ThumbnailAssetId { get; set; }
        public string Uri { get; set; }
        public int ReviewCount { get; set; }
        public bool Listed { get; set; }
        public Images Images { get; set; }
    }

    public class Images
    {
        public Grid Grid { get; set; }
        public Detail Detail { get; set; }
        public Large Large { get; set; }
        public Max Max { get; set; }
    }

    public class Grid
    {
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Detail
    {
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Large
    {
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Max
    {
        public string Url { get; set; }
        public int Width { get; set; }
    }
}
