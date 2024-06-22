namespace Identity.Test;

public class Test
{
	public Test()
	{
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        app.Run();
    }
}
