namespace IYun.Models.Dto
{
    /// <summary>
    /// 补录个税抵扣所需信息Input
    /// </summary>
    public class SupplementaryTaxInput
    {
        public int Id { get; set; }

        public string CardType { get; set; }

        public bool IsWorking { get; set; }

        public string ParentName1 { get; set; }

        public string ParentCard1 { get; set; }

        public string ParentCardType1 { get; set; }

        public string ParentName2 { get; set; }
        
        public string ParentCard2 { get; set; }

        public string ParentCardType2 { get; set; }
    }
}