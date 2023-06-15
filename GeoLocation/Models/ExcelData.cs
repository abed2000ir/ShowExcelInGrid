namespace GeoLocation.Models
{
    public class ExcelData
    {
        [@Column(1)]
        public string State { get; set; }

        [@Column(2)]
        public string Area { get; set; }

        [@Column(3)]
        public string SchoolName { get; set; }

        [@Column(4)]
        public string LevelStudy { get; set; }

        [@Column(5)]
        public string Gender { get; set; }

        [@Column(6)]
        public string SchoolType { get; set; }

        [@Column(7)]
        public string Type { get; set; }

        [@Column(8)]
        public string Character { get; set; }

        [@Column(9)]
        public string Phone { get; set; }


        [@Column(10)]
        public string Postalcode { get; set; }

        [@Column(11)]
        public string Address { get; set; }


    }

    [AttributeUsage(AttributeTargets.All)]
    public class Column : System.Attribute
    {
        public int ColumnIndex { get; set; }


        public Column(int column)
        {
            ColumnIndex = column;
        }
    }

}
