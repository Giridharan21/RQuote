using System.ComponentModel.DataAnnotations.Schema;

namespace RQuote.Data.Tables
{
    public class ProductProperties
    {
        public int Id { get; set; }
        public Products Product { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string ModelNumber { get; set; }
        public string BodyColour { get; set; }
        public string Wattage { get; set; }
        public string LEDSource { get; set; }
        public string BeamAngle { get; set; }
        public string ColoutTemperatureCCT { get; set; }
        public string Dimension { get; set; }
        public string Diameter { get; set; }
        public string CutOut { get; set; }
        public string Depth { get; set; }
        public string OutputArea { get; set; }
        public string LumensPackage { get; set; }
        public string LastAvailable { get; set; }
        public string Price { get; set; }
        public string LuminousFlux { get; set; }
        public string Remote { get; set; }
        public string Misc { get; set; }
        public string MountingPipe { get; set; }
        public string HolderType { get; set; }
        public string Lumenswt { get; set; }
        public string HousingMaterial { get; set; }
        public string InputVoltage { get; set; }
        public string LEDConstantCurrent { get; set; }
        public string PCBSize { get; set; }
        public string RatedFrequency { get; set; }
        public string PackingCarton { get; set; }
        public string Diffuser { get; set; }
        public string LEDcurrent { get; set; }
        public string CRI { get; set; }
        public string PowerFactor { get; set; }
        public string ChipLumen { get; set; }
        public string THDBallast { get; set; }
        public string WorkingTemperature { get; set; }
        public string BallastMake { get; set; }
        public string MountingAccessories { get; set; }
        public string MountingType { get; set; }
        public string Current { get; set; }
        public string Packing { get; set; }
        public string LifeSpan { get; set; }
        public string CuttingUnit { get; set; }
        public string MaximumConnection { get; set; }
        public string LEDQuantity { get; set; }
        public string IPRating { get; set; }
        public string AllProductDetails { get; set; }
        public string catalogueId { get; set; }
        public string TotalHeight { get; set; }
        public string ArmLength { get; set; }
        public string Luminaires { get; set; }
        public string Arms { get; set; }
        public string SuitablePipe { get; set; }
        public string SuitableBasePlate { get; set; }
        public string LEDsperm { get; set; }
        public string CutLength { get; set; }
        public string LED { get; set; }
        public string OutputCurrent { get; set; }
        public string IP { get; set; }
        public string SuitableFor { get; set; }
        public string Length { get; set; }
        public string COD { get; set; }
        public string D1 { get; set; }
        public string D2 { get; set; }
        public string BC { get; set; }
        public string PoleSize { get; set; }
        public string Width { get; set; }
        public string MountingDetails { get; set; }
        public string AValue { get; set; }
        public string Colour { get; set; }
        public string Direction { get; set; }
        public string Type { get; set; }
        public string CommunicationProtocol { get; set; }
        public string Shape { get; set; }
        public string Warranty { get; set; }
        public string Brand { get; set; }
        public string NumberOfLEDs { get; set; }
        public string LumensEfficiency { get; set; }
        public string OutputVoltage { get; set; }
        public string MAXwatts { get; set; }
        public string Track { get; set; }
        public string Driver { get; set; }
        public string Wirelength { get; set; }
        public string HighEfficiency { get; set; }
        public string ControllableDistance { get; set; }
        public string DimmingRange { get; set; }
        public string OneOutput { get; set; }
        public string TwoOutput { get; set; }
        public string ControlMethod { get; set; }

    }

}
