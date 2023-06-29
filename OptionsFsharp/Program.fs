//These types are called records. See below how a record is constructed.
type Person = {
    Firstname: string
    Lastname: string option
}

type Book = {
    Title: string
    Author: Person option
}

//Using factory methods to create the book. Another common design is to have different types for each of the variants. 
let createBook title =
    {Title=title; Author = None }
let createBookWithAuthor title author =
    {createBook title with Author = Some(author)}

let GetLabel person =
    match person with
        | {Firstname = firstname; Lastname = Some(lastname)} -> $"{firstname} {lastname}"
        | {Firstname = firstname} -> $"{firstname}"
    
//Variant where I'm deconstructing the book already in the argument.
let GetBookLabel {Title = title; Author = author} =
    match author with
        | Some(author) -> $"{title} by {GetLabel author}" //Note, I've redefined author as string.
                                                          //I could have used another name. This is called shadowing.
        | _ -> $"{title}" //Rest case, same as in C#.

let mann = {Firstname = "Thomas"; Lastname = Some("Mann") }
let aristotle = {Firstname = "Aristotle"; Lastname = None}
let faustus = createBookWithAuthor "Doktor Faustos" mann
let rhetoric = createBookWithAuthor "Rhetoric" aristotle
let nights = createBook "One Thousand and One Nights"

printfn $"{GetBookLabel faustus}"
printfn $"{GetBookLabel rhetoric}"
printfn $"{GetBookLabel nights}"
