using FF1Lib.Sanity;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace FF1Lib
{
	public class OwMapExchangeData
	{
	    public OwMapExchangeData() { }

	    public OwMapExchangeData(OwMapExchangeData copy) {
		this.StartingLocation = copy.StartingLocation;
		this.AirShipLocation = copy.AirShipLocation;
		this.BridgeLocation = copy.BridgeLocation;
		this.CanalLocation = copy.CanalLocation;
		this.ShipLocations = copy.ShipLocations;
		this.TeleporterFixups = copy.TeleporterFixups;
		this.DomainFixups = copy.DomainFixups;
		this.DomainUpdates = copy.DomainUpdates;
		this.OverworldCoordinates = copy.OverworldCoordinates;
	        this.DecompressedMapRows = copy.DecompressedMapRows;
	        this.FFRVersion = copy.FFRVersion;
	        this.Checksum = copy.Checksum;
	        this.Seed = copy.Seed;
	    }

	    [JsonProperty(Order = 1)]
		public SCCoords? StartingLocation { get; set; }

	    [JsonProperty(Order = 2)]
		public SCCoords? AirShipLocation { get; set; }

	    [JsonProperty(Order = 3)]
	        public SCCoords? BridgeLocation { get; set; }

	    [JsonProperty(Order = 4)]
	        public SCCoords? CanalLocation { get; set; }

	    [JsonProperty(Order = 5)]
		public ShipLocation[] ShipLocations { get; set; }

	    [JsonProperty(Order = 6)]
		public TeleportFixup[] TeleporterFixups { get; set; }

	    [JsonProperty(Order = 7)]
		public DomainFixup[] DomainFixups { get; set; }

	    [JsonProperty(Order = 8)]
	        public DomainFixup[] DomainUpdates { get; set; }

	    [JsonProperty(Order = 9)]
		public Dictionary<string, SCCoords> OverworldCoordinates { get; set; }

	    [JsonProperty(Order = 10)]
	        public List<string> DecompressedMapRows { get; set; }

	    [JsonProperty(Order = 11)]
	        public bool HorizontalBridge { get; set; }

	    [JsonProperty(Order = -3)]
	        public string FFRVersion { get; set; }

	    [JsonProperty(Order = -2)]
	        public string Checksum { get; set; }

	    [JsonProperty(Order = -1)]
	        public int Seed { get; set; }

	        public string ComputeChecksum() {
		    var copy = new OwMapExchangeData(this);
		    copy.FFRVersion = "";
		    copy.Checksum = "";
		    copy.Seed = 0;

		    var content = JsonConvert.SerializeObject(copy);
			using (SHA256 hasher = SHA256.Create())
			{
				Blob JsonBlob = Encoding.UTF8.GetBytes(content);
				Blob hash = hasher.ComputeHash(JsonBlob);
				return Convert.ToHexString(hash).Substring(0, 32);
			}

	        }
	}
}
