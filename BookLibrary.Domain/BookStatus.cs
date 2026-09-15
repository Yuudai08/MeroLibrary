namespace BookLibrary.Domain;

// Maps to CHECK CK_Books_Status: 0-3.
// Stored as TINYINT in SQL Server, converted to this enum in C#.
public enum BookStatus
{
    WantToRead = 0,
    Reading = 1,
    Completed = 2,
    Dropped = 3
}
