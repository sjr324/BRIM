using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Moq;
using System.Linq;
using BackendClassLibrary;

namespace BackendClassUnitTests
{
    public class InventoryTests
    {
        private List<Item> fakeDBItems;
        private List<Recipe> fakeDBRecipes;
        private int latestItemID;
        private int latestRecipeID;
        //create Mock of Database manager
        private Mock<IDatabaseManager> mockDBManager = new Mock<IDatabaseManager>();
        private Inventory inventory;

        public InventoryTests()
        {
            //set up Fake Database Data
            fakeDBItems = new List<Item> {
                new Drink {
                    ID = 1,
                    Name = "Fake Drink 1",
                    Price = 9.99,
                    LowerEstimate = 13000.00,
                    UpperEstimate = 15000.00,
                    ParLevel = 7000.00,
                    IdealLevel = 10000.00,
                    Measurement = unit.milliliters,
                    Status = status.aboveIdeal,
                    BottleSize = 200,
                    Brand = "Fake Brand 1",
                    UnitsPerCase = 10,
                    Vintage = false
                },
                new Drink {
                    ID = 2,
                    Name = "Fake Drink 2",
                    Price = 12.99,
                    LowerEstimate = 8500.00,
                    UpperEstimate = 9250.00,
                    ParLevel = 8000.00,
                    IdealLevel = 9750.00,
                    Measurement = unit.milliliters,
                    Status = status.belowIdeal,
                    BottleSize = 400,
                    Brand = "Fake Brand 2",
                    UnitsPerCase = 60,
                    Vintage = false
                },
                new Drink {
                    ID = 3,
                    Name = "Fake Drink 3",
                    Price = 16.99,
                    LowerEstimate = 5000.00,
                    UpperEstimate = 6000.00,
                    ParLevel = 6500.00,
                    IdealLevel = 9000.00,
                    Measurement = unit.milliliters,
                    Status = status.belowPar,
                    BottleSize = 200,
                    Brand = "Fake Brand 1",
                    UnitsPerCase = 10,
                    Vintage = true
                },
                new Drink {
                    ID = 4,
                    Name = "Fake Drink 4",
                    Price = 16.99,
                    LowerEstimate = 0.00,
                    UpperEstimate = 0.00,
                    ParLevel = 220.00,
                    IdealLevel = 300.00,
                    Measurement = unit.ounces,
                    Status = status.empty,
                    BottleSize = 25,
                    Brand = "Fake Brand 3",
                    UnitsPerCase = 14,
                    Vintage = false
                }
            };

            fakeDBRecipes = new List<Recipe> {
                new Recipe {
                    ID = 1,
                    Name = "Fake Recipe 1",
                    BaseLiquor = "Base Liquor 1",
                    ItemList = new List<(Item item, double quantity)> {
                        (fakeDBItems[0], 15.00)
                    }
                },
                new Recipe {
                    ID = 2,
                    Name = "Fake Recipe 2",
                    BaseLiquor = "Base Liquor 2",
                    ItemList = new List<(Item item, double quantity)> {
                        (fakeDBItems[0], 10.00),
                        (fakeDBItems[2], 15.00)
                    }
                },
                new Recipe {
                    ID = 3,
                    Name = "Fake Recipe 3",
                    BaseLiquor = "Base Liquor 3",
                    ItemList = new List<(Item item, double quantity)> {
                        (fakeDBItems[0], 8.00),
                        (fakeDBItems[2], 12.00),
                        (fakeDBItems[3], 2.30)
                    }
                },
            };
            
            //to simulate the autoincrement effect that the Database has for the Ids
            latestItemID = fakeDBItems.Count();
            latestRecipeID = fakeDBRecipes.Count();

            //Mockup Methods in mockDBManager
            //Mockups for Drink related Methods
            mockDBManager.Setup(x => x.deleteDrink(It.IsAny<Drink>()))
                .Returns((Drink drink) => {
                    
                    fakeDBItems = fakeDBItems.Where(x => x.ID != drink.ID).ToList();
                    return true;    
                    //If a delete Drink fails/returns false, it's probably not going to be the 
                    //Invetory Method's fault
                });

            mockDBManager.Setup(x => x.addDrink(It.IsAny<Drink>()))
                .Returns((Drink drink) => {
                    latestItemID += 1;
                    drink.ID = latestItemID;

                    fakeDBItems.Add(drink);
                    return true;
                });
            
            mockDBManager.Setup(x => x.updateDrink(It.IsAny<Drink>()))
                .Returns((Drink drink) => {
                    int itemToUpdateIndex = fakeDBItems.FindIndex(x => x.ID == drink.ID);
                    if (itemToUpdateIndex != -1) {
                        fakeDBItems[itemToUpdateIndex] = drink;
                        return true;
                    } else {
                        return false;
                    }
                });

            mockDBManager.Setup(x => x.getDrinks()).Returns(fakeDBItems);

            mockDBManager.Setup(x => x.deleteRecipe(It.IsAny<int>()))
                .Returns((int recipeID) => {
                    int recipeToDeleteIndex = fakeDBRecipes.FindIndex(x => x.ID == recipeID);
                    if (recipeToDeleteIndex == -1) {   
                        //entry not being present in DB doesn't count as the operation failing
                        return true; 
                    } else if (fakeDBRecipes[recipeToDeleteIndex].ItemList.Count() > 0) {
                        //entry having a foriegn Key in a anothr table WOULD make a delete operation fail
                        return false;
                    } else {
                        fakeDBRecipes.RemoveAt(recipeToDeleteIndex);
                        return true;
                    }    
                });

            mockDBManager.Setup(x => x.deleteDrinkRecipesByRecipeID(It.IsAny<int>()))
                .Returns((int recipeID) => {
                   Recipe recipe = fakeDBRecipes.FirstOrDefault(x => x.ID == recipeID);
                   if (recipe != null) {
                       recipe.ItemList = new List<(Item item, double quantity)>();
                   }
                   return true;   
                });

            mockDBManager.Setup(x => x.addRecipe(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string name, string baseLiquor) => {
                    latestRecipeID += 1;
                    Recipe recipeToAdd = new Recipe {
                        ID = latestRecipeID,
                        Name = name,
                        BaseLiquor = baseLiquor
                    };
                    
                    fakeDBRecipes.Add(recipeToAdd);
                    return latestRecipeID;
                });
                
            mockDBManager.Setup(x => x.addDrinkRecipe(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>()))
                .Returns((int recipeID, int drinkID, double itemQuantity) => {
                    Drink drink = (Drink) fakeDBItems.FirstOrDefault(x => x.ID == drinkID);
                    Recipe recipe = fakeDBRecipes.FirstOrDefault(x => x.ID == recipeID);

                    if (drink == null || recipe == null || itemQuantity < 0) {
                        return -1;
                    } else {
                        recipe.ItemList.Add((drink, itemQuantity));
                        return 0;   //we don't care about Drink Recipe Ids anyways
                    }
                });

            mockDBManager.Setup(x => x.updateRecipe(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((int recipeID, string name, string baseLiquor) => {
                    Recipe recipe = fakeDBRecipes.FirstOrDefault(x => x.ID == recipeID);
                    if (recipe != null) {
                        recipe.Name = name;
                        recipe.BaseLiquor = baseLiquor;
                        return true;
                    } else {
                        return false; 
                    }
                });

            mockDBManager.Setup(x => x.getRecipes()).Returns(fakeDBRecipes);
            inventory = new Inventory(mockDBManager.Object);
        }

        [Fact]
        public void GetItemListTest() {
            //Arrange

            //Act
            inventory.GetItemList();

            //Assert
            string resultOfGetString = JsonConvert.SerializeObject(inventory.ItemList);
            string fakeDBItemsString = JsonConvert.SerializeObject(fakeDBItems);
            Assert.Equal(resultOfGetString, fakeDBItemsString);
        }

        [Fact]
        public void UpdateItemTestSuccess() {
            //Arrange
            Drink updatedItem1 = new Drink {
                ID = 1,
                Name = "Updated Drink 1",
                Price = 9.99,
                LowerEstimate = 12750.00,
                UpperEstimate = 14250.00,
                ParLevel = 7000.00,
                IdealLevel = 10000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 200,
                Brand = "Fake Brand 4",
                UnitsPerCase = 10,
                Vintage = false
            };

            //Act
            inventory.UpdateItem(updatedItem1);
            
            //Assert
            Assert.Equal(fakeDBItems[0].Name, "Updated Drink 1");
            Assert.Equal(fakeDBItems[0].LowerEstimate, 12750.00);
            Assert.Equal(fakeDBItems[0].UpperEstimate, 14250.00);
            Assert.Equal(((Drink) fakeDBItems[0]).Brand, "Fake Brand 4");
        }

        [Fact]
        public void updateItemTestFailure_NonExistentDrink() {
            //Arrange
            Drink fakeItem = new Drink {
                ID = -1,
                Name = "FakeItem 1",
                Price = 0.99,
                LowerEstimate = 12750.00,
                UpperEstimate = 14250.00,
                ParLevel = 7000.00,
                IdealLevel = 10000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 200,
                Brand = "Fake Brand 4",
                UnitsPerCase = 10,
                Vintage = false
            };

            //Act
            int returnCode = inventory.UpdateItem(fakeItem);

            //assert
            Assert.Equal(returnCode, 0);
        }

        [Fact]
        public void AddItemTestSuccess() {
            //Arrange
            Drink newItem = new Drink {
                Name = "New Item",
                Price = 1.99,
                LowerEstimate = 24750.00,
                UpperEstimate = 30000.00,
                ParLevel = 17000.00,
                IdealLevel = 35000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 450,
                Brand = "Fake Brand 4",
                UnitsPerCase = 7,
                Vintage = false
            };
            int oldDBItemCount = fakeDBItems.Count();

            //Act
            inventory.AddItem(newItem);

            //Assert
            Assert.Equal(fakeDBItems.Count(), oldDBItemCount + 1);
            var addedItem = fakeDBItems.FirstOrDefault(x => x.ID == latestItemID);
            string addedItemString = addedItem != null ? JsonConvert.SerializeObject(addedItem) : "";
            string newItemString = JsonConvert.SerializeObject(newItem);
            Assert.Equal(newItemString, addedItemString);
        }

        [Fact]
        public void removeItemTestSuccess() {
            //Arrange
            var itemToRemove = fakeDBItems[1];
            int oldDBItemCount = fakeDBItems.Count();

            //Act
            inventory.RemoveItem(itemToRemove);

            //Assert
            Assert.Equal(fakeDBItems.Count(), oldDBItemCount - 1);
            Assert.Equal(fakeDBItems.FirstOrDefault(x => x.ID == itemToRemove.ID), null);
        }
    }
}
