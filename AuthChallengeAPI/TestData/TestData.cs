using AuthChallengeAPI.Models;

namespace AuthChallengeAPI.TestData;

public static class FakeData
{
    public static readonly List<UserModel> AllUsers = new()
    {
        new UserModel(1, "caltman", UserTitles.Owner),
        new UserModel(2, "nwolfe", UserTitles.Secretary),
        new UserModel(3, "bminick", UserTitles.Manager),
        new UserModel(4, "cavery", UserTitles.Janitor),
    };

    public static UserModel? GetUserById(int id)
    {
        return AllUsers.Find(u => u.Id == id);
    }
    
    public static UserModel? GetUserByUserName(string userName)
    {
        return AllUsers.Find(u => u.UserName == userName);
    }
}