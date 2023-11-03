// See https://aka.ms/new-console-template for more information



public class Program
{
    public static void Main(string[] args)
    {
        SecurityPersonal s1 = new SecurityPersonal(1, "James", "Bond");
        SecurityPersonal s2 = new SecurityPersonal(2, "Eathen", "Hunt");
        InternalContact i1 = new InternalContact(3, "Nirbheeksinh", "Vihol");

        ServailanceHub hub = new ServailanceHub();

        IDisposable ds1 =  hub.Subscribe(s1);
        IDisposable ds2 = hub.Subscribe(s2);
        IDisposable di1 = hub.Subscribe(i1);

        hub.ConfirmExternalVisitorEntry(1, "Deep", "lodu", i1._employee.FirstName, 3);
        hub.ConfirmExternalVisitorEntry(1, "Parth", "lodu", i1._employee.FirstName, 3);

    }
}



public class ExternalVisitor
{
    public int id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ClientName { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public int clientId { get; set; }
    public bool IsInside { get;set; }

}

public interface IEmployee
{
    public string LastName { get; set; }
    public int Id { get; set; }
    public string FirstName { get; set; }
}


public class Employee : IEmployee
{
    public string LastName { get; set; }
    public int Id { get; set; }
    public string FirstName { get; set; }
}


public class Unsubscribe : IDisposable
{

    IObserver<ExternalVisitor> _observer;

    List<IObserver<ExternalVisitor>> _observers; 
    public Unsubscribe(IObserver<ExternalVisitor> observer, ref List<IObserver<ExternalVisitor>> observers)
    {
        _observer = observer;
        _observers = observers;

    }
    public void Dispose()
    {
        if (_observers.Contains(_observer)){
            _observers.Remove(_observer);
        }
        
    }
}

public static class Formatter
{
    public enum TextOutputTheme
    {
        Security,
        Employee, 
        Reset

    }
    public static void ChangeOutputTheme(TextOutputTheme textOutputTheme)
    {
        if (textOutputTheme == TextOutputTheme.Employee)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (textOutputTheme == TextOutputTheme.Security)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ResetColor();
        }

    }
}


public class InternalContact : IObserver<ExternalVisitor>
{
    public IEmployee _employee = null;

    public InternalContact(int id, string firstname, string lastname)
    {
        _employee = new Employee
        {
            Id = id,
            FirstName = firstname,
            LastName = lastname
        };
    }


    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ExternalVisitor value)
    {
        Formatter.ChangeOutputTheme(Formatter.TextOutputTheme.Employee);
        Console.Write($"{value.ClientName}, You client {value.FirstName} {value.LastName} is here. Entry time : {value.EntryTime} ");
        Formatter.ChangeOutputTheme(Formatter.TextOutputTheme.Reset);
        Console.WriteLine();
    }
}

public class SecurityPersonal : IObserver<ExternalVisitor>
{
    IEmployee _employee = null;

    public SecurityPersonal(int id,string Firstname, string LastName)
    {
        _employee = new Employee
        {
            Id = id,
            FirstName = Firstname,
            LastName = LastName
        };
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ExternalVisitor value)
    {
        Formatter.ChangeOutputTheme(Formatter.TextOutputTheme.Security);
        Console.Write($"Security Notification: Guest{value.FirstName} {value.LastName} has enter the Building at {value.EntryTime}. His contact is {value.ClientName}.");
        Formatter.ChangeOutputTheme(Formatter.TextOutputTheme.Reset);
        Console.WriteLine();    

    }
}



public class ServailanceHub : IObservable<ExternalVisitor>
{

    public List<ExternalVisitor> _externalVisitors = null;
    public List<IObserver<ExternalVisitor>> _observers = null;

    public ServailanceHub()
    {
        _externalVisitors =  new List<ExternalVisitor>();
        _observers = new List<IObserver<ExternalVisitor>>();
    }


    public IDisposable Subscribe(IObserver<ExternalVisitor> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }

        foreach(ExternalVisitor visitor in _externalVisitors)
        {
            observer.OnNext(visitor);
        }
        Console.WriteLine($"Number of Subscriber: {_observers.Count}");

        return new Unsubscribe(observer, ref _observers);
    }

    public void ConfirmExternalVisitorEntry(int Id, string FirstName, string LastName, string ClientName,int clientId)
    {
        ExternalVisitor externalVisitor = new ExternalVisitor {

            id = Id,
            FirstName = FirstName,
            LastName = LastName,
            ClientName = ClientName,
            EntryTime = DateTime.Now,
            clientId = clientId,
            IsInside = true
        
        };

        _externalVisitors.Add(externalVisitor);

        foreach(IObserver<ExternalVisitor> observer in _observers)
        {
            observer.OnNext(externalVisitor);
        }

    }

    public void ConfirmExternalVisitorExit(ExternalVisitor externalVisitor) 
    {
        ExternalVisitor e = _externalVisitors.FirstOrDefault(e => e.id == externalVisitor.id);

        if(e != null)
        {
            e.ExitTime = DateTime.Now;
            e.IsInside = false;
        }

        foreach(IObserver<ExternalVisitor> observer in _observers)
        {
            observer.OnNext(e);
        }
    }

}
