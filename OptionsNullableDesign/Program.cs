Person? mann = Person.Create("Thomas", "Mann");
Person? aristotle = Person.Create("Aristotle");
Person? p3 = null;

Book faustus = new Book { Title = "Doktor Faustus", Author = mann };
Book rhetoric = new Book { Title = "Rhetoric", Author = aristotle };
Book nights = new Book { Title = "One Thousand and One Nights" };

Console.WriteLine(GetLabel(mann));
Console.WriteLine(GetLabel(aristotle));
Console.WriteLine(GetLabel(p3));

//One question that came up during BBL is when he says "and that's all there is" in the video.
//I'm struggling to interpret exactly what he means as he follows up with something to the effect of lambdas and functions not being allowed.
//As we can see below, this is not not what he intends to say.
var x = nights?.Author?.Lastname?.Length ?? -1;
var y = rhetoric?.Author?.Lastname?.Substring(0, 3) ?? "unk";
var z = nights?.Illustrator()?.Lastname?.Substring(0, 3) ?? GetLabel(p3);

//Later on, when speaking about the functional implementation of GetLabel he concludes that this is precisely what you cannot do in the nullable version without branching.
//Apparently he does not consider using ?. and ?? as branching.
//I suppose that is questionable but he may be right.
//In any case, the point he is trying to make is in the GetBookLabel below. 

//This must be considered branching and I cannot for the life of me produce the two format variants using only .? and ??.
//Can you?
string GetBookLabel(Book book) =>
    GetLabel(book.Author) is string author ? $"{book.Title} by {author}" : book.Title;

//Rider suggests this pattern as the preferred, instead of the above. Fine, whatever.
string GetBookLabelV2(Book book) =>
    GetLabel(book.Author) is {} author ? $"{book.Title} by {author}" : book.Title;

//Here is another version.
string GetBookLabelV3(Book book) =>
    GetLabel(book.Author) switch
    {
        {} author => $"{book.Title} by {author}",
        null => book.Title
    };


string? GetLabel(Person? person)
{
    if (person is null) return null;

    string name =
        person.Lastname is null ? person.Firstname
            : $"{person.Firstname} {person.Lastname}";

    return name;
}

//For this class I've used the constructorless design with the required keyword, available in C# 11.
public class Book
{
    public required string Title { get; init; }
    public Person? Author { get; init; }
    public Person? Illustrator() => new Person("Imagius","Drawn");
}
//For this type I've used the primary constructor pattern available in the preview of C# 12 (June 2023).
public class Person(string firstname, string? lastname) //primary constructor
{
    public string Firstname => firstname;
    public string? Lastname => lastname;
    public static Person? Create(string firstname, string? lastname = null) =>
        new Person(firstname, lastname);
}
