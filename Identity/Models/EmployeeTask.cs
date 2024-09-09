namespace Identity.Models;

public class EmployeeTask
{
    private static Random random = new();

    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }


    private DateTime _createdDate;
    public DateTime CreatedDate
    {
        get
        {
            // Convert UTC to local time when getting
            return _createdDate.Kind == DateTimeKind.Utc
                ? _createdDate.ToLocalTime()
                : _createdDate;
        }
        set
        {
            // Convert to UTC if not already UTC when setting
            _createdDate = value.Kind == DateTimeKind.Utc
                ? value
                : value.ToUniversalTime();
        }
    }


    private DateTime _targetDate;
    public DateTime TargetDate
    {
        get
        {
            // Convert UTC to local time when getting
            return _targetDate.Kind == DateTimeKind.Utc
                ? _targetDate.ToLocalTime()
                : _targetDate;
        }
        set
        {
            // Convert to UTC if not already UTC when setting
            _targetDate = value.Kind == DateTimeKind.Utc
                ? value
                : value.ToUniversalTime();
        }
    }


    private DateTime _dateCompleted;
    public DateTime DateCompleted
    {
        get
        {
            // Convert UTC to local time when getting
            return _dateCompleted.Kind == DateTimeKind.Utc
                ? _dateCompleted.ToLocalTime()
                : _dateCompleted;
        }
        set
        {
            // Convert to UTC if not already UTC when setting
            _dateCompleted = value.Kind == DateTimeKind.Utc
                ? value
                : value.ToUniversalTime();
        }
    }


    public EmployeeTaskStatus Status { get; set; }

    public List<TaskNote> Notes { get; set; } = [];

    public List<TaskDocument> TaskDocuments { get; set; } = [];

    public EmployeeTask()
    {
        Id = GenerateRandomId();
    }

    private int GenerateRandomId()
    {
        // Generates a 6-digit number between 100000 and 999999
        return random.Next(100000, 1000000);
    }

}
