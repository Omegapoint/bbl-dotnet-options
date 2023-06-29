Person mann = Person.Create("Thomas", "Mann");
Person aristotle = Person.Create("Aristotle");
//Person? p3 = null;
//The above is included in the nullable design. In the video it is subtly removed, making a throw possible in GetLabel.
//If we don't have a null guard in GetLabel then perhaps it should also take an Option<Person>. C# is not a functional language. Functional constructs have a tendency to proliferate. 

Book faustus = new Book { Title = "Doktor Faustus", Author = Option<Person>.Some(mann) };
Book rhetoric = new Book { Title = "Rhetoric", Author = Option<Person>.Some(aristotle) };
Book nights = new Book { Title = "One Thousand and One Nights" };//Explicit initialization of Author not necessary. I invite the user to check that this results in Option<Person>.None().

Console.WriteLine(GetLabel(mann));
Console.WriteLine(GetLabel(aristotle));
Console.WriteLine(GetBookLabel(faustus));
Console.WriteLine(GetBookLabel(rhetoric));
Console.WriteLine(GetBookLabel(nights));

//Note: no null guard on the actual Person. We rely on nullable reference checks.
string GetLabel(Person person) => person
    .Lastname
    .Map(lastname => $"{person.Firstname} {lastname}")
    .Reduce(person.Firstname);

string GetBookLabel(Book book) => book
    .Author
    .Map(GetLabel)
    .Map(author => $"{book.Title} by {author}")
    .Reduce(book.Title);

//Some experiments with value types and non-null defaults.
var optionalInt = Option<int>.Some(5);
var noneInt = Option<int>.None();

Console.WriteLine(optionalInt.Reduce(-1)); //Prints 5 as expected.
Console.WriteLine(noneInt.Reduce(-1)); //Prints 0, which is default(int).
                                       //For non-nullable value types there is never an alternate Reduce case. 

//One thing glossed over in the video was, what is the purpose of the reference type constraint and can you do without?
//The generic monad at least compiles without the  constraint (where T : class).
class Monad<T> where T : class
{
    public static Monad<T> Create(T obj) => new();
    public Monad<TResult> Bind<TResult>(Func<T, TResult> map) where TResult : class => new();
}

//As demonstrated, this also appears to work fine without the reference type constraint, with a minor modification.
//Not sure of the benefit of the constraint. I will include some experiments with value types.
public class Option<T>// where T : class
{
    private T? _object = default; 

    public static Option<T> Some(T obj) => new() { _object = obj };
    public static Option<T> None() => new();

    public Option<TResult> Map<TResult>(Func<T, TResult> map)//where TResult : class 
        => _object is null ? Option<TResult>.None() : Option<TResult>.Some(map(_object));

    public T Reduce(T @default) => _object ?? @default;
}

//For this class I've used the constructorless design with the required keyword, available in C# 11.
//In the video there is a regular constructor with the deconstruction pattern, commented in the code below. Also quite nice!
public class Book
{
    // public Book(string title, Option<Person> author) =>
    //     (Title, Author) = (title, author);
    public required string Title { get; init; }
    public Option<Person> Author { get; init; } = Option<Person>.None();
}
//For this type I've used the primary constructor pattern available in the preview of C# 12 (June 2023).
public class Person(string firstname, Option<string> lastname)
{
    public string Firstname => firstname;
    public Option<string> Lastname => lastname;
    public static Person Create(string firstname, string lastname) => new Person(firstname, Option<string>.Some(lastname));
    public static Person Create(string name) => new Person(name, Option<string>.None());

}
