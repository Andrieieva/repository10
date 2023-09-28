using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

internal class Program
{
    static string database = @"C:\users\Admin\source\repos\09-01\09-01\Phonebook.txt";
    static (string name, string phone, DateTime birth)[] contacts;

    static void Main(string[] args)
    {
        string[] records = ReadDatabaseAllTextLines(database);
        contacts = ConvertStringsToContacts(records);

        var fs = default(FileStream);
        try
        {
            // Opens a text file.
            fs = new FileStream(@"C:\users\Admin\source\repos\09-01\09-01\Phonebook.txt", FileMode.Open);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"[Data File Missing] {e}");
            throw new FileNotFoundException(@"[Phonebook.txt not in c:\users\Admin\source\repos\09-01\09-01 directory]", e);
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }

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

        ulong input = 0;
        bool tryAgain = true;
        while (tryAgain)
        {
            try
            {
                input = ulong.Parse(Console.ReadLine(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                tryAgain = false;
            }
            catch (FormatException)
            {
                Console.Write("You've entered a wrong choice, please try again: ");
            }
            catch (OverflowException)
            {
                Console.Write("Oleksii says that you suck at math, try a POSITIVE number: ");
            }
            catch (SystemException)
            {
                Console.WriteLine("Sorry, some system happened");
            }
            catch
            {
                Console.WriteLine("Sorry, idk what happened");
            }
            finally
            {
            }
        }

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
            case 7: // Option to close the phonebook
                Console.WriteLine("Closing the phonebook. Goodbye!");
                Environment.Exit(0); // Terminate the program
                break;
            default:
                Console.WriteLine("No such operation.");
                break;
        }
    }

    static void AddNewContact()
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

    static void EditContact()
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

    static (string name, string phone, DateTime date)[] ConvertStringsToContacts(string[] records)
    {
        var contacts = new (string name, string phone, DateTime date)[records.Length];
        try
        {
            for (int i = 0; i < records.Length; ++i)
            {
                string[] array = records[i].Split(','); // "Oleksii", "+38090873928", "30.03.1993"
                if (array.Length != 3)
                {
                    Console.WriteLine($"Line #{i + 1}: '{records[i]}' cannot be parsed");
                    continue;
                }
                contacts[i].name = array[0];
                contacts[i].phone = array[1];
                contacts[i].date = DateTime.Parse(array[2]);
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("An error occurred due to an invalid array index. Check your data.");
        }
        return contacts;
    }

    static void DeleteContact()
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

    static void SaveContactsToFile()
    {
        string[] lines = new string[contacts.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{contacts[i].Item1},{contacts[i].Item2},{contacts[i].Item3}";
        }
        File.WriteAllLines(database, lines);
    }

    static string[] ReadDatabaseAllTextLines(string file)
    {
        try
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"The file was not found");
                return null;
            }

            using (StreamReader sr = File.OpenText(file))
            {
                string line = sr.ReadLine();
                Console.WriteLine($"The first line of this file is: {line}");
                return File.ReadAllLines(file);
            }
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"The directory was not found");
        }
        catch (IOException)
        {
            Console.WriteLine($"The file could not be opened");
        }

        return null;
    }
}
