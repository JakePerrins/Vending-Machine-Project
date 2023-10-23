using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Xml.Serialization;
using System.Net;

namespace Vending_Machine_Project
{
    internal class Program
    {

        struct Product
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



            double total = 0.00;

            /*------------------------------------------------------------------*/

            List<Product> products = GetProducts("ProductData.bin");

            Console.WriteLine(products);

            FileStream ProductData;
            ProductData = new FileStream("ProductData.bin", FileMode.Append);

            if (ProductData.Length == 0)
            {
                AddProducts(ProductData);
                products = GetProducts("ProductData.bin");
            }

            Console.WriteLine(Title);

            RESTART:

            Console.WriteLine(mainOptions);

            int choice = (int) DeclareInput("", "System.Int32", "Enter a number: ");
            choice = RangeCheck(choice, 1, 4);

            switch (choice) 
            {
                case 1:

                    ClearLines(6);
                    InsertCoins(ref total);
                    goto RESTART;

                case 2:



                    break;

                case 3:


                    break;

                case 4:


                    break;

            }

            
            
        }

        /* User Interactions */

        static void InsertCoins(ref double total)
        {
            List<double> validValues = new List<double>() {1, 2, 5, 10, 20 ,50};

            double coin = 1;

            while (coin != 0)
            {

                Console.WriteLine($"Total: £{total:f2}");

                coin = (double) DeclareInput("Enter Coin: (0 for menu) ", "System.Double", "Enter a number: ");

                if ((validValues.IndexOf(coin) != -1) && (coin != 0))
                {
                    total = coin <= 2.00 ? total += coin : total += (coin / 100);
                }

                else if (coin != 0)
                {
                    Console.WriteLine("Invalid Value Enter (£1, £2, 5p, 10p, 20p, 50p): ");
                    Console.ReadLine();
                    ClearLines(2);
                }

                ClearLines(2);
            }
        }

        /*-------------------------------------------------------------------------------*/

        /* File Interactions */

        static List<Product> GetProducts(string FileName)
        {
            FileStream MyFile;

            List<Product> products = new List<Product>();

            try
            {
                MyFile = new FileStream(FileName, FileMode.Append);
                // Opens File In append
            }
            catch
            {
                // File doesnt exist so create it
                MyFile = new FileStream(FileName, FileMode.Create);
            }

            BinaryReader MyFileRead;

            try
            {
                MyFileRead = new BinaryReader(MyFile);
            }
            catch
            {
                MyFile.Close();
                return products;
            }

            int position = 0;

            while (position < MyFile.Length)
            {
                Product product = new Product();

                product.Name = MyFileRead.ReadString();
                product.Price = MyFileRead.ReadDouble();
                product.Stock = MyFileRead.ReadInt32();

                products.Add(product);
                position++;
            }

            MyFile.Close();

            return products;

        }
        static void SaveProducts(Product product, string FileName)
        {
            FileStream MyFile;

            try
            {
                MyFile = new FileStream(FileName, FileMode.Append);
                // Opens File In append
            }
            catch
            {
                // File doesnt exist so create it
                MyFile = new FileStream(FileName, FileMode.Create);
            }

            BinaryWriter MyFileWrite = new BinaryWriter(MyFile);

            MyFileWrite.Write(product.Name);
            MyFileWrite.Write(product.Price);
            MyFileWrite.Write(product.Stock);

            MyFile.Close();
        }

        static void AddProducts(FileStream MyFile)
        {
            BinaryWriter MyFileWrite = new BinaryWriter(MyFile);

            List<string> productsName = new List<string>()  {"Twix", "Yorkie", "Wispa", "Doritos", "Fanta", "Coca Cola"}   ;
            List<double> productsPrice = new List<double>() {2.00  , 2.50    , 2.50   , 2.00     , 1.50   , 1.00       }   ;
            List<int> productsStock = new List<int>()       {10    , 10      , 10     , 10       , 10     , 10         }   ;

            for (int product = 1; product <= 6; product++)
            {
                MyFileWrite.Write(productsName[product-1]);
                MyFileWrite.Write(productsPrice[product - 1]);
                MyFileWrite.Write(productsStock[product - 1]);
            }



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

            do
            {
                valid = true;
                if (num < Min || num > Max)
                {
                    ClearLines(1);
                    Console.Write("Enter a valid number:  ");
                    num = (int)Validate(Console.ReadLine(), "System.Int32", "Enter a number: ");
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
