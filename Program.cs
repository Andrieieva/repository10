using System;

internal class Program
{
    static string database = @"./Phonebook.txt";
    static (string name, string phone, DateTime birth)[] contacts;

    static void Main(string[] args)
    {
        string[] records = ReadDatabaseAllTextLines(database);
        contacts = ConvertStringsToContacts(records);

        while (true)
        {
            UserInteraction();
        }
    }

    static void UserInteraction()
    {
        Console.WriteLine("1. Write all contacts");
        Console.WriteLine("2. Add new contact");
        Console.WriteLine("3. Edit contact");
        Console.WriteLine("4. Search by name");
        Console.WriteLine("5. Delete contact");
        Console.WriteLine("6. Save");
        Console.WriteLine("7. Close the phonebook");
        Console.Write("Enter your choice: ");

        int input;
        if (int.TryParse(Console.ReadLine(), out input))
        {
            try
            {
                switch (input)
                {
                    case 1:
                        WriteAllContactsToConsole();
                        break;
                    case 2:
                        AddNewContact();
                        break;
                    case 3:
                        EditContact();
                        break;
                    case 4:
                        SearchContact();
                        break;
                    case 5:
                        DeleteContact();
                        break;
                    case 6:
                        SaveContactsToFile();
                        break;
                    case 7:
                        Console.WriteLine("Closing the phonebook. Goodbye!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("No such operation.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid numeric choice.");
        }
    }
    static (string name, string phone, DateTime birth)[] ConvertStringsToContacts(string[] records)
    {
        var contacts = new (string name, string phone, DateTime birth)[records.Length];
        for (int i = 0; i < records.Length; ++i)
        {
            string[] array = records[i].Split(',');
            if (array.Length != 3)
            {
                Console.WriteLine($"Line #{i + 1}: '{records[i]}' cannot be parsed");
                continue;
            }
            contacts[i].name = array[0];
            contacts[i].phone = array[1];
            contacts[i].birth = DateTime.ParseExact(array[2], "dd.MM.yyyy", null);
        }
        return contacts;
    }

    static void AddNewContact()
    {
        try
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Enter Date of Birth (dd.mm.yyyy): ");
            DateTime birth;
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birth))
            {
                var newContact = (name, phone, birth);
                contacts = contacts.Concat(new[] { newContact }).ToArray();
                Console.WriteLine("Contact added successfully.");
            }
            else
            {
                Console.WriteLine("Invalid date format. Contact not added.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding a contact: {ex.Message}");
        }
    }

    static void EditContact()
    {
        try
        {
            Console.Write("Enter the name to edit: ");
            string nameToEdit = Console.ReadLine();
            var indexToEdit = -1;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].name.Equals(nameToEdit, StringComparison.OrdinalIgnoreCase))
                {
                    indexToEdit = i;
                    break;
                }
            }
            if (indexToEdit != -1)
            {
                Console.Write("Enter new Name: ");
                string newName = Console.ReadLine();
                Console.Write("Enter new Phone: ");
                string newPhone = Console.ReadLine();
                Console.Write("Enter new Date of Birth (dd.mm.yyyy): ");
                DateTime birth;

                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birth))
                {
                    var updatedContact = (newName, newPhone, birth);
                    contacts[indexToEdit] = updatedContact;
                    Console.WriteLine("Contact edited successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid date format. Contact not edited.");
                }
            }
            else
            {
                Console.WriteLine($"Contact with name '{nameToEdit}' not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while editing a contact: {ex.Message}");
        }
    }

    static void SearchContact()
    {
        try
        {
            Console.Write("Enter the name to search for: ");
            string nameToSearch = Console.ReadLine();

            List<(string name, string phone, DateTime birth)> matchingContacts = new List<(string name, string phone, DateTime birth)>();

            foreach (var contact in contacts)
            {
                if (contact.name.Contains(nameToSearch, StringComparison.OrdinalIgnoreCase))
                {
                    matchingContacts.Add(contact);
                }
            }

            if (matchingContacts.Count > 0)
            {
                Console.WriteLine("Matching Contacts:");
                foreach (var contact in matchingContacts)
                {
                    int age = DateTime.Now.Year - contact.birth.Year;
                    Console.WriteLine($"Name: {contact.name}, Phone: {contact.phone}, Age: {age}");
                }
            }
            else
            {
                Console.WriteLine($"No contacts found with the name '{nameToSearch}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while searching for contacts: {ex.Message}");
        }
    }


    static void WriteAllContactsToConsole()
    {
        for (int i = 0; i < contacts.Length; i++)
        {
            int age = DateTime.Now.Year - contacts[i].birth.Year;
            Console.WriteLine($"#{i + 1}: Name: {contacts[i].Item1}, Phone: {contacts[i].Item2}, Age: {age}");
        }
    }

    static void DeleteContact()
    {
        try
        {
            Console.Write("Enter the name to delete: ");
            string nameToDelete = Console.ReadLine();
            var indexToDelete = -1;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].name.Equals(nameToDelete, StringComparison.OrdinalIgnoreCase))
                {
                    indexToDelete = i;
                    break;
                }
            }
            if (indexToDelete != -1)
            {
                var updatedContacts = new (string name, string phone, DateTime birth)[contacts.Length - 1];
                for (int i = 0, j = 0; i < contacts.Length; i++)
                {
                    if (i != indexToDelete)
                    {
                        updatedContacts[j] = contacts[i];
                        j++;
                    }
                }

                contacts = updatedContacts;
                Console.WriteLine("Contact deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Contact with name '{nameToDelete}' not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting a contact: {ex.Message}");
        }
    }

    static void SaveContactsToFile()
    {
        try
        {
            string[] lines = new string[contacts.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"{contacts[i].Item1},{contacts[i].Item2},{contacts[i].Item3:dd.MM.yyyy}";
            }
            File.WriteAllLines(database, lines);
            Console.WriteLine("Contacts saved to file successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving contacts to a file: {ex.Message}");
        }
    }

    static string[] ReadDatabaseAllTextLines(string file)
    {
        try
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
            }
            return File.ReadAllLines(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the database file: {ex.Message}");
            return new string[0];
        }
    }
}
