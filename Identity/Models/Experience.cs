namespace Identity.Models;

public class Experience
{
    public Guid Id { get; set; }

    public string? Title { get; set; }


    private DateTime? _startDate;
    public DateTime? StartDate
    {
        get
        {
            if (_startDate.HasValue)
            {
                // Convert UTC to local time when getting
                return _startDate.Value.Kind == DateTimeKind.Utc
                    ? _startDate.Value.ToLocalTime()
                    : _startDate.Value;
            }
            return null;
        }
        set
        {
            if (value.HasValue)
            {
                // Convert to UTC if not already UTC when setting
                _startDate = value.Value.Kind == DateTimeKind.Utc
                    ? value
                    : value.Value.ToUniversalTime();
            }
            else
            {
                _startDate = null;
            }
        }
    }


    private DateTime? _endDate;
    public DateTime? EndDate
    {
        get
        {
            if (_endDate.HasValue)
            {
                // Convert UTC to local time when getting
                return _endDate.Value.Kind == DateTimeKind.Utc
                    ? _endDate.Value.ToLocalTime()
                    : _endDate.Value;
            }
            return null;
        }
        set
        {
            if (value.HasValue)
            {
                // Convert to UTC if not already UTC when setting
                _endDate = value.Value.Kind == DateTimeKind.Utc
                    ? value
                    : value.Value.ToUniversalTime();
            }
            else
            {
                _endDate = null;
            }
        }
    }



    public bool? PresentlyWorking { get; set; }

    public string? Description { get; set; }    
}
