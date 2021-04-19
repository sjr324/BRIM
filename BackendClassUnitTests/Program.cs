namespace BackendClassUnitTests{
    public class Program
    {
        public static void Main(string[] args)
        {
            new InventoryTests().AddItemTestSuccess();
            new InventoryTests().AddRecipeTestFailure_ItemListInvalid();
        }
    }
}
