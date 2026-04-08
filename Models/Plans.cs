public class Plan
{
    public int Id { get; set; }
    public string? Name { get; set; }    // Mobile, Basic, Standard, Premium
    public decimal Price { get; set; }              // NGN price
    public string?VideoQuality { get; set; } // Fair, Good, Great, Best
    public string Resolution { get; set; } = null!;   // 480p, 720p, 1080p, 4K HDR
    public bool SpatialAudio { get; set; } = false;  // True for Premium
    public int MaxDevices { get; set; }              // Household simultaneous devices
    public int MaxDownloadDevices { get; set; }      // Devices allowed for downloads
    public string AllowedDeviceTypes { get; set; } = "mobile,tv,tablet,computer";
    public string?Description { get; set; } 
}