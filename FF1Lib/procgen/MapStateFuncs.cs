using FF1Lib.Sanity;
using OwGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.OwResult>;

namespace FF1Lib.Procgen
{

    public partial class MapState {
	public async Task<MapResult> WipeMap(byte tile) {
	    this.OwnTilemap();

	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    this.Tilemap[y, x] = tile;
		}
	    }

	    return await this.NextStep();
	}

	public async Task<MapResult> PlaceShop() {
	    this.RenderFeature(new SCCoords(5, 5), DungeonTiles.BLACK_MAGIC_SHOP.Tiles);
	    return await this.NextStep();
	}
    }
}
