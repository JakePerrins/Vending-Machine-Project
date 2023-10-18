using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.WebRequestMethods;

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
            FileStream ProductData;
            ProductData = new FileStream("ProductData.bin", FileMode.Append);

            ProductData.

        }

         /* File Interactions */

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
                //file doesnt exist so create it
                MyFile = new FileStream(FileName, FileMode.Create);
            }

            BinaryWriter MyFileWrite = new BinaryWriter(MyFile);

            MyFileWrite.Write(product.Name);
            MyFileWrite.Write(product.Price);
            MyFileWrite.Write(product.Stock);

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
