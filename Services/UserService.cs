using System.Collections.Generic;
using System.Linq;
public class UserService{
    private List<User> users = new List<User>();


    public UserService(){
        LoadUsers();
    }
    public User Find(int id){
        var qr = from p in users
                 where p.Id == id select p;

        return qr.FirstOrDefault();
    }

    public List<User> AllUsers() => users;
    public void LoadUsers(){
        users.Clear();

        users.Add(new User() {
            Id = 1,
            Name = "Vu1",
            Email = "Id1@gmail.com",
            Password = "1",
            PrivateKey = "Id1@1",
        });

        users.Add(new User() {
            Id = 2,
            Name = "Vu2",
            Email = "Id2@gmail.com",
            Password = "2",
            PrivateKey = "Id2@2",
        });

        users.Add(new User() {
            Id = 3,
            Name = "Vu3",
            Email = "Id3@gmail.com",
            Password = "3",
            PrivateKey = "Id3@3",
        });
    }
}