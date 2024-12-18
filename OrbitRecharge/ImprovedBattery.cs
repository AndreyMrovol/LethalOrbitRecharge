namespace OrbitRecharge
{
	public class ImprovedBattery : Battery
	{
		public ImprovedBattery(bool isRechargeable, float charge)
			: base(isRechargeable, charge) { }

		public override string ToString()
		{
			return $"Charged {charge * 100}%";
		}
	}
}
