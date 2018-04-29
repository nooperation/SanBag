using System;

namespace AtlasView.Models
{
    public class AtlasResponse
    {
        public Meta Meta { get; set; }
        public Datum[] Data { get; set; }
    }

    public class Meta
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
    }

    public class Datum
    {
        public string Id { get; set; }
        public int AccessControls { get; set; }
        public string CompatVersion { get; set; }
        public int Curation { get; set; }
        public string Description { get; set; }
        public int HeadCount { get; set; }
        public string Handle { get; set; }
        public string Name { get; set; }
        public string PersonaName { get; set; }
        public string PersonaHandle { get; set; }
        public string PersonaId { get; set; }
        public string SceneAssetId { get; set; }
        public string ThumbnailAssetId { get; set; }
        public string Uri { get; set; }
        public int ReviewCount { get; set; }
        public bool Listed { get; set; }
        public Image Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Image
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string AssetId { get; set; }
        public ImageSize[] Sizes { get; set; }
    }

    public class ImageSize
    {
        public string Size { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }
}
