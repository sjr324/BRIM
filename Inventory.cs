using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM
{
    //The main class of the program. 
    public class Inventory
    {
        public List<Item> ItemList = new List<Item>(); //holds all items registered to this BRIM instance
        public List<Recipe> RecipeList = new List<Recipe>(); //holds all of the recipes for this BRIM instance
        public string Country;

        public int UpdateItem(Item info)
        {
            return 0;
        }

        public int AddItem(Item i)
        {
            return 0;
        }

        public int RemoveItem(Item i)
        {
            return 0;
        }

        public int PurchseItem(Recipe r)
        {
            return 0;
        }

        /// <summary>
        /// Makes a call to the drinks table of the brim database to get the list of all drinks 
        /// that the user has registerd. 
        /// Creates a new item list and replaces the global ItemList with that when it is finished.
        /// In the future will also query for any other items a user can have in their inventory
        /// such as food.
        /// </summary>
        /// <returns>an integer value that represents the exit status of the method</returns>
        public int GetItemList()
        {
            List<Item> newList = new List<Item>();
            DataTable dt = new DataTable();
            int rowsReturned;
            string connString = @"SERVER=68.84.78.85;PORT=3306;DATABASE=brim;UID=dev;PASSWORD=devpassword;";
            //read from drink table and add to item list, can do food later down the line
            string query = @"select * from drinks";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            conn.Open();
            rowsReturned = adapter.Fill(dt);
            conn.Close();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("The database query returned no data");
            } else
            {
                foreach(DataRow dr in dt.Rows)
                {
                    Drink tempDrink = new Drink();
                    tempDrink.Name = dr.Field<string>("name");
                    tempDrink.LowerEstimate = dr.Field<double>("lowerEstimate");
                    tempDrink.UpperEstimate = dr.Field<double>("upperEstimate");
                    tempDrink.Measurement = dr.Field<unit>("measurmentUnit");
                    tempDrink.ParLevel = dr.Field<double>("parLevel");
                    tempDrink.IdealLevel = dr.Field<double>("idealLevel");
                    tempDrink.BottleSize = dr.Field<int>("bottleSize");
                    tempDrink.Brand = dr.Field<string>("brand");
                    tempDrink.UnitsPerCase = dr.Field<int>("bottlesPerCase");
                    tempDrink.Vintage = dr.Field<int>("vintage");

                    newList.Add(tempDrink);
                }
            }

            ItemList = newList;

            return 0;
        }

        public void EmitEvent()
        {

        }

        public void ListenEvent()
        {

        }

        /// <summary>
        /// This method gets all of the recipes that have been registered in the dratabase and adds them to the 
        /// global recipeList variable. It overwrites the current global variable when it reads from the database.
        /// </summary>
        /// <returns>It returns an int representing the status of the request</returns>
        public int GetRecipeList()
        {
            List<Recipe> newList = new List<Recipe>();
            DataTable dt = new DataTable();
            int rowsReturned;
            string connString = @"SERVER=68.84.78.85;PORT=3306;DATABASE=brim;UID=dev;PASSWORD=devpassword;";
            //read from drink table and add to item list, can do food later down the line
            string query = @"select * from recipes";

            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            conn.Open();
            rowsReturned = adapter.Fill(dt);
            conn.Close();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("The database query returned no data");
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //TODO: Build out recipe grabbing fucntionality. takes a lot more calls to do
                }
            }

            RecipeList = newList;

            return 0;
        }

        public int AddRecipe()
        {
            return 0;
        }

        public int UpdateRecipe()
        {
            return 0;
        }

        public int RemoveRecipe()
        {
            return 0;
        }
    }
}
