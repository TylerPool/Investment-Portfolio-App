namespace Portfolio.Api.Models;

public enum UserTypes
{
    Client = 1,
    Advisor = 2,
    Manager = 3
}
public class User
{
    public required string Id { get; set; }
    public UserTypes userType { get; set; }
    public string FilePath { get; set; } = "";
    public User()
    {
        //For now no input constructor returns an example user for testing/dev
        this.Id = "U-001";
        this.userType = UserTypes.Advisor;
    }
}