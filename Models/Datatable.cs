namespace RevisioneNew.Models
{
    public class Datatable<T>
    {
        public string Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public IEnumerable<T> Data {get; set;}
    }
}
