using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Moq;
using System.Linq;
using BRIM.BackendClassLibrary;

namespace BackendClassUnitTests
{
    public class InventoryTests
    {
        private List<Item> fakeDBItems;
        private List<Recipe> fakeDBRecipes;
        private List<Tag> fakeDBTags;
        private int latestItemID;
        private int latestRecipeID;
        private int latestTagID;
        //create Mock of Database manager
        private Mock<IDatabaseManager> mockDBManager = new Mock<IDatabaseManager>();
        private Inventory inventory;

        public InventoryTests()
        {
            //set up Mock Database Data
            fakeDBTags = new List<Tag> {
                new Tag {
                    ID = 1,
                    Name = "Fake Tag 1"
                },
                new Tag {
                    ID = 2,
                    Name = "Fake Tag 2"
                },
                new Tag {
                    ID = 3,
                    Name = "Fake Tag 3"
                }
            };

            fakeDBItems = new List<Item> {
                new Drink {
                    ID = 1,
                    Name = "Fake Drink 1",
                    Price = 9.99,
                    Estimate = 14000.00,
                    ParLevel = 7000.00,
                    IdealLevel = 10000.00,
                    Measurement = unit.milliliters,
                    Status = status.aboveIdeal,
                    BottleSize = 200,
                    Brand = "Fake Brand 1",
                    UnitsPerCase = 10,
                    Vintage = null,
                    Tags = new List<Tag> {
                        fakeDBTags[0]
                    }
                },
                //drink not used in Any Recipes
                new Drink {
                    ID = 2,
                    Name = "Fake Drink 2",
                    Price = 12.99,
                    Estimate = 8875.00,
                    ParLevel = 8000.00,
                    IdealLevel = 9750.00,
                    Measurement = unit.milliliters,
                    Status = status.belowIdeal,
                    BottleSize = 400,
                    Brand = "Fake Brand 2",
                    UnitsPerCase = 60,
                    Vintage = null,
                    Tags = new List<Tag> {
                        fakeDBTags[1]
                    }
                },
                new Drink {
                    ID = 3,
                    Name = "Fake Drink 3",
                    Price = 16.99,
                    Estimate = 5500.00,
                    ParLevel = 6500.00,
                    IdealLevel = 9000.00,
                    Measurement = unit.milliliters,
                    Status = status.belowPar,
                    BottleSize = 200,
                    Brand = "Fake Brand 1",
                    UnitsPerCase = 10,
                    Vintage = 1998,
                    Tags = new List<Tag> {
                        fakeDBTags[0],
                        fakeDBTags[1]
                    }
                },
                new Drink {
                    ID = 4,
                    Name = "Fake Drink 4",
                    Price = 16.99,
                    Estimate = 0.00,
                    ParLevel = 220.00,
                    IdealLevel = 300.00,
                    Measurement = unit.ounces,
                    Status = status.empty,
                    BottleSize = 25,
                    Brand = "Fake Brand 3",
                    UnitsPerCase = 14,
                    Vintage = null,
                    Tags = new List<Tag> {
                        fakeDBTags[2]
                    }
                }
            };

            fakeDBRecipes = new List<Recipe> {
                new Recipe {
                    ID = 1,
                    Name = "Fake Recipe 1",
                    BaseLiquor = "Base Liquor 1",
                    ItemList = new List<RecipeItem> {
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[0], 
                            Quantity = 15.00
                        }
                    }
                },
                new Recipe {
                    ID = 2,
                    Name = "Fake Recipe 2",
                    BaseLiquor = "Base Liquor 2",
                    ItemList = new List<RecipeItem> {
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[0], 
                            Quantity = 10.00
                        },
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[2], 
                            Quantity = 15.00
                        }
                    }
                },
                new Recipe {
                    ID = 3,
                    Name = "Fake Recipe 3",
                    BaseLiquor = "Base Liquor 3",
                    ItemList = new List<RecipeItem> {
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[0], 
                            Quantity = 8.00
                        },
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[2], 
                            Quantity = 12.00
                        },
                        new RecipeItem {
                            Item = (Drink) fakeDBItems[3],
                            Quantity = 2.30
                        }
                    }
                },
            };
            
            //to simulate the autoincrement effect that the Database has for the Ids
            latestItemID = fakeDBItems.Count();
            latestRecipeID = fakeDBRecipes.Count();
            latestTagID = fakeDBTags.Count();

            //Mockup Methods in mockDBManager
            //Mockups for Drink related Methods
            mockDBManager.Setup(x => x.deleteDrink(It.IsAny<Drink>()))
                .Returns((Drink drink) => {
                    bool drinkUnused = true;
                    //go through all recipe Items and check to see if Drink in used in any of them
                    //DataBase will (or at least should), prevent deletions of Drinks with connected
                    //entries in the DrinkRecipes Table
                    for (int i = 0; i < latestRecipeID; i++) {
                        for (int j = 0; j < fakeDBRecipes[i].ItemList.Count(); j++) {
                            if (fakeDBRecipes[i].ItemList[j].Item.ID == drink.ID) {
                                drinkUnused = false;
                                break;
                            }
                        }
                        if (!drinkUnused) {
                            break;
                        }
                    }

                    //If a Recipe uses that drink, drink deletion will Fail
                    if (!drinkUnused) {
                        return false;
                    }

                    fakeDBItems = fakeDBItems.Where(x => x.ID != drink.ID).ToList();
                    return true;    
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
                        //preserve Item's tag's since updateDrink does not change entries within 
                        //DrinkTags table
                        List<Tag> tagListHolder = ((Drink) fakeDBItems[itemToUpdateIndex]).Tags;
                        fakeDBItems[itemToUpdateIndex] = drink;
                        ((Drink) fakeDBItems[itemToUpdateIndex]).Tags = tagListHolder;
                        return true;
                    } else {
                        return false;
                    }
                });

            mockDBManager.Setup(x => x.getDrinks()).Returns(fakeDBItems);

            mockDBManager.Setup(x => x.getTags()).Returns(fakeDBTags);

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
                       recipe.ItemList = new List<RecipeItem>();
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
                        recipe.ItemList.Add(new RecipeItem(drink, itemQuantity));
                        return 0;   //we don't care about Drink Recipe Ids anyways
                    }
                });

            mockDBManager.Setup(x => x.addDrinkTag(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int drinkID, int tagID) => {
                    Item drink = fakeDBItems.FirstOrDefault(x => x.ID == drinkID);
                    Tag tag = fakeDBTags.FirstOrDefault(x => x.ID == tagID);

                    if (drink == null || tag == null) {
                        return false;
                    } else {
                        drink.Tags.Add(tag);
                        return true;
                    }
                });
            
            mockDBManager.Setup(x => x.addTag(It.IsAny<string>()))
                .Returns((string newTagName) => {
                    latestTagID += 1;
                    Tag tagToAdd = new Tag {
                        ID = latestTagID,
                        Name = newTagName
                    };

                    fakeDBTags.Add(tagToAdd);
                    return latestTagID;
                });

            mockDBManager.Setup(x => x.deleteTag(It.IsAny<int>()))
                .Returns((int tagID) => {
                    //DataBase Manager Should Automatically delete DrinkTags for Deleted Tags
                    for (int i = 0; i < latestItemID; i++) {
                        for (int j = 0; j < ((Drink) fakeDBItems[i]).Tags.Count(); j++) {
                            if (fakeDBItems[i].Tags[j].ID == tagID) {
                                fakeDBItems[i].Tags.RemoveAt(j);
                            }
                        }
                    }

                    fakeDBTags = fakeDBTags.Where(x => x.ID != tagID).ToList();
                    return true;   
                });

            mockDBManager.Setup(x => x.deleteDrinkTagsByDrinkID(It.IsAny<int>()))
                .Returns((int drinkID) => {
                    Item drink = fakeDBItems.FirstOrDefault(x => x.ID == drinkID);
                    if (drink != null) {
                       drink.Tags = new List<Tag>();
                    }
                    return true; 
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
            Assert.Equal(fakeDBItemsString, resultOfGetString);
        }

        [Fact]
        public void UpdateItemTestSuccess() {
            //Arrange
            Drink updatedItem1 = new Drink {
                ID = 1,
                Name = "Updated Drink 1",
                Price = 9.99,
                Estimate = 13500.00,
                ParLevel = 7000.00,
                IdealLevel = 10000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 200,
                Brand = "Fake Brand 4",
                UnitsPerCase = 10,
                Vintage = null
            };
            string expectedName = "Updated Drink 1";
            double expectedEstimate = 13500.00;
            string expectedBrand = "Fake Brand 4";
            int expectedReturnCode = 0;

            //Act
            int returnCode = inventory.UpdateItem(updatedItem1);
            
            //Assert
            Assert.Equal(expectedName, fakeDBItems[0].Name);
            Assert.Equal(expectedEstimate, fakeDBItems[0].Estimate);
            Assert.Equal(expectedBrand, ((Drink) fakeDBItems[0]).Brand);
            Assert.Equal(expectedReturnCode, returnCode);
        }

        [Fact]
        public void updateItemTestFailure_NonExistentDrink() {
            //Arrange
            Drink fakeItem = new Drink {
                ID = -1,
                Name = "FakeItem 1",
                Price = 0.99,
                Estimate = 13500.00,
                ParLevel = 7000.00,
                IdealLevel = 10000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 200,
                Brand = "Fake Brand 4",
                UnitsPerCase = 10,
                Vintage = null
            };
            int expectedReturnCode = 1;

            //Act
            int returnCode = inventory.UpdateItem(fakeItem);

            //assert
            Assert.Equal(expectedReturnCode, returnCode);
        }

        [Fact]
        public void AddItemTestSuccess() {
            //Arrange
            Drink newItem = new Drink {
                Name = "New Item",
                Price = 1.99,
                Estimate = 27375.00,
                ParLevel = 17000.00,
                IdealLevel = 35000.00,
                Measurement = unit.milliliters,
                Status = status.aboveIdeal,
                BottleSize = 450,
                Brand = "Fake Brand 4",
                UnitsPerCase = 7,
                Vintage = null,
                Tags = new List<Tag> {}
            };
            int oldDBItemCount = fakeDBItems.Count();
            int expectedReturnCode = 0;

            //Act
            int returnCode = inventory.AddItem(newItem);

            //Assert
            Assert.Equal(fakeDBItems.Count(), oldDBItemCount + 1);
            var addedItem = fakeDBItems.FirstOrDefault(x => x.ID == latestItemID);
            string addedItemString = addedItem != null ? JsonConvert.SerializeObject(addedItem) : "";
            string expectedNewItemString = JsonConvert.SerializeObject(newItem);
            Assert.Equal(expectedNewItemString, addedItemString);
            Assert.Equal(expectedReturnCode, returnCode);
        }

        [Fact]
        public void removeItemTestSuccess() {
            //Arrange
            var itemToRemove = fakeDBItems[1];
            int oldDBItemCount = fakeDBItems.Count();
            int expectedReturnCode = 0;

            //Act
            int returnCode = inventory.RemoveItem(itemToRemove);

            //Assert
            Assert.Equal(fakeDBItems.Count(), oldDBItemCount - 1);
            Assert.Null(fakeDBItems.FirstOrDefault(x => x.ID == itemToRemove.ID));
            Assert.Equal(expectedReturnCode, returnCode);
        }

        [Fact]
        public void removeItemTestFailure_ItemUsedInRecipe() {
            //Arrange
            var itemToRemove = fakeDBItems[2];
            int oldDBItemCount = fakeDBItems.Count();
            int expectedReturnCode = 1;

            //Act
            int returnCode = inventory.RemoveItem(itemToRemove);
            //Assert
            Assert.Equal(fakeDBItems.Count(), oldDBItemCount);
            Assert.NotNull(fakeDBItems.FirstOrDefault(x => x.ID == itemToRemove.ID));
            Assert.Equal(expectedReturnCode, returnCode);
        }
    }
}
