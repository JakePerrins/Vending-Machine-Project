using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Xml.Serialization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Claims;

namespace Vending_Machine_Project
{
    internal class Program
    {

        public struct Product
        {
            public string Name;
            public double Price;
            public int Stock;
        }

        static void Main(string[] args)
        {
            /*------------------------------------------------------------------*/
            /* Variables                                                        */
            /*------------------------------------------------------------------*/

            string Title = ("--------------------"  + "\n" +
                            "| Vending Machine! |"  + "\n" +
                            "--------------------"  + "\n" );

            string mainOptions = ("1. Insert Coins" + "\n" +
                                  "2. Select Product" + "\n" +
                                  "3. Exit" + "\n" +
                                  "4. Admin" + "\n");

            string adminOptions = ("1. Enter Product" + "\n" +
                                  "2. Delete Product" + "\n" +
                                  "3. Change Stock" + "\n" +
                                  "4. Menu" + "\n");

            List<Product> products = new List<Product>();

            double total = 0.00;

            /*------------------------------------------------------------------*/

            /*------------------------------------------------------------------*/
            /* Main Program                                                     */
            /*------------------------------------------------------------------*/

            FileStream MyFile;

            try { MyFile = new FileStream("ProductData.bin", FileMode.Open); }   // Checking if PoductData exists and if not creating it.
            catch 
            {
                MyFile = new FileStream("ProductData.bin", FileMode.Create);
                MyFile.Close();

                AddProducts("ProductData.bin");

                products = GetProducts("ProductData.bin");
            }

            MyFile.Close();

            products = GetProducts("ProductData.bin"); // Loads products from the file into the list

            Console.WriteLine(Title + DisplayProducts(products, false));

            RESTART: // Main Loop

            Console.WriteLine(mainOptions); 

            int choice = (int) DeclareInput("", "System.Int32", "Enter a number: ");
            choice = RangeCheck(choice, 1, 4);

            switch (choice) 
            {
                case 1:

                    ClearLines(6);
                    InsertCoins(ref total); // Loops until exited
                    goto RESTART;

                case 2:

                    ClearLines(6);

                    choice = (int) DeclareInput("(0 for Menu) ", "System.Int32", "Enter a number: ");
                    choice = RangeCheck (choice, 0, FileLength("ProductData.bin")); // Gets an input for choice within the valid range

                    if (choice == 0) { Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false)); goto RESTART; } // Restarts

                    Product product = products[choice-1];

                    if (product.Stock == 0) // Restarts if product is out of stock
                    {
                        Console.Write("Out of stock: ");
                        Console.ReadLine();
                        Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));
                        goto RESTART;
                    }

                    if (total < product.Price) // Restarts if insufficient funds
                    {
                        Console.Write("Insufficient funds: ");
                        Console.ReadLine();
                        Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));
                        goto RESTART;
                    }

                    total -= product.Price; // Updating total
                    ClearLines(1); Console.WriteLine($"Total: £{total:f2}");

                    Console.ReadLine();

                    Product changeProduct = new Product(); // Could only change like so.

                    changeProduct.Name = products[choice - 1].Name;
                    changeProduct.Price = products[choice - 1].Price;
                    changeProduct.Stock = products[choice - 1].Stock - 1;

                    products[choice - 1] = changeProduct;

                    SaveProducts(products, "ProductData.bin"); // Saving change to file

                    Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));

                    goto RESTART;

                case 3:

                    ClearLines(1);

                    if (total > 0.00) // Refunding if money is left over
                    {
                        string refund = (string) DeclareInput($"Refund £{total:f2}? (y/n) ", "System.String", "");

                        if (refund == "y")
                        {
                            total = 0.00;
                            ClearLines(1); Console.WriteLine($"Total: £{total:f2}"); Console.ReadLine();
                        }

                        total = 0.00;
                    }

                    Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));

                    goto RESTART;

                case 4:

                    ClearLines(6);

                    string password = (string) DeclareInput("Enter the password: ", "System.String", ""); // Very secure password system

                    if (password != "SuperSecretDontShare")
                    {
                        Console.WriteLine("Access Denied. ");
                        Console.ReadLine();

                        Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));
                        goto RESTART;
                    }

                    ADMIN_RESTART: // Admin loop

                    Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false));

                    Console.WriteLine(adminOptions);

                    choice = (int) DeclareInput("", "System.Int32", "Enter a number: ");

                    switch (choice)
                    {
                        case 1:

                            ClearLines(6);

                            Product newProduct = new Product(); // Creating a new product to be made by the user

                            newProduct.Name = (string) DeclareInput("Enter the name: ", "System.String", "");
                            newProduct.Price = (double) DeclareInput("Enter the Price: ", "System.Double", "Enter a double: ");
                            newProduct.Stock = (int) DeclareInput("Enter the Stock: ", "System.Int32", "Enter a number: ");

                            ClearLines(3);

                            products.Add(newProduct); 

                            SaveProducts(products, "ProductData.bin"); // Saving product list to file

                            goto ADMIN_RESTART;

                        case 2:

                            ClearLines(6);

                            choice = (int) DeclareInput("Which product do you want to delete? (0 to cancel) ", "System.Int32", "Enter a number: "); // Asking for product

                            if (choice == 0) { goto ADMIN_RESTART; }

                            products.RemoveAt(choice - 1); // Removing from the list

                            SaveProducts(products, "ProductData.bin"); // Saving product list to file

                            goto ADMIN_RESTART;

                        case 3:

                            ClearLines(6);

                            choice = (int) DeclareInput("Which item do you want to change? (0 for cancel) ", "System.Int32", "Enter a number: "); // Asking for product

                            ClearLines(1);

                            int newStock = (int) DeclareInput("What is the new Stock? ", "System.Int32", "Enter a number: "); // Asking for new stock

                            changeProduct = new Product(); // Changing the list values to have the new stock

                            changeProduct.Name = products[choice - 1].Name;
                            changeProduct.Price = products[choice - 1].Price;
                            changeProduct.Stock = newStock;

                            products[choice - 1] = changeProduct;

                            SaveProducts(products, "ProductData.bin"); // Saving product list to file

                            goto ADMIN_RESTART;

                        case 4:

                            Console.Clear(); Console.WriteLine(Title + DisplayProducts(products, false)); // Back to main menu

                            goto RESTART;
                    }

                    break;

            }

            
            
        }

        /* User Interactions */

        static void InsertCoins(ref double total)
        {
            List<double> validValues = new List<double>() {1, 2, 5, 10, 20 ,50}; // List of valid values

            double coin = 1; // Temp coin value

            while (coin != 0) // Looping until exited
            {

                Console.WriteLine($"Total: £{total:f2}"); // Writing total

                coin = (double) DeclareInput("Enter Coin: (0 for menu) ", "System.Double", "Enter a number: "); // Coin input

                if ((validValues.IndexOf(coin) != -1) && (coin != 0)) // Checking if input is valid and not 0
                {
                    total = coin <= 2.00 ? total += coin : total += (coin / 100); // Adding coin in pounds or pence
                }

                else if (coin != 0) // Prompting for valid value if invalid
                {
                    Console.WriteLine("Invalid Value Enter (£1, £2, 5p, 10p, 20p, 50p): ");
                    Console.ReadLine();
                    ClearLines(2);
                }

                ClearLines(2);
            }
        }

        static string DisplayProducts(List<Product> products, bool Write)
        {
            string productString = "";

            for (int productCount = 1; productCount <= products.Count; productCount++) // Loops through product list
            {
                Product product = products[productCount - 1];

                productString += $"{productCount}: {product.Name}: £{product.Price:f2}, {product.Stock} in Stock\n\n"; // Adds to string

                if (Write)
                {
                    Console.WriteLine($"{productCount}: {product.Name}: £{product.Price:f2}, {product.Stock} in Stock\n"); // If write, write
                }
            }

            return productString;
        }

        /*-------------------------------------------------------------------------------*/

        /* File Interactions */

        static int FileLength(string FileName)
        {
            FileStream MyFile = new FileStream(FileName, FileMode.Open);

            BinaryReader MyFileReader = new BinaryReader(MyFile); // Making a binary reader for Product Data

            int length = 0;

            while (MyFile.Position < MyFile.Length)
            {
                MyFileReader.ReadString();
                MyFileReader.ReadDouble();
                MyFileReader.ReadInt32();

                length++; // Adds 1 to length for each product
            }

            MyFile.Close();

            return length;
        }

        static List<Product> GetProducts(string FileName)
        {
            FileStream MyFile = new FileStream(FileName, FileMode.Open);

            List<Product> products = new List<Product>();

            BinaryReader MyFileRead; // Trying to open a Binary Reader

            try
            {
                MyFileRead = new BinaryReader(MyFile);
            }
            catch
            {
                return products;
            }

            while (MyFile.Position < MyFile.Length) // Loop until end of file
            {
                Product product = new Product();

                product.Name = MyFileRead.ReadString();
                product.Price = MyFileRead.ReadDouble();
                product.Stock = MyFileRead.ReadInt32();

                products.Add(product); // Adds product to list
            }

            MyFileRead.Close();
            MyFile.Close();

            return products; // Returns List

        }
        static void SaveProducts(List<Product> products, string FileName)
        {
            FileStream MyFile = new FileStream(FileName, FileMode.Truncate);

            BinaryWriter MyFileWrite = new BinaryWriter(MyFile); // Opens a binary reader after deleting contents of the file

            for (int productCount = 0; productCount < products.Count; productCount++) // Loops through product list
            {
                Product product = products[productCount];

                MyFileWrite.Write(product.Name);
                MyFileWrite.Write(product.Price);
                MyFileWrite.Write(product.Stock);

            } // Adds products to file

            MyFileWrite.Close();
            MyFile.Close();
        }

        static void AddProducts(string FileName)
        {
            FileStream MyFile = new FileStream(FileName, FileMode.Append);

            BinaryWriter MyFileWrite = new BinaryWriter(MyFile); // Opening writer

            List<string> productsName = new List<string>()  {"Twix", "Yorkie", "Wispa", "Doritos", "Fanta", "Coca Cola"}   ; // List of products
            List<double> productsPrice = new List<double>() {2.00  , 2.50    , 2.50   , 2.00     , 1.50   , 1.00       }   ;
            List<int> productsStock = new List<int>()       {10    , 10      , 10     , 10       , 10     , 10         }   ;

            for (int product = 1; product <= 6; product++) // Loop for products
            {
                MyFileWrite.Write(productsName[product-1]);
                MyFileWrite.Write(productsPrice[product - 1]);
                MyFileWrite.Write(productsStock[product - 1]);
            } // Adds to file

            MyFileWrite.Close();
            MyFile.Close();
        }


        /*-------------------------------------------------------------------------------*/

        /* Improvements to interface */

        static void ClearLines(int numLines) // Method for clearing a certain number of lines.
        {
            for (int linesCleared = 0; linesCleared < numLines; linesCleared++) // Looping for number of lines specified.
            {
                Console.SetCursorPosition(0, Console.CursorTop);                                                          // Deletes a single line.
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
            }
        }

        /*-------------------------------------------------------------------------------*/

        /* Validation */

        static int RangeCheck(int num, int Min, int Max)
        {
            bool valid = true;

            do // Loops until valid
            {
                valid = true;
                if (num < Min || num > Max) // If not within range (inclusive)
                {
                    ClearLines(1);
                    Console.Write("Enter a valid number:  ");
                    num = (int) Validate(Console.ReadLine(), "System.Int32", "Enter a number: "); // Asks for a new input
                    valid = false;
                }

            } while (valid == false);

            return num;
        }

        static object Validate(object input, string targetType, string errorMessage) // Validation for a variable of any type. Parameter are input of any type, target type and error message.
        {
            Type type = Type.GetType(targetType); // Gets the target type from the given string.

            bool valid = false; // Assumes false.

            while (valid == false) // Loop while false.
            {
                try // tries to convert the input to the target type.
                {
                    input = Convert.ChangeType(input, type);
                    valid = true; // Ends loop if possible.
                }
                catch // Asks for a new input if not possible.
                {
                    ClearLines(1);
                    Console.Write(errorMessage);
                    input = Console.ReadLine();
                }
            }
            return input; // Returns input.
        }

        static object DeclareInput(string prompt, string targetType, string errorMessage) // Method for declaring an input on one line.
        {
            Type type = Type.GetType(targetType); // Gets target type to pass to validate.

            Console.Write(prompt); // Asks using prompt.

            object variable = Convert.ChangeType(Validate(Console.ReadLine(), targetType, errorMessage), type); // Validates and then changes user input to the correct type after.

            return variable; // Returning.
        }

        /*-------------------------------------------------------------------------------*/

    }
}
