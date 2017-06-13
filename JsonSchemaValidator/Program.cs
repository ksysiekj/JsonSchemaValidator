using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace JsonSchemaValidator
{
    class Program
    {
        private const string JsonSchema = @"{
   ""properties"":{
    
      ""Electric"":{
         ""interval"":4,
         ""properties"":{
            ""I1"":{
               ""description"":""Current 1"",
               ""type"":""object"",
               ""unit"":""A""
            },
            ""I2"":{
""description"":""Current 2"",
               ""type"":""object"",
               ""unit"":""A""
            },
            ""I3"":{
               ""description"":""Current 3"",
               ""type"":""object"",
               ""unit"":""A""
            }
         },
         ""type"":""object""
      },
      ""Group2"":{
         ""interval"":60,
         ""properties"":{
            ""Param1"":{
               ""description"":""Parametr testowy ogólny""
            }
         },
         ""type"":""object""
      }
   },
   ""title"":""schemat encji pomiarowej"",
   ""type"":""object""
}";

        private const string TestEntityJson = @"{
   ""Diagnostics"":{
      ""P1"":123
   },
 
     ""Electric"":{
      ""I1"":1,
      ""I2"":3,
      ""U"":230,
      ""I3"":2 
   }
}";


        static void Main(string[] args)
        {
            MeasureTime(RemoveSchemaInvalidData);

            Console.ReadLine();
        }

        private static void RemoveSchemaInvalidData()
        {
            JSchema schema = JSchema.Parse(JsonSchema);
            JObject entity = JObject.Parse(TestEntityJson);

            Console.WriteLine("Input:");
            Console.WriteLine(entity);

            IList<ValidationError> errors;
            bool isValid = entity.IsValid(schema, out errors);

            if (!isValid)
            {
                RemoveInvalidJTokens(errors, entity);
            }

            Console.WriteLine("Output:");
            Console.WriteLine(entity);
        }

        private static void RemoveInvalidJTokens(IList<ValidationError> errors, JObject entity)
        {
            Console.WriteLine("\nValidation errors:");

            foreach (ValidationError error in errors)
            {
                Console.WriteLine(
                    $"[ERROR] : {error.Message} ({error.LineNumber},{error.LinePosition})");

                JToken token = entity.SelectToken(error.Path);
                token?.Parent.Remove();
            }

            Console.WriteLine();
        }


        private static void MeasureTime(Action action)
        {
            Stopwatch stopwatch=new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();

            Console.WriteLine($"\nElapsed: {stopwatch.Elapsed.TotalMilliseconds}[ms]");
        }
    }
}
