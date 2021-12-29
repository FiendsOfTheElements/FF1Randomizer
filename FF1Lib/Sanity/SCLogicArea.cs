namespace FF1Lib.Sanity
{
	public class SCLogicArea
	{
		public short AreaId { get; }

		public SCOwArea Area { get; }

		public SCRequirementsSet Requirements { get; }

		public SCLogicArea(short areaId, SCOwArea area, SCRequirements flags)
		{
			AreaId = areaId;
			Area = area;
			Requirements = new SCRequirementsSet(flags);
		}

		public SCLogicArea(short areaId, SCOwArea area, SCRequirementsSet newFlags)
		{
			AreaId = areaId;
			Area = area;
			Requirements = newFlags;
		}
	}
}
