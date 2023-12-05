using System.Security.Cryptography.X509Certificates;

namespace Todo_app.Models
{
    public class Filters
    {
        public Filters(string filterstring)
        {
            Filterstring = filterstring ?? "all-all-all";
            string[] filters = Filterstring.Split('-');

            CategoryId = filters.Length > 0 ? filters[0] : "all";
            Due = filters.Length > 1 ? filters[1] : "all";
            StatusId = filters.Length > 2 ? filters[2] : "all";

        }
        public string Filterstring { get; }
        public string CategoryId { get; }

        public string Due { get; }
        public string StatusId { get; }
        public bool HasCategory => CategoryId.ToLower() != "all";
        public bool HasDue => Due.ToLower() != "all";
        public bool HasStatus => StatusId.ToLower() != "all";
        public static Dictionary<string, string> DueFilterValues =>
            new Dictionary<string, string>{
            {"future", "Future" },
            {"past", "Past"},
            {"today", "Today"}
            };
        public bool IsPast => Due.ToLower() == "past";
        public bool IsToday => Due.ToLower() == "today";
        public bool IsFuture => Due.ToLower() == "future";
    }
}
