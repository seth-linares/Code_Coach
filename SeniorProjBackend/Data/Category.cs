namespace SeniorProjBackend.Data
{
    public class Category
    {
        /*
         * 5.  **Categories Table**:
    
                *   `CategoryID` (Primary Key, INT).
                *   `CategoryName` (VARCHAR).
         */
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        // Navigation Properties
        public ICollection<ProblemCategory> ProblemCategories { get; set; }
    }
}
